<template>
  <div id="app">
    <div class="container">
      <app-nav />
      <router-view />
      <app-notification />
    </div>
  </div>
</template>

<script lang="ts">
import Nav from "./views/Nav.vue";
import Notification from "./components/Notification.vue";
export default {
  components: {
    appNav: Nav,
    appNotification: Notification
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
