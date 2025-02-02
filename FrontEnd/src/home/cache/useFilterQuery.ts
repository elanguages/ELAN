import { useQuery } from "@tanstack/react-query";
import axios from "axios";
import { FiltersData } from "../../entities";
const getFilters = async (): Promise<FiltersData> => {
  const response = await axios.get(`esolang/api/Esolang/filters`);
  return response.data;
};

export const useFiltersQuery = () => {
  return useQuery<FiltersData>({
    queryKey: ["filters", "filters"],
    queryFn: () => getFilters(),
  });
};
