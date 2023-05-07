using InvoiceApiFinal.DTOs.Sorting.CustomerSortings;
using Microsoft.AspNetCore.Mvc;

namespace InvoiceApiFinal.DTOs.Sorting.InvoiceSortings
{
    public class InvoiceSortingRequest
    {
        [FromQuery(Name = "Model")]
        public InvoiceSortingModel? model { get; set; }

        [FromQuery(Name = "Type")]
        public InvoiceSortingType? type { get; set; }
    }
}
