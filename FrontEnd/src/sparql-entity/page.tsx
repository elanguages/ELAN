import React from "react";
import { useEntityQuery } from "./cache";
import { useParams } from "react-router-dom";
import {
  Center,
  Spinner,
  Text,
  VStack,
  Table,
  Stack,
  Image,
} from "@chakra-ui/react";
export const SPARQLEntityView = () => {
  const { id } = useParams();
  const { isLoading, data, error } = useEntityQuery(id!);
  const rmven = (str: string | undefined | null) => {
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
  const statements = data?.statements || {};
  const keys = Object.keys(statements);
  return (
    <VStack>
      <Text fontWeight="bold">{rmven(data?.description?.propertyLabel)}</Text>
      <Text>{rmven(data?.description?.propertyDescription)}</Text>
      <Stack margin="20px" overflowX="auto" width="100%">
        <Table.Root>
          <Table.Header>
            <Table.Row>
              <Table.ColumnHeader>Property</Table.ColumnHeader>
              <Table.ColumnHeader>Description</Table.ColumnHeader>
              <Table.ColumnHeader>Values</Table.ColumnHeader>
              <Table.ColumnHeader>Description</Table.ColumnHeader>
            </Table.Row>
          </Table.Header>
          <Table.Body>
            {keys.map((key, rowIndex) => {
              const property = statements[key];
              return (
                <Table.Row key={rowIndex}>
                  <Table.Cell fontWeight="bold">
                    <a
                      href={property.propertyLink}
                      target="_blank"
                      rel="noopener noreferrer"
                    >
                      {rmven(key)}
                    </a>
                  </Table.Cell>
                  <Table.Cell>{rmven(property.propertyDescription)}</Table.Cell>
                  <Table.Cell>
                    {property.values.map((val, valIndex) => {
                      const linkValue = val.value;
                      const displayValueLabel = rmven(val.valueLabel);
                      return (
                        <div key={valIndex}>
                          {displayValueLabel ? (
                            <a
                              href={linkValue}
                              target="_blank"
                              rel="noopener noreferrer"
                              style={{ textDecoration: "underline" }}
                            >
                              {displayValueLabel}
                            </a>
                          ) : linkValue.endsWith(".png") ||
                            linkValue.endsWith(".jpg") ||
                            linkValue.endsWith(".jpeg") ||
                            linkValue.endsWith(".svg") ? (
                            <Image src={linkValue} boxSize="100px" />
                          ) : linkValue.startsWith("http") ? (
                            <a
                              href={linkValue}
                              target="_blank"
                              rel="noopener noreferrer"
                              style={{ textDecoration: "underline" }}
                            >
                              {linkValue}
                            </a>
                          ) : (
                            <Text color="red">{linkValue}</Text>
                          )}
                        </div>
                      );
                    })}
                  </Table.Cell>
                  <Table.Cell>
                    {property.values.map((val, valIndex) => {
                      const displayValueLabel = rmven(val.valueLabel);
                      const displayDrescription = rmven(val.valueDescription);
                      return (
                        <div key={valIndex}>
                          <Text>
                            <Text as="span" fontWeight="semibold">
                              {displayValueLabel}
                            </Text>{" "}
                            - {displayDrescription}
                          </Text>
                        </div>
                      );
                    })}
                  </Table.Cell>
                </Table.Row>
              );
            })}
          </Table.Body>
        </Table.Root>
      </Stack>
    </VStack>
  );
};

export default SPARQLEntityView;
