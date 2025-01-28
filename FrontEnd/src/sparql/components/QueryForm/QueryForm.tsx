import React, { useRef } from "react";
import { IconButton } from "@chakra-ui/react";
import { IoSendOutline } from "react-icons/io5";
import { SparqlResult } from "../../../entities";
import { TextInputContainer } from "./QueryForm.layout";
interface QueryFormProps {
  createSPARQLPostMutation: {
    isPending: boolean;
    mutateAsync: (query: string) => Promise<{ data: SparqlResult }>;
  };
  querry: string;
  setQuerry: React.Dispatch<React.SetStateAction<string>>;
  setQuerryRes: React.Dispatch<React.SetStateAction<SparqlResult>>;
}

export const QueryForm: React.FC<QueryFormProps> = ({
  createSPARQLPostMutation,
  querry,
  setQuerry,
  setQuerryRes,
}) => {
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
      const response = await createSPARQLPostMutation.mutateAsync(querry);
      setQuerryRes(response.data);
    } catch (error) {
      setQuerryRes({} as SparqlResult);
    }
  };
  return (
    <form onSubmit={handleSubmit}>
      <TextInputContainer
        ref={textareaRef}
        onInput={handleInput}
        value={querry}
        onChange={(e) => setQuerry(e.target.value)}
      />
      <IconButton type="submit">
        <IoSendOutline />
      </IconButton>
    </form>
  );
};

export default QueryForm;
