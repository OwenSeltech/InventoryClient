using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace InventoryClient.Models
{
    public class CustomerRequestModel
    {
        [Required(ErrorMessage = "The Customer FirstName field is required.")]
        [JsonPropertyName("customerFirstName")] public string CustomerFirstName { get; set; }

        [Required(ErrorMessage = "The Customer LastName field is required.")]
        [JsonPropertyName("customerLastName")] public string CustomerLastName { get; set; }
        [Required(ErrorMessage = "The Customer Address field is required.")]
        [JsonPropertyName("customerAddress")] public string CustomerAddress { get; set; }
        [Required(ErrorMessage = "The Customer Email Address field is required.")]
        [RegularExpression(@"^[\w\.-]+@[\w\.-]+\.\w+$", ErrorMessage = "Invalid email address")]
        [JsonPropertyName("customerEmailAddress")] public string CustomerEmailAddress { get; set; }
        [JsonPropertyName("customerID")] public int? CustomerID { get; set; }
    }
}
