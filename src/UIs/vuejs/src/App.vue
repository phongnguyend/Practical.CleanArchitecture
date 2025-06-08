<template>
  <div id="app">
    <div class="container">
      <app-nav />
      <router-view />
      <app-notification />
    </div>
  </div>
</template>

<script setup lang="ts">
import AppNav from './views/Nav.vue'
import AppNotification from './components/Notification.vue'
</script>

<style lang="css">
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
