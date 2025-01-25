import { Link, Text } from "@chakra-ui/react";
import { NAVBAR_LINKS } from "../../constants";
export const Logo = () => {
  return (
    <Link href={NAVBAR_LINKS[0].href}>
      <Text fontWeight="bold" fontSize="4xl">
        ELAN
      </Text>
    </Link>
  );
};
export default Logo;
