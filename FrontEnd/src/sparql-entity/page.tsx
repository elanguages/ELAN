import React from "react";
import { useEntityQuery } from "./cache";
import { useParams } from "react-router-dom";
import { Center, Spinner, Text, VStack, Table } from "@chakra-ui/react";
export const SPARQLEntityView = () => {
  const { id } = useParams();
  const { isLoading, data, error } = useEntityQuery(id!);

  console.log(data);
  const rmven = (str: string | undefined) => {
    if (str && str.endsWith("@en")) {
      return str.slice(0, -3);
    }
    return str;
  };
  if (isLoading) {
    return (
      <Center mt={36}>
        <Spinner />
      </Center>
    );
  }
  if (error) {
    return <>Error</>;
  }

  return (
    <VStack>
      <Text fontWeight="bold">{rmven(data?.description.propertyLabel)}</Text>
      <Text>{rmven(data?.description.propertyDescription)}</Text>
    </VStack>
  );
};

export default SPARQLEntityView;
