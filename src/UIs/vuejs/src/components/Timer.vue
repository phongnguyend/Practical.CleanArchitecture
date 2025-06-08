<template>
  <span>
    {{ years }}-{{ months < 10 ? "0" + months : months }}-{{ days < 10 ? "0" + days : days }}{{ " " }}
    {{ hours < 10 ? " 0" + hours : hours }}:{{ minutes < 10 ? "0" + minutes : minutes }}:{{ seconds < 10 ? "0" + seconds : seconds }}
  </span>
</template>

<script setup lang="ts">
import { ref, onMounted, onUnmounted } from 'vue'

const years = ref(0)
const months = ref(0)
const days = ref(0)
const hours = ref(0)
const minutes = ref(0)
const seconds = ref(0)
let interval: NodeJS.Timeout | null = null

const getTime = () => {
  const currentDateTime = new Date()
  years.value = currentDateTime.getFullYear()
  months.value = currentDateTime.getMonth() + 1
  days.value = currentDateTime.getDate()
  hours.value = currentDateTime.getHours()
  minutes.value = currentDateTime.getMinutes()
  seconds.value = currentDateTime.getSeconds()
}

onMounted(() => {
  interval = setInterval(() => {
    getTime()
  }, 1000)
})

onUnmounted(() => {
  if (interval) {
    clearInterval(interval)
  }
})
</script>

<style scoped>

</style>
