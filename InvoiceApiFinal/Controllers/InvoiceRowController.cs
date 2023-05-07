using InvoiceApiFinal.DTOs.RowInvoice;
using InvoiceApiFinal.Models;
using InvoiceApiFinal.Providers;
using InvoiceApiFinal.Services.InvoiceServices;
using InvoiceApiFinal.Services.RowServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace InvoiceApiFinal.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class InvoiceRowController : ControllerBase
    {
        private readonly IRowService _rowService;

        private readonly IUserProvider _userProvider;

        public InvoiceRowController(IRowService rowService, IUserProvider userProvider)
        {
            _rowService = rowService;
            _userProvider = userProvider;
        }

        /// <summary>
        /// Adds a new row to the invoice with the given ID.
        /// </summary>
        /// <param name="customerId">The ID of the customer who owns the invoice.</param>
        /// <param name="invoiceId">The ID of the invoice to add the row to.</param>
        /// <param name="createRequest">The request object containing the details of the row to add.</param>
        /// <returns>The newly created invoice row.</returns>

        [HttpPost("{customerId}/{invoiceId}/AddRow")]
        public async Task<ActionResult<InvoiceRow>> AddRow(int customerId,int invoiceId,
            [FromBody] RowCreateRequest createRequest)
        {
            var userCookiee = _userProvider.GetUserInfo();
            if (userCookiee is null)
            {
                Log.Warning("Unauthorized access to invoiceRow: User cookie is null");
                return Unauthorized();
            }

            var customer = await _rowService.CheckCustomer(userCookiee.Id, customerId);
            if (customer is null)
            {
                Log.Warning("Customer not found in invoiceRow. CustomerID {CustomerId}", customerId);
                return NotFound("Customer not found in This User");
            }

            var invoice = await _rowService.CheckInvoice(customerId, invoiceId);
            if (invoice is null)
            {
                Log.Warning("Invoice not found in invoiceRow. InvoiceId: {InvoiceId}", invoiceId);
                return NotFound("Invoice not found in This Customer");
            }

            var added = await _rowService.AddRow(createRequest, invoice);

            if (added is null)
            {
                Log.Error("Failed to add row to the invoice. InvoiceId {InvoiceId}", invoiceId);
                return Problem(" ==> Row doesn't add");
            }

            Log.Information("Added row to the invoice is successfully. RowId {RowId}", added.Id);
            return added;
        }


        /// <summary>
        /// Deletes a row from an invoice.
        /// </summary>
        /// <param name="customerId">The ID of the customer that owns the invoice.</param>
        /// <param name="invoiceId">The ID of the invoice.</param>
        /// <param name="rowId">The ID of the row to be deleted.</param>
        /// <returns>The deleted row.</returns>

        [HttpDelete("{customerId}/{invoiceId}/{rowId}/DeleteRowById")]
        public async Task<ActionResult<InvoiceRow>> DeleteRow(int customerId,int invoiceId,int rowId)
        {
            var userCookiee = _userProvider.GetUserInfo();
            if (userCookiee is null)
            {
                Log.Warning("Unauthorized access to invoiceRow: User cookie is null");
                return Unauthorized();
            }

            var customer = await _rowService.CheckCustomer(userCookiee.Id, customerId);
            if (customer is null)
            {
                Log.Warning("Customer not found in invoiceRow. CustomerId: {CustomerId}", customerId);
                return NotFound("Customer not found in This User");
            }

            var invoice = await _rowService.CheckInvoice(customerId, invoiceId);
            if (invoice is null)
            {
                Log.Warning("Invoice not found in invoiceRow. InvoiceId: {InvoiceId}", invoiceId);
                return NotFound("Invoice not found in This Customer");
            }

            var row = await _rowService.CheckRow(invoiceId, rowId);
            if (row is null)
            {
                Log.Warning("Row not found in Invoice. RowId: {RowId}", rowId);
                return NotFound("Row not found in This Invoice");
            }

            var deleted = await _rowService.DeleteRow(row, invoice);
            if (deleted is null)
            {
                Log.Error("Failed to delete row. RowId: {RowId}", rowId);
                return Problem(" ==> Row doesn't delete");
            }

            Log.Information("Deleted row to the invoice is successfully. RowId {RowId}", rowId);
            return deleted;
        }
    }
}
