<template>
  <ToastContainer />
</template>

<script setup lang="ts">
import { onMounted, onBeforeUnmount } from 'vue'
import { HubConnectionBuilder } from '@microsoft/signalr'
import ToastContainer from './Toast/ToastContainer.vue'
import { toast } from './Toast/toast'

import authService from '../auth/authService'
import env from '../../environments'

let cleanup: (() => void) | undefined
onBeforeUnmount(() => cleanup?.())

onMounted(() => {
  const connection = new HubConnectionBuilder()
    .withUrl(env.ResourceServer.NotificationEndpoint, {
      accessTokenFactory: () => authService.getAccessToken(),
    })
    .withAutomaticReconnect()
    .build()

  let disposed = false
  cleanup = () => {
    disposed = true
    void connection.stop()
  }
  connection.start().then(
    function () {
      if (disposed) return
      toast.success('Connected to NotificationHub', {
        duration: 2000,
      })
    },
    function () {
      if (disposed) return
      toast.error('Cannot connect to NotificationHub: ' + env.ResourceServer.NotificationEndpoint)
    },
  )

  connection.on('ReceiveMessage', (message) => {
    toast.info('Received Message from NotificationHub: ' + message)
  })
})
</script>
