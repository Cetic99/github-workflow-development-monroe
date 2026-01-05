using System.Text.Json;
using System.Text.Json.Serialization;
using CashVault.Application.Interfaces;
using CashVault.Application.Interfaces.Persistence;
using CashVault.Domain;
using CashVault.Domain.Aggregates.DeviceAggregate.Interfaces;
using CashVault.Domain.Common.Events;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CashVault.Infrastructure.PersistentStorage;

public class UnitOfWork : IUnitOfWork
{
    private readonly CashVaultContext _dbContext;
    private readonly IMessageRepository _messageRepository;
    private readonly ITerminalRepository _deviceRepository;
    private readonly ITransactionRepository _transactionRepository;
    private readonly ITicketRepository _ticketRepository;
    private readonly IOperatorRepository _operatorRepository;
    private readonly IMoneyStatusRepository _moneyStatusRepository;
    private readonly IMediator _mediator;
    private readonly ILogger<UnitOfWork> _logger;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly IEventLogRepository _eventLogRepository;
    private readonly ISessionService _sessionService;
    private readonly IParcelLockerRepository _parcelLockerRepository;

    private readonly JsonSerializerOptions _jsonOptionsEventsSerialization = new()
    {
        ReferenceHandler = ReferenceHandler.IgnoreCycles,
    };

    public IMessageRepository MessageRepository => _messageRepository;

    public ITerminalRepository TerminalRepository => _deviceRepository;

    public ITransactionRepository TransactionRepository => _transactionRepository;

    public ITicketRepository TicketRepository => _ticketRepository;

    public IOperatorRepository OperatorRepository => _operatorRepository;
    public IMoneyStatusRepository MoneyStatusRepository => _moneyStatusRepository;
    public IEventLogRepository EventLogRepository => _eventLogRepository;
    public IParcelLockerRepository ParcelLockerRepository => _parcelLockerRepository;

    public UnitOfWork
        (CashVaultContext dbContext,
         IMessageRepository messageRepository,
         ITerminalRepository deviceRepository,
         ITransactionRepository transactionRepository,
         ITicketRepository ticketRepository,
         IOperatorRepository operatorRepository,
         IMoneyStatusRepository moneyStatusRepository,
         IMediator mediator,
         ILogger<UnitOfWork> logger,
         IServiceScopeFactory serviceScopeFactory,
         IEventLogRepository eventLogRepository,
         ISessionService sessionService,
         IParcelLockerRepository parcelLockerRepository)
    {
        _dbContext = dbContext;
        _messageRepository = messageRepository;
        _deviceRepository = deviceRepository;
        _transactionRepository = transactionRepository;
        _ticketRepository = ticketRepository;
        _operatorRepository = operatorRepository;
        _moneyStatusRepository = moneyStatusRepository;
        _mediator = mediator;
        _logger = logger;
        _serviceScopeFactory = serviceScopeFactory;
        _eventLogRepository = eventLogRepository;
        _sessionService = sessionService;
        _parcelLockerRepository = parcelLockerRepository;
    }

    public async Task<bool> SaveChangesAsync()
    {
        int result = await _dbContext.SaveChangesAsync();
        await PublishDomainEvents();

        return result == 1;
    }

    public bool SaveChanges()
    {
        int result = _dbContext.SaveChanges();
        PublishDomainEvents().GetAwaiter().GetResult();

        return result == 1;
    }

