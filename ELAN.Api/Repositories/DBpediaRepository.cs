using VDS.RDF.Query;

namespace ELAN.Api.Repositories
{
    public class DBpediaRepository
    {
        private const string SparqlEndpoint = "https://dbpedia.org/sparql";
        private readonly HttpClient _httpClient;

        public DBpediaRepository(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<string>> FetchProgrammingLanguagesAsync()
        {
            var endpoint = new SparqlQueryClient(_httpClient, new Uri(SparqlEndpoint));
            var query = @"
                    SELECT DISTINCT ?language ?label WHERE {
                      ?language a dbo:ProgrammingLanguage .
                      ?language rdfs:label ?label .
                      FILTER (lang(?label) = 'en' && regex(?label, 'programming language', 'i'))
                    }
                    LIMIT 50";

            var results = await endpoint.QueryWithResultSetAsync(query);
            var languages = new List<string>();

            foreach (var result in results)
            {
                var label = result["label"].ToString();

                // Remove the language tag (e.g., "@en") and filter unwanted entries
                if (label.Contains('@'))
                {
                    label = label.Substring(0, label.IndexOf("@"));
                }

                if (!label.Contains("reference") && !label.Contains("specification"))
                {
                    languages.Add(label);
                }
            }

            return languages;
        }


        public class ProgrammingLanguage
        {
            public string Uri { get; set; }
            public string Name { get; set; }
            public string Paradigm { get; set; }
        }
    }
}