<template>
  <!-- This component doesn't render anything visible, it only handles notifications -->
</template>

<script setup lang="ts">
import { onMounted } from 'vue'
import { HubConnectionBuilder } from '@microsoft/signalr'
import { useToast } from 'vue-toastification'

import authService from '../auth/authService'
import env from '../../environments'

const toast = useToast()

onMounted(() => {
  const connection = new HubConnectionBuilder()
    .withUrl(env.ResourceServer.NotificationEndpoint, {
      accessTokenFactory: () => authService.getAccessToken(),
    })
    .withAutomaticReconnect()
    .build()

  connection.start().then(
    function () {
      toast.success('Connected to NotificationHub', {
        timeout: 2000,
      })
    },
    function () {
      toast.error('Cannot connect to NotificationHub: ' + env.ResourceServer.NotificationEndpoint)
    },
  )

  connection.on('ReceiveMessage', (message) => {
    toast.info('Received Message from NotificationHub: ' + message)
  })
})
</script>
