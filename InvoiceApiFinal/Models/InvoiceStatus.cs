using System.Text.Json.Serialization;

namespace InvoiceApiFinal.Models
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum InvoiceStatus
    {
        Created,
        Sent,
        Received,
        Paid,
        Cancelled,
        Rejected
    }
}
