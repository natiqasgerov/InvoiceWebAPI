using Microsoft.AspNetCore.Mvc;

namespace InvoiceApiFinal.DTOs.Filtering
{
    public class CustomerFilterRequest
    {
        /// <summary>
        /// Search by Text
        /// </summary>
        [FromQuery(Name = "SearchByName")]
        public string? Name { get; set; }

        [FromQuery(Name = "SearchByAddress")]
        public string? Address { get; set; }

    }
}
