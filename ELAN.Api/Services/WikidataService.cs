using System.Text.RegularExpressions;
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
            var origin = _httpContextAccessor.HttpContext?.Request.Headers["Origin"].FirstOrDefault() ??
                         $"{_httpContextAccessor.HttpContext?.Request.Scheme}://{_httpContextAccessor.HttpContext?.Request.Host}";
            return $"{origin}/sparql-entity/";
        }

        public async Task<Dictionary<string, string>?> GetEntityDescription(string id)
        {
            var descriptionQuery = SparqlQueries.GetEntityDescription(id);
            var descriptionResults = await _sparqlRepository.ExecuteQuery(descriptionQuery);
            var description = descriptionResults.Results.FirstOrDefault()?.ToDictionary(
                binding => binding.Key,
                binding => binding.Value?.ToString() ?? string.Empty
            );

            return description;
        }

        public async Task<Dictionary<string, object>?> GetEntityStatements(string id)
        {
            var baseUrl = GetBaseUrl();
            var statementsQuery = SparqlQueries.GetEntityStatements(id);
            var statementsResults = await _sparqlRepository.ExecuteQuery(statementsQuery);

            var groupedStatements = statementsResults.Results
                .Where(result => result.HasValue("propertyLabel"))
                .GroupBy(
                    result => result["propertyLabel"].ToString(),
                    result => new
                    {
                        Value = ModifyWikidataLinks(result.HasValue("value") ? result["value"]?.ToString() : null, baseUrl),
                        ValueLabel = result.HasValue("valueLabel") ? result["valueLabel"]?.ToString() : null,
                        ValueDescription = result.HasValue("valueDescription") ? result["valueDescription"]?.ToString() : null
                    }
                )
                .ToDictionary(
                    group => group.Key,
                    group =>
                    {
                        var matchingResult = statementsResults.Results
                            .FirstOrDefault(r => r.HasValue("propertyLabel") && r["propertyLabel"].ToString() == group.Key);

                        return new
                        {
                            PropertyDescription = matchingResult != null && matchingResult.TryGetValue("propertyDescription", out var propertyDescription)
                                ? propertyDescription?.ToString()
                                : null,
                            PropertyLink = matchingResult != null && matchingResult.TryGetValue("property", out var property)
                                ? ModifyWikidataLinks(property?.ToString(), baseUrl)
                                : null,
                            Values = group.Distinct().ToList()
                        };
                    }
                );

            var response = new Dictionary<string, object>
            {
                { "Statements", groupedStatements }
            };

            return response;
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
