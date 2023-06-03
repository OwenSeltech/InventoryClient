using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace InventoryClient.Models
{
    public class ProductRequestModel
    {
       
        [Required(ErrorMessage = "The Product Name field is required.")]
        [JsonPropertyName("productName")] public string ProductName { get; set; }
        [Required(ErrorMessage = "The Inventory Quantity field is required.")]
        [JsonPropertyName("inventoryQuantity")] public int InventoryQuantity { get; set; }
        [Required(ErrorMessage = "The Price field is required.")]
        [JsonPropertyName("price")] public decimal Price { get; set; }
        [JsonPropertyName("productID")] public int? ProductID { get; set; }
    }
}
