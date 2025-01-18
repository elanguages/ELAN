using VDS.RDF;
using VDS.RDF.Parsing;
using VDS.RDF.Query;

namespace ELAN.Api.Repositories
{
    public class OntologyRepository
    {
        private readonly IGraph _ontologyGraph;

        public OntologyRepository(string ontologyPath)
        {
            _ontologyGraph = new Graph();
            _ontologyGraph.LoadFromFile(ontologyPath);
        }

        public SparqlResultSet ExecuteQuery(string query)
        {
            try
            {
                var store = new TripleStore();
                store.Add(_ontologyGraph);
                var processor = new LeviathanQueryProcessor(store);
                var parser = new SparqlQueryParser();
                var parsedQuery = parser.ParseFromString(query);

                if (processor.ProcessQuery(parsedQuery) is not SparqlResultSet result)
                {
                    throw new ApplicationException("Query did not return a SparqlResultSet.");
                }

                return result;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error executing SPARQL query on ontology.", ex);
            }
        }

        public List<string> GetAllLanguages()
        {
            const string query = @"
                SELECT ?label WHERE {
                    ?language a <http://example.org/esolang#EsotericLanguage> .
                    ?language <http://www.w3.org/2000/01/rdf-schema#label> ?label .
                }";

            var results = ExecuteQuery(query);
            var languages = new List<string>();

            foreach (var result in results)
            {
                languages.Add(result["label"].ToString());
            }

            return languages;
        }

        public List<KeyValuePair<string, string>> GetLanguageDetails(string name)
        {
            var query = $@"
                SELECT ?property ?value WHERE {{
                    ?language a <http://example.org/esolang#EsotericLanguage> ;
                        <http://www.w3.org/2000/01/rdf-schema#label> '{name}'@en ;
                        ?property ?value .
                }}";

            var results = ExecuteQuery(query);

            return results.Results.Select(result =>
                new KeyValuePair<string, string>(
                    result["property"].ToString(),
                    result["value"].ToString()
                )).ToList();
        }

        public List<string> GetToolsForLanguage(string name)
        {
            var query = $@"
                SELECT ?tool WHERE {{
                    ?language a <http://example.org/esolang#EsotericLanguage> ;
                        <http://www.w3.org/2000/01/rdf-schema#label> '{name}'@en ;
                        <http://example.org/esolang#hasTool> ?tool .
                }}";

            var results = ExecuteQuery(query);

            return results.Results.Select(result => result["tool"].ToString()).ToList();
        }

        public List<string> GetRelatedLanguages(string name)
        {
            // Normalize name by removing any @en suffix
            var normalizedName = name.Contains("@") ? name.Substring(0, name.IndexOf("@")) : name;

            var query = $@"
                SELECT ?related WHERE {{
                    ?language a <http://example.org/esolang#EsotericLanguage> ;
                        <http://www.w3.org/2000/01/rdf-schema#label> '{normalizedName}'@en ;
                        <http://example.org/esolang#relatedLanguage> ?related .
                }}";

            var results = ExecuteQuery(query);

            return results.Results.Select(result =>
                result["related"].ToString().Replace("http://example.org/esolang#", ""))
                .ToList();
        }

    }
}
