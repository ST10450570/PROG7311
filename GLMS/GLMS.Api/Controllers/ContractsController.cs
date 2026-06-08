using GLMS.Api.DTOs.Contracts;
using GLMS.Api.DTOs.ServiceRequests;
using GLMS.Api.Services.Interfaces;
using GLMS.Api.DTOs.Contracts;
using GLMS.Api.DTOs.ServiceRequests;
using GLMS.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GLMS.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ContractsController : ControllerBase
    {
        private readonly IContractService _contractService;
        private readonly IWebHostEnvironment _env;

        public ContractsController(IContractService contractService, IWebHostEnvironment env)
        {
            _contractService = contractService;
            _env = env;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] string? status, [FromQuery] DateTime? startDateFrom, [FromQuery] DateTime? startDateTo)
        {
            var contracts = await _contractService.GetAllAsync(status, startDateFrom, startDateTo);
            var dtos = contracts.Select(c => new ContractDto
            {
                Id = c.Id,
                ClientId = c.ClientId,
                StartDate = c.StartDate,
                EndDate = c.EndDate,
                Status = c.Status.ToString(),
                ServiceLevel = c.ServiceLevel.ToString(),
                SignedAgreementPath = c.SignedAgreementPath
            });
            return Ok(dtos);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var c = await _contractService.GetByIdAsync(id);
            if (c == null) return NotFound();

            var dto = new ContractDto
            {
                Id = c.Id,
                ClientId = c.ClientId,
                StartDate = c.StartDate,
                EndDate = c.EndDate,
                Status = c.Status.ToString(),
                ServiceLevel = c.ServiceLevel.ToString(),
                SignedAgreementPath = c.SignedAgreementPath,
                ServiceRequests = c.ServiceRequests.Select(sr => new ServiceRequestDto
                {
                    Id = sr.Id,
                    ContractId = sr.ContractId,
                    Description = sr.Description,
                    CostUsd = sr.CostUsd,
                    CostZar = sr.CostZar,
                    Status = sr.Status.ToString(),
                    CreatedOn = sr.CreatedOn
                }).ToList()
            };
            return Ok(dto);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateContractDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var contract = await _contractService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = contract.Id }, contract);
        }

        [HttpPatch("{id}/status")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateContractStatusDto dto)
        {
            try
            {
                await _contractService.UpdateStatusAsync(id, dto);
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
            await _contractService.DeleteAsync(id);
            return NoContent();
        }

        [HttpPost("{id}/upload-agreement")]
        public async Task<IActionResult> UploadAgreement(int id, IFormFile file)
        {
            if (file == null || file.Length == 0) return BadRequest("No file uploaded.");
            if (file.ContentType != "application/pdf" || !file.FileName.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
            {
                return BadRequest("Only PDF files are allowed.");
            }

            var contract = await _contractService.GetByIdAsync(id);
            if (contract == null) return NotFound("Contract not found.");

            var webRootPath = _env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            var uploadsFolder = Path.Combine(webRootPath, "uploads");
            Directory.CreateDirectory(uploadsFolder);

            var fileName = Guid.NewGuid().ToString() + ".pdf";
            var filePath = Path.Combine(uploadsFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            await _contractService.UpdatePathAsync(id, filePath);
            return Ok(new { Path = filePath });
        }

        [HttpGet("{id}/download-agreement")]
        public async Task<IActionResult> DownloadAgreement(int id)
        {
            var contract = await _contractService.GetByIdAsync(id);
            if (contract == null || string.IsNullOrEmpty(contract.SignedAgreementPath))
            {
                return NotFound("No agreement uploaded for this contract.");
            }

            if (!System.IO.File.Exists(contract.SignedAgreementPath))
            {
                return NotFound("Agreement file could not be found on server.");
            }

            return PhysicalFile(contract.SignedAgreementPath, "application/pdf", $"Agreement_{id}.pdf");
        }
    }
}