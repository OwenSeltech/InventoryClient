using InventoryClient.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Text;
using System.Text.Json;

namespace InventoryClient.Controllers
{
    public class QuotationsController : Controller
    {
        private readonly HttpClient _httpClient;

        public QuotationsController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("MyApiClient");
        }
        public async Task<IActionResult> Index()
        {

            ViewBag.mssg = TempData["mssg"] as string;
            ViewBag.mssgPop = TempData["mssgPop"] as string;
            ViewBag.mssgEdit = TempData["mssgEdit"] as string;
            ViewBag.mssgDelete = TempData["mssgDelete"] as string;

            var response = await _httpClient.GetAsync("Quotation");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var quotations = JsonSerializer.Deserialize<List<Quotation>>(content);

            return View(quotations);

        }

        [HttpGet]
        public async Task<ActionResult> _AddQuotation()
        {
            var response = await _httpClient.GetAsync("Customer");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var customers = JsonSerializer.Deserialize<List<Customer>>(content);

            var responseProduct = await _httpClient.GetAsync("Product");
            responseProduct.EnsureSuccessStatusCode();

            var contentProduct = await responseProduct.Content.ReadAsStringAsync();
            var products = JsonSerializer.Deserialize<List<Product>>(contentProduct);

            var quotationRequestModel = new QuotationRequestModel
            {
                Customers = customers.Select(c => new SelectListItem
                {
                    Value = c.customerID.ToString(),
                    Text = c.customerFirstName + " " + c.customerLastName
                }),
                Products = products.Select(p => new SelectListItem
                {
                    Value = p.productID.ToString(),
                    Text = p.productName
                })
            };
            return PartialView(quotationRequestModel);
        }

