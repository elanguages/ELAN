using Microsoft.AspNetCore.Mvc;
using ELAN.Api.Repositories.Interfaces;
using System.Text.RegularExpressions;

namespace ELAN.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EntityController : ControllerBase
    {
        private readonly ISparqlRepository _sparqlRepository;

        public EntityController(ISparqlRepository sparqlRepository)
        {
            _sparqlRepository = sparqlRepository;
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
                var baseUrl = $"{Request.Scheme}://localhost:5173/sparql-entity/";

                var descriptionQuery = $@"
                    SELECT ?propertyLabel ?propertyDescription WHERE {{
                        wd:{id} rdfs:label ?propertyLabel;
                               schema:description ?propertyDescription.
                        FILTER(LANG(?propertyLabel) = 'en')
                        FILTER(LANG(?propertyDescription) = 'en')
                    }}
                ";

                var descriptionResults = await _sparqlRepository.ExecuteQuery(descriptionQuery);
                var description = descriptionResults.Results.FirstOrDefault()?.ToDictionary(
                    binding => binding.Key,
                    binding => ModifyWikidataLinks(binding.Value?.ToString(), baseUrl)
                );

                var statementsQuery = $@"
                   SELECT ?property ?propertyLabel ?propertyDescription ?value ?valueLabel ?valueDescription WHERE {{
                      wd:{id} ?property ?value.
  
                      OPTIONAL {{
                        ?property rdfs:label ?propertyLabel.
                        FILTER(LANG(?propertyLabel) = ""en"")
                      }}
                      OPTIONAL {{
                        ?property schema:description ?propertyDescription.
                        FILTER(LANG(?propertyDescription) = ""en"")
                      }}

                      OPTIONAL {{
                        ?value rdfs:label ?valueLabel.
                        FILTER(LANG(?valueLabel) = ""en"")
                      }}
                      OPTIONAL {{
                        ?value schema:description ?valueDescription.
                        FILTER(LANG(?valueDescription) = ""en"")
                      }}

                      BIND(IRI(REPLACE(STR(?property), ""http://www.wikidata.org/prop/direct/"", ""http://www.wikidata.org/entity/"")) AS ?propertyEntity)
                      OPTIONAL {{
                        ?propertyEntity rdfs:label ?propertyLabel.
                        FILTER(LANG(?propertyLabel) = ""en"")
                      }}
                      OPTIONAL {{
                        ?propertyEntity schema:description ?propertyDescription.
                        FILTER(LANG(?propertyDescription) = ""en"")
                      }}

                      FILTER(BOUND(?propertyLabel))
                    }}
                ";

                var statementsResults = await _sparqlRepository.ExecuteQuery(statementsQuery);

                // **Group statements by propertyLabel**
                var groupedStatements = statementsResults.Results
                    .Where(result => result.HasValue("propertyLabel"))
                    .GroupBy(
                        result => result["propertyLabel"].ToString(),
                        result => new
                        {
                            Value = ModifyWikidataLinks(result.HasValue("value") ? result["value"]?.ToString() : null, baseUrl),
                            ValueLabel = result.HasValue("valueLabel") ? result["valueLabel"]?.ToString() : null,
                            ValueDescription = result.HasValue("valueDescription") ? result["valueDescription"]?.ToString() : null
                        }
                    )
                    .ToDictionary(
                        group => group.Key,
                        group => new
                        {
                            PropertyDescription = statementsResults.Results
                                .FirstOrDefault(r => r.HasValue("propertyLabel") && r["propertyLabel"].ToString() == group.Key)?
                                .TryGetValue("propertyDescription", out var propertyDescription) == true ? propertyDescription.ToString() : null,
                            PropertyLink = statementsResults.Results
                                .FirstOrDefault(r => r.HasValue("propertyLabel") && r["propertyLabel"].ToString() == group.Key)?
                                .TryGetValue("property", out var property) == true ? property.ToString() : null,
                            Values = group.Distinct().ToList()
                        }
                    );

                var response = new
                {
                    Description = description,
                    Statements = groupedStatements
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        private static string ModifyWikidataLinks(string? input, string baseUrl)
        {
            if (string.IsNullOrEmpty(input)) return input;

            return Regex.Replace(
                input,
                @"https?:\/\/www\.wikidata\.org\/(?:entity|wiki)\/(Q\d+)",
                match => $"{baseUrl}{match.Groups[1].Value}"
            );
        }
    }
}