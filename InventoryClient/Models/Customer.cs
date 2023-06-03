using System.ComponentModel.DataAnnotations.Schema;

namespace InventoryClient.Models
{
    public class Customer
    {
        public int customerID { get; set; }
        public string customerFirstName { get; set; }
        public string customerLastName { get; set; }
        public string customerAddress { get; set; }
        public string customerEmailAddress { get; set; }
        public DateTime dateAdded { get; set; }
    }
}
