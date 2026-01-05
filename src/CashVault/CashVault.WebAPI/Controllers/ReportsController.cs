using CashVault.Application.Features.DeviceFeatures.Queries;
using CashVault.Application.Features.ReportFeatures.Commands;
using CashVault.Domain.Aggregates.OperatorAggregate;
using CashVault.WebAPI.Common.CustomAttributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CashVault.WebAPI.Controllers
{
    [Authorize]
    [Route("api/reports")]
    public class ReportsController : BaseController
    {
        [HasPermission(PermissionEnum.Reports)]
        [HttpGet("daily-media")]
        public async Task<DailyMediaReportDto> GetDailyMedia([FromQuery] DateTime date)
        {
            return await Mediator.Send(new GetDailyMediaReportQuery()
            {
                Date = date
            });
        }

        [HasPermission(PermissionEnum.Reports)]
        [HttpPost("daily-media/print")]
        public async Task<ActionResult> PrintDailyMediaReport([FromQuery] DateTime date)
        {
            var dailyMediaDto = await Mediator.Send(new GetDailyMediaReportQuery()
            {
                Date = date
            });

            if (dailyMediaDto == null)
            {
                return NotFound();
            }

            return Ok(await Mediator.Send(new PrintDailyMediaReportCommand()
            {
                Date = date,
                DailyMediaReportDto = dailyMediaDto
            }));
        }

        [HasPermission(PermissionEnum.Reports)]
        [HttpGet("end-of-shift")]
        public async Task<EndOfShiftReportDto> GetEndOfShift()
        {
            return await Mediator.Send(new GetEndOfShiftReportQuery());
        }

        [HasPermission(PermissionEnum.Reports)]
        [HttpPost("end-of-shift/print")]
        public async Task<ActionResult> PrintEndOfShiftReport()
        {
            var endOfShiftReportDto = await Mediator.Send(new GetEndOfShiftReportQuery());

            if (endOfShiftReportDto == null)
            {
                return NotFound();
            }

            return Ok(await Mediator.Send(new PrintEndOfShiftReportCommand()
            {
                EndOfShiftReportDto = endOfShiftReportDto
            }));
        }

        [HasPermission(PermissionEnum.MoneyService)]
        [HttpGet("money-service")]
        public async Task<MoneyServiceReportDto> MoneyService()
        {
            return await Mediator.Send(new GetMoneyServiceReportQuery());
        }
    }
}
