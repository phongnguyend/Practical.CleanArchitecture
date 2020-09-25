<template>
  <div id="app">
    <div class="container">
      <app-nav />
      <router-view />
    </div>
  </div>
</template>

<script>
import Nav from "./views/Nav.vue";
export default {
  components: {
    appNav: Nav
  }
};
</script>

<style lang="scss">
div.card-header {
  font-size: large;
}

div.card {
  margin-top: 10px;
}

.table {
  margin-top: 10px;
}
</style>
