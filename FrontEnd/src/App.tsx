import React from "react";
import {
  createBrowserRouter,
  createRoutesFromElements,
  Route,
  RouterProvider,
} from "react-router-dom";

const HomeView = React.lazy(() => import("./home/page"));
const PresentationView = React.lazy(() => import("./elan-presentation/page"));

const router = createBrowserRouter(
  createRoutesFromElements(
    <Route path="/">
      <Route path="/home/*" element={<HomeView />} />
      <Route path="/presentation/*" element={<PresentationView />} />
    </Route>
  )
);

export const App = () => {
  return <RouterProvider router={router} />;
};
