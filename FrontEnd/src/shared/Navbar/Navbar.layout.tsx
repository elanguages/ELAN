import React from "react";
import { VStack } from "@chakra-ui/react";

export const NavBarContainer: React.FC<React.PropsWithChildren<{}>> = ({
  children,
}) => {
  return (
    <VStack
      padding="5px 20px 5px 20px"
      borderBottom="1px solid black"
      alignItems="flex-start"
    >
      {children}
    </VStack>
  );
};
