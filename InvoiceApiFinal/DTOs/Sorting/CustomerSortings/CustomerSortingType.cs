using System.Text.Json.Serialization;
namespace InvoiceApiFinal.DTOs.Sorting.CustomerSortings
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum CustomerSortingType
    {
        Desc,
        Asc
    }

}
