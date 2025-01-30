using ELAN.Api.Models;
using ELAN.Api.Queries;
using ELAN.Api.Repositories.Interfaces;

namespace ELAN.Api.Services
{
    public class EsolangService
    {
        private readonly List<string> _keysToExclude = new()
            {
                "image@en", "C64-Wiki ID@en", "Namuwiki ID@en", "icon@en", "Quora topic ID@en", "MathWorld ID@en",
                "X username@en", "Alexa rank@en", "NicoNicoPedia ID@en", "Commons category@en", "logo image@en",
                "File Format Wiki page ID@en", "Rosetta Code page ID@en", "BabelNet ID@en", "Facebook username@en",
                "subreddit@en", "Microsoft Academic ID@en", "Google Knowledge Graph ID@en", "Freebase ID@en",
                "Stack Exchange tag@en", "FOLDOC ID@en", "maintained by WikiProject@e", "uses@en", "has use@en",
                "GitHub topic@en", "country@en", "discoverer or inventor@en", "maintained by WikiProject@en",
                "author name string@en", "subclass of@en", "designed by@en", "Fandom article ID@en", "different from@en",
                "writing system@en", "writable file format@en", "readable file format@en", "described by source@en",
                "operating system@en", "language of work or name@en", "user manual URL@en", "named after@en",
                "copyright status@en"
            };

        private readonly WikidataService _wikidataService;
        private readonly ISparqlRepository _sparqlRepository;

        public EsolangService(WikidataService wikidataService, ISparqlRepository sparqlRepository)
        {
            _wikidataService = wikidataService;
            _sparqlRepository = sparqlRepository;
        }

        public async Task<List<FilteredEntityResponse>> GetLanguagesEntities()
        {
            var languagesResults = await _sparqlRepository.ExecuteQuery(SparqlQueries.GetEsotericLanguages());
            var languages = languagesResults.Results
                .Select(result => result["programminglanguage"]?.ToString()?.Split('/').Last())
                .Where(id => !string.IsNullOrEmpty(id))
                .ToList();

            var filteredEntities = new List<FilteredEntityResponse>();

            foreach (var languageId in languages)
            {
                if (languageId == null) continue;

                // Fetch full entity details
                var entityDetails = await _wikidataService.GetEntityDetails(languageId);
                if (entityDetails == null) continue;

                // Filter statements based on excluded keys
                var filteredStatements = entityDetails.Statements?
                    .Where(statement => statement.PropertyLabel != null && !_keysToExclude.Contains(statement.PropertyLabel))
                    .ToList();

                // Add the entity to the response list
                filteredEntities.Add(new FilteredEntityResponse
                {
                    EntityId = languageId,
                    Description = entityDetails.Description,
                    Statements = filteredStatements ?? []
                });
            }

            return filteredEntities;
        }
    }

    public class FilteredEntityResponse
    {
        public string EntityId { get; set; } = string.Empty;
        public EntityDescription? Description { get; set; }
        public List<EntityStatement> Statements { get; set; } = [];
    }
}
