using Microsoft.AspNetCore.Mvc;

namespace InvoiceApiFinal.DTOs.Filtering
{
    public class InvoiceFilterRequest
    {
        [FromQuery(Name = "SearchByTitle")]
        public string? Title { get; set; }

    }
}
