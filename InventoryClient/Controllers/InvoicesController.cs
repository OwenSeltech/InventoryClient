using InventoryClient.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Text;
using System.Text.Json;

namespace InventoryClient.Controllers
{
    public class InvoicesController : Controller
    {
        private readonly HttpClient _httpClient;

        public InvoicesController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("MyApiClient");
        }
        public async Task<IActionResult> Index()
        {

            ViewBag.mssg = TempData["mssg"] as string;
            ViewBag.mssgPop = TempData["mssgPop"] as string;
            ViewBag.mssgEdit = TempData["mssgEdit"] as string;
            ViewBag.mssgDelete = TempData["mssgDelete"] as string;


            var response = await _httpClient.GetAsync("Invoice");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var invoices = JsonSerializer.Deserialize<List<Invoice>>(content);

            return View(invoices);

        }

        [HttpGet]
        public async Task<ActionResult> _AddInvoice()
        {
            var response = await _httpClient.GetAsync("Customer");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var customers = JsonSerializer.Deserialize<List<Customer>>(content);

            var responseProduct = await _httpClient.GetAsync("Product");
            responseProduct.EnsureSuccessStatusCode();

            var contentProduct = await responseProduct.Content.ReadAsStringAsync();
            var products = JsonSerializer.Deserialize<List<Product>>(contentProduct);

            var invoiceRequestModel = new InvoiceRequestModel
            {
                Customers = customers.Select(c => new SelectListItem
                {
                    Value = c.customerID.ToString(),
                    Text = c.customerFirstName + " " + c.customerLastName
                }),
                Products  = products.Select(p => new SelectListItem
                {
                    Value = p.productID.ToString(),
                    Text = p.productName
                })
            };
            return PartialView(invoiceRequestModel);
        }

        [HttpPost]
        public async Task<ActionResult> _AddInvoice(InvoiceRequestModel invoiceRequest)
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
                return RedirectToAction("Index", "Invoices");
            }
            try
            {
                invoiceRequest.InvoiceID = 0;
                var json = JsonSerializer.Serialize(invoiceRequest);
                var httpContent = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("Invoice/addInvoice", httpContent);
                response.EnsureSuccessStatusCode();
                TempData["mssgPop"] = "Invoice Added Successfully";
                return RedirectToAction("Index", "Invoices");
            }
            catch (HttpRequestException ex)
            {
                TempData["mssg"] = $"HTTP request error: {ex.Message}";
                return RedirectToAction("Index", "Invoices");
            }
            catch (Exception ex)
            {
                TempData["mssg"] = $"An error occurred: {ex.Message}";
                return RedirectToAction("Index", "Invoices");

            }

        }
        [HttpGet]
        public async Task<ActionResult> _EditInvoice(int invoiceId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"Invoice/{invoiceId}");
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                var invoice = JsonSerializer.Deserialize<InvoiceRequestModel>(content);

                var responseCustomer = await _httpClient.GetAsync("Customer");
                responseCustomer.EnsureSuccessStatusCode();

                var contentCustomer = await responseCustomer.Content.ReadAsStringAsync();
                var customers = JsonSerializer.Deserialize<List<Customer>>(contentCustomer);

                var responseProduct = await _httpClient.GetAsync("Product");
                responseProduct.EnsureSuccessStatusCode();

                var contentProduct = await responseProduct.Content.ReadAsStringAsync();
                var products = JsonSerializer.Deserialize<List<Product>>(contentProduct);

                var invoiceRequestModel = new InvoiceRequestModel
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
                    ItemsNo = invoice.ItemsNo,
                    CustomerID = invoice.CustomerID,
                    ProductID = invoice.ProductID,
                };
                return PartialView(invoiceRequestModel);

            }
            catch (HttpRequestException ex)
            {
                TempData["mssg"] = $"HTTP request error: {ex.Message}";
                return RedirectToAction("Index", "Invoices");
            }
            catch (Exception ex)
            {
                TempData["mssg"] = $"An error occurred: {ex.Message}";
                return RedirectToAction("Index", "Invoices");

            }

        }
        [HttpPost]
        public async Task<ActionResult> _EditInvoice(InvoiceRequestModel invoiceRequest)
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
                TempData["mssgEdit"] = invoiceRequest.InvoiceID + ":" + errorMessageString;
                return RedirectToAction("Index", "Invoices");
            }
            try
            {
                var json = JsonSerializer.Serialize(invoiceRequest);
                var httpContent = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("Invoice/updateInvoice", httpContent);
                response.EnsureSuccessStatusCode();
                TempData["mssgPop"] = "Invoice Edited Successfully";
                return RedirectToAction("Index", "Invoices");
            }
            catch (HttpRequestException ex)
            {
                TempData["mssgEdit"] = $"HTTP request error: {ex.Message}";
                return RedirectToAction("Index", "Invoices");
            }
            catch (Exception ex)
            {
                TempData["mssgEdit"] = $"An error occurred: {ex.Message}";
                return RedirectToAction("Index", "Invoices");

            }

        }
        [HttpGet]
        public async Task<ActionResult> _DeleteInvoice(int invoiceId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"Invoice/{invoiceId}");
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                var invoice = JsonSerializer.Deserialize<InvoiceRequestModel>(content);

                return PartialView(invoice);

            }
            catch (HttpRequestException ex)
            {
                TempData["mssg"] = $"HTTP request error: {ex.Message}";
                return RedirectToAction("Index", "Invoices");
            }
            catch (Exception ex)
            {
                TempData["mssg"] = $"An error occurred: {ex.Message}";
                return RedirectToAction("Index", "Invoices");

            }

        }
        [HttpPost]
        public async Task<ActionResult> _DeleteInvoice(InvoiceRequestModel invoiceRequest)
        {
            TempData["mssg"] = string.Empty;
            TempData["mssgPop"] = string.Empty;
            TempData["mssgEdit"] = string.Empty;
            try
            {
                var invoiceId = invoiceRequest.InvoiceID;
                var response = await _httpClient.PostAsync($"Invoice/deleteInvoice/{invoiceId}", null);
                response.EnsureSuccessStatusCode();
                TempData["mssgPop"] = "Invoice Deleted Successfully";
                return RedirectToAction("Index", "Invoices");
            }
            catch (HttpRequestException ex)
            {
                TempData["mssgPop"] = $"HTTP request error: {ex.Message}";
                return RedirectToAction("Index", "Invoices");
            }
            catch (Exception ex)
            {
                TempData["mssgPop"] = $"An error occurred: {ex.Message}";
                return RedirectToAction("Index", "Invoices");

            }

        }
    }
}
