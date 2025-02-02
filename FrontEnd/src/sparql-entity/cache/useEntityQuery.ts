import { useQuery } from "@tanstack/react-query";
import axios from "axios";
import { EntityData } from "../../entities";
const getEntity = async (id: string): Promise<EntityData> => {
  const response = await axios.get(`esolang/api/entity/${id}`);
  return response.data;
};

export const useEntityQuery = (entityId: string) => {
  return useQuery<EntityData>({
    queryKey: ["entity", entityId],
    queryFn: () => getEntity(entityId),
  });
};
