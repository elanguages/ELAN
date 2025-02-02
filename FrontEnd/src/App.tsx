import React from "react";
import {
  BrowserRouter as Router,
  Routes,
  Route,
  Navigate,
} from "react-router-dom";
import RootLayout from "./layout";

const HomeView = React.lazy(() => import("./home/page"));
const PresentationView = React.lazy(() => import("./elan-presentation/page"));
const SPARQLView = React.lazy(() => import("./sparql/page"));
const SPARQLEntityView = React.lazy(() => import("./sparql-entity/page"));

export const App = () => {
  return (
    <Router>
      <RootLayout>
        <Routes>
          <Route path="/home/*" element={<HomeView />} />
          <Route path="/presentation/*" element={<PresentationView />} />
          <Route path="/sparql-entity/:id" element={<SPARQLEntityView />} />
          <Route path="/sparql/*" element={<SPARQLView />} />

          {/* Catch-all route to redirect to home */}
          <Route path="*" element={<Navigate to="/home" replace />} />
        </Routes>
      </RootLayout>
    </Router>
  );
};
