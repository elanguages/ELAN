using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using ELAN.Api.Models;
using VDS.RDF.Parsing;
using VDS.RDF.Query;

namespace ELAN.Api.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class ValidateSparqlQueryAttribute : Attribute, IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            // Check if request body is provided
            if (!context.ActionArguments.TryGetValue("request", out var value) || value == null)
            {
                context.Result = new BadRequestObjectResult(new { error = "Request body is required." });
                return;
            }

            if (value is SparqlQueryRequest request)
            {
                var rawQuery = request.SparqlQuery;

                // Create a SparqlParameterizedString and add namespaces
                var queryString = new SparqlParameterizedString
                {
                    CommandText = rawQuery
                };

                AddNamespaces(queryString);

                try
                {
                    // Parse and validate the query
                    var parser = new SparqlQueryParser();
                    parser.ParseFromString(queryString.ToString());
                }
                catch (Exception ex)
                {
                    // Return error details with SPARQL parsing error
                    context.Result = new BadRequestObjectResult(new
                    {
                        error = "Invalid SPARQL query.",
                        details = ex.Message
                    });
                    return;
                }
            }
            else
            {
                context.Result = new BadRequestObjectResult(new { error = "Invalid request format." });
                return;
            }

            // Proceed to the next middleware/controller action if validation succeeds
            await next();
        }

        private void AddNamespaces(SparqlParameterizedString queryString)
        {
            queryString.Namespaces.AddNamespace("bd", new Uri("http://www.bigdata.com/rdf#"));
            queryString.Namespaces.AddNamespace("cc", new Uri("http://creativecommons.org/ns#"));
            queryString.Namespaces.AddNamespace("dct", new Uri("http://purl.org/dc/terms/"));
            queryString.Namespaces.AddNamespace("geo", new Uri("http://www.opengis.net/ont/geosparql#"));
            queryString.Namespaces.AddNamespace("ontolex", new Uri("http://www.w3.org/ns/lemon/ontolex#"));
            queryString.Namespaces.AddNamespace("owl", new Uri("http://www.w3.org/2002/07/owl#"));
            queryString.Namespaces.AddNamespace("p", new Uri("http://www.wikidata.org/prop/"));
            queryString.Namespaces.AddNamespace("pq", new Uri("http://www.wikidata.org/prop/qualifier/"));
            queryString.Namespaces.AddNamespace("pqn", new Uri("http://www.wikidata.org/prop/qualifier/value-normalized/"));
            queryString.Namespaces.AddNamespace("pqv", new Uri("http://www.wikidata.org/prop/qualifier/value/"));
            queryString.Namespaces.AddNamespace("pr", new Uri("http://www.wikidata.org/prop/reference/"));
            queryString.Namespaces.AddNamespace("prn", new Uri("http://www.wikidata.org/prop/reference/value-normalized/"));
            queryString.Namespaces.AddNamespace("prov", new Uri("http://www.w3.org/ns/prov#"));
            queryString.Namespaces.AddNamespace("prv", new Uri("http://www.wikidata.org/prop/reference/value/"));
            queryString.Namespaces.AddNamespace("ps", new Uri("http://www.wikidata.org/prop/statement/"));
            queryString.Namespaces.AddNamespace("psn", new Uri("http://www.wikidata.org/prop/statement/value-normalized/"));
            queryString.Namespaces.AddNamespace("psv", new Uri("http://www.wikidata.org/prop/statement/value/"));
            queryString.Namespaces.AddNamespace("rdf", new Uri("http://www.w3.org/1999/02/22-rdf-syntax-ns#"));
            queryString.Namespaces.AddNamespace("rdfs", new Uri("http://www.w3.org/2000/01/rdf-schema#"));
            queryString.Namespaces.AddNamespace("schema", new Uri("http://schema.org/"));
            queryString.Namespaces.AddNamespace("skos", new Uri("http://www.w3.org/2004/02/skos/core#"));
            queryString.Namespaces.AddNamespace("wd", new Uri("http://www.wikidata.org/entity/"));
            queryString.Namespaces.AddNamespace("wdata", new Uri("http://www.wikidata.org/wiki/Special:EntityData/"));
            queryString.Namespaces.AddNamespace("wdno", new Uri("http://www.wikidata.org/prop/novalue/"));
            queryString.Namespaces.AddNamespace("wdref", new Uri("http://www.wikidata.org/reference/"));
            queryString.Namespaces.AddNamespace("wds", new Uri("http://www.wikidata.org/entity/statement/"));
            queryString.Namespaces.AddNamespace("wdt", new Uri("http://www.wikidata.org/prop/direct/"));
            queryString.Namespaces.AddNamespace("wdtn", new Uri("http://www.wikidata.org/prop/direct-normalized/"));
            queryString.Namespaces.AddNamespace("wdv", new Uri("http://www.wikidata.org/value/"));
            queryString.Namespaces.AddNamespace("wikibase", new Uri("http://wikiba.se/ontology#"));
            queryString.Namespaces.AddNamespace("xsd", new Uri("http://www.w3.org/2001/XMLSchema#"));
        }
    }
}
