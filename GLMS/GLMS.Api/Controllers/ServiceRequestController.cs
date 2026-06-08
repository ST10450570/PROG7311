using GLMS.Api.DTOs.ServiceRequests;
using GLMS.Api.Services.Interfaces;
using GLMS.Api.DTOs.ServiceRequests;
using GLMS.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GLMS.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ServiceRequestsController : ControllerBase
    {
        private readonly IServiceRequestService _serviceRequestService;

        public ServiceRequestsController(IServiceRequestService serviceRequestService)
        {
            _serviceRequestService = serviceRequestService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var requests = await _serviceRequestService.GetAllAsync();
            var dtos = requests.Select(sr => new ServiceRequestDto
            {
                Id = sr.Id,
                ContractId = sr.ContractId,
                Description = sr.Description,
                CostUsd = sr.CostUsd,
                CostZar = sr.CostZar,
                Status = sr.Status.ToString(),
                CreatedOn = sr.CreatedOn,
                ClientName = sr.Contract?.Client?.Name
            });
            return Ok(dtos);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var sr = await _serviceRequestService.GetByIdAsync(id);
            if (sr == null) return NotFound();

            var dto = new ServiceRequestDto
            {
                Id = sr.Id,
                ContractId = sr.ContractId,
                Description = sr.Description,
                CostUsd = sr.CostUsd,
                CostZar = sr.CostZar,
                Status = sr.Status.ToString(),
                CreatedOn = sr.CreatedOn,
                ClientName = sr.Contract?.Client?.Name
            };
            return Ok(dto);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateServiceRequestDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var request = await _serviceRequestService.CreateAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = request.Id }, request);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while creating the service request.", Details = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _serviceRequestService.DeleteAsync(id);
            return NoContent();
        }
    }
}