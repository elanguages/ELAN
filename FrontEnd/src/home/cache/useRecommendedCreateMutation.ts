import { useMutation } from "@tanstack/react-query";
import axios, { AxiosError } from "axios";
import { handleRequestError } from "../../shared/utils";

export const useRecommendedCreateMutation = () => {
  return useMutation({
    mutationFn: async (Filters: object) => {
      const filteredFilters = Object.entries(Filters).reduce(
        (acc, [key, value]) => {
          if (value !== undefined && value !== false) {
            acc[key] = [value]; // Ensure the value is wrapped in an array
          }
          return acc;
        },
        {} as Record<string, any>
      );
      const response = await axios.post(
        "esolang/api/Esolang/recommend-esolang-entities",
        filteredFilters // Use the dynamically generated object
      );
      return response;
    },
    onSuccess: (res) => {
      console.log(res);
    },
    onError: (error: AxiosError) => {
      handleRequestError(error);
    },
  });
};
