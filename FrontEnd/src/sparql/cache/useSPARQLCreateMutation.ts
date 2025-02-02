import { useMutation } from "@tanstack/react-query";
import axios, { AxiosError } from "axios";
import { handleRequestError } from "../../shared/utils";

export const useSPARQLCreateMutation = () => {
  return useMutation({
    mutationFn: async (sparqlQuery: string) => {
      const response = await axios.post("elan/api/Sparql", { sparqlQuery });
      return response;
    },
    onSuccess: (res) => {
      console.log(res.data);
    },
    onError: (error: AxiosError) => {
      handleRequestError(error);
    },
  });
};
