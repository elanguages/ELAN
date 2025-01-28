import React, { forwardRef } from "react";
import { Textarea, TextareaProps } from "@chakra-ui/react";

// Define the props for the TextInputContainer
interface TextInputContainerProps extends TextareaProps {
  value: string;
  onChange: (e: React.ChangeEvent<HTMLTextAreaElement>) => void;
  onInput?: (e: React.FormEvent<HTMLTextAreaElement>) => void;
}

export const TextInputContainer = forwardRef<
  HTMLTextAreaElement,
  TextInputContainerProps
>(({ onInput, value, onChange }, ref) => {
  return (
    <Textarea
      ref={ref}
      placeholder="Insert Query..."
      _placeholder={{ color: "white" }}
      bg="black"
      color="white"
      spellCheck="false"
      resize="none"
      overflow="hidden"
      onInput={onInput}
      value={value}
      onChange={onChange}
    />
  );
});
