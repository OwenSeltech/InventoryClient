namespace InventoryClient.Models
{
    public class Invoice
    {
        public int invoiceID { get; set; }
        public decimal invoiceAmount { get; set; }
        public int itemsNo { get; set; }
        public DateTime dateAdded { get; set; }
        public Customer customer { get; set; }
        public Product product { get; set; }
		public string FullName => $"{customer.customerFirstName} {customer.customerLastName}";

	}
}
