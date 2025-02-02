import React from "react";
import { Stack, Table } from "@chakra-ui/react";
import { SparqlResult } from "../../../entities";

interface QueryFormProps {
  queryRes: SparqlResult;
}

export const QueryTable: React.FC<QueryFormProps> = ({ queryRes }) => {
  const replaceWikidataUrl = (url: string): string => {
    // for development environment concat with microservice port
    const baseUrl = import.meta.env.VITE_API_BASE_URL;

    const wikidataPrefix = "http://www.wikidata.org/entity/Q";
    const localPrefix = `${baseUrl}sparql-entity/Q`;
    if (url.startsWith(wikidataPrefix)) {
      return url.replace(wikidataPrefix, localPrefix);
    }
    return url;
  };
  return (
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
              {queryRes.columns.map((column, colIndex) => {
                const cellValue = row[column] ? row[column] : "No data";
                const displayValue = replaceWikidataUrl(cellValue);

                const isWikidataUrl = cellValue.startsWith(
                  "http://www.wikidata.org/entity/Q"
                );
                return (
                  <Table.Cell key={colIndex}>
                    {isWikidataUrl ? (
                      <a
                        href={displayValue}
                        target="_blank"
                        rel="noopener noreferrer"
                      >
                        {displayValue}
                      </a>
                    ) : (
                      displayValue
                    )}
                  </Table.Cell>
                );
              })}
            </Table.Row>
          ))}
        </Table.Body>
      </Table.Root>
    </Stack>
  );
};

export default QueryTable;
