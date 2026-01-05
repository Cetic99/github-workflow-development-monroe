using CashVault.Application.Interfaces.Persistence;
using CashVault.Domain.Aggregates.MessageAggregate;
using MediatR;

namespace CashVault.Application.Features.AdministrationFeatures.Queries
{
    public record GetAlllMessagesQuery : IRequest<AllMessagesDto> { }

    internal sealed class GetAlllMessagesQueryHandler : IRequestHandler<GetAlllMessagesQuery, AllMessagesDto>
    {
        private readonly IMessageRepository _db;
        private readonly ITerminalRepository _regionalDb;

        public GetAlllMessagesQueryHandler(IMessageRepository db, ITerminalRepository regionalDb)
        {
            _db = db;
            _regionalDb = regionalDb;
        }

        public async Task<AllMessagesDto> Handle(GetAlllMessagesQuery request, CancellationToken cancellationToken)
        {
            var messagesResult = new List<MessageItemDto>();
            var result = new AllMessagesDto();
            var messages = await _db.GetAllMessagesAsync();

            var regionalConfig = await _regionalDb.GetCurrentRegionalConfigurationAsync();
            result.DefaultLanguageCode = regionalConfig == null || regionalConfig.DefaultLanguage == null ? Language.Default.Code : regionalConfig.DefaultLanguage;


            if (!messages.Any()) return result;

            foreach (var message in messages)
            {
                messagesResult.Add(new MessageItemDto()
                {
                    Id = message.Id,
                    LanguageCode = message.LanguageCode,
                    Key = message.Key,
                    Value = message.Value
                });
            }

            result.Messages = messagesResult;

            return result;
        }
    }
}