using CashVault.Application.Interfaces.Persistence;
using FluentValidation;
using MediatR;

namespace CashVault.Application.Features.AdministrationFeatures.Queries
{
    public class GetMessagesQueryValidator : AbstractValidator<GetMessagesQuery>
    {
        public GetMessagesQueryValidator()
        {
            RuleFor(x => x.Page).NotEmpty().GreaterThan(0).WithMessage("Page is required.");
            RuleFor(x => x.PageSize).NotEmpty().GreaterThan(0).WithMessage("PageSize is required.");
        }
    }

    public record GetMessagesQuery : IRequest<MessagesDto>
    {
        public int Page { get; init; }
        public int PageSize { get; init; }
        public string? Key { get; init; }
        public string? Value { get; init; }
        public string? LanguageCode { get; init; }
    }

    internal sealed class GetMessagesQueryHandler : IRequestHandler<GetMessagesQuery, MessagesDto>
    {
        private readonly IMessageRepository _db;

        public GetMessagesQueryHandler(IMessageRepository db)
        {
            _db = db;
        }

        public async Task<MessagesDto> Handle(GetMessagesQuery request, CancellationToken cancellationToken)
        {
            var messages = await _db.GetMessagesAsync(request.Page, request.PageSize, request.Key, request.Value, request.LanguageCode);

            var result = new MessagesDto(messages.Page, messages.PageSize, messages.TotalCount);

            if (messages == null || !messages.Data.Any())
                return result;

            foreach (var message in messages.Data)
            {
                result.Items.Add(new MessageItemDto()
                {
                    Id = message.Id,
                    LanguageCode = message.LanguageCode,
                    Key = message.Key,
                    Value = message.Value
                });
            }

            return result;
        }
    }
}
