<template>
  <span>
    {{ years }}-{{ months < 10 ? "0" + months : months }}-{{ days < 10 ? "0" + days : days }}{{ " " }} {{ hours < 10
        ? " 0" + hours : hours
    }}:{{ minutes < 10 ? "0" + minutes : minutes }}: {{ seconds < 10 ? "0" + seconds : seconds
}} </span>
</template>
<script lang="ts">
import { defineComponent } from "vue";

export default defineComponent({
  data() {
    return {
      interval: 0 as any,
      years: 0,
      months: 0,
      days: 0,
      hours: 0,
      minutes: 0,
      seconds: 0
    };
  },
  computed: {
  },
  methods: {
    getTime() {
      const currentDateTime = new Date();
      this.years = currentDateTime.getFullYear();
      this.months = currentDateTime.getMonth() + 1;
      this.days = currentDateTime.getDate();
      this.hours = currentDateTime.getHours();
      this.minutes = currentDateTime.getMinutes();
      this.seconds = currentDateTime.getSeconds();
    }
  },
  created() {
    this.interval = setInterval(() => {
      this.getTime();
    }, 1000);
  },
  unmounted() {
    clearInterval(this.interval);
  }
});
</script>
<style scoped>

</style>
