using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using VDS.RDF.Query;

namespace ELAN.Api.Repositories
{
    public class WikidataRepository
    {
        private const string SparqlEndpoint = "https://query.wikidata.org/sparql";
        private readonly HttpClient _httpClient;

        public WikidataRepository(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<ProgrammingLanguageTool>> FetchProgrammingLanguagesWithToolsAsync()
        {
            // Set User-Agent header to comply with Wikidata policies
            _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("ELAN/1.0 (http://example.org; contact@example.org)");

            var endpoint = new SparqlQueryClient(_httpClient, new Uri(SparqlEndpoint));
            var query = @"
                SELECT DISTINCT ?language ?languageLabel ?tool ?toolLabel WHERE {
                  ?language wdt:P31 wd:Q9143 .  # Instance of Programming Language
                  OPTIONAL { ?language wdt:P178 ?tool . }  # Tools/Creators
                  SERVICE wikibase:label { bd:serviceParam wikibase:language 'en'. }
                }
                LIMIT 50";

            var results = await endpoint.QueryWithResultSetAsync(query);
            var languagesWithTools = new List<ProgrammingLanguageTool>();

            foreach (var result in results)
            {
                // Safely extract and handle empty or null values
                var languageUri = result.HasValue("language") ? result["language"]?.ToString() : null;
                var languageLabel = result.HasValue("languageLabel") ? result["languageLabel"]?.ToString() : null;
                var toolUri = result.HasValue("tool") && !string.IsNullOrEmpty(result["tool"]?.ToString())
                    ? result["tool"]?.ToString()
                    : null;
                var toolLabel = result.HasValue("toolLabel") && !string.IsNullOrEmpty(result["toolLabel"]?.ToString())
                    ? result["toolLabel"]?.ToString()
                    : null;

                // Only add valid entries to the list
                if (!string.IsNullOrEmpty(languageUri) && !string.IsNullOrEmpty(languageLabel))
                {
                    languagesWithTools.Add(new ProgrammingLanguageTool
                    {
                        LanguageUri = languageUri,
                        Language = languageLabel,
                        ToolUri = toolUri,
                        Tool = toolLabel
                    });
                }
            }


            return languagesWithTools;
        }


    }

    public class ProgrammingLanguageTool
    {
        public string LanguageUri { get; set; }
        public string Language { get; set; }
        public string ToolUri { get; set; }
        public string Tool { get; set; }
    }
}