    /// <summary>
    /// Publish events using Mediatr.
    /// </summary>
    /// <returns></returns>
    private async Task PublishDomainEvents()
    {
        var domainEntities = _dbContext.ChangeTracker
            .Entries<IAggregateRoot>()
            .Where(x => x.Entity.DomainEvents != null && x.Entity.DomainEvents.Any())
            .ToList();

        var domainEvents = domainEntities
            .SelectMany(x => x.Entity.DomainEvents)
            .ToList();

        AttachUserContextToEvents(domainEvents);

        using var scope = _serviceScopeFactory.CreateScope();
        var scopedProvider = scope.ServiceProvider;

        var terminal = scopedProvider.GetRequiredService<ITerminal>();
        var terminalDomanEvents = terminal?.DomainEvents ?? new List<BaseEvent>();

        if (terminalDomanEvents.Any())
            domainEvents.AddRange(terminalDomanEvents);

        terminal?.ClearDomainEvents();

        domainEntities?.ForEach(entity => entity.Entity.ClearDomainEvents());

        if (domainEvents.Any())
        {
            await SaveEventsToDb(domainEvents);

            foreach (var domainEvent in domainEvents)
            {
                try
                {
                    await _mediator.Publish(domainEvent);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"An error occurred while publishing domain event: {nameof(domainEvent)}");
                }
            }

            _ = Task.Run(async () =>
            {
                await SendEventsToCMS(domainEvents, terminal);
            });
        }
    }

    private void AttachUserContextToEvents(List<BaseEvent> domainEvents)
    {
        if (domainEvents != null && domainEvents.Any())
        {
            foreach (var domainEvent in domainEvents)
            {
                domainEvent.CreatedByUser = _sessionService.User.Username;
                domainEvent.CreatedByUserFullName = _sessionService.User.FullName;
                domainEvent.CreatedByUserCompany = _sessionService.User.Company;
            }
        }
    }

    /// <summary>
    /// Save device events and transactin events to database.
    /// </summary>
    /// <param name="domainEvents"></param>
    /// <returns></returns>
    private async Task SaveEventsToDb(List<BaseEvent> domainEvents)
    {
        try
        {
            var baseEventLogs = domainEvents.OfType<BaseEvent>().ToList();

            foreach (var ev in baseEventLogs)
            {
                try
                {
                    ev.SetJson(JsonSerializer.Serialize(ev, ev.GetType(), _jsonOptionsEventsSerialization));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"An error occurred while serializing event to JSON [event name: {ev.EventName}]");
                }
            }

            if (baseEventLogs.Any())
            {
                await _dbContext.EventLogs.AddRangeAsync(baseEventLogs);
                await _dbContext.SaveChangesAsync();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while saving domain events to the database");
        }
    }

    /// <summary>
    /// Send events to the CMS (if configured).
    /// </summary>
    /// <param name="events"></param>
    /// <param name="onlineIntegrations"></param>
    /// <param name="cmsProtocol"></param>
    /// <returns></returns>
    private async Task<bool> SendEventsToCMS(List<BaseEvent> events, ITerminal terminal)
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var scopedProvider = scope.ServiceProvider;
        var unitOfWork = scope.ServiceProvider.GetService<IUnitOfWork>();
        var onlineIntegrations = terminal?.OnlineIntegrationsConfiguration;
        var cmsProtocol = scopedProvider.GetRequiredService<ICMSService>();
        var moneyStatus = await unitOfWork.MoneyStatusRepository.GetDispenserBillCountStatusAsync();
        var acceptorMoneyStatus = await unitOfWork.MoneyStatusRepository.GetBillTicketAcceptorBillCountStatusAsync();

        if (onlineIntegrations == null || !onlineIntegrations.CasinoManagementSystem || string.IsNullOrEmpty(onlineIntegrations.Url))
        {
            _logger.LogTrace("CMS integration is not enabled. Skiping sending events to CMS.");
            return true;
        }

        try
        {
            _logger.LogTrace($"Sending events to CMS...");

            //TODO: Maybe we should send all events batched in one request?
            foreach (var e in events)
            {
                if (e is DeviceFailEvent deviceFailEvent)
                {
                    await cmsProtocol.SendEvent(deviceFailEvent);
                }
                else if (e is DeviceEvent deviceEvent)
                {
                    await cmsProtocol.SendEvent(deviceEvent);
                }
                else if (e is TransactionEvent transactionEvent)
                {
                    await cmsProtocol.SendTransactionEvent(transactionEvent, moneyStatus, acceptorMoneyStatus);
                }
            }

            _logger.LogTrace($"Events sent to CMS successfully.");

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"An error occurred while sending events to the remote CMS");
        }

        return false;
    }
}
