using InventoryClient.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Text;
using System.Text.Json;

namespace InventoryClient.Controllers
{
    public class CreditNotesController : Controller
    {
        private readonly HttpClient _httpClient;

        public CreditNotesController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("MyApiClient");
        }
        public async Task<IActionResult> Index()
        {

            ViewBag.mssg = TempData["mssg"] as string;
            ViewBag.mssgPop = TempData["mssgPop"] as string;
            ViewBag.mssgEdit = TempData["mssgEdit"] as string;
            ViewBag.mssgDelete = TempData["mssgDelete"] as string;


            var response = await _httpClient.GetAsync("CreditNote");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var creditNotes = JsonSerializer.Deserialize<List<CreditNote>>(content);

            return View(creditNotes);

        }

        [HttpGet]
        public async Task<ActionResult> _AddCreditNote()
        {
            var response = await _httpClient.GetAsync("Invoice");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var invoices = JsonSerializer.Deserialize<List<Invoice>>(content);

            var creditNoteRequestModel = new CreditNoteRequestModel
            {
                Invoices = invoices.Select(i => new SelectListItem
                {
                    Value = i.invoiceID.ToString(),
                    Text = i.FullName  + "-" + i.invoiceAmount.ToString()
                }),
            };
            return PartialView(creditNoteRequestModel);
        }

        [HttpPost]
        public async Task<ActionResult> _AddCreditNote(CreditNoteRequestModel creditNoteRequest)
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
                return RedirectToAction("Index", "CreditNotes");
            }
            try
            {
                creditNoteRequest.CreditNoteID = 0;
                var json = JsonSerializer.Serialize(creditNoteRequest);
                var httpContent = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("CreditNote/addCreditNote", httpContent);
                response.EnsureSuccessStatusCode();
                TempData["mssgPop"] = "CreditNote Added Successfully";
                return RedirectToAction("Index", "CreditNotes");
            }
            catch (HttpRequestException ex)
            {
                TempData["mssg"] = $"HTTP request error: {ex.Message}";
                return RedirectToAction("Index", "CreditNotes");
            }
            catch (Exception ex)
            {
                TempData["mssg"] = $"An error occurred: {ex.Message}";
                return RedirectToAction("Index", "CreditNotes");

            }

        }
        [HttpGet]
        public async Task<ActionResult> _EditCreditNote(int creditNoteId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"CreditNote/{creditNoteId}");
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                var creditNote = JsonSerializer.Deserialize<CreditNoteRequestModel>(content);

                var responseInvoices = await _httpClient.GetAsync("Invoice");
                responseInvoices.EnsureSuccessStatusCode();

                var contentInvoices = await responseInvoices.Content.ReadAsStringAsync();
                var invoices = JsonSerializer.Deserialize<List<Invoice>>(contentInvoices);

                var creditNoteRequestModel = new CreditNoteRequestModel
                {
                    Invoices = invoices.Select(i => new SelectListItem
                    {
                        Value = i.invoiceID.ToString(),
                        Text = i.FullName  + "-" + i.invoiceAmount.ToString()
                    }),
                    CreditAmount = creditNote.CreditAmount,
                    CreditNoteID = creditNote.CreditNoteID,
                    InvoiceID = creditNote.InvoiceID,
                };
                return PartialView(creditNoteRequestModel);

            }
            catch (HttpRequestException ex)
            {
                TempData["mssg"] = $"HTTP request error: {ex.Message}";
                return RedirectToAction("Index", "CreditNotes");
            }
            catch (Exception ex)
            {
                TempData["mssg"] = $"An error occurred: {ex.Message}";
                return RedirectToAction("Index", "CreditNotes");

            }

        }
        [HttpPost]
        public async Task<ActionResult> _EditCreditNote(CreditNoteRequestModel creditNoteRequest)
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
                TempData["mssgEdit"] = creditNoteRequest.CreditNoteID + ":" + errorMessageString;
                return RedirectToAction("Index", "CreditNotes");
            }
            try
            {
                var json = JsonSerializer.Serialize(creditNoteRequest);
                var httpContent = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("CreditNote/updateCreditNote", httpContent);
                response.EnsureSuccessStatusCode();
                TempData["mssgPop"] = "CreditNote Edited Successfully";
                return RedirectToAction("Index", "CreditNotes");
            }
            catch (HttpRequestException ex)
            {
                TempData["mssgEdit"] = $"HTTP request error: {ex.Message}";
                return RedirectToAction("Index", "CreditNotes");
            }
            catch (Exception ex)
            {
                TempData["mssgEdit"] = $"An error occurred: {ex.Message}";
                return RedirectToAction("Index", "CreditNotes");

            }

        }
        [HttpGet]
        public async Task<ActionResult> _DeleteCreditNote(int creditNoteId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"CreditNote/{creditNoteId}");
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                var creditNote = JsonSerializer.Deserialize<CreditNoteRequestModel>(content);

                return PartialView(creditNote);

            }
            catch (HttpRequestException ex)
            {
                TempData["mssg"] = $"HTTP request error: {ex.Message}";
                return RedirectToAction("Index", "CreditNotes");
            }
            catch (Exception ex)
            {
                TempData["mssg"] = $"An error occurred: {ex.Message}";
                return RedirectToAction("Index", "CreditNotes");

            }

        }
        [HttpPost]
        public async Task<ActionResult> _DeleteCreditNote(CreditNoteRequestModel creditNoteRequest)
        {
            TempData["mssg"] = string.Empty;
            TempData["mssgPop"] = string.Empty;
            TempData["mssgEdit"] = string.Empty;
            try
            {
                var creditNoteId = creditNoteRequest.CreditNoteID;
                var response = await _httpClient.PostAsync($"CreditNote/deleteCreditNote/{creditNoteId}", null);
                response.EnsureSuccessStatusCode();
                TempData["mssgPop"] = "CreditNote Deleted Successfully";
                return RedirectToAction("Index", "CreditNotes");
            }
            catch (HttpRequestException ex)
            {
                TempData["mssgPop"] = $"HTTP request error: {ex.Message}";
                return RedirectToAction("Index", "CreditNotes");
            }
            catch (Exception ex)
            {
                TempData["mssgPop"] = $"An error occurred: {ex.Message}";
                return RedirectToAction("Index", "CreditNotes");

            }

        }
    }
}
