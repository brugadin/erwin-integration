<template>
  <div class="app-container">
    <CytoscapeGraph :erwinData="erwinData"></CytoscapeGraph>
    <div class="upload-container">
      <input type="file" @change="handleFileUpload" accept=".erwin" />
      <button @click="submitFile" :disabled="!selectedFile">Upload</button>
    </div>
  </div>
</template>

<script lang="ts">
import { defineComponent, ref } from "vue";
import CytoscapeGraph from "./components/CytoscapeGraph.vue"; // Adjust the path as necessary

export default defineComponent({
  name: "App",
  components: {
    CytoscapeGraph,
  },
  setup() {
    const erwinData = ref([]);
    const selectedFile = ref<File | null>(null);

    function handleFileUpload(event: Event) {
      const target = event.target as HTMLInputElement;
      if (target.files?.length) {
        selectedFile.value = target.files[0];
      }
    }

    async function submitFile() {
      if (selectedFile.value) {
        const formData = new FormData();
        formData.append("file", selectedFile.value);

        try {
          const response = await fetch(
            "https://6f87-84-85-61-153.ngrok-free.app/erwindata",
            {
              method: "POST",
              body: formData,
            }
          );
          if (!response.ok) {
            throw new Error("Network response was not ok");
          }
          const data = await response.json();
          erwinData.value = data; // Update your component's data with the response
        } catch (error) {
          console.error("Error uploading Erwin file:", error);
        }
      }
    }

    return {
      erwinData,
      handleFileUpload,
      submitFile,
      selectedFile,
    };
  },
});
</script>

<style>
.app-container {
  display: flex;
  flex-direction: column;
  align-items: center;
}

.upload-container {
  margin-top: 20px;
}
</style>
