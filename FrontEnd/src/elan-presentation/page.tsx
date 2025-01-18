import { PRESENTATION_TEXT } from "./constants";
import { PresentationCard } from "./components";
export const PresentationView = () => {
  return <PresentationCard {...PRESENTATION_TEXT} />;
};
export default PresentationView;
