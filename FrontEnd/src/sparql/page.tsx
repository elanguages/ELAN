import React, { useRef } from "react";
import { Stack, Textarea, IconButton, Table } from "@chakra-ui/react";
import { IoSendOutline } from "react-icons/io5";
import { useSessionStorage } from "../hooks/useSessionStorage";
import { useSPARQLCreateMutation } from "./cache/useSPARQLCreateMutation";

type SparqlResult = {
  columns: string[];
  rows: {
    [key: string]: string | null;
  }[];
};

export const SPARQLView = () => {
  const createSPARQLPostMutation = useSPARQLCreateMutation();
  const [query, setQuery] = useSessionStorage<string>("sparql-query", "");
  const [queryRes, setQeryRes] = useSessionStorage<SparqlResult>(
    "sparql-res",
    {} as SparqlResult
  );

  const textareaRef = useRef<HTMLTextAreaElement>(null);
  const handleInput = () => {
    const textarea = textareaRef.current;
    if (textarea) {
      textarea.style.height = "auto";
      textarea.style.height = `${textarea.scrollHeight}px`;
    }
  };

  const handleSubmit = async (e: React.FormEvent<HTMLFormElement>) => {
    e.preventDefault();
    if (createSPARQLPostMutation.isPending) {
      alert("Request is processing");
      return;
    }
    try {
      const response = await createSPARQLPostMutation.mutateAsync(query);
      setQeryRes(response.data);
    } catch (error) {
      setQeryRes({} as SparqlResult);
    }
  };

  return (
    <>
      <Stack
        p={4}
        borderWidth="1px"
        borderRadius="lg"
        margin="0px 20px 0px 20px"
        direction="column"
      >
        <form onSubmit={handleSubmit}>
          <Textarea
            ref={textareaRef}
            placeholder="Insert Query..."
            _placeholder={{ color: "white" }}
            bg="black"
            color="white"
            spellCheck="false"
            resize="none" // Prevent manual resizing; handled dynamically
            onInput={handleInput} // Call auto-resize on input
            overflow="hidden" // Prevent scrollbars
            value={query}
            onChange={(e) => setQuery(e.target.value)}
          />
          <IconButton type="submit">
            <IoSendOutline />
          </IconButton>
        </form>
      </Stack>
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
