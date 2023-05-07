using Microsoft.AspNetCore.Mvc;

namespace InvoiceApiFinal.DTOs.Sorting.CustomerSortings
{
    public class CustomerSortingRequest
    {

        [FromQuery(Name = "Model")]
        public CustomerSortingModel? model { get; set; }

        [FromQuery(Name = "Type")]
        public CustomerSortingType? type { get; set; }
        
    }
}
