using CashVault.Domain.Aggregates.MessageAggregate;

namespace CashVault.Application.Features.AdministrationFeatures.Queries
{
    public class AllMessagesDto
    {
        public List<MessageItemDto> Messages { get; set; }
        public string DefaultLanguageCode { get; set; }

        public AllMessagesDto()
        {
            Messages = [];
            DefaultLanguageCode = Language.Default.Code;
        }
    }
}
