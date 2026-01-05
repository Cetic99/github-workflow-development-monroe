using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CashVault.WebAPI.Controllers
{
    [Route("api/[controller]")]
    public class TicketController : BaseController
    {
        [Route("local")]
        [HttpGet]
        public IActionResult GetLocalTickets()
        {
            return Ok("Local tickets");
        }
    }
}
