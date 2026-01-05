using CashVault.Infrastructure.PersistentStorage;
using Microsoft.AspNetCore.Mvc;

namespace CashVault.WebAPI.Controllers
{
    [ApiController]
    [Route("account")]
    public class AccountController : ControllerBase
    {
        private readonly ILogger<AccountController> _logger;
        private readonly CashVaultContext _db;

        public AccountController(ILogger<AccountController> logger, CashVaultContext db)
        {
            _logger = logger;
            _db = db;
        }

        [HttpGet("credit")]
        public decimal GetCurrentCreditAmount()
        {
            return 100;
        }
    }
}
