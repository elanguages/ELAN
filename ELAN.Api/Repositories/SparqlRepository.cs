using System.Text.RegularExpressions;
using ELAN.Api.Repositories.Interfaces;
using VDS.RDF.Parsing;
using VDS.RDF.Query;

namespace ELAN.Api.Repositories
{
    public class SparqlRepository : ISparqlRepository
    {
        private const int PrefixLines = 35;
        private const string SparqlEndpointUrl = "https://query.wikidata.org/sparql";
        private readonly SparqlQueryClient _sparqlQueryClient;

        public SparqlRepository()
        {
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("ELAN-SPARQL-Client/1.0 (http://example.org; contact@example.org)");

            _sparqlQueryClient = new SparqlQueryClient(httpClient, new Uri(SparqlEndpointUrl));
        }

        public async Task<SparqlResultSet> ExecuteQuery(string query)
        {
            return await _sparqlQueryClient.QueryWithResultSetAsync(query);
        }

        public string ValidateQuery(string query)
        {
            try
            {
                var prefixedQuery = PrependPrefixes(query);

                var parser = new SparqlQueryParser();
                parser.ParseFromString(prefixedQuery);

                return "Valid";
            }
            catch (Exception ex)
            {
                var adjustedError = AdjustErrorLineNumbers(ex.Message);

                return adjustedError;
            }
        }

        private static string PrependPrefixes(string query)
        {
            // Define commonly used prefixes
            var prefixes = @"
                PREFIX bd: <http://www.bigdata.com/rdf#>
                PREFIX cc: <http://creativecommons.org/ns#>
                PREFIX dct: <http://purl.org/dc/terms/>
                PREFIX geo: <http://www.opengis.net/ont/geosparql#>
                PREFIX ontolex: <http://www.w3.org/ns/lemon/ontolex#>
                PREFIX owl: <http://www.w3.org/2002/07/owl#>
                PREFIX p: <http://www.wikidata.org/prop/>
                PREFIX pq: <http://www.wikidata.org/prop/qualifier/>
                PREFIX pqn: <http://www.wikidata.org/prop/qualifier/value-normalized/>
                PREFIX pqv: <http://www.wikidata.org/prop/qualifier/value/>
                PREFIX pr: <http://www.wikidata.org/prop/reference/>
                PREFIX prn: <http://www.wikidata.org/prop/reference/value-normalized/>
                PREFIX prov: <http://www.w3.org/ns/prov#>
                PREFIX prv: <http://www.wikidata.org/prop/reference/value/>
                PREFIX ps: <http://www.wikidata.org/prop/statement/>
                PREFIX psn: <http://www.wikidata.org/prop/statement/value-normalized/>
                PREFIX psv: <http://www.wikidata.org/prop/statement/value/>
                PREFIX rdf: <http://www.w3.org/1999/02/22-rdf-syntax-ns#>
                PREFIX rdfs: <http://www.w3.org/2000/01/rdf-schema#>
                PREFIX schema: <http://schema.org/>
                PREFIX skos: <http://www.w3.org/2004/02/skos/core#>
                PREFIX wd: <http://www.wikidata.org/entity/>
                PREFIX wdata: <http://www.wikidata.org/wiki/Special:EntityData/>
                PREFIX wdno: <http://www.wikidata.org/prop/novalue/>
                PREFIX wdref: <http://www.wikidata.org/reference/>
                PREFIX wds: <http://www.wikidata.org/entity/statement/>
                PREFIX wdt: <http://www.wikidata.org/prop/direct/>
                PREFIX wdtn: <http://www.wikidata.org/prop/direct-normalized/>
                PREFIX wdv: <http://www.wikidata.org/value/>
                PREFIX wikibase: <http://wikiba.se/ontology#>
                PREFIX xsd: <http://www.w3.org/2001/XMLSchema#>";

            // Combine prefixes with the raw query
            return $"{prefixes}\n{query}";
        }

        private static string AdjustErrorLineNumbers(string errorMessage)
        {
            // Regex to find line numbers in the error message
            var regex = new Regex(@"Line (?<line>\d+)");
            return regex.Replace(errorMessage, match =>
            {
                // Parse the line number and subtract the number of prefix lines
                if (int.TryParse(match.Groups["line"].Value, out var originalLine))
                {
                    var adjustedLine = originalLine - PrefixLines;
                    return $"Line {Math.Max(adjustedLine, 1)}"; // Ensure line numbers are at least 1
                }
                return match.Value; // Return the original match if parsing fails
            });
        }
    }
}
