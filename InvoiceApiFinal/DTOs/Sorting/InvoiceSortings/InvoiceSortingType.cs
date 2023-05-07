using System.Text.Json.Serialization;

namespace InvoiceApiFinal.DTOs.Sorting.InvoiceSortings
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum InvoiceSortingType
    {
        Desc,
        Asc
    }
}
