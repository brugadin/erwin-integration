<template>
  <CytoscapeGraph :erwinData="erwinData"></CytoscapeGraph>
</template>

<script lang="ts">
import { defineComponent, ref, onMounted } from "vue";
import CytoscapeGraph from "./components/CytoscapeGraph.vue"; // Adjust the path as necessary

export default defineComponent({
  name: "App",
  components: {
    CytoscapeGraph,
  },
  setup() {
    const erwinData = ref([]);

    async function fetchData() {
      try {
        const response = await fetch("http://localhost:5155/erwindata");
        if (!response.ok) {
          throw new Error("Network response was not ok");
        }
        const data = await response.json();
        erwinData.value = data; // Make sure this matches the expected structure for your component
      } catch (error) {
        console.error("Error fetching Erwin data:", error);
      }
    }

    onMounted(fetchData);

    return {
      erwinData,
    };
  },
});
</script>
