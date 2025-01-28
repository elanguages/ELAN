import React from "react";
import { Stack } from "@chakra-ui/react";

export const QueryConsoleContainer: React.FC<React.PropsWithChildren<{}>> = ({
  children,
}) => {
  return (
    <Stack
      p={4}
      borderWidth="1px"
      borderRadius="lg"
      margin="0px 20px 0px 20px"
      direction="column"
    >
      {children}
    </Stack>
  );
};
