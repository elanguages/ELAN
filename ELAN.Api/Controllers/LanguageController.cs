using Microsoft.AspNetCore.Mvc;
using ELAN.Api.Repositories;
using System.Linq;
using System.Threading.Tasks;

namespace ELAN.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LanguagesController : ControllerBase
    {
        private readonly OntologyRepository _ontologyRepository;
        private readonly DBpediaRepository _dbpediaRepository;
        private readonly WikidataRepository _wikidataRepository;

        public LanguagesController(
            OntologyRepository ontologyRepository,
            DBpediaRepository dbpediaRepository,
            WikidataRepository wikidataRepository)
        {
            _ontologyRepository = ontologyRepository;
            _dbpediaRepository = dbpediaRepository;
            _wikidataRepository = wikidataRepository;
        }

        // GET: /api/languages
        [HttpGet]
        public async Task<IActionResult> GetAllLanguages()
        {
            var ontologyLanguages = _ontologyRepository.GetAllLanguages();
            var dbpediaLanguages = await _dbpediaRepository.FetchProgrammingLanguagesAsync();

            var combined = ontologyLanguages
                .Concat(dbpediaLanguages)
                .Distinct()
                .Select(name => new { Name = name })
                .ToList();

            return Ok(combined);
        }

        [HttpGet("{name}")]
        public async Task<IActionResult> GetLanguageDetails(string name)
        {
            // Normalize name by removing any @en suffix
            var normalizedName = name.Contains("@") ? name.Substring(0, name.IndexOf("@")) : name;

            var ontologyDetails = _ontologyRepository.GetLanguageDetails(normalizedName);
            var dbpediaLanguages = await _dbpediaRepository.FetchProgrammingLanguagesAsync();

            // Match against normalized names
            var dbpediaDetails = dbpediaLanguages
                .Where(l => l.Equals(normalizedName, StringComparison.OrdinalIgnoreCase))
                .FirstOrDefault();

            return Ok(new
            {
                Ontology = ontologyDetails,
                DBpedia = dbpediaDetails
            });
        }

        // GET: /api/languages/{name}/tools
        [HttpGet("{name}/tools")]
        public async Task<IActionResult> GetLanguageTools(string name)
        {
            var ontologyTools = _ontologyRepository.GetToolsForLanguage(name);
            var wikidataTools = await _wikidataRepository.FetchProgrammingLanguagesWithToolsAsync();
            var toolsForLanguage = wikidataTools
                .Where(t => t.Language == name)
                .Select(t => t.Tool)
                .Distinct()
                .ToList();

            var combinedTools = ontologyTools.Concat(toolsForLanguage).Distinct().ToList();

            return Ok(combinedTools);
        }

        // GET: /api/languages/{name}/related
        [HttpGet("{name}/related")]
        public IActionResult GetRelatedLanguages(string name)
        {
            var relatedLanguages = _ontologyRepository.GetRelatedLanguages(name);
            return Ok(relatedLanguages);
        }
    }
}
