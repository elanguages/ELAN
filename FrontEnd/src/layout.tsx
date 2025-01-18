import React from "react";

import { Footer, Navbar } from "./shared";

export const RootLayout: React.FC<React.PropsWithChildren<{}>> = ({
  children,
}) => {
  return (
    <>
      <Navbar />
      <div>{children}</div>
      <Footer />
    </>
  );
};
export default RootLayout;
