import React from "react";
import { VStack, Box } from "@chakra-ui/react";

import { Footer, Navbar } from "./shared";
import { motion } from "framer-motion";

export const RootLayout: React.FC<React.PropsWithChildren<{}>> = ({
  children,
}) => {
  return (
    <VStack minHeight="100vh" align="stretch">
      <Navbar />

      <Box flex="1">
        {/* If there is an issue just delete this */}
        <motion.div
          initial={{ opacity: 0 }}
          animate={{ opacity: 1 }}
          exit={{ opacity: 0 }}
          transition={{ duration: 0.7 }}
        >
          {children}
        </motion.div>
      </Box>

      <Footer />
    </VStack>
  );
};
export default RootLayout;
