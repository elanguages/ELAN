// import { useState } from 'react'
// import reactLogo from './assets/react.svg'
// import viteLogo from '/vite.svg'

import { VStack, Text, Box } from "@chakra-ui/react";
import { AiOutlinePaperClip } from "react-icons/ai";

// Chakra provides five breakpoints by default:
// const breakpoints = {
//   base: "0em", // 0px
//   sm: "30em", // ~480px
//   md: "48em", // ~768px
//   lg: "62em", // ~992px
//   xl: "80em", // ~1280px
//   "2xl": "96em", // ~1536px
// }

function App() {
  return (
    <VStack>
      <AiOutlinePaperClip />
      <Text fontSize={{ base: "xs", md: "md", lg: "lg" }}>Text</Text>
      <Box bg="red.500" boxShadow="0 4px 6px purple, 0 1px 3px purple">
        This is styled
      </Box>
    </VStack>
  );
}

export default App;
