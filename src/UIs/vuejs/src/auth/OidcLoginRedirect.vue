<template>
  <div>Loading ...</div>
</template>

<script setup lang="ts">
import { onMounted } from 'vue'
import { UserManager, WebStorageStateStore, type UserManagerSettings } from 'oidc-client-ts'
import env from '../../environments'

const config: UserManagerSettings = {
  authority: env.OpenIdConnect.Authority,
  client_id: env.OpenIdConnect.ClientId,
  userStore: new WebStorageStateStore({ store: window.localStorage }),
  response_mode: 'query'
}

const mgr = new UserManager(config)

onMounted(() => {
  mgr.signinRedirectCallback().then(
    () => {
      window.history.replaceState(
        {},
        window.document.title,
        window.location.origin
      )

      const returnUrl = localStorage.getItem('returnUrl')
      if (returnUrl) {
        localStorage.removeItem('returnUrl')
        window.location.href = returnUrl
      } else {
        window.location.href = '/'
      }
    },
    (error) => {
      console.error('OIDC signin redirect callback error:', error)
    }
  )
})
</script>
