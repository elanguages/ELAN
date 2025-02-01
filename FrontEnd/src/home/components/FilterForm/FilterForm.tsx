import React, { useRef } from "react";
import { FiltersData } from "../../../entities";
import { Box, HStack, Button } from "@chakra-ui/react";
import { rmven } from "../../../shared/utils";
import {
  SelectContent,
  SelectItem,
  SelectLabel,
  SelectRoot,
  SelectTrigger,
  SelectValueText,
} from "../../../components/ui/select";
interface FilterFormProps {
  filters: any;
  setFilters: React.Dispatch<React.SetStateAction<any>>;
  dataFilters: FiltersData;
}

export const FilterForm: React.FC<FilterFormProps> = ({
  filters,
  setFilters,
  dataFilters,
}) => {
  console.log(dataFilters);

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
  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    console.log("Form Data to be Sent:", filters);
  };
  const handleReset = (e: React.FormEvent) => {
    e.preventDefault();
    if (formRef.current != null) {
      formRef.current.reset();
      setFilters("");
    }
  };
  const handleRecomend = (e: React.FormEvent) => {
    e.preventDefault();
    console.log("recomend");
  };

  const renderFormField = (key: string, value: any) => {
    if (Array.isArray(value)) {
      return (
        <div key={key}>
          <label>{rmven(key)}</label>
          <select
            multiple
            value={filters[key] || []}
            onChange={(e) => {
              const selectedOptions = Array.from(
                e.target.selectedOptions,
                (option) => option.value
              );
              handleChange(key, selectedOptions);
            }}
          >
            {value.map((option: string, index: number) => (
              <option key={index} value={option}>
                {renderText(option)}
              </option>
            ))}
          </select>
        </div>
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
        <HStack wrap="wrap">
          {Object.entries(dataFilters).map(([key, value]) =>
            renderFormField(key, value)
          )}
          {Object.entries(dataFilters).map(([key, value]) =>
            renderFormFieldBoolean(key, value)
          )}
        </HStack>
        <HStack
          wrap="wrap"
          justifyContent="space-between"
          width="full"
          marginTop="5px"
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
