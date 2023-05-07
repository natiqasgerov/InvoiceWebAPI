using System.Text.Json.Serialization;

namespace InvoiceApiFinal.DTOs.Sorting.InvoiceSortings
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum InvoiceSortingModel
    {
        Id,
        TotalSum,
        StartDate,
        EndDate,
        CreatedAt,
        UpdatedAt
    }
}
