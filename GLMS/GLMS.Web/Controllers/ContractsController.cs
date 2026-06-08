using GLMS.Web.Data;
using GLMS.Web.Factories;
using GLMS.Web.Models;
using GLMS.Web.Observers;
using GLMS.Web.Services;
using GLMS.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace GLMS.Web.Controllers
{
    //The base of the controllers in terms of how to go about the design was Ai assisted but I though of and code the logic myself.
    public class ContractsController : Controller
    {
        private readonly AppDbContext _db;
        private readonly FileService _fileService;
        private readonly ContractFactoryResolver _factoryResolver;
        private readonly ContractSubject _subject;

        public ContractsController(
            AppDbContext db,
            FileService fileService,
            ContractFactoryResolver factoryResolver,
            ContractSubject subject)
        {
            _db = db;
            _fileService = fileService;
            _factoryResolver = factoryResolver;
            _subject = subject;
        }

        public async Task<IActionResult> Index(ContractFilterViewModel filter)
        {
            var query = _db.Contracts.Include(c => c.Client).AsQueryable();

            if (filter.StartDateFrom.HasValue)
                query = query.Where(c => c.StartDate >= filter.StartDateFrom.Value);

            if (filter.StartDateTo.HasValue)
                query = query.Where(c => c.StartDate <= filter.StartDateTo.Value);

            if (filter.Status.HasValue)
                query = query.Where(c => c.Status == filter.Status.Value);

            filter.Results = await query.OrderByDescending(c => c.Id).ToListAsync();
            return View(filter);
        }

        public async Task<IActionResult> Details(int id)
        {
            var contract = await _db.Contracts
                .Include(c => c.Client)
                .Include(c => c.ServiceRequests)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (contract == null) return NotFound();
            return View(contract);
        }

        public IActionResult Create()
        {
            ViewBag.Clients = new SelectList(_db.Clients, "Id", "Name");
            return View(new CreateContractViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateContractViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Clients = new SelectList(_db.Clients, "Id", "Name");
                return View(vm);
            }

            if (vm.SignedAgreement != null && !FileService.IsValidPdf(vm.SignedAgreement))
            {
                ModelState.AddModelError("SignedAgreement", "Only PDF files are allowed.");
                ViewBag.Clients = new SelectList(_db.Clients, "Id", "Name");
                return View(vm);
            }

            var factory = _factoryResolver.Resolve(vm.ServiceLevel);
            var contract = factory.CreateContract(vm.ClientId, vm.StartDate, vm.EndDate);

            if (vm.SignedAgreement != null)
                contract.SignedAgreementPath = await _fileService.SaveAgreementAsync(vm.SignedAgreement);

            _db.Contracts.Add(contract);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var contract = await _db.Contracts.FindAsync(id);
            if (contract == null) return NotFound();
            ViewBag.Clients = new SelectList(_db.Clients, "Id", "Name", contract.ClientId);
            return View(contract);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Contract contract, IFormFile? signedAgreement)
        {
            if (id != contract.Id) return BadRequest();
            if (!ModelState.IsValid)
            {
                ViewBag.Clients = new SelectList(_db.Clients, "Id", "Name", contract.ClientId);
                return View(contract);
            }

            var existing = await _db.Contracts.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id);
            if (existing == null) return NotFound();

            if (existing.Status != contract.Status)
                _subject.NotifyAll(contract.Id, contract.Status.ToString());

            if (signedAgreement != null)
            {
                if (!FileService.IsValidPdf(signedAgreement))
                {
                    ModelState.AddModelError("", "Only PDF files are allowed.");
                    ViewBag.Clients = new SelectList(_db.Clients, "Id", "Name", contract.ClientId);
                    return View(contract);
                }
                contract.SignedAgreementPath = await _fileService.SaveAgreementAsync(signedAgreement);
            }
            else
            {
                contract.SignedAgreementPath = existing.SignedAgreementPath;
            }

            _db.Contracts.Update(contract);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var contract = await _db.Contracts.Include(c => c.Client).FirstOrDefaultAsync(c => c.Id == id);
            if (contract == null) return NotFound();
            return View(contract);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var contract = await _db.Contracts.FindAsync(id);
            if (contract != null) _db.Contracts.Remove(contract);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Download(int id)
        {
            var contract = await _db.Contracts.FindAsync(id);
            if (contract == null || string.IsNullOrEmpty(contract.SignedAgreementPath))
                return NotFound();

            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", contract.SignedAgreementPath);
            if (!System.IO.File.Exists(path)) return NotFound();

            return PhysicalFile(path, "application/pdf", contract.SignedAgreementPath);
        }
    }
}
