import { Stack, Text, Image, Link, VStack, Separator } from "@chakra-ui/react";
import uaic from "../../assets/uaic.svg";
import git from "../../assets/git.svg";

import { LINKS, FILLER_TEXT } from "./constants";
export const Footer = () => {
  return (
    <VStack padding="20px 0px 20px 0px">
      <Stack
        justifyContent="center"
        alignItems="center"
        direction={{ base: "column", sm: "row" }}
      >
        <Link href={LINKS.link1} boxSize={{ base: "30px", sm: "40px" }}>
          <Image src={uaic} />
        </Link>

        <Text>{FILLER_TEXT.title}</Text>

        <Link href={LINKS.link2} boxSize={{ base: "30px", sm: "40px" }}>
          <Image src={git} />
        </Link>
      </Stack>

      <Separator borderColor="black" w="100%" />
      <Link href={LINKS.link3}>
        <Text>{FILLER_TEXT.license}</Text>
      </Link>
    </VStack>
  );
};

export default Footer;
