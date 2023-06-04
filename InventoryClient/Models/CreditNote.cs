namespace InventoryClient.Models
{
    public class CreditNote
    {
        public int creditNoteID { get; set; }
        public decimal creditAmount { get; set; }
        public DateTime dateAdded { get; set; }
        public Customer customer { get; set; }
        public Invoice invoice { get; set; }
        public string FullName => $"{customer.customerFirstName} {customer.customerLastName}";
    }
}
