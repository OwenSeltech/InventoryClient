using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace InventoryClient.Models
{
    public class QuotationRequestModel
    {
        [JsonPropertyName("quotationID")] public int QuotationID { get; set; }
        [JsonPropertyName("customerID")] public int CustomerID { get; set; }
        [JsonPropertyName("productID")] public int ProductID { get; set; }
        [Required(ErrorMessage = "The Items field is required.")]
        [JsonPropertyName("itemsNo")] public int ItemsNo { get; set; }
        [JsonIgnore]
        public IEnumerable<SelectListItem>? Customers { get; set; }
        [JsonIgnore]
        public IEnumerable<SelectListItem>? Products { get; set; }
    }
}
