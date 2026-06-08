using GLMS.Web.ApiServices;
using GLMS.Web.ViewModels.Clients;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GLMS.Web.Controllers
{
    [Authorize]
    public class ClientsController : Controller
    {
        private readonly ClientApiService _clientService;

        public ClientsController(ClientApiService clientService)
        {
            _clientService = clientService;
        }

        public async Task<IActionResult> Index()
        {
            var clients = await _clientService.GetAllAsync();
            return View(clients);
        }

        public async Task<IActionResult> Details(int id)
        {
            var client = await _clientService.GetByIdAsync(id);
            if (client == null) return NotFound();
            return View(client);
        }

        public IActionResult Create()
        {
            return View(new CreateClientViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateClientViewModel model)
        {
            if (ModelState.IsValid)
            {
                var success = await _clientService.CreateAsync(model);
                if (success) return RedirectToAction(nameof(Index));
                ModelState.AddModelError("", "Failed to create client.");
            }
            return View(model);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var client = await _clientService.GetByIdAsync(id);
            if (client == null) return NotFound();

            var model = new CreateClientViewModel
            {
                Name = client.Name,
                ContactEmail = client.ContactEmail,
                ContactPhone = client.ContactPhone,
                Region = client.Region
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CreateClientViewModel model)
        {
            if (ModelState.IsValid)
            {
                var success = await _clientService.UpdateAsync(id, model);
                if (success) return RedirectToAction(nameof(Index));
                ModelState.AddModelError("", "Failed to update client.");
            }
            return View(model);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var client = await _clientService.GetByIdAsync(id);
            if (client == null) return NotFound();
            return View(client);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _clientService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}