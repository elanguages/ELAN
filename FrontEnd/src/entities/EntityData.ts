type Description = {
  propertyDescription: string;
  propertyLabel: string;
};

type Value = {
  value: string;
  valueLabel?: string | null;
  valueDescription?: string | null;
};

type Property = {
  propertyDescription: string;
  values: Value[];
};

export type EntityData = {
  description: Description;
  statements: Record<string, Property>;
};
