using System.Text.RegularExpressions;
using Elan.Api.Esolang.Models;
using Elan.Api.Esolang.Queries;
using Elan.Api.Esolang.Repositories.Interfaces;

namespace Elan.Api.Esolang.Services
{
    public class WikidataService : IWikidataService
    {
        private readonly ISparqlRepository _sparqlRepository;

        public WikidataService(ISparqlRepository sparqlRepository)
        {
            _sparqlRepository = sparqlRepository;
        }

        private string GetBaseUrl()
        {
            //var origin = _httpContextAccessor.HttpContext?.Request.Headers.Origin.FirstOrDefault() ??
            //             $"{_httpContextAccessor.HttpContext?.Request.Scheme}://{_httpContextAccessor.HttpContext?.Request.Host}";

            //commented because caching does not have http context
            return "https://elanguages.site/sparql-entity/";
        }

        public async Task<EntityDetails?> GetEntityDetails(string id)
        {
            var baseUrl = GetBaseUrl();

            // Fetch the description
            var descriptionQuery = SparqlQueries.GetEntityDescription(id);
            var descriptionResults = await _sparqlRepository.ExecuteQuery(descriptionQuery);

            var description = descriptionResults.Results.FirstOrDefault()?.ToDictionary(
                binding => binding.Key,
                binding => binding.Value?.ToString() ?? string.Empty
            );

            // Fetch the statements
            var statementsQuery = SparqlQueries.GetEntityStatements(id);
            var statementsResults = await _sparqlRepository.ExecuteQuery(statementsQuery);

            var groupedStatements = statementsResults.Results
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
                .ToDictionary(
                    group => group.Key,
                    group =>
                    {
                        var matchingResult = statementsResults.Results
                            .FirstOrDefault(r => r.HasValue("propertyLabel") && r["propertyLabel"].ToString() == group.Key);

                        return new StatementDetails
                        {
                            PropertyDescription = matchingResult?.TryGetValue("propertyDescription", out var description)
                                == true ? description?.ToString() : null,
                            PropertyLink = matchingResult?.TryGetValue("property", out var property)
                                == true ? ModifyWikidataLinks(property?.ToString(), baseUrl) : null,
                            Values = group.Distinct().ToList()
                        };
                    }
                );

            return new EntityDetails
            {
                Description = description,
                Statements = groupedStatements
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
