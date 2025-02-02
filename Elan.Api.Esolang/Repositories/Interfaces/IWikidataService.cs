using Elan.Api.Esolang.Models;

namespace Elan.Api.Esolang.Repositories.Interfaces
{
    public interface IWikidataService
    {
        Task<EntityDetails?> GetEntityDetails(string entityId);
    }
}
