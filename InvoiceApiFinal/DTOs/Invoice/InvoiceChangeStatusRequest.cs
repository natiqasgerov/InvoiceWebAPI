using InvoiceApiFinal.Models;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace InvoiceApiFinal.DTOs.Invoice
{
    public class InvoiceChangeStatusRequest
    {
        [FromQuery(Name = "Status")]
        [Required]
        public InvoiceStatus Status { get; set; }
    }
}
