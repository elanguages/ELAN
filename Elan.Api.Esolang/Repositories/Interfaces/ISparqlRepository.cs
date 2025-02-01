using VDS.RDF.Query;

namespace Elan.Api.Esolang.Repositories.Interfaces
{
    public interface ISparqlRepository
    {
        Task<SparqlResultSet> ExecuteQuery(string query);

        string ValidateQuery(string query);
    }
}
