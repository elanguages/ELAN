
using VDS.RDF.Query;

namespace ELAN.Api.Repositories.Interfaces
{
    public interface ISparqlRepository
    {
        SparqlResultSet ExecuteQuery(string query);
    }
}