        [HttpPost]
        public async Task<ActionResult> _AddQuotation(QuotationRequestModel quotationRequest)
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
                        if (errorMessages.ElementAt(0) == "The value '' is invalid.")
                        {
                            if (entry.Key == "CustomerID") errorMessageString = "Please Select Customer Name";
                            else if (entry.Key == "ProductID") errorMessageString = "Please Select Product";
                            else errorMessageString = "The field Quantity is required";
                        }
                        else errorMessageString = string.Join("; ", errorMessages.ElementAt(0));
                    }
                }
                TempData["mssg"] = errorMessageString;
                return RedirectToAction("Index", "Quotations");
            }
            try
            {
                quotationRequest.QuotationID = 0;
                var json = JsonSerializer.Serialize(quotationRequest);
                var httpContent = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("Quotation/addQuotation", httpContent);
                response.EnsureSuccessStatusCode();
                TempData["mssgPop"] = "Quotation Added Successfully";
                return RedirectToAction("Index", "Quotations");
            }
            catch (HttpRequestException ex)
            {
                TempData["mssg"] = $"HTTP request error: {ex.Message}";
                return RedirectToAction("Index", "Quotations");
            }
            catch (Exception ex)
            {
                TempData["mssg"] = $"An error occurred: {ex.Message}";
                return RedirectToAction("Index", "Quotations");

            }

        }
        [HttpGet]
        public async Task<ActionResult> _EditQuotation(int quotationId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"Quotation/{quotationId}");
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                var quotation = JsonSerializer.Deserialize<QuotationRequestModel>(content);

                var responseCustomer = await _httpClient.GetAsync("Customer");
                responseCustomer.EnsureSuccessStatusCode();

                var contentCustomer = await responseCustomer.Content.ReadAsStringAsync();
                var customers = JsonSerializer.Deserialize<List<Customer>>(contentCustomer);

                var responseProduct = await _httpClient.GetAsync("Product");
                responseProduct.EnsureSuccessStatusCode();

                var contentProduct = await responseProduct.Content.ReadAsStringAsync();
                var products = JsonSerializer.Deserialize<List<Product>>(contentProduct);

                var quotationRequestModel = new QuotationRequestModel
                {
                    Customers = customers.Select(c => new SelectListItem
                    {
                        Value = c.customerID.ToString(),
                        Text = c.customerFirstName + " " + c.customerLastName
                    }),
                    Products = products.Select(p => new SelectListItem
                    {
                        Value = p.productID.ToString(),
                        Text = p.productName
                    }),
                    ItemsNo = quotation.ItemsNo,
                    CustomerID = quotation.CustomerID,
                    ProductID = quotation.ProductID,
                };
                return PartialView(quotationRequestModel);

            }
            catch (HttpRequestException ex)
            {
                TempData["mssg"] = $"HTTP request error: {ex.Message}";
                return RedirectToAction("Index", "Quotations");
            }
            catch (Exception ex)
            {
                TempData["mssg"] = $"An error occurred: {ex.Message}";
                return RedirectToAction("Index", "Quotations");

            }

        }
        [HttpPost]
        public async Task<ActionResult> _EditQuotation(QuotationRequestModel quotationRequest)
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
                        if (errorMessages.ElementAt(0) == "The value '' is invalid.")
                        {
                            if (entry.Key == "CustomerID") errorMessageString = "Please Select Customer Name";
                            else if (entry.Key == "ProductID") errorMessageString = "Please Select Product";
                            else errorMessageString = "The field Quantity is required";
                        }
                        else errorMessageString = string.Join("; ", errorMessages.ElementAt(0));
                    }
                }
                TempData["mssgEdit"] = quotationRequest.QuotationID + ":" + errorMessageString;
                return RedirectToAction("Index", "Quotations");
            }
            try
            {
                var json = JsonSerializer.Serialize(quotationRequest);
                var httpContent = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("Quotation/updateQuotation", httpContent);
                response.EnsureSuccessStatusCode();
                TempData["mssgPop"] = "Quotation Edited Successfully";
                return RedirectToAction("Index", "Quotations");
            }
            catch (HttpRequestException ex)
            {
                TempData["mssgEdit"] = $"HTTP request error: {ex.Message}";
                return RedirectToAction("Index", "Quotations");
            }
            catch (Exception ex)
            {
                TempData["mssgEdit"] = $"An error occurred: {ex.Message}";
                return RedirectToAction("Index", "Quotations");

            }

        }
        [HttpGet]
        public async Task<ActionResult> _DeleteQuotation(int quotationId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"Quotation/{quotationId}");
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                var quotation = JsonSerializer.Deserialize<QuotationRequestModel>(content);

                return PartialView(quotation);

            }
            catch (HttpRequestException ex)
            {
                TempData["mssg"] = $"HTTP request error: {ex.Message}";
                return RedirectToAction("Index", "Quotations");
            }
            catch (Exception ex)
            {
                TempData["mssg"] = $"An error occurred: {ex.Message}";
                return RedirectToAction("Index", "Quotations");

            }

        }
        [HttpPost]
        public async Task<ActionResult> _DeleteQuotation(QuotationRequestModel quotationRequest)
        {
            TempData["mssg"] = string.Empty;
            TempData["mssgPop"] = string.Empty;
            TempData["mssgEdit"] = string.Empty;
            try
            {
                var quotationId = quotationRequest.QuotationID;
                var response = await _httpClient.PostAsync($"Quotation/deleteQuotation/{quotationId}", null);
                response.EnsureSuccessStatusCode();
                TempData["mssgPop"] = "Quotation Deleted Successfully";
                return RedirectToAction("Index", "Quotations");
            }
            catch (HttpRequestException ex)
            {
                TempData["mssgPop"] = $"HTTP request error: {ex.Message}";
                return RedirectToAction("Index", "Quotations");
            }
            catch (Exception ex)
            {
                TempData["mssgPop"] = $"An error occurred: {ex.Message}";
                return RedirectToAction("Index", "Quotations");

            }

        }
    }
}
