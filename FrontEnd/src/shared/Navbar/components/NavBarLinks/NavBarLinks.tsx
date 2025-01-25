import { Link, Stack } from "@chakra-ui/react";
import { NAVBAR_LINKS } from "../../constants";
interface LogoProps {
  selectedModule: string | null;
  setSelectedModule: (module: string | null) => void;
  isMobile: boolean | undefined;
}
export const NavBarLinks = ({
  selectedModule,
  setSelectedModule,
  isMobile,
}: LogoProps) => {
  return (
    <Stack
      direction={isMobile ? "column" : "row"}
      align={isMobile ? "flex-start" : "default"}
    >
      {NAVBAR_LINKS.map((module) => (
        <Link
          key={module.text}
          variant="plain"
          onClick={() => setSelectedModule(module.text)}
          href={module.href}
          borderBottom={
            selectedModule === module.text ? "2px solid black" : "none"
          }
        >
          {module.text}
        </Link>
      ))}
    </Stack>
  );
};
export default NavBarLinks;
