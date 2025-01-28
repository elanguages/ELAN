export type SparqlResult = {
  columns: string[];
  rows: {
    [key: string]: string | null;
  }[];
};
