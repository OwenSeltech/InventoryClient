using InventoryClient.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text;
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
            
            ViewBag.mssg = TempData["mssg"] as string;
            ViewBag.mssgPop = TempData["mssgPop"] as string;
            ViewBag.mssgEdit = TempData["mssgEdit"] as string;
            ViewBag.mssgDelete = TempData["mssgDelete"] as string;
            

            var response = await _httpClient.GetAsync("Product");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var products = JsonSerializer.Deserialize<List<Product>>(content);

            return View(products);
          
          
        }

        [HttpGet]
        public async Task<ActionResult> _AddProduct()
        {
            return PartialView();
        }

        [HttpPost]
        public async Task<ActionResult> _AddProduct(ProductRequestModel productRequest)
        {
            string message = string.Empty;
            TempData["mssg"] = string.Empty;
            TempData["mssgPop"] = string.Empty;
            TempData["mssgEdit"] = string.Empty;
            if (!ModelState.IsValid)
            {
                string errorMessageString = "";
                foreach (var entry in ModelState)
                {
                    var errorMessages = entry.Value.Errors.Select(e => e.ErrorMessage);

                    if (errorMessages != null && errorMessages.Any())
                    {
                        if(errorMessages.ElementAt(0) == "The value '' is invalid.") errorMessageString = string.Format("The field {0} is invalid",entry.Key);
                        else errorMessageString = string.Join("; ", errorMessages.ElementAt(0));
                    }
                }
                TempData["mssg"] = errorMessageString;
                return RedirectToAction("Index", "Products");
            }
            try
            {
                productRequest.ProductID = 0;
                var json = JsonSerializer.Serialize(productRequest);
                var httpContent = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("Product/addProduct", httpContent);
                response.EnsureSuccessStatusCode();
                TempData["mssgPop"] = "Product Added Successfully";
                return RedirectToAction("Index", "Products");
            }
            catch (HttpRequestException ex)
            {
                TempData["mssg"] = $"HTTP request error: {ex.Message}";
                return RedirectToAction("Index", "Products");
            }
            catch (Exception ex)
            {
                TempData["mssg"] = $"An error occurred: {ex.Message}";
                return RedirectToAction("Index", "Products");
                
            }

        }
        [HttpGet]
        public async Task<ActionResult> _EditProduct(int productId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"Product/{productId}");
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                var product = JsonSerializer.Deserialize<ProductRequestModel>(content);

                return PartialView(product);

            }
            catch (HttpRequestException ex)
            {
                TempData["mssg"] = $"HTTP request error: {ex.Message}";
                return RedirectToAction("Index", "Products");
            }
            catch (Exception ex)
            {
                TempData["mssg"] = $"An error occurred: {ex.Message}";
                return RedirectToAction("Index", "Products");

            }
           
        }
        [HttpPost]
        public async Task<ActionResult> _EditProduct(ProductRequestModel productRequest)
        {
            string message = string.Empty;
            TempData["mssg"] = string.Empty;
            TempData["mssgPop"] = string.Empty;
            TempData["mssgEdit"] = string.Empty;
            if (!ModelState.IsValid)
            {
                string errorMessageString = "";
                foreach (var entry in ModelState)
                {
                    var errorMessages = entry.Value.Errors.Select(e => e.ErrorMessage);

                    if (errorMessages != null && errorMessages.Any())
                    {
                        if (errorMessages.ElementAt(0) == "The value '' is invalid.") errorMessageString = string.Format("The field {0} is invalid", entry.Key);
                        else errorMessageString = string.Join("; ", errorMessages.ElementAt(0));
                    }
                }
                TempData["mssgEdit"] = productRequest.ProductID + ":" + errorMessageString;
                return RedirectToAction("Index", "Products");
            }
            try
            {
                var json = JsonSerializer.Serialize(productRequest);
                var httpContent = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("Product/updateProduct", httpContent);
                response.EnsureSuccessStatusCode();
                TempData["mssgPop"] = "Product Edited Successfully";
                return RedirectToAction("Index", "Products");
            }
            catch (HttpRequestException ex)
            {
                TempData["mssgEdit"] = $"HTTP request error: {ex.Message}";
                return RedirectToAction("Index", "Products");
            }
            catch (Exception ex)
            {
                TempData["mssgEdit"] = $"An error occurred: {ex.Message}";
                return RedirectToAction("Index", "Products");

            }

        }
        [HttpGet]
        public async Task<ActionResult> _DeleteProduct(int productId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"Product/{productId}");
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                var product = JsonSerializer.Deserialize<ProductRequestModel>(content);

                return PartialView(product);

            }
            catch (HttpRequestException ex)
            {
                TempData["mssg"] = $"HTTP request error: {ex.Message}";
                return RedirectToAction("Index", "Products");
            }
            catch (Exception ex)
            {
                TempData["mssg"] = $"An error occurred: {ex.Message}";
                return RedirectToAction("Index", "Products");

            }

        }
        [HttpPost]
        public async Task<ActionResult> _DeleteProduct(ProductRequestModel productRequest)
        {
            TempData["mssg"] = string.Empty;
            TempData["mssgPop"] = string.Empty;
            TempData["mssgEdit"] = string.Empty;
            try
            {
                var productId = productRequest.ProductID;
                var response = await _httpClient.PostAsync($"Product/deleteProduct/{productId}",null);
                response.EnsureSuccessStatusCode();
                TempData["mssgPop"] = "Product Deleted Successfully";
                return RedirectToAction("Index", "Products");
            }
            catch (HttpRequestException ex)
            {
                TempData["mssgPop"] = $"HTTP request error: {ex.Message}";
                return RedirectToAction("Index", "Products");
            }
            catch (Exception ex)
            {
                TempData["mssgPop"] = $"An error occurred: {ex.Message}";
                return RedirectToAction("Index", "Products");

            }

        }

        

    }
}
