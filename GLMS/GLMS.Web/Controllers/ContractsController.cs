using GLMS.Web.ApiServices;
using GLMS.Web.Services;
using GLMS.Web.ViewModels;
using GLMS.Web.ViewModels.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GLMS.Web.Controllers
{
    [Authorize]
    public class ContractsController : Controller
    {
        private readonly ContractApiService _contractService;
        private readonly ClientApiService _clientService;
        private readonly FileService _fileService;

        public ContractsController(
            ContractApiService contractService,
            ClientApiService clientService,
            FileService fileService)
        {
            _contractService = contractService;
            _clientService = clientService;
            _fileService = fileService;
        }

        public async Task<IActionResult> Index(ContractFilterViewModel filter)
        {
            var contracts = await _contractService.GetContractsAsync(
                filter.Status, filter.StartDateFrom, filter.StartDateTo);

            var clients = await _clientService.GetAllAsync();
            var clientDict = clients.ToDictionary(c => c.Id, c => c.Name);

            foreach (var c in contracts)
            {
                if (clientDict.TryGetValue(c.ClientId, out string? name))
                    c.ClientName = name;
            }

            filter.Results = contracts;
            return View(filter);
        }

        public async Task<IActionResult> Details(int id)
        {
            var contract = await _contractService.GetByIdAsync(id);
            if (contract == null) return NotFound();

            var client = await _clientService.GetByIdAsync(contract.ClientId);
            if (client != null) contract.ClientName = client.Name;

            return View(contract);
        }

        public async Task<IActionResult> Create()
        {
            await PopulateClientsDropDown();
            return View(new CreateContractViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateContractViewModel model)
        {
            if (model.SignedAgreement != null && !_fileService.IsValidPdf(model.SignedAgreement))
                ModelState.AddModelError("SignedAgreement", "Only PDF files are allowed.");

            if (ModelState.IsValid)
            {
                var contract = await _contractService.CreateAsync(model);
                if (contract != null)
                {
                    if (model.SignedAgreement != null)
                        await _contractService.UploadAgreementAsync(contract.Id, model.SignedAgreement);
                    return RedirectToAction(nameof(Index));
                }
                ModelState.AddModelError("", "Failed to create contract.");
            }

            await PopulateClientsDropDown();
            return View(model);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var contract = await _contractService.GetByIdAsync(id);
            if (contract == null) return NotFound();

            var model = new UpdateContractStatusViewModel
            {
                Id = contract.Id,
                Status = Enum.Parse<ContractStatus>(contract.Status)
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UpdateContractStatusViewModel model)
        {
            if (id != model.Id) return BadRequest();

            if (ModelState.IsValid)
            {
                var success = await _contractService.UpdateStatusAsync(id, model.Status);
                if (success) return RedirectToAction(nameof(Index));
                ModelState.AddModelError("", "Failed to update contract status.");
            }
            return View(model);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var contract = await _contractService.GetByIdAsync(id);
            if (contract == null) return NotFound();
            return View(contract);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _contractService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Download(int id)
        {
            var stream = await _contractService.DownloadAgreementAsync(id);
            if (stream == null) return NotFound("Agreement file not found.");
            return File(stream, "application/pdf", $"Agreement_{id}.pdf");
        }

        private async Task PopulateClientsDropDown()
        {
            var clients = await _clientService.GetAllAsync();
            ViewBag.Clients = new SelectList(clients, "Id", "Name");
        }
    }
}