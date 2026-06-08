using GLMS.Api.DTOs.Clients;
using GLMS.Api.DTOs.Contracts;
using GLMS.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GLMS.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ClientsController : ControllerBase
    {
        private readonly IClientService _clientService;

        public ClientsController(IClientService clientService)
        {
            _clientService = clientService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var clients = await _clientService.GetAllAsync();
            var dtos = clients.Select(c => new ClientDto
            {
                Id = c.Id,
                Name = c.Name,
                ContactEmail = c.ContactEmail,
                ContactPhone = c.ContactPhone,
                Region = c.Region
            });
            return Ok(dtos);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var c = await _clientService.GetByIdAsync(id);
            if (c == null) return NotFound();

            var dto = new ClientDto
            {
                Id = c.Id,
                Name = c.Name,
                ContactEmail = c.ContactEmail,
                ContactPhone = c.ContactPhone,
                Region = c.Region,
                Contracts = c.Contracts.Select(co => new ContractDto
                {
                    Id = co.Id,
                    ClientId = co.ClientId,
                    StartDate = co.StartDate,
                    EndDate = co.EndDate,
                    Status = co.Status.ToString(),
                    ServiceLevel = co.ServiceLevel.ToString(),
                    SignedAgreementPath = co.SignedAgreementPath
                }).ToList()
            };
            return Ok(dto);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateClientDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var client = await _clientService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = client.Id }, new ClientDto
            {
                Id = client.Id,
                Name = client.Name,
                ContactEmail = client.ContactEmail,
                ContactPhone = client.ContactPhone,
                Region = client.Region
            });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] CreateClientDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            try
            {
                await _clientService.UpdateAsync(id, dto);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var existing = await _clientService.GetByIdAsync(id);
            if (existing == null) return NotFound();
            await _clientService.DeleteAsync(id);
            return NoContent();
        }
    }
}