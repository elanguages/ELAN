import { useMutation } from "@tanstack/react-query";
import axios, { AxiosError } from "axios";

export const useSPARQLCreateMutation = () => {
  return useMutation({
    mutationFn: async (sparqlQuery: string) => {
      const response = await axios.post("/api/Sparql", { sparqlQuery });
      return response;
    },
    onSuccess: (res) => {
      console.log(res.data);
    },
    onError: (error: AxiosError) => {
      console.log(error);
    },
  });
};
