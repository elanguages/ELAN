using Elan.Api.Esolang.Models;
using Elan.Api.Esolang.Queries;
using Elan.Api.Esolang.Repositories.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;

namespace Elan.Api.Esolang.Services
{
    public class EsolangService
    {
        private readonly List<string> _keysToExclude =
        [
            "image@en", "C64-Wiki ID@en", "Namuwiki ID@en", "icon@en", "Quora topic ID@en", "MathWorld ID@en",
            "X username@en", "Alexa rank@en", "NicoNicoPedia ID@en", "Commons category@en", "logo image@en",
            "File Format Wiki page ID@en", "Rosetta Code page ID@en", "BabelNet ID@en", "Facebook username@en",
            "subreddit@en", "Microsoft Academic ID@en", "Google Knowledge Graph ID@en", "Freebase ID@en",
            "Stack Exchange tag@en", "FOLDOC ID@en", "maintained by WikiProject@e", "uses@en", "has use@en",
            "GitHub topic@en", "country@en", "discoverer or inventor@en", "maintained by WikiProject@en",
            "author name string@en", "subclass of@en", "designed by@en", "Fandom article ID@en", "different from@en",
            "writing system@en", "writable file format@en", "readable file format@en", "described by source@en",
            "operating system@en", "language of work or name@en", "user manual URL@en", "named after@en",
            "copyright status@en"
        ];

        private readonly List<string> _excludedEntityIds =
        [
            "Q1853192", "Q17326337", "Q43267126", "Q107539410"
        ];

        private readonly HashSet<string> _keysOnlyProperty =
        [
            "described at URL@en", "source code repository URL@en",
            "Homebrew formula name@en", "software version identifier@en", "official website@en", "Debian stable package@en"
        ];

        private readonly IWikidataService _wikidataService;
        private readonly ISparqlRepository _sparqlRepository;
        private readonly IMemoryCache _cache;
        private readonly ILogger<EsolangService> _logger;

        public EsolangService(IWikidataService wikidataService, ISparqlRepository sparqlRepository, IMemoryCache cache, ILogger<EsolangService> logger)
        {
            _wikidataService = wikidataService;
            _sparqlRepository = sparqlRepository;
            _cache = cache;
            _logger = logger;
        }

        public async Task InitializeCacheAsync()
        {
            try
            {
                _logger.LogInformation("Starting cache initialization...");

                await GetLanguagesEntities();
                await GetEsolangFilters();

                _logger.LogInformation("Cache initialization completed successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during cache initialization.");
            }
        }

        public async Task<List<EsolangEntityResponse>> GetLanguagesEntities()
        {
            const string cacheKey = "esolang_entities";

            // Try to get from cache
            if (_cache.TryGetValue(cacheKey, out var cachedEntitiesObj) && cachedEntitiesObj is List<EsolangEntityResponse> cachedEntities)
            {
                return cachedEntities;
            }

            var languagesResults = await _sparqlRepository.ExecuteQuery(SparqlQueries.GetEsotericLanguages());
            var languages = languagesResults.Results
                .Select(result => result["programminglanguage"]?.ToString()?.Split('/').Last())
                .Where(id => !string.IsNullOrEmpty(id))
                .ToList();

            var filteredEntities = new List<EsolangEntityResponse>();

            foreach (var languageId in languages)
            {
                if (languageId == null || _excludedEntityIds.Contains(languageId)) continue;

                var entityDetails = await _wikidataService.GetEntityDetails(languageId);
                if (entityDetails == null || entityDetails.Statements == null) continue;

                var filteredStatements = entityDetails.Statements
                    .Where(statement => !_keysToExclude.Contains(statement.Key))
                    .ToDictionary(statement => statement.Key, statement => statement.Value);

                filteredEntities.Add(new EsolangEntityResponse
                {
                    EntityId = languageId,
                    Description = entityDetails.Description,
                    Statements = filteredStatements
                });
            }

            _cache.Set(cacheKey, filteredEntities, TimeSpan.FromDays(1));

            return filteredEntities;
        }

