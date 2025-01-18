import { Text, VStack } from "@chakra-ui/react";
interface PresentationCardProps {
  firstText: string;
  secondText: string;
}
export const PresentationCard = ({
  firstText,
  secondText,
}: PresentationCardProps) => {
  return (
    <VStack>
      <Text>{firstText}</Text>
      <Text>{secondText}</Text>
    </VStack>
  );
};
export default PresentationCard;
