using ELAN.Api.Repositories.Interfaces;
using VDS.RDF.Query;

namespace ELAN.Api.Repositories
{
    public class SparqlRepository : ISparqlRepository
    {
        private const string SparqlEndpointUrl = "https://dbpedia.org/sparql";
        private readonly SparqlQueryClient _sparqlQueryClient;

        public SparqlRepository()
        {
            var httpClient = new HttpClient();
            _sparqlQueryClient = new SparqlQueryClient(httpClient, new Uri(SparqlEndpointUrl));
        }

        public SparqlResultSet ExecuteQuery(string query)
        {
            try
            {
                return _sparqlQueryClient.QueryWithResultSetAsync(query).Result;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error executing SPARQL query.", ex);
            }
        }
    }
}
