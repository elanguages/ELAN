
using VDS.RDF.Query;

namespace ELAN.Api.Repositories.Interfaces
{
    public interface ISparqlRepository
    {
        Task<SparqlResultSet> ExecuteQuery(string query);

        string ValidateQuery(string query);
    }
}
