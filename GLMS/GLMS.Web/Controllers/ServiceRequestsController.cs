using GLMS.Web.Data;
using GLMS.Web.Models;
using GLMS.Web.Services;
using GLMS.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace GLMS.Web.Controllers
{
    public class ServiceRequestsController : Controller
    {
        private readonly AppDbContext _db;
        private readonly ExchangeRateService _exchangeRateService;

        public ServiceRequestsController(AppDbContext db, ExchangeRateService exchangeRateService)
        {
            _db = db;
            _exchangeRateService = exchangeRateService;
        }

        public async Task<IActionResult> Index()
        {
            var requests = await _db.ServiceRequests
                .Include(sr => sr.Contract)
                    .ThenInclude(c => c!.Client)
                .OrderByDescending(sr => sr.CreatedOn)
                .ToListAsync();
            return View(requests);
        }

        public async Task<IActionResult> Create(int? contractId)
        {
            var rate = await _exchangeRateService.GetUsdToZarRateAsync();
            ViewBag.ExchangeRate = rate;

            var activeContracts = await _db.Contracts
                .Include(c => c.Client)
                .Where(c => c.Status == ContractStatus.Active)
                .ToListAsync();

            ViewBag.Contracts = new SelectList(
                activeContracts.Select(c => new { c.Id, Display = $"#{c.Id} — {c.Client!.Name}" }),
                "Id", "Display", contractId);

            return View(new CreateServiceRequestViewModel
            {
                ContractId = contractId ?? 0,
                ExchangeRate = rate
            });
        }


        //I used Ai to debug and fix an issue regarding my code not using the api for live exchange rate when creating a request.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateServiceRequestViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                await RebuildCreateViewBag(vm);
                return View(vm);
            }

            
            var contract = await _db.Contracts.FindAsync(vm.ContractId.Value);
            if (contract == null)
            {
                ModelState.AddModelError("", "Contract not found.");
                await RebuildCreateViewBag(vm);
                return View(vm);
            }

            if (contract.Status == ContractStatus.Expired || contract.Status == ContractStatus.OnHold)
            {
                ModelState.AddModelError("", $"Cannot create a service request against a contract that is {contract.Status}.");
                await RebuildCreateViewBag(vm);
                return View(vm);
            }

            var currencyContext = new CurrencyContext(new UsdToZarStrategy());

            var sr = new ServiceRequest
            {
                ContractId = vm.ContractId.Value,
                Description = vm.Description,
                CostUsd = vm.CostUsd.Value,
                CostZar = currencyContext.Convert(vm.CostUsd.Value, vm.ExchangeRate),
                Status = ServiceRequestStatus.Pending,
                CreatedOn = DateTime.UtcNow
            };

            _db.ServiceRequests.Add(sr);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Details(int id)
        {
            var sr = await _db.ServiceRequests
                .Include(s => s.Contract)
                    .ThenInclude(c => c!.Client)
                .FirstOrDefaultAsync(s => s.Id == id);
            if (sr == null) return NotFound();
            return View(sr);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var sr = await _db.ServiceRequests
                .Include(s => s.Contract)
                .FirstOrDefaultAsync(s => s.Id == id);
            if (sr == null) return NotFound();
            return View(sr);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var sr = await _db.ServiceRequests.FindAsync(id);
            if (sr != null) _db.ServiceRequests.Remove(sr);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private async Task RebuildCreateViewBag(CreateServiceRequestViewModel vm)
        {
            vm.ExchangeRate = await _exchangeRateService.GetUsdToZarRateAsync();
            ViewBag.ExchangeRate = vm.ExchangeRate;

            var activeContracts = await _db.Contracts
                .Include(c => c.Client)
                .Where(c => c.Status == ContractStatus.Active)
                .ToListAsync();

            ViewBag.Contracts = new SelectList(
                activeContracts.Select(c => new { c.Id, Display = $"#{c.Id} — {c.Client!.Name}" }),
                "Id", "Display", vm.ContractId);
        }
    }
}
