import { useSessionStorage } from "../hooks/useSessionStorage";
import { useSPARQLCreateMutation } from "./cache/useSPARQLCreateMutation";
import { QueryConsoleContainer } from "./page.layout";
import { SparqlResult } from "../entities";
import { QueryForm, QueryTable } from "./components";

export const SPARQLView = () => {
  const createSPARQLPostMutation = useSPARQLCreateMutation();
  const [query, setQuery] = useSessionStorage<string>("sparql-query", "");
  const [queryRes, setQueryRes] = useSessionStorage<SparqlResult>(
    "sparql-res",
    {} as SparqlResult
  );

  return (
    <>
      <QueryConsoleContainer>
        <QueryForm
          createSPARQLPostMutation={createSPARQLPostMutation}
          querry={query}
          setQuerry={setQuery}
          setQuerryRes={setQueryRes}
        />
      </QueryConsoleContainer>
      {queryRes.columns?.length > 0 && <QueryTable queryRes={queryRes} />}
    </>
  );
};

export default SPARQLView;
