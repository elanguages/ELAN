import { Grid, GridItem, Text } from "@chakra-ui/react";
import { LICENSE, SCHOLARLY, SWAGGER, ELAN } from "./constants";

export const PresentationView = () => {
  return (
    <Grid
      templateColumns={{ base: "1fr", md: "repeat(2, 1fr)" }}
      padding={{ base: "5px 20px 0px 20px", "2xl": "40px 160px 0px 160px" }}
    >
      <GridItem
        // bg="green.100"
        display="flex-column"
        alignItems="center"
        justifyContent="center"
        borderRight={{ base: "0px", md: "1px solid black" }}
        borderBottom="1px solid black"
        padding="10px"
      >
        <Text fontSize="xl" fontWeight="bold">
          {SCHOLARLY.title}
        </Text>
        <Text>
          <Text as="span">{SCHOLARLY.body}</Text>{" "}
          <a
            href={SCHOLARLY.link}
            target="_blank"
            rel="noopener noreferrer"
            style={{ textDecoration: "underline" }}
          >
            here
          </a>
          .
        </Text>
      </GridItem>
      <GridItem
        // bg="red.100"
        display="flex-column"
        alignItems="center"
        justifyContent="center"
        borderBottom="1px solid black"
        padding="10px"
      >
        <Text fontSize="xl" fontWeight="bold">
          {SWAGGER.title}
        </Text>

        <Text>
          <Text as="span">{SWAGGER.body}</Text>{" "}
          <a
            href={SWAGGER.link}
            target="_blank"
            rel="noopener noreferrer"
            style={{ textDecoration: "underline" }}
          >
            here
          </a>
          .
        </Text>
      </GridItem>
      <GridItem
        // bg="blue.100"
        display="flex-column"
        alignItems="center"
        justifyContent="center"
        borderRight={{ base: "", md: "1px solid black" }}
        borderBottom={{ base: "1px solid black", md: "0px" }}
        padding="10px"
      >
        <Text fontSize="xl" fontWeight="bold">
          {ELAN.title}
        </Text>
        <Text>{ELAN.body}</Text>
      </GridItem>
      <GridItem
        // bg="yellow.100"
        display="flex-column"
        alignItems="center"
        justifyContent="center"
        padding="10px"
      >
        <Text fontSize="xl" fontWeight="bold">
          {LICENSE.title}
        </Text>
        <Text>{LICENSE.body}</Text>
      </GridItem>
    </Grid>
  );
};

export default PresentationView;
