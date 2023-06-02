using InventoryClient.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace InventoryClient.Controllers
{
	public class ProductsController : Controller
	{
		private readonly HttpClient _httpClient;

		public ProductsController(IHttpClientFactory httpClientFactory)
		{
			_httpClient = httpClientFactory.CreateClient("MyApiClient");
		}
		public async Task<IActionResult> Index()
		{
			var response = await _httpClient.GetAsync("Product");
			response.EnsureSuccessStatusCode();

			var content = await response.Content.ReadAsStringAsync();
			var products = JsonSerializer.Deserialize<List<Product>>(content);

			return View(products);
		}
	}
}
