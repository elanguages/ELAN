export type EntitiesData = {
  entityId: string;
  description: {
    propertyDescription: string;
    propertyLabel: string;
  };
  statements: Record<
    string,
    {
      propertyDescription: string;
      propertyLink: string;
      values: {
        value: string;
        valueLabel?: string | null;
        valueDescription?: string | null;
      }[];
    }
  >;
}[];
