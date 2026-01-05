using CashVault.Application.Common.Models;
using CashVault.Application.Interfaces;

namespace CashVault.WebAPI.Services
{
    public class SessionService : ISessionService
    {
        private string _language;
        public SessionUser User { get; set; }
        public bool IsAuthenticated { get; set; } = false;
        public string? IPAddress { get; set; }
        public string Language
        {
            get => _language ?? "en";
            set
            {
                if (!string.IsNullOrEmpty(value))
                    _language = value;
            }
        }

        public SessionService()
        {
            IPAddress = "";
            User = new SessionUser();
        }

        public void AuthenticateUser()
        {
            if (User is null) throw new ArgumentException(nameof(User));

            IsAuthenticated = true;
        }
    }
}
