<template>
  <div id="app">
    <Busca />
  </div>
</template>

<script>
import Busca from "./components/Busca.vue";

export default {
  name: "App",
  components: {
    Busca
  }
};
</script>

<style>
#app {
  font-family: Avenir, Helvetica, Arial, sans-serif;
  -webkit-font-smoothing: antialiased;
  -moz-osx-font-smoothing: grayscale;
  text-align: center;
  color: #2c3e50;
  margin-top: 30px;
}
</style>
