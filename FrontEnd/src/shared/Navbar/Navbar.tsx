import { HStack, useBreakpointValue, Image } from "@chakra-ui/react";
import menu from "../../assets/menu.svg";
import closeMenu from "../../assets/closemenu.svg";
import { useState, useEffect } from "react";
import { Logo, NavBarLinks } from "./components";
import { useLocation } from "react-router-dom";
import { NAVBAR_LINKS } from "./constants";
import { NavBarContainer } from "./Navbar.layout";
export const Navbar = () => {
  const isMobile = useBreakpointValue(
    { base: true, md: false },
    { ssr: false }
  );

  const location = useLocation();
  const [isNavbarOpen, setIsNavbarOpen] = useState(false);
  const [selectedModule, setSelectedModule] = useState<string | null>("/home");

  useEffect(() => {
    const pathName = location.pathname;
    const matchedModule = NAVBAR_LINKS.find((module) =>
      pathName.includes(module.href)
    );
    if (matchedModule) {
      setSelectedModule(matchedModule.text);
    }
  }, [location.pathname]);

  return (
    <NavBarContainer>
      <HStack justifyContent="space-between" width="full">
        <Logo />
        {isMobile ? (
          <Image
            src={isNavbarOpen ? closeMenu : menu}
            onClick={() => setIsNavbarOpen(!isNavbarOpen)}
          />
        ) : (
          <NavBarLinks
            selectedModule={selectedModule}
            setSelectedModule={setSelectedModule}
            isMobile={isMobile}
          />
        )}
      </HStack>
      {isMobile && isNavbarOpen && (
        <NavBarLinks
          selectedModule={selectedModule}
          setSelectedModule={setSelectedModule}
          isMobile={isMobile}
        />
      )}
    </NavBarContainer>
  );
};

export default Navbar;
