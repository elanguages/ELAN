using Microsoft.AspNetCore.Mvc;
using ELAN.Api.Repositories;

namespace ELAN.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OntologyController : ControllerBase
    {
        private readonly OntologyRepository _ontologyRepository;

        public OntologyController(OntologyRepository ontologyRepository)
        {
            _ontologyRepository = ontologyRepository;
        }

        // GET: /api/ontology/languages
        [HttpGet("languages")]
        public IActionResult GetAllLanguages()
        {
            var languages = _ontologyRepository.GetAllLanguages();
            return Ok(languages);
        }

        // GET: /api/ontology/languages/{name}
        [HttpGet("languages/{name}")]
        public IActionResult GetLanguageDetails(string name)
        {
            string query = $@"
                SELECT ?property ?value WHERE {{
                    ?language a <http://example.org/esolang#EsotericLanguage> ;
                        <http://www.w3.org/2000/01/rdf-schema#label> '{name}'@en ;
                        ?property ?value .
                }}";

            var results = _ontologyRepository.ExecuteQuery(query);

            var details = results.Results.Select(result => new
            {
                Property = result["property"].ToString(),
                Value = result["value"].ToString()
            }).ToList();

            return Ok(details);
        }

        // GET: /api/ontology/languages/{name}/tools
        [HttpGet("languages/{name}/tools")]
        public IActionResult GetLanguageTools(string name)
        {
            string query = $@"
                SELECT ?tool ?label WHERE {{
                    ?language a <http://example.org/esolang#EsotericLanguage> ;
                        <http://www.w3.org/2000/01/rdf-schema#label> '{name}'@en ;
                        <http://example.org/esolang#hasTool> ?tool .
                    ?tool <http://www.w3.org/2000/01/rdf-schema#label> ?label .
                }}";

            var results = _ontologyRepository.ExecuteQuery(query);

            var tools = results.Results.Select(result => new
            {
                ToolUri = result["tool"].ToString(),
                ToolLabel = result["label"].ToString()
            }).ToList();

            return Ok(tools);
        }
    }
}
