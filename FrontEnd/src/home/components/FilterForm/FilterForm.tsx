import React, { useRef } from "react";
import { FiltersData } from "../../../entities";
import { Box, HStack, Button } from "@chakra-ui/react";
import { rmven } from "../../../shared/utils";
import { createListCollection } from "@chakra-ui/react";
import {
  SelectContent,
  SelectItem,
  SelectRoot,
  SelectTrigger,
  SelectValueText,
} from "../../../components/ui/select";
import { EntitiesData } from "../../../entities";
import { Graph } from "../../../entities";
interface FilterFormProps {
  setFilteredGraph: React.Dispatch<React.SetStateAction<Graph | null>>;
  transformDataToGraph: (data: EntitiesData) => Graph;
  filters: any;
  setFilters: React.Dispatch<React.SetStateAction<any>>;
  dataFilters: FiltersData;
  createFilterPostMutation: {
    isPending: boolean;
    mutateAsync: (query: object) => Promise<{ data: EntitiesData }>;
  };
  createRecommendPostMutation: {
    isPending: boolean;
    mutateAsync: (query: object) => Promise<{ data: EntitiesData }>;
  };
}

export const FilterForm: React.FC<FilterFormProps> = ({
  filters,
  setFilters,
  dataFilters,
  createFilterPostMutation,
  createRecommendPostMutation,
  setFilteredGraph,
  transformDataToGraph,
}) => {
  const formRef = useRef<HTMLFormElement | null>(null);
  const handleChange = (key: string, value: any) => {
    setFilters((prevFilters: any) => ({
      ...prevFilters,
      [key]: value,
    }));
  };
  const renderText = (text: string): string => {
    if (text.startsWith("http")) {
      return text;
    }
    if (text.includes("T00:00:00Z^^")) {
      return text.split("T00:00:00Z^^")[0];
    }
    if (text.includes("^^")) {
      return text.split("^^")[0];
    }
    return rmven(text) || text;
  };
  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (createFilterPostMutation.isPending) {
      alert("Request is processing");
      return;
    }
    try {
      const response = await createFilterPostMutation.mutateAsync(filters);
      handleReset();
      setFilteredGraph(transformDataToGraph(response.data));
    } catch (error) {
      console.log(error);
    }
  };
  const handleReset = () => {
    if (formRef.current != null) {
      formRef.current.reset();
      setFilters({});
    }
  };
  const handleRecomend = async (e: React.FormEvent) => {
    e.preventDefault();
    console.log("recomend");
    if (createRecommendPostMutation.isPending) {
      alert("Request is processing");
      return;
    }
    try {
      const response = await createRecommendPostMutation.mutateAsync(filters);
      handleReset();
      setFilteredGraph(transformDataToGraph(response.data));
    } catch (error) {
      console.log(error);
    }
  };

  const renderFormField = (key: string, value: any) => {
    if (Array.isArray(value)) {
      const collection = createListCollection({
        items: value.map((option: string) => ({
          label: renderText(option), // Use renderText to clean up the label
          value: option, // Use the original value as the value
        })),
      });

      return (
        <SelectRoot
          collection={collection}
          size="sm"
          width="240px"
          key={key}
          onValueChange={(selectedValues) => {
            handleChange(key, selectedValues.value[0]);
          }}
        >
          <SelectTrigger>
            <SelectValueText placeholder={rmven(key) as string} />
          </SelectTrigger>
          <SelectContent>
            {collection.items.map((item) => (
              <SelectItem item={item} key={item.value as string}>
                {item.label}
              </SelectItem>
            ))}
          </SelectContent>
        </SelectRoot>
      );
    }
  };

  const renderFormFieldBoolean = (key: string, value: any) => {
    if (typeof value === "boolean") {
      return (
        <HStack key={key} gap="4px">
          <label>{rmven(key)}</label>
          <input
            type="checkbox"
            checked={filters[key] || false}
            onChange={(e) => handleChange(key, e.target.checked)}
          />
        </HStack>
      );
    }
  };
  return (
    <Box padding="0px 20px 0px 20px">
      <form onSubmit={handleSubmit} ref={formRef}>
        <HStack wrap="wrap" justifyContent="center" marginTop="10px">
          {Object.entries(dataFilters).map(([key, value]) =>
            renderFormField(key, value)
          )}
        </HStack>
        <HStack wrap="wrap" justifyContent="center" marginTop="10px">
          {Object.entries(dataFilters).map(([key, value]) =>
            renderFormFieldBoolean(key, value)
          )}
        </HStack>

        <HStack
          wrap="wrap"
          justifyContent="space-between"
          width="full"
          marginTop="10px"
        >
          <Button type="submit">Submit!</Button>
          <Button onClick={handleReset}>Reset fields!</Button>
          <Button onClick={handleRecomend}>Recommend me!</Button>
        </HStack>
      </form>
    </Box>
  );
};

export default FilterForm;
