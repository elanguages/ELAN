import { useQuery } from "@tanstack/react-query";
import axios from "axios";
import { EntitiesData } from "../../entities";
const getEntity = async (): Promise<EntitiesData> => {
  const response = await axios.get(`esolang/api/Esolang/esolang-entities`);
  return response.data;
};

export const useEntitiesQuery = () => {
  return useQuery<EntitiesData>({
    queryKey: ["entities", "entities"],
    queryFn: () => getEntity(),
  });
};
