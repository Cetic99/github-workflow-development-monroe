using CashVault.Application.Common.Models;

namespace CashVault.Application.Interfaces
{
    public interface ISessionService
    {
        SessionUser User { get; }
        bool IsAuthenticated { get; set; }
        string? IPAddress { get; set; }
        string? Language { get; set; }

        /// <summary>
        /// Sets the IsAuthenticated property to true if the User object from sessinService is not null.
        /// </summary>
        public void AuthenticateUser();
    }
}
