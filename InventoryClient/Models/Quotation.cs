namespace InventoryClient.Models
{
    public class Quotation
    {
        public int quotationID { get; set; }
        public int itemsNo { get; set; }
        public decimal quotationAmount { get; set; }
        public Customer customer { get; set; }
        public Product product { get; set; }
        public DateTime dateAdded { get; set; }
        public string FullName => $"{customer.customerFirstName} {customer.customerLastName}";

    }
}
