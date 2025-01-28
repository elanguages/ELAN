import React from "react";
import { Stack, Table } from "@chakra-ui/react";

import { useSessionStorage } from "../hooks/useSessionStorage";
import { useSPARQLCreateMutation } from "./cache/useSPARQLCreateMutation";
import { QuerryConsoleContainer } from "./page.layout";
import { SparqlResult } from "./entities";
import { QueryForm } from "./components";

export const SPARQLView = () => {
  const createSPARQLPostMutation = useSPARQLCreateMutation();
  const [query, setQuery] = useSessionStorage<string>("sparql-query", "");
  const [queryRes, setQueryRes] = useSessionStorage<SparqlResult>(
    "sparql-res",
    {} as SparqlResult
  );
  return (
    <>
      <QuerryConsoleContainer>
        <QueryForm
          createSPARQLPostMutation={createSPARQLPostMutation}
          querry={query}
          setQuerry={setQuery}
          setQuerryRes={setQueryRes}
        />
      </QuerryConsoleContainer>
      {queryRes.columns?.length > 0 && (
        <Stack margin="0px 20px 0px 20px" overflowX="auto">
          <Table.Root>
            <Table.Header>
              <Table.Row>
                {queryRes.columns.map((column, index) => (
                  <Table.ColumnHeader key={index}>{column}</Table.ColumnHeader>
                ))}
              </Table.Row>
            </Table.Header>
            <Table.Body>
              {queryRes.rows.map((row, rowIndex) => (
                <Table.Row key={rowIndex}>
                  {queryRes.columns.map((column, colIndex) => (
                    <Table.Cell key={colIndex}>
                      {row[column] ? row[column] : "No data"}
                    </Table.Cell>
                  ))}
                </Table.Row>
              ))}
            </Table.Body>
          </Table.Root>
        </Stack>
      )}
    </>
  );
};

export default SPARQLView;
