using InvoiceApiFinal.DTOs.Customer;
using InvoiceApiFinal.Models;
using InvoiceApiFinal.Providers;
using InvoiceApiFinal.Services.ReportServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace InvoiceApiFinal.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ReportController : ControllerBase
    {
        private readonly IUserProvider _userProvider;

        private readonly IReportService _reportService;
        public ReportController(IUserProvider userProvider, IReportService reportService)
        {
            _userProvider = userProvider;
            _reportService = reportService;
        }

        /// <summary>
        /// Returns a report containing all customers for a user.
        /// </summary>
        /// <returns>A list of CustomerReportDto objects representing the report.</returns>

        [HttpGet("GetCustomersReport")]
        public async Task<ActionResult<List<CustomerReportDto>>> GetCustomersReport()
        {
            var userCookiee = _userProvider.GetUserInfo();

            if (userCookiee is null)
            {
                Log.Warning("User cookie is null in GetCustomersReport");
                return NotFound();
            }
            var listt = await _reportService.GetCustomers(userCookiee.Id);

            if (listt is null)
            {
                Log.Warning("List is null in GetCustomersReport");
                return NotFound();
            }

            Log.Information("GetCustomersReport executed successfully");
            return listt;
        }
    }
}
