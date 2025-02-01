namespace Elan.Api.Esolang.Queries
{
    public static class SparqlQueries
    {
        public static string GetEntityDescription(string entityId) => $@"
            SELECT ?propertyLabel ?propertyDescription WHERE {{
                        wd:{entityId} rdfs:label ?propertyLabel;
                               schema:description ?propertyDescription.
                        FILTER(LANG(?propertyLabel) = 'en')
                        FILTER(LANG(?propertyDescription) = 'en')
                }}
        ";

        public static string GetEntityStatements(string entityId) => $@"
            SELECT ?property ?propertyLabel ?propertyDescription ?value ?valueLabel ?valueDescription WHERE {{
                wd:{entityId} ?property ?value.
  
                OPTIONAL {{
                ?property rdfs:label ?propertyLabel.
                FILTER(LANG(?propertyLabel) = ""en"")
                }}
                OPTIONAL {{
                ?property schema:description ?propertyDescription.
                FILTER(LANG(?propertyDescription) = ""en"")
                }}

                OPTIONAL {{
                ?value rdfs:label ?valueLabel.
                FILTER(LANG(?valueLabel) = ""en"")
                }}
                OPTIONAL {{
                ?value schema:description ?valueDescription.
                FILTER(LANG(?valueDescription) = ""en"")
                }}

                BIND(IRI(REPLACE(STR(?property), ""http://www.wikidata.org/prop/direct/"", ""http://www.wikidata.org/entity/"")) AS ?propertyEntity)
                OPTIONAL {{
                ?propertyEntity rdfs:label ?propertyLabel.
                FILTER(LANG(?propertyLabel) = ""en"")
                }}
                OPTIONAL {{
                ?propertyEntity schema:description ?propertyDescription.
                FILTER(LANG(?propertyDescription) = ""en"")
                }}

                FILTER(BOUND(?propertyLabel))
            }}
        ";

        public static string GetEsotericLanguages() => @"
            SELECT ?programminglanguage WHERE {
                   ?programminglanguage wdt:P31 wd:Q610140 .
            }
        ";
    }
}
