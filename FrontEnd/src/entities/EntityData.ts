type Property = {
  property: string;
  propertyLabel: string;
  propertyDescription: string;
  value: string | null;
  valueLabel: string | null;
};

export type EntityData = {
  description: {
    propertyDescription: string;
    propertyLabel: string;
  };
  statements: Property[];
};
