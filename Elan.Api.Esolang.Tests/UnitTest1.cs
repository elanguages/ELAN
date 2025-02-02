using Elan.Api.Esolang.Models;
using Elan.Api.Esolang.Repositories.Interfaces;
using Elan.Api.Esolang.Services;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Newtonsoft.Json;
using VDS.RDF;
using VDS.RDF.Query;

namespace Elan.Api.Esolang.Tests
{
    public class FakeSparqlRepository : ISparqlRepository
    {
        public Task<SparqlResultSet> ExecuteQuery(string query)
        {
            // Simulate a query result with one entry.
            var resultSet = new SparqlResultSet();
            // Create a fake node that simulates a URL ending with Q12345.
            INode node = new UriNode(new Uri("http://wikidata.org/entity/Q12345"));
            var dict = new Dictionary<string, INode>
            {
                { "programminglanguage", node }
            };
            resultSet.Results.Add(new SparqlResult(dict));
            return Task.FromResult(resultSet);
        }

        public string ValidateQuery(string query)
        {
            return string.Empty;
        }
    }

    public class FakeWikidataService : IWikidataService
    {
        // Parameterless constructor.
        public FakeWikidataService()
        {
        }

        public Task<EntityDetails?> GetEntityDetails(string entityId)
        {
            // For testing, return a dummy entity with a description and one statement.
            var details = new EntityDetails
            {
                Description = new Dictionary<string, string> { { "en", "Test Language" } },
                Statements = new Dictionary<string, StatementDetails>
                {
                    {
                        "someProperty@en",
                        new StatementDetails
                        {
                            // Using StatementValue from your production models.
                            Values = new List<StatementValue>
                            {
                                new StatementValue
                                {
                                    Value = "Value1",
                                    ValueLabel = "Label1",
                                    ValueDescription = "Test Description"
                                }
                            }
                        }
                    }
                }
            };

            return Task.FromResult<EntityDetails?>(details);
        }
    }

    // Unit tests for EsolangService.
    public class EsolangServiceTests
    {
        [Fact]
        public async Task GetLanguagesEntities_Returns_Entities()
        {
            // Arrange
            var sparqlRepository = new FakeSparqlRepository();
            // Use the fake Wikidata service (parameterless).
            IWikidataService wikidataService = new FakeWikidataService();
            var memoryCache = new MemoryCache(new MemoryCacheOptions());
            ILogger<EsolangService> logger = NullLogger<EsolangService>.Instance;
            var service = new EsolangService(wikidataService, sparqlRepository, memoryCache, logger);

            // Act
            var entities = await service.GetLanguagesEntities();

            // Assert
            Assert.NotNull(entities);
            Assert.NotEmpty(entities);
            // The fake repository returns a URL ending with Q12345 so the entity ID should be "Q12345".
            Assert.Equal("Q12345", entities[0].EntityId);
        }

        [Fact]
        public async Task GetEsolangFilters_Returns_Filters()
        {
            // Arrange
            var sparqlRepository = new FakeSparqlRepository();
            IWikidataService wikidataService = new FakeWikidataService();
            var memoryCache = new MemoryCache(new MemoryCacheOptions());
            ILogger<EsolangService> logger = NullLogger<EsolangService>.Instance;
            var service = new EsolangService(wikidataService, sparqlRepository, memoryCache, logger);

            // Act
            var filters = await service.GetEsolangFilters();

            // Assert
            Assert.NotNull(filters);
            // Our fake entity provides one statement with key "someProperty@en", so the filters should contain that key.
            Assert.True(filters.ContainsKey("someProperty@en"));
        }

        [Fact]
        public async Task GetFilteredLanguagesEntities_Filters_Correctly()
        {
            // Arrange
            var sparqlRepository = new FakeSparqlRepository();
            IWikidataService wikidataService = new FakeWikidataService();
            var memoryCache = new MemoryCache(new MemoryCacheOptions());
            ILogger<EsolangService> logger = NullLogger<EsolangService>.Instance;
            var service = new EsolangService(wikidataService, sparqlRepository, memoryCache, logger);

            // Create a filter that expects "someProperty@en" to include "Label1".
            var filters = new Dictionary<string, object>
            {
                { "someProperty@en", JsonConvert.SerializeObject(new List<string> { "Label1" }) }
            };

            // Act
            var filteredEntities = await service.GetFilteredLanguagesEntities(filters);

            // Assert
            Assert.NotNull(filteredEntities);
            Assert.NotEmpty(filteredEntities);
        }

        [Fact]
        public async Task GetRecommendLanguagesEntities_Returns_Recommendations()
        {
            // Arrange
            var sparqlRepository = new FakeSparqlRepository();
            IWikidataService wikidataService = new FakeWikidataService();
            var memoryCache = new MemoryCache(new MemoryCacheOptions());
            ILogger<EsolangService> logger = NullLogger<EsolangService>.Instance;
            var service = new EsolangService(wikidataService, sparqlRepository, memoryCache, logger);

            // Use a filter that expects the property "someProperty@en" with value "Label1".
            var filters = new Dictionary<string, object>
            {
                { "someProperty@en", JsonConvert.SerializeObject(new List<string> { "Label1" }) }
            };

            // Act
            var recommendations = await service.GetRecommendLanguagesEntities(filters);

            // Assert
            Assert.NotNull(recommendations);
            // We expect at least one recommended entity.
            Assert.NotEmpty(recommendations);
        }
    }
}