        public async Task<Dictionary<string, object>> GetEsolangFilters()
        {
            const string cacheKey = "esolang_filters";

            if (_cache.TryGetValue(cacheKey, out Dictionary<string, object> cachedFilters))
            {
                return cachedFilters;
            }

            var entities = await GetLanguagesEntities();
            var filters = new Dictionary<string, object>();

            foreach (var entity in entities)
            {
                if (entity.Statements == null) continue;

                foreach (var statement in entity.Statements)
                {
                    var propertyLabel = statement.Key;

                    if (_keysOnlyProperty.Contains(propertyLabel))
                    {
                        filters[propertyLabel] = true;
                        continue;
                    }

                    if (!filters.ContainsKey(propertyLabel))
                    {
                        filters[propertyLabel] = new HashSet<string>();
                    }

                    var filterValues = (HashSet<string>)filters[propertyLabel];

                    foreach (var value in statement.Value.Values)
                    {
                        filterValues.Add(value.ValueLabel ?? value.Value);
                    }
                }
            }

            var formattedFilters = filters.ToDictionary(
                kvp => kvp.Key,
                kvp => kvp.Value is HashSet<string> set ? (object)set.ToList() : kvp.Value
            );

            _cache.Set(cacheKey, formattedFilters, TimeSpan.FromDays(1));

            return formattedFilters;
        }

        public async Task<List<EsolangEntityResponse>> GetFilteredLanguagesEntities(Dictionary<string, object> filters)
        {
            var entities = await GetLanguagesEntities();

            var filteredEntities = entities.Where(entity =>
            {
                if (entity.Statements == null) return false;

                // Check if the entity matches **all** filters
                foreach (var filter in filters)
                {
                    var filterKey = filter.Key;

                    if (_keysOnlyProperty.Contains(filterKey))
                    {
                        // Keys-only check: the entity must contain this key
                        if (!entity.Statements.ContainsKey(filterKey)) return false;
                    }
                    else
                    {
                        entity.Statements.TryGetValue(filterKey, out var statementDetails);

                        if (statementDetails == null) return false;

                        var filterValues = JsonConvert.DeserializeObject<List<string>>(filter.Value?.ToString() ?? string.Empty);
                        if (filterValues == null) return false;

                        var entityValues = statementDetails.Values
                         .Select(value => value.ValueLabel ?? value.Value ?? string.Empty)
                         .ToHashSet();

                        // Check if all filter values are present in entity values
                        if (!filterValues.All(filterVal => entityValues.Contains(filterVal))) return false;
                    }
                }

                return true; // Entity matches all filters
            }).ToList();

            return filteredEntities;
        }

        public async Task<List<EsolangEntityResponse>> GetRecommendLanguagesEntities(Dictionary<string, object> filters)
        {
            var entities = await GetLanguagesEntities();

            var exactMatches = new List<EsolangEntityResponse>();
            var partialMatches = new List<(EsolangEntityResponse entity, int matchCount)>();

            foreach (var entity in entities)
            {
                if (entity.Statements == null) continue;

                int totalFilters = filters.Count;
                int matchedFilters = 0;
                bool isExactMatch = true;

                foreach (var filter in filters)
                {
                    var filterKey = filter.Key;

                    if (_keysOnlyProperty.Contains(filterKey))
                    {
                        // Keys-only check: the entity must contain this key
                        if (!entity.Statements.ContainsKey(filterKey))
                        {
                            isExactMatch = false;
                            continue;
                        }
                    }
                    else
                    {
                        entity.Statements.TryGetValue(filterKey, out var statementDetails);

                        if (statementDetails == null)
                        {
                            isExactMatch = false;
                            continue;
                        }

                        var filterValues = JsonConvert.DeserializeObject<List<string>>(filter.Value?.ToString() ?? string.Empty);
                        if (filterValues == null) continue;

                        var entityValues = statementDetails.Values
                            .Select(value => value.ValueLabel ?? value.Value ?? string.Empty)
                            .ToHashSet();

                        // Check if all filter values are present in entity values
                        if (!filterValues.All(filterVal => entityValues.Contains(filterVal)))
                        {
                            isExactMatch = false;
                            continue;
                        }
                    }

                    matchedFilters++;
                }

                if (isExactMatch)
                {
                    exactMatches.Add(entity);
                }
                else if (matchedFilters > 0)
                {
                    partialMatches.Add((entity, matchedFilters));
                }
            }

            // Sort partial matches by the number of filters they match (descending order)
            var sortedPartialMatches = partialMatches
                .OrderByDescending(e => e.matchCount)
                .Select(e => e.entity)
                .ToList();

            // Combine exact matches with partial matches
            return exactMatches.Concat(sortedPartialMatches).ToList();

        }
        public class EsolangEntityResponse
        {
            public string EntityId { get; set; } = string.Empty;
            public Dictionary<string, string>? Description { get; set; }
            public Dictionary<string, StatementDetails>? Statements { get; set; }
        }
    }
}