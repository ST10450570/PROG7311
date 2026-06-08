using GLMS.Web.ApiServices;
using GLMS.Web.ViewModels;
using GLMS.Web.ViewModels.ServiceRequests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GLMS.Web.Controllers
{
    [Authorize]
    public class ServiceRequestsController : Controller
    {
        private readonly ServiceRequestApiService _srService;
        private readonly ContractApiService _contractService;
        private readonly ClientApiService _clientService;

        public ServiceRequestsController(
            ServiceRequestApiService srService,
            ContractApiService contractService,
            ClientApiService clientService)
        {
            _srService = srService;
            _contractService = contractService;
            _clientService = clientService;
        }

        public async Task<IActionResult> Index()
        {
            var requests = await _srService.GetAllAsync();
            return View(requests);
        }

        public async Task<IActionResult> Details(int id)
        {
            var request = await _srService.GetByIdAsync(id);
            if (request == null) return NotFound();
            return View(request);
        }

        public async Task<IActionResult> Create()
        {
            await PopulateActiveContractsDropDown();
            var rate = await _srService.GetLiveExchangeRateAsync();
            return View(new CreateServiceRequestViewModel { ExchangeRate = rate });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateServiceRequestViewModel model)
        {
            if (ModelState.IsValid)
            {
                var success = await _srService.CreateAsync(model);
                if (success) return RedirectToAction(nameof(Index));
                ModelState.AddModelError("",
                    "Failed to create service request. Check if contract is active.");
            }

            await PopulateActiveContractsDropDown();
            return View(model);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var request = await _srService.GetByIdAsync(id);
            if (request == null) return NotFound();
            return View(request);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _srService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }

        private async Task PopulateActiveContractsDropDown()
        {
            var contracts = await _contractService.GetContractsAsync(ContractStatus.Active);
            var clients = await _clientService.GetAllAsync();
            var clientDict = clients.ToDictionary(c => c.Id, c => c.Name);

            var list = contracts.Select(c => new
            {
                c.Id,
                DisplayName = $"Contract #{c.Id} - " +
                    (clientDict.ContainsKey(c.ClientId) ? clientDict[c.ClientId] : "Unknown")
            });

            ViewBag.ActiveContracts = new SelectList(list, "Id", "DisplayName");
        }
    }
}