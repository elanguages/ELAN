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
        public IActionResult Query([FromBody] SparqlQueryRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.SparqlQuery))
            {
                return BadRequest(new { error = "sparqlQuery is required." });
            }

            try
            {
                var rawResults = _sparqlRepository.ExecuteQuery(request.SparqlQuery);

                var formattedResults = rawResults.Results.Select(result =>
                {
                    return result.ToDictionary(binding => binding.Key, binding => binding.Value.ToString());
                }).ToList();

                return Ok(formattedResults);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }
}
