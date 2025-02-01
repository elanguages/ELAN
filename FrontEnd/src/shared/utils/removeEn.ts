export const rmven = (str: string | undefined | null) => {
  if (str && str.endsWith("@en")) {
    return str.slice(0, -3);
  }
  return str;
};
