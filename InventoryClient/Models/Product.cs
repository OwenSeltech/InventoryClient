namespace InventoryClient.Models
{
	public class Product
	{
		public int productID { get; set; }
		public string productName { get; set; }
		public int inventoryQuantity { get; set; }
		public decimal price { get; set; }
		public DateTime dateAdded { get; set; }

	}
}
