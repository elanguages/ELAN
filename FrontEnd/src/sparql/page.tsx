import React, { useState, useRef } from "react";
import { Box, Textarea, IconButton } from "@chakra-ui/react";
import { IoSendOutline } from "react-icons/io5";
import { useSessionStorage } from "../hooks/useSessionStorage";
import { useSPARQLCreateMutation } from "./cache/useSPARQLCreateMutation";
import axios from "axios";
export const SPARQLView = () => {
  const createSPARQLPostMutation = useSPARQLCreateMutation();
  const textareaRef = useRef<HTMLTextAreaElement>(null);
  const handleInput = () => {
    const textarea = textareaRef.current;
    if (textarea) {
      textarea.style.height = "auto";
      textarea.style.height = `${textarea.scrollHeight}px`;
    }
  };
  const [query, setQuery] = useSessionStorage<string>("sparql-query", "");
  const [queryRes, setQueryRes] = useSessionStorage<Record<string, any>>(
    "sparql-res",
    {}
  );
  const handleSubmit = async (e: React.FormEvent<HTMLFormElement>) => {
    e.preventDefault();
    // console.log("submit");
    // console.log(query);
    // const res = await createSPARQLPostMutation.mutateAsync(query);
    const response = await axios.post("/api/Sparql", { query });
    console.log(response);
  };

  return (
    <Box p={4} borderWidth="1px" borderRadius="lg" margin="0px 20px 0px 20px">
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
    </Box>
  );
};

export default SPARQLView;
