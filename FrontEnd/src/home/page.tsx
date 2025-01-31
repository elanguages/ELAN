import React, { useState, useEffect } from "react";
import { useEntitiesQuery } from "./cache";
import { Center, Spinner, Box } from "@chakra-ui/react";
import { EntitiesData } from "../entities";
import { ForceGraph2D } from "react-force-graph";

type Node = {
  id: string;
  name: string;
  link: string;
  group: number;
};

type Edge = {
  source: string;
  target: string;
  // label?: string;
};

type Graph = {
  nodes: Node[];
  links: Edge[];
};

export const HomeView = () => {
  const { isLoading, data, error } = useEntitiesQuery();
  const [graph, setGraph] = useState<Graph | null>(null);

  useEffect(() => {
    if (data) {
      console.log(data);
      const graphData = transformDataToGraph(data);
      console.log(graphData);
      setGraph(graphData);
    }
  }, [data]);

  const rmven = (str: string | undefined | null) => {
    if (str && str.endsWith("@en")) {
      return str.slice(0, -3);
    }
    return str;
  };

  const transformDataToGraph = (data: EntitiesData): Graph => {
    const nodes: Node[] = [];
    const links: Edge[] = [];

    // Create the central node
    const centralNode: Node = {
      id: "esolang",
      name: "EPL",
      link: "http://localhost:5173/sparql-entity/Q30312498",
      group: 1,
    };
    nodes.push(centralNode);
    data.forEach((entry) => {
      const entityId = entry.entityId;
      const entityLabel = rmven(entry.description?.propertyLabel) || entityId;

      // Create entity node
      const langNode: Node = {
        id: entityId,
        name: entityLabel,
        link: `http://localhost:5173/sparql-entity/${entityId}`,
        group: 2,
      };
      if (entityId != entityLabel) {
        nodes.push(langNode);

        // Connect entity to the central node
        links.push({ source: "esolang", target: entityId });

        const statements = entry?.statements || {};
        const keys = Object.keys(statements);
        keys.forEach((key) => {
          const propNode: Node = {
            id: key,
            name: rmven(key) as string,
            link: statements[key].propertyLink,
            group: 3,
          };

          if (!nodes.some((node) => node.id === propNode.id)) {
            nodes.push(propNode);
          }
          links.push({ source: entityId, target: key });
        });
      }
    });
    // Validate links
    return { nodes, links };
  };

  if (isLoading) {
    return (
      <Center mt={36}>
        <Spinner />
      </Center>
    );
  }

  if (error) {
    return <>Error</>;
  }

  return graph ? (
    <Box>
      <ForceGraph2D
        graphData={graph}
        nodeLabel="name"
        nodeAutoColorBy="group"
        linkDirectionalArrowLength={3.5}
        linkDirectionalArrowRelPos={1}
        linkCurvature={0.25}
        onNodeClick={(node) => {
          // Navigate to the link when a node is clicked
          window.open(node.link, "_blank");
        }}
        nodeCanvasObject={(node, ctx, globalScale) => {
          const label = node.name;
          const fontSize = 12 / globalScale; // Scale font size based on zoom level
          ctx.font = `${fontSize}px Sans-Serif`;
          ctx.textAlign = "center";
          ctx.textBaseline = "middle";
          ctx.fillStyle =
            node.group == 1
              ? "#007ACC"
              : node.group == 2
              ? "#FF6600"
              : "#00CC66"; // Label color
          ctx.fillText(label, node.x as number, node.y as number);
        }}
      />
    </Box>
  ) : (
    <p>Loading graph...</p>
  );
};

export default HomeView;
