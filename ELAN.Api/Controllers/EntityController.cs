using Microsoft.AspNetCore.Mvc;
using ELAN.Api.Services;
using ELAN.Api.Models;

namespace ELAN.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EntityController : ControllerBase
    {
        private readonly WikidataService _wikidataService;

        public EntityController(WikidataService wikidataService)
        {
            _wikidataService = wikidataService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetEntityDetails(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return BadRequest(new { error = "Entity ID is required." });
            }

            try
            {
                var description = await _wikidataService.GetEntityDescription(id);
                var statements = await _wikidataService.GetEntityStatements(id);

                if (description == null)
                {
                    return NotFound(new { error = "Entity not found." });
                }

                var response = new EntityDetails
                {
                    Description = description,
                    Statements = statements
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }
}
