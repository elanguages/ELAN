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
import { rmven } from "../shared/utils";

export const SPARQLEntityView = () => {
  const { id } = useParams();
  const { isLoading, data, error } = useEntityQuery(id!);
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
  const renderValue = (
    linkValue: string,
    displayValueLabel: string | undefined | null
  ) => {
    if (displayValueLabel) {
      return (
        <a
          href={linkValue}
          target="_blank"
          rel="noopener noreferrer"
          style={{ textDecoration: "underline" }}
        >
          {displayValueLabel}
        </a>
      );
    }

    const isImage = /\.(png|jpg|jpeg|svg)$/.test(linkValue);
    if (isImage) {
      return <Image src={linkValue} boxSize="100px" />;
    }

    if (linkValue.startsWith("http")) {
      return (
        <a
          href={linkValue}
          target="_blank"
          rel="noopener noreferrer"
          style={{ textDecoration: "underline" }}
        >
          {linkValue}
        </a>
      );
    }

    if (linkValue.includes("T00:00:00Z^^")) {
      return <Text>{linkValue.split("T00:00:00Z^^")[0]}</Text>;
    }

    if (linkValue.includes("^^")) {
      return <Text>{linkValue.split("^^")[0]}</Text>;
    }

    return <Text color="red">{linkValue}</Text>;
  };
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
                          {renderValue(linkValue, displayValueLabel)}
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
