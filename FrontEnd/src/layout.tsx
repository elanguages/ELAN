import React from "react";
import { VStack, Box } from "@chakra-ui/react";

import { Footer, Navbar } from "./shared";

export const RootLayout: React.FC<React.PropsWithChildren<{}>> = ({
  children,
}) => {
  return (
    <VStack minHeight="100vh" align="stretch">
      <Navbar />
      <Box flex="1">{children}</Box>
      <Footer />
    </VStack>
  );
};
export default RootLayout;
