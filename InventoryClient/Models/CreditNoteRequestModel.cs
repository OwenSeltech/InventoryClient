using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace InventoryClient.Models
{
    public class CreditNoteRequestModel
    {
        [JsonPropertyName("creditNoteID")] public int CreditNoteID { get; set; }
        [JsonPropertyName("invoiceID")] public int InvoiceID { get; set; }
        [Required(ErrorMessage = "The Amount field is required.")]
        [JsonPropertyName("creditAmount")] public decimal CreditAmount { get; set; }
        [JsonIgnore]
        public IEnumerable<SelectListItem>? Invoices { get; set; }
    }
}
