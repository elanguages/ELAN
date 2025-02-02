import { useState, useEffect } from "react";
import {
  useEntitiesQuery,
  useFilterCreateMutation,
  useFiltersQuery,
  useRecommendedCreateMutation,
} from "./cache";
import { Box, Center, Spinner, Stack, Button } from "@chakra-ui/react";
import { EntitiesData } from "../entities";
import { ForceGraph3D } from "react-force-graph";
import SpriteText from "three-spritetext";
import { Node, Edge, Graph, Target, Source } from "../entities";
import { rmven } from "../shared/utils";
import { FilterForm } from "./components";
export const HomeView = () => {
  // for development environment concat with microservice port
  const baseUrl = import.meta.env.VITE_API_BASE_URL;

  const { isLoading, data, error } = useEntitiesQuery();
  const {
    isLoading: isLoadingFilters,
    data: dataFilters,
    error: errorFilters,
  } = useFiltersQuery();
  const [filters, setFilters] = useState("");
  const [graph, setGraph] = useState<Graph | null>(null);
  const [filteredGraph, setFilteredGraph] = useState<Graph | null>(null);
  const [seeValues, setSeeValues] = useState<boolean>(true);
  const [seeFilter, setSeeFilter] = useState<boolean>(false);
  const createFilterPostMutation = useFilterCreateMutation();
  const createRecommendPostMutation = useRecommendedCreateMutation();

  useEffect(() => {
    if (data) {
      const graphData = transformDataToGraph(data);
      setGraph(graphData);
      setFilteredGraph(graphData); // Initialize filteredGraph with the full graph
    }
  }, [data, seeValues]);

  const renderValue = (
    linkValue: string,
    displayValueLabel: string | undefined | null
  ): Node => {
    const valueNode: Node = {
      id: "",
      name: "",
      link: "",
      group: 4,
    };
    valueNode.id = linkValue;
    if (displayValueLabel) {
      valueNode.link = linkValue;
      valueNode.name = rmven(displayValueLabel) || displayValueLabel;
      return valueNode;
    }
    if (linkValue.startsWith("http")) {
      valueNode.link = linkValue;
      valueNode.name = linkValue;
      return valueNode;
    }
    if (linkValue.includes("T00:00:00Z^^")) {
      valueNode.link = "";
      valueNode.name = linkValue.split("T00:00:00Z^^")[0];
      return valueNode;
    }
    if (linkValue.includes("^^")) {
      valueNode.link = "";
      valueNode.name = linkValue.split("^^")[0];
      return valueNode;
    }

    valueNode.link = "";
    valueNode.name = linkValue;
    return valueNode;
  };
  const transformDataToGraph = (data: EntitiesData): Graph => {
    const nodes: Node[] = [];
    const links: Edge[] = [];

    // Create the central node
    const centralNode: Node = {
      id: "esolang",
      name: "EPL",
      link: `${baseUrl}sparql-entity/Q30312498`,
      group: 1,
    };
    nodes.push(centralNode);

    const excludedKeys = [
      "described at URL@en",
      "source code repository URL@en",
      "Homebrew formula name@en",
      "software version identifier@en",
      "official website@en",
      "Debian stable package@en",
    ];

    data.forEach((entry) => {
      const entityId = entry.entityId;
      const entityLabel = rmven(entry.description?.propertyLabel) || entityId;

      // Create entity node
      const langNode: Node = {
        id: entityId,
        name: entityLabel,
        link: `${baseUrl}sparql-entity/${entityId}`,
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
          //
          statements[key].values.forEach((value) => {
            if (!excludedKeys.includes(key) && seeValues) {
              const valueNode: Node = renderValue(
                value.value,
                value.valueLabel
              );

              if (!nodes.some((node) => node.id === valueNode.id)) {
                nodes.push(valueNode);
              }

              // Connect property node to value node
              links.push({ source: key, target: value.value });
            }
          });
        });
      }
    });

    return { nodes, links };
  };

  const getParentId = (nodeId: string, graph: Graph): string | null => {
    // Find the link where the target is the nodeId
    const link = graph.links.find(
      (link) => (link.target as Target).id === nodeId
    );
    if (link) {
      // Find the parent node using the source from the link
      const parentNode = graph.nodes.find(
        (node) => node.id === (link.source as Source).id
      );
      return parentNode?.id || null; // Return the parent node or null if not found
    }

    // Return null if no parent is found
    return null;
  };

  const filterGraph = (nodeId: string, graph: Graph): Graph => {
    const node = graph.nodes.find((node) => node.id === nodeId);
    const group = node?.group;
    if (group == 1) {
      return graph;
    } else if (group == 2 && data) {
      const selectedEntity = data.find((entry) => entry.entityId === nodeId);

      if (selectedEntity) {
        const filteredData: EntitiesData = [selectedEntity];
        const filteredGraph = transformDataToGraph(filteredData);
        return filteredGraph;
      }
    } else if (group == 3 && data) {
      // Filter for property nodes (nodeId ends with "@en")
      const filteredData: EntitiesData = data.filter((entry) => {
        // Check if the nodeId exists in the keys of the statements
        return (
          entry.statements && Object.keys(entry.statements).includes(nodeId)
        );
      });

      if (filteredData.length > 0) {
        const filteredGraph = transformDataToGraph(filteredData);
        return filteredGraph;
      }
    } else if (group == 4 && data) {
      const parentId = getParentId(nodeId, graph);

      if (parentId) {
        // Filter entities that have the parent key in statements
        const filteredData: EntitiesData = data.filter((entry) => {
          if (!entry.statements || !entry.statements[parentId]) {
            return false; // Skip entities that don't have the parent key
          }

          // Check if nodeId is in the values array of the parent key
          const values = entry.statements[parentId].values;
          return values.some((value) => value.value === nodeId);
        });

        if (filteredData.length > 0) {
          const filteredGraph = transformDataToGraph(filteredData);
          return filteredGraph;
        }
      } else {
        console.error(
          `Value node ${nodeId} has no parent. Check graph construction.`
        );
      }
    }
    return graph;
  };

  // eslint-disable-next-line @typescript-eslint/no-explicit-any
  const handleNodeRightClick = (node: any) => {
    if (graph) {
      const filtered = filterGraph(node.id, graph);
      setFilteredGraph(filtered);
    }
  };
  const handleSeeValue = () => {
    setSeeValues((prev) => !prev);
  };
  const resetFilter = () => {
    setFilteredGraph(graph);
  };

  if (isLoading || isLoadingFilters) {
    return (
      <Center mt={36}>
        <Spinner />
      </Center>
    );
  }

  if (error || errorFilters) {
    return <>Error</>;
  }

  return filteredGraph && dataFilters ? (
    <Box overflowX={"hidden"}>
      <Stack direction="column">
        <Stack direction="row" margin="0px 20px 0px 20px">
          <Button onClick={() => setSeeFilter((prev) => !prev)}>Filters</Button>
          <Button onClick={() => handleSeeValue()}>See Values</Button>
        </Stack>
        {seeFilter && (
          <FilterForm
            createFilterPostMutation={createFilterPostMutation}
            createRecommendPostMutation={createRecommendPostMutation}
            dataFilters={dataFilters}
            setFilters={setFilters}
            filters={filters}
            setFilteredGraph={setFilteredGraph}
            transformDataToGraph={transformDataToGraph}
          />
        )}
        <ForceGraph3D
          graphData={filteredGraph}
          nodeLabel="name"
          nodeAutoColorBy="group"
          linkCurvature={0.25}
          onNodeClick={(node) => {
            if (node.link != "") {
              window.open(node.link, "_blank");
            }
          }}
          onNodeRightClick={handleNodeRightClick}
          onBackgroundClick={resetFilter} // Reset filter when clicking on the background
          // eslint-disable-next-line @typescript-eslint/no-explicit-any
          nodeThreeObject={(node: any) => {
            const sprite = new SpriteText(node.name);
            sprite.color = node.color;
            sprite.textHeight = 8;
            return sprite;
          }}
        />
      </Stack>
    </Box>
  ) : (
    <p>Loading graph...</p>
  );
};

export default HomeView;
