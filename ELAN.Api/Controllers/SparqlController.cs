using Microsoft.AspNetCore.Mvc;
using ELAN.Api.Repositories.Interfaces;
using ELAN.Api.Models;

namespace ELAN.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SparqlController : ControllerBase
    {
        private readonly ISparqlRepository _sparqlRepository;

        public SparqlController(ISparqlRepository sparqlRepository)
        {
            _sparqlRepository = sparqlRepository;
        }

        [HttpPost]
        public async Task<IActionResult> Query([FromBody] SparqlQueryRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.SparqlQuery))
            {
                return BadRequest(new { error = "sparqlQuery is required." });
            }

            try
            {
                var sanitizedQuery = request.SparqlQuery.Replace("\n", " ").Replace("\r", " ");

                var rawResults = await _sparqlRepository.ExecuteQuery(sanitizedQuery);

                var columns = rawResults.Variables; 
                var rows = rawResults.Results.Select(result =>
                {
                    return columns.ToDictionary(
                        column => column,
                        column => result.HasValue(column) ? result[column]?.ToString() : null
                    );
                }).ToList();

                return Ok(new
                {
                    columns,
                    rows
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }
}
