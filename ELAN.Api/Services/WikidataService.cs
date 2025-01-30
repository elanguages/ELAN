using System.Text.RegularExpressions;
using ELAN.Api.Models;
using ELAN.Api.Queries;
using ELAN.Api.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;

namespace ELAN.Api.Services
{
    public class WikidataService
    {
        private readonly ISparqlRepository _sparqlRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public WikidataService(ISparqlRepository sparqlRepository, IHttpContextAccessor httpContextAccessor)
        {
            _sparqlRepository = sparqlRepository;
            _httpContextAccessor = httpContextAccessor;
        }

        private string GetBaseUrl()
        {
            var origin = _httpContextAccessor.HttpContext?.Request.Headers.Origin.FirstOrDefault() ??
                         $"{_httpContextAccessor.HttpContext?.Request.Scheme}://{_httpContextAccessor.HttpContext?.Request.Host}";
            return $"{origin}/sparql-entity/";
        }

        public async Task<EntityDescription?> GetEntityDescription(string id)
        {
            var query = SparqlQueries.GetEntityDescription(id);
            var results = await _sparqlRepository.ExecuteQuery(query);

            var firstResult = results.Results.FirstOrDefault();
            if (firstResult == null) return null;

            return new EntityDescription
            {
                Label = firstResult.TryGetValue("propertyLabel", out var label) ? label?.ToString() : null,
                Description = firstResult.TryGetValue("propertyDescription", out var description) ? description?.ToString() : null
            };
        }

        public async Task<List<EntityStatement>?> GetEntityStatements(string id)
        {
            var baseUrl = GetBaseUrl();
            var query = SparqlQueries.GetEntityStatements(id);
            var results = await _sparqlRepository.ExecuteQuery(query);

            var groupedStatements = results.Results
                .Where(result => result.HasValue("propertyLabel"))
                .GroupBy(
                    result => result["propertyLabel"].ToString(),
                    result => new StatementValue
                    {
                        Value = ModifyWikidataLinks(result["value"]?.ToString(), baseUrl),
                        ValueLabel = result["valueLabel"]?.ToString(),
                        ValueDescription = result["valueDescription"]?.ToString()
                    }
                )
                .Select(group =>
                {
                    var matchingResult = results.Results
                        .FirstOrDefault(r => r.HasValue("propertyLabel") && r["propertyLabel"].ToString() == group.Key);

                    return new EntityStatement
                    {
                        PropertyLabel = group.Key,
                        PropertyDescription = matchingResult != null && matchingResult.TryGetValue("propertyDescription", out var description)
                            ? description?.ToString()
                            : null,
                        PropertyLink = matchingResult != null && matchingResult.TryGetValue("property", out var property)
                            ? ModifyWikidataLinks(property?.ToString(), baseUrl)
                            : null,
                        Values = group.Distinct().ToList()
                    };
                })
                .ToList();

            return groupedStatements;
        }

        public async Task<EntityDetails?> GetEntityDetails(string id)
        {
            var description = await GetEntityDescription(id);
            var statements = await GetEntityStatements(id);

            return new EntityDetails
            {
                Description = description,
                Statements = statements
            };
        }


        private static string ModifyWikidataLinks(string? input, string baseUrl)
        {
            if (string.IsNullOrEmpty(input)) return input;

            return Regex.Replace(
                input,
                @"https?:\/\/www\.wikidata\.org\/(?:entity|wiki)\/(Q\d+)",
                match => $"{baseUrl}{match.Groups[1].Value}"
            );
        }
    }
}
