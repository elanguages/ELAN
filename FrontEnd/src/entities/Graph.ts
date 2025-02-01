export type Node = {
  id: string;
  name: string;
  link: string;
  group: number;
};

export type Edge = {
  source: Source | string;
  target: Target | string;
};

export type Graph = {
  nodes: Node[];
  links: Edge[];
};

export type Source = {
  id: string;
  name: string;
  link: string;
  group: number;
  color?: string;
  index?: number;
  vx?: number;
  vy?: number;
  vz?: number;
  x?: number;
  y?: number;
  z?: number;
};

export type Target = {
  id: string;
  name: string;
  link: string;
  group: number;
  color?: string;
  index?: number;
  vx?: number;
  vy?: number;
  vz?: number;
  x?: number;
  y?: number;
  z?: number;
};
