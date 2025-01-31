using Microsoft.AspNetCore.Mvc;
using ELAN.Api.Services;

namespace ELAN.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EsolangController : ControllerBase
    {
        private readonly EsolangService _esolangService;

        public EsolangController(EsolangService esolangService)
        {
            _esolangService = esolangService;
        }

        [HttpGet("esolang-entities")]
        public async Task<IActionResult> GetLanguagesEntities()
        {
            try
            {
                var entities = await _esolangService.GetLanguagesEntities();
                return Ok(entities);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpGet("filters")]
        public async Task<IActionResult> GetEsolangFilters()
        {
            try
            {
                var filters = await _esolangService.GetEsolangFilters();
                return Ok(filters);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpPost("filtered-esolang-entities")]
        public async Task<IActionResult> GetFilteredLanguagesEntities([FromBody] Dictionary<string, object> filters)
        {
            try
            {
                var filteredEntities = await _esolangService.GetFilteredLanguagesEntities(filters);
                return Ok(filteredEntities);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }
}
