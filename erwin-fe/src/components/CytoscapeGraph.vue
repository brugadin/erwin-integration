<script lang="ts">
import { defineComponent, onMounted, ref, watch, onBeforeUnmount } from "vue";
import cytoscape, { ElementsDefinition } from "cytoscape";

interface Relationship {
  Name: string;
  Id: string;
  ParentId: string;
  ChildId: string;
}

interface Entity {
  Name: string;
  Id: string;
  Relationships: Relationship[];
}

export default defineComponent({
  name: "CytoscapeGraph",
  props: {
    erwinData: {
      type: Array as () => Entity[],
      required: true,
    },
  },
  setup(props) {
    const cyElement = ref<HTMLElement | null>(null);

    // Transform Erwin data to Cytoscape elements
    const transformData = (data: Entity[]): ElementsDefinition => {
      const nodes = data.map((entity) => ({
        data: { id: entity.Id, label: entity.Name },
      }));

      const edges = data.flatMap((entity) =>
        entity.Relationships.map((rel) => ({
          data: {
            id: rel.Id,
            source: rel.ParentId,
            target: rel.ChildId,
            label: rel.Name,
          },
        }))
      );

      return { nodes, edges };
    };

    let cy: cytoscape.Core;

    onMounted(() => {
      const elements = transformData(props.erwinData);

      if (cyElement.value) {
        cy = cytoscape({
          container: cyElement.value,
          elements,
          style: [
            // Define your cytoscape styles here
            {
              selector: "node",
              style: {
                "background-color": "#666",
                label: "data(label)",
              },
            },
            {
              selector: "edge",
              style: {
                width: 3,
                "line-color": "#ccc",
                "target-arrow-color": "#ccc",
                "target-arrow-shape": "triangle",
                "curve-style": "bezier",
                label: "data(label)",
              },
            },
          ],
          layout: {
            name: "grid",
          },
        });
      }
    });

    watch(
      () => props.erwinData,
      (newData) => {
        const elements = transformData(newData);
        if (cy) {
          cy.elements().remove();
          cy.add(elements);
          cy.layout({ name: "grid" }).run();
        }
      },
      { deep: true }
    );

    onBeforeUnmount(() => {
      if (cy) {
        cy.destroy();
      }
    });

    return {
      cyElement,
    };
  },
});
</script>

<template>
  <div ref="cyElement" class="cy-container"></div>
</template>

<style>
.cy-container {
  width: 100%;
  height: 600px; /* Adjust the height as needed */
}
</style>
