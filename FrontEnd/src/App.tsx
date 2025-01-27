import React from "react";
import { BrowserRouter as Router, Routes, Route } from "react-router-dom";
import RootLayout from "./layout";

const HomeView = React.lazy(() => import("./home/page"));
const PresentationView = React.lazy(() => import("./elan-presentation/page"));
const SPARQLView = React.lazy(() => import("./sparql/page"));

export const App = () => {
  return (
    <Router>
      <RootLayout>
        <Routes>
          <Route path="/home/*" element={<HomeView />} />
          <Route path="/presentation/*" element={<PresentationView />} />
          <Route path="/sparql/*" element={<SPARQLView />} />
        </Routes>
      </RootLayout>
    </Router>
  );
};
