using InventoryClient.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;

namespace InventoryClient.Controllers
{
    public class CustomersController : Controller
    {
        private readonly HttpClient _httpClient;

        public CustomersController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("MyApiClient");
        }
        public async Task<IActionResult> Index()
        {

            ViewBag.mssg = TempData["mssg"] as string;
            ViewBag.mssgPop = TempData["mssgPop"] as string;
            ViewBag.mssgEdit = TempData["mssgEdit"] as string;

            var response = await _httpClient.GetAsync("Customer");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var customers = JsonSerializer.Deserialize<List<Customer>>(content);
            return View(customers);

        }

        [HttpGet]
        public async Task<ActionResult> _AddCustomer()
        {
            return PartialView();
        }

        [HttpPost]
        public async Task<ActionResult> _AddCustomer(CustomerRequestModel customerRequest)
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
                TempData["mssg"] = errorMessageString;
                return RedirectToAction("Index", "Customers");
            }
            try
            {
                customerRequest.CustomerID = 0;
                var json = JsonSerializer.Serialize(customerRequest);
                var httpContent = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("Customer/addCustomer", httpContent);
                response.EnsureSuccessStatusCode();
                TempData["mssgPop"] = "Customer Added Successfully";
                return RedirectToAction("Index", "Customers");
            }
            catch (HttpRequestException ex)
            {
                TempData["mssg"] = $"HTTP request error: {ex.Message}";
                return RedirectToAction("Index", "Customers");
            }
            catch (Exception ex)
            {
                TempData["mssg"] = $"An error occurred: {ex.Message}";
                return RedirectToAction("Index", "Customers");

            }

        }
        [HttpGet]
        public async Task<ActionResult> _EditCustomer(int customerId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"Customer/{customerId}");
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                var Customer = JsonSerializer.Deserialize<CustomerRequestModel>(content);

                return PartialView(Customer);

            }
            catch (HttpRequestException ex)
            {
                TempData["mssg"] = $"HTTP request error: {ex.Message}";
                return RedirectToAction("Index", "Customers");
            }
            catch (Exception ex)
            {
                TempData["mssg"] = $"An error occurred: {ex.Message}";
                return RedirectToAction("Index", "Customers");

            }

        }
        [HttpPost]
        public async Task<ActionResult> _EditCustomer(CustomerRequestModel customerRequest)
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
                TempData["mssgEdit"] = customerRequest.CustomerID + ":" + errorMessageString;
                return RedirectToAction("Index", "Customers");
            }
            try
            {
                var json = JsonSerializer.Serialize(customerRequest);
                var httpContent = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("Customer/updateCustomer", httpContent);
                response.EnsureSuccessStatusCode();
                TempData["mssgPop"] = "Customer Edited Successfully";
                return RedirectToAction("Index", "Customers");
            }
            catch (HttpRequestException ex)
            {
                TempData["mssgEdit"] = $"HTTP request error: {ex.Message}";
                return RedirectToAction("Index", "Customers");
            }
            catch (Exception ex)
            {
                TempData["mssgEdit"] = $"An error occurred: {ex.Message}";
                return RedirectToAction("Index", "Customers");

            }

        }
        [HttpGet]
        public async Task<ActionResult> _DeleteCustomer(int customerId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"Customer/{customerId}");
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                var Customer = JsonSerializer.Deserialize<CustomerRequestModel>(content);

                return PartialView(Customer);

            }
            catch (HttpRequestException ex)
            {
                TempData["mssg"] = $"HTTP request error: {ex.Message}";
                return RedirectToAction("Index", "Customers");
            }
            catch (Exception ex)
            {
                TempData["mssg"] = $"An error occurred: {ex.Message}";
                return RedirectToAction("Index", "Customers");

            }

        }
        [HttpPost]
        public async Task<ActionResult> _DeleteCustomer(CustomerRequestModel customerRequest)
        {
            TempData["mssg"] = string.Empty;
            TempData["mssgPop"] = string.Empty;
            TempData["mssgEdit"] = string.Empty;
            try
            {
                var customerId = customerRequest.CustomerID;
                var response = await _httpClient.PostAsync($"Customer/deleteCustomer/{customerId}", null);
                response.EnsureSuccessStatusCode();
                TempData["mssgPop"] = "Customer Deleted Successfully";
                return RedirectToAction("Index", "Customers");
            }
            catch (HttpRequestException ex)
            {
                TempData["mssgPop"] = $"HTTP request error: {ex.Message}";
                return RedirectToAction("Index", "Customers");
            }
            catch (Exception ex)
            {
                TempData["mssgPop"] = $"An error occurred: {ex.Message}";
                return RedirectToAction("Index", "Customers");

            }

        }
    }
}
