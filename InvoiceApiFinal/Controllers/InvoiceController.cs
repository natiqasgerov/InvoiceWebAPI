using InvoiceApiFinal.DTOs.Filtering;
using InvoiceApiFinal.DTOs.Invoice;
using InvoiceApiFinal.DTOs.Pagination;
using InvoiceApiFinal.DTOs.Sorting.InvoiceSortings;
using InvoiceApiFinal.Models;
using InvoiceApiFinal.Providers;
using InvoiceApiFinal.Services.InvoiceServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using Syncfusion.HtmlConverter;
using Syncfusion.Pdf;
using System.Threading.Channels;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace InvoiceApiFinal.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class InvoiceController : ControllerBase
    {
        private readonly IInvoiceService _invoiceService;

        private readonly IUserProvider _userProvider;

        public InvoiceController(IInvoiceService invoiceService, IUserProvider userProvider)
        {
            _invoiceService = invoiceService;
            _userProvider = userProvider;
        }


        /// <summary>
        /// Creates a new invoice for the specified customer.
        /// </summary>
        /// <param name="customerId">The ID of the customer for which the invoice will be created.</param>
        /// <param name="createRequest">The data required to create the invoice.</param>
        /// <returns>The newly created invoice information.</returns>
        
        [HttpPost("{customerId}/CreateInvoice")]
        public async Task<ActionResult<InvoiceInfoDto>> CreateInvoice(int customerId,[FromBody] InvoiceCreateRequest createRequest)
        {
            var userCookiee = _userProvider.GetUserInfo();
            if (userCookiee is null)
            {
                Log.Warning("Unauthorized access for creating invoice.");
                return Unauthorized();
            }

            var customer = await _invoiceService.CheckCustomer(userCookiee.Id, customerId);

            if (customer is null)
            {
                Log.Warning("Customer not found for creating invoice. CustomerId: {CustomerId}", customerId);
                return NotFound("Customer not found in This User");
            }

            var created = await _invoiceService.CreateInvoice(customer, createRequest);

            if (created is null)
            {
                Log.Warning("Failed to create invoice for customer. CustomerId: {CustomerId}", customerId);
                return NotFound();
            }

            Log.Information("Invoice created successfully. InvoiceId: {InvoiceId}", created.Id);
            return new InvoiceInfoDto
            {
                CustomerName = customer.Name,
                CustomerEmail = customer.Email,
                InvoiceId = created.Id,
                Comment = created.Comment,
                Title = created.Title,
                TotalSum = created.TotalSum,
                Status = created.Status,
                CreatedAt = created.CreatedAt,
                UpdatedAt = created.UpdatedAt,
                StartDate = created.StartDate,
                EndDate = created.EndDate,
                InvoiceRows = created.Rows
            };
        }


        /// <summary>
        /// Edits an existing invoice for a specific customer.
        /// </summary>
        /// <param name="customerId">The ID of the customer whose invoice is being edited.</param>
        /// <param name="invoiceId">The ID of the invoice to be edited.</param>
        /// <param name="editRequest">The updated information for the invoice.</param>
        /// <returns>The updated invoice information.</returns>

        [HttpPut("{customerId}/{invoiceId}/EditInvoice")]
        public async Task<ActionResult<InvoiceInfoDto>> EditInvoice(int customerId,int invoiceId,
            [FromBody] InvoiceEditRequest editRequest)
        {
            var userCookiee = _userProvider.GetUserInfo();
            if (userCookiee is null)
            {
                Log.Warning("Unauthorized access for editing invoice.");
                return Unauthorized();
            }

            var customer = await _invoiceService.CheckCustomer(userCookiee.Id, customerId);
            if (customer is null)
            {
                Log.Warning("Customer not found for editing invoice. CustomerId: {CustomerId}", customerId);
                return NotFound("Customer not found in This User");
            }

            var invoice = await _invoiceService.CheckInvoice(customerId, invoiceId);
            if (invoice is null)
            {
                Log.Warning("Invoice not found for editing. CustomerId: {CustomerId}, InvoiceId: {InvoiceId}", customerId, invoiceId);
                return NotFound("Invoice not found in This Customer");
            }

            if (invoice.Status == "Sent" || invoice.Status == "Received" || invoice.Status == "Paid")
            {
                Log.Error("Cannot edit invoice with status: {InvoiceStatus}. CustomerId: {CustomerId}, InvoiceId: {InvoiceId}", invoice.Status, customerId, invoiceId);
                return Problem(" ==> The Invoice status does not allow it");
            }

            var updated = await _invoiceService.UpdateInvoice(invoice, editRequest);

            if (updated is null)
            {
                Log.Warning("Failed to update invoice. CustomerId: {CustomerId}, InvoiceId: {InvoiceId}", customerId, invoiceId);
                return NotFound();
            }

            Log.Information("Invoice updated successfully. InvoiceId: {InvoiceId}", updated.Id);
            return new InvoiceInfoDto
            {
                CustomerName = customer.Name,
                CustomerEmail = customer.Email,
                InvoiceId = updated.Id,
                Comment = updated.Comment,
                Title = updated.Title,
                TotalSum = updated.TotalSum,
                Status = updated.Status,
                CreatedAt = updated.CreatedAt,
                UpdatedAt = updated.UpdatedAt,
                StartDate = updated.StartDate,
                EndDate = updated.EndDate,
                InvoiceRows = updated.Rows
            };
        }



        /// <summary>
        /// Changes the status of the specified invoice for the given customer.
        /// </summary>
        /// <param name="customerId">The ID of the customer who owns the invoice.</param>
        /// <param name="invoiceId">The ID of the invoice to change the status of.</param>
        /// <param name="changeRequest">The request object containing the new status value.</param>
        /// <returns>The updated invoice information.</returns>
        
        [HttpPatch("{customerId}/{invoiceId}/ChangeInvoiceStatus")]
        public async Task<ActionResult<InvoiceInfoDto>> ChangeInvoiceStatus(int customerId,int invoiceId,
            [FromQuery] InvoiceChangeStatusRequest changeRequest)
        {
            var userCookiee = _userProvider.GetUserInfo();
            if (userCookiee is null)
            {
                Log.Warning("Unauthorized access for changing invoice status.");
                return Unauthorized();
            }

            var customer = await _invoiceService.CheckCustomer(userCookiee.Id, customerId);
            if (customer is null)
            {
                Log.Warning("Customer not found for changing invoice status. CustomerId: {CustomerId}", customerId);
                return NotFound("Customer not found in This User");
            }

            var invoice = await _invoiceService.CheckInvoice(customerId, invoiceId);
            if (invoice is null)
            {
                Log.Warning("Invoice not found for changing status. CustomerId: {CustomerId}, InvoiceId: {InvoiceId}", customerId, invoiceId);
                return NotFound("Invoice not found in This Customer");
            }

            var changed = await _invoiceService.ChangeInvoiceStatus(invoice, changeRequest.Status.ToString());

            if (changed is null)
            {
                Log.Error("Failed to change invoice status. CustomerId: {CustomerId}, InvoiceId: {InvoiceId}", customerId, invoiceId);
                return Problem("Invoice Status not changed,Please try again");
            }

            Log.Information("Invoice status changed successfully. InvoiceId: {InvoiceId}, NewStatus: {NewStatus}", changed.Id, changed.Status);
            return new InvoiceInfoDto{
                CustomerName = customer.Name,
                CustomerEmail = customer.Email,
                InvoiceId = changed.Id,
                Comment = changed.Comment,
                Title = changed.Title,
                TotalSum = changed.TotalSum,
                Status = changed.Status,
                CreatedAt = changed.CreatedAt,
                UpdatedAt = changed.UpdatedAt,
                StartDate = changed.StartDate,
                EndDate = changed.EndDate,
                InvoiceRows = changed.Rows
            };

        }



        /// <summary>
        /// Deletes the invoice with the specified ID for the given customer.
        /// </summary>
        /// <param name="customerId">The ID of the customer whose invoice to delete.</param>
        /// <param name="invoiceId">The ID of the invoice to delete.</param>
        /// <returns>The deleted invoice information.</returns>

        [HttpDelete("{customerId}/{invoiceId}/DeleteInvoice")]
        public async Task<ActionResult<InvoiceInfoDto>> DeleteInvoice(int customerId, int invoiceId)
        {
            var userCookiee = _userProvider.GetUserInfo();
            if (userCookiee is null)
            {
                Log.Warning("Unauthorized access for deleting invoice.");
                return Unauthorized();
            }

            var customer = await _invoiceService.CheckCustomer(userCookiee.Id, customerId);
            if (customer is null)
            {
                Log.Warning("Customer not found for deleting invoice. CustomerId: {CustomerId}", customerId);
                return NotFound("Customer not found in This User");
            }

            var invoice = await _invoiceService.CheckInvoice(customerId, invoiceId);
            if (invoice is null)
            {
                Log.Warning("Invoice not found for deletion. CustomerId: {CustomerId}, InvoiceId: {InvoiceId}", customerId, invoiceId);
                return NotFound("Invoice not found in This Customer");
            }

            if (invoice.Status == "Sent" || invoice.Status == "Received" || invoice.Status == "Paid")
            {
                Log.Error("Invoice status does not allow deletion. InvoiceId: {InvoiceId}, Status: {Status}", invoiceId, invoice.Status);
                return Problem(" ==> The Invoice status does not allow it");
            }


            var deleted = await _invoiceService.DeleteInvoice(invoice);
            if (deleted is null)
            {
                Log.Error("Failed to delete invoice. InvoiceId: {InvoiceId}", invoiceId);
                return Problem(" ==> Invoice not deleted, Please try again");
            }

            Log.Information("Invoice deleted successfully. InvoiceId: {InvoiceId}", deleted.Id);
            return new InvoiceInfoDto {
                CustomerName = customer.Name,
                CustomerEmail = customer.Email,
                InvoiceId = deleted.Id,
                Comment = deleted.Comment,
                Title = deleted.Title,
                TotalSum = deleted.TotalSum,
                Status = deleted.Status,
                CreatedAt = deleted.CreatedAt,
                UpdatedAt = deleted.UpdatedAt,
                StartDate = deleted.StartDate,
                EndDate = deleted.EndDate,
                InvoiceRows = deleted.Rows
            };
        }



        /// <summary>
        /// Archives the invoice with the specified ID for the given customer.
        /// </summary>
        /// <param name="customerId">The ID of the customer who owns the invoice.</param>
        /// <param name="invoiceId">The ID of the invoice to archive.</param>
        /// <returns>The archived invoice information.</returns>

        [HttpDelete("{customerId}/{invoiceId}/ArchiveInvoice")]
        public async Task<ActionResult<ArchiveInfoDto>> ArchiveInvoice(int customerId, int invoiceId)
        {

            var userCookiee = _userProvider.GetUserInfo();
            if (userCookiee is null)
            {
                Log.Warning("Unauthorized access for archiving invoice.");
                return Unauthorized();
            }

            var customer = await _invoiceService.CheckCustomer(userCookiee.Id, customerId);
            if (customer is null)
            {
                Log.Warning("Customer not found for archiving invoice. CustomerId: {CustomerId}", customerId);
                return NotFound("Customer not found in This User");
            }

            var invoice = await _invoiceService.CheckInvoice(customerId, invoiceId);
            if (invoice is null)
            {
                Log.Warning("Invoice not found for archiving. CustomerId: {CustomerId}, InvoiceId: {InvoiceId}", customerId, invoiceId);
                return NotFound("Invoice not found in This Customer");
            }

            if (invoice.Status == "Sent" || invoice.Status == "Received" || invoice.Status == "Paid")
            {
                Log.Error("Invoice status does not allow archiving. InvoiceId: {InvoiceId}, Status: {Status}", invoiceId, invoice.Status);
                return Problem(" ==> The Invoice status does not allow it");
            }

            var archived = await _invoiceService.ArchiveInvoice(invoice);

            if (archived is null)
            {
                Log.Error("Archiving not successful. InvoiceId: {InvoiceId}", invoiceId);
                return Problem(" ==> Archived not work, Please try again");
            }

            Log.Information("Invoice archived successfully. InvoiceId: {InvoiceId}", archived.Id);
            return new ArchiveInfoDto
            {
                CustomerName = customer.Name,
                CustomerEmail = customer.Email,
                InvoiceId = archived.Id,
                Comment = archived.Comment,
                Title = archived.Title,
                TotalSum = archived.TotalSum,
                Status = archived.Status,
                CreatedAt = archived.CreatedAt,
                UpdatedAt = archived.UpdatedAt,
                StartDate = archived.StartDate,
                EndDate = archived.EndDate,
                DeletedAt = archived.DeletedAt,
                IsDeleted = archived.IsDeleted,
                InvoiceRows = archived.Rows
            };

        }


        /// <summary>
        /// Get the list of invoices for a specific customer.
        /// </summary>
        /// <param name="customerId">The ID of the customer to get the invoices for.</param>
        /// <param name="sortingRequest">The sorting options for the list of invoices.</param>
        /// <param name="filterRequest">The filtering options for the list of invoices.</param>
        /// <param name="paginationRequest">The pagination options for the list of invoices.</param>
        /// <returns>A paginated list of invoice information objects.</returns>

        [HttpGet("{customerId}/GetInvoiceList/filtering/sorting")]
        public async Task<ActionResult<PaginatedListDto<InvoiceInfoDto>>> GetInvoiceList(int customerId,
            [FromQuery] InvoiceSortingRequest sortingRequest, [FromQuery] InvoiceFilterRequest filterRequest,
            [FromQuery] PaginationRequest paginationRequest)
        {
            var userCookiee = _userProvider.GetUserInfo();
            if (userCookiee is null)
            {
                Log.Warning("Unauthorized access for getting list of invoice.");
                return Unauthorized();
            }

            var customer = await _invoiceService.CheckCustomer(userCookiee.Id, customerId);
            if (customer is null)
            {
                Log.Warning("Customer not found for getting list of invoice. CustomerId: {CustomerId}", customerId);
                return NotFound("Customer not found in This User");
            }

            return await _invoiceService.GetInvoices(customerId,sortingRequest.model.ToString(),
                sortingRequest.type.ToString(),filterRequest.Title,
                paginationRequest.Page,paginationRequest.PageSize);

        }


        /// <summary>
        /// Gets an invoice by ID for a specific customer.
        /// </summary>
        /// <param name="customerId">The ID of the customer who owns the invoice.</param>
        /// <param name="invoiceId">The ID of the invoice to retrieve.</param>
        /// <returns>The invoice with the specified ID belonging to the specified customer.</returns>

        [HttpGet("{customerId}/{invoiceId}/GetInvoiceById")]
        public async Task<ActionResult<InvoiceInfoDto>> GetInvoiceById(int customerId,int invoiceId)
        {
            var userCookiee = _userProvider.GetUserInfo();
            if (userCookiee is null)
            {
                Log.Warning("Unauthorized access for getting invoice by ID.");
                return Unauthorized();
            }

            var customer = await _invoiceService.CheckCustomer(userCookiee.Id, customerId);
            if (customer is null)
            {
                Log.Warning("Customer not found for getting invoice by ID. CustomerId: {CustomerId}", customerId);
                return NotFound("Customer not found in This User");
            }

            var invoice = await _invoiceService.CheckInvoice(customerId, invoiceId);
            if (invoice is null)
            {
                Log.Warning("Invoice not found for getting invoice by ID. CustomerId: {CustomerId}, InvoiceId: {InvoiceId}", customerId, invoiceId);
                return NotFound("Invoice not found in This Customer");
            }


            Log.Information("Invoice retrieved successfully by ID. InvoiceId: {InvoiceId}", invoice.Id);
            return new InvoiceInfoDto
            {
                CustomerName = customer.Name,
                CustomerEmail = customer.Email,
                InvoiceId = invoice.Id,
                Comment = invoice.Comment,
                Title = invoice.Title,
                TotalSum = invoice.TotalSum,
                Status = invoice.Status,
                StartDate = invoice.StartDate,
                EndDate = invoice.EndDate,
                CreatedAt = invoice.CreatedAt,
                UpdatedAt = invoice.UpdatedAt,
                InvoiceRows = invoice.Rows
            };
            
        }


        /// <summary>
        /// Generates a PDF file of an invoice and returns it as a file download.
        /// </summary>
        /// <param name="customerId">The ID of the customer who owns the invoice.</param>
        /// <param name="invoiceId">The ID of the invoice to download.</param>
        /// <returns>A file download of the PDF invoice.</returns>

        [HttpGet("{customerId}/{invoiceId}/DownloadInvoice")]
        public async Task<ActionResult> DownloadInvoice(int customerId, int invoiceId)
        {
            var userCookiee = _userProvider.GetUserInfo();
            if (userCookiee is null)
            {
                Log.Warning("Unauthorized access for downloading invoice.");
                return Unauthorized();
            }

            var customer = await _invoiceService.CheckCustomer(userCookiee.Id, customerId);
            if (customer is null)
            {
                Log.Warning("Customer not found for downloading invoice. CustomerId: {CustomerId}", customerId);
                return NotFound("Customer not found in This User");
            }

            var invoice = await _invoiceService.CheckInvoice(customerId, invoiceId);
            if (invoice is null)
            {
                Log.Warning("Invoice not found for downloading invoice. CustomerId: {CustomerId}, InvoiceId: {InvoiceId}", customerId, invoiceId);
                return NotFound("Invoice not found in This Customer");
            }


            if (invoice.Rows.Count == 0)
            {
                Log.Error("Cannot download invoice without invoice rows. InvoiceId: {InvoiceId}", invoice.Id);
                return Problem("Your invoice doesn't have any invoice row");
            }

            HtmlToPdfConverter htmlConverter = new HtmlToPdfConverter();


            string baseUrl = "https://cdn.jsdelivr.net/npm/bootstrap@4.4.1/dist/css/bootstrap.min.css";



            string text = System.IO.File.ReadAllText("Assets/CssCode.txt");


            string htmlText = $@"<link href=""https://cdn.jsdelivr.net/npm/bootstrap@4.4.1/dist/css/bootstrap.min.css"" rel=""stylesheet"" />
<body>
    <br><br><br>
    <div class=""page-content container"">
        <div class=""page-header text-blue-d2"">
            <h1 class=""page-title text-secondary-d1"">
                Invoice
                <small class=""page-info"">
                    <i class=""fa fa-angle-double-right text-80""></i>
                    ID: #{invoice.Id}
                </small>
            </h1>
                    
        </div>
    
        <div class=""container px-0"">
            <div class=""row mt-4"">
                <div class=""col-12 col-lg-12"">
                    <div class=""row"">
                        <div class=""col-12"">
                            <div class=""text-center text-150"">
                                <span class=""text-default-d3"">Information</span>
                            </div>
                        </div>
                    </div>
                    <!-- .row -->
    
                    <hr class=""row brc-default-l1 mx-n1 mb-4"" />
    
                    <div class=""row"">
                        <div class=""col-sm-6"">
                            <div>
                                <span class=""text-sm text-600 text-grey-m2 align-middle"">To:</span>
                                <span class=""text-600 text-110 text-blue align-middle""> {customer.Name}</span>
                            </div>
                            <div class=""text-grey-m2"">
                                <div class=""my-1"">
                                    <span class=""text-600 text-90"">Email: </span>{customer.Email}
                                </div>
                                <div class=""my-1"">
                                    <span class=""text-600 text-90"">Address: </span>{customer.Address}
                                </div>
                                
                                <div class=""my-1"">
                                    <span class=""text-600 text-90"">Phone: </span>{customer.PhoneNumber}
                                </div>

                                <div class=""my-1"">
                                    <span class=""text-600 text-90"">Creat Date: </span>{customer.CreatedAt.ToString("MMMM dd, yyyy H:mm")}
                                </div>
                            </div>
                        </div>
                        <!-- /.col -->
    
                        <div class=""text-95 col-sm-6 align-self-start d-sm-flex justify-content-end"">
                            <hr class=""d-sm-none"" />
                            <div class=""text-grey-m2"">
                                <div class=""mt-1 mb-2 text-secondary-m1 text-600 text-125"">
                                    Invoice
                                </div>
    
                                <div class=""my-2""><i class=""fa fa-circle text-blue-m2 text-xs mr-1""></i> <span class=""text-600 text-90"">ID: </span>{invoice.Id}</div>
    
                                <div class=""my-2""><i class=""fa fa-circle text-blue-m2 text-xs mr-1""></i> <span class=""text-600 text-90"">Issue Date: </span>{invoice.CreatedAt.ToString("MMMM dd, yyyy H:mm")}</div>
    
                                <div class=""my-2""><i class=""fa fa-circle text-blue-m2 text-xs mr-1""></i> <span class=""text-600 text-90"">Status: </span> <span class=""badge badge-warning badge-pill px-25"">{invoice.Status}</span></div>
                            </div>
                        </div>
                        <!-- /.col -->
                    </div>
                    <div class=""mt-4"">
                        <div class=""row text-600 text-white bgc-default-tp1 py-25"">
                            <div class=""d-none d-sm-block col-1"">#</div>
                            <div class=""col-9 col-sm-5"">Description</div>
                            <div class=""d-none d-sm-block col-4 col-sm-2"">Quantity</div>
                            <div class=""d-none d-sm-block col-sm-2"">Unit Price</div>
                            <div class=""col-2"">Amount</div>
                        </div>
    
                        <div class=""text-95 text-secondary-d3"">
";

            for (int i = 0; i < invoice.Rows.Count; i++)
            {

                var id = i;
                var item = invoice.Rows.ToList()[i];

                if (i % 2 == 0)
                {
                    htmlText += @$"<div class=""row mb-2 mb-sm-0 py-25"">
                                <div class=""d-none d-sm-block col-1"">{++id}</div>
                                <div class=""col-9 col-sm-5"">{item.Description}</div>
                                <div class=""d-none d-sm-block col-2"">{item.Quantity}</div>
                                <div class=""d-none d-sm-block col-2 text-95"">${item.Amount}</div>
                                <div class=""col-2 text-secondary-d2"">${item.Sum}</div>
                            </div>";
                }
                else
                {
                    htmlText += @$"<div class=""row mb-2 mb-sm-0 py-25 bgc-default-l4"">
                                <div class=""d-none d-sm-block col-1"">{++id}</div>
                                <div class=""col-9 col-sm-5"">{item.Description}</div>
                                <div class=""d-none d-sm-block col-2"">{item.Quantity}</div>
                                <div class=""d-none d-sm-block col-2 text-95"">${item.Amount}</div>
                                <div class=""col-2 text-secondary-d2"">${item.Sum}</div>
                            </div>";
                }
            }

            
            htmlText += @$" </div>
    
                        <div class=""row border-b-2 brc-default-l2""></div>
    
                        
    
                        <div class=""row mt-3"">
                            <div class=""col-12 col-sm-7 text-grey-d2 text-95 mt-2 mt-lg-0"">
                                Extra note such as company or payment information...
                            </div>
    
                            <div class=""col-12 col-sm-5 text-grey text-90 order-first order-sm-last"">
                                
                                <div class=""row my-2 align-items-center bgc-primary-l3 p-2"">
                                    <div class=""col-7 text-right"">
                                        Total Amount
                                    </div>
                                    <div class=""col-5"">
                                        <span class=""text-150 text-success-d3 opacity-2"">${invoice.TotalSum}</span>
                                    </div>
                                </div>
                            </div>
                        </div>
    
                        <hr />
    
                        <div>
                            <span class=""text-secondary-d1 text-105"">Thank you for your business</span>
                        </div>
                        <br><br>
                    </div>
                </div>
            </div>
        </div>
    </div>
</div>
</body>
</html>";                
                            

            text += htmlText;

            PdfDocument document = htmlConverter.Convert(text, baseUrl);

            byte[]? response = null;
            using (MemoryStream ms = new MemoryStream())
            {
                document.Save(ms);
                response = ms.ToArray();
            }
            string Filename = "Invoice_" + invoice.Id + ".pdf";

            Log.Information("Invoice pdf downloading successfully by ID. InvoiceId: {InvoiceId}", invoice.Id);
            return File(response, "application/pdf", Filename);
        }
    }
}
