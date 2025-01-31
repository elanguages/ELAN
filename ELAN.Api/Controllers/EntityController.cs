using Microsoft.AspNetCore.Mvc;
using ELAN.Api.Services;

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
                var entityDetails = await _wikidataService.GetEntityDetails(id);

                if (entityDetails?.Description == null)
                {
                    return NotFound(new { error = "Entity not found." });
                }

                return Ok(entityDetails);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }
}
