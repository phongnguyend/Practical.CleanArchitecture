<template>
  <h1>{{ name }}</h1><p>{{ push ? "Queue a notification for your own registered devices." : "Queue a message for delivery by the existing " + name + " worker." }}</p>
  <div v-if="info" role="status" class="alert alert-info">Provider: <strong>{{ info.provider }}</strong>. <template v-if="push">Registered devices: {{ info.deviceCount }}. </template>{{ info.provider === 'Fake' ? 'Delivery is simulated; no message is sent.' : 'Queued messages may be delivered to the specified recipient.' }}</div>
  <p v-if="error" role="alert" class="alert alert-danger">{{ error }}</p><p v-if="success" role="status" class="alert alert-success">{{ success }}</p>
  <form novalidate @submit.prevent="queue" class="d-grid gap-3" style="max-width:720px">
    <div v-if="email"><label for="message-from" class="form-label">From</label><input id="message-from" type="email" class="form-control" maxlength="254" v-model="from" :disabled="busy" required /></div>
    <div v-if="!push"><label for="message-recipient" class="form-label">{{ email ? 'To' : 'Phone number' }}</label><input id="message-recipient" :type="email ? 'email' : 'tel'" class="form-control" :maxlength="email ? 254 : 16" v-model="recipient" :disabled="busy" required /><small v-if="!email">International format, for example +15551234567.</small></div>
    <div v-if="email || push"><label for="message-subject" class="form-label">{{ push ? 'Title' : 'Subject' }}</label><input id="message-subject" class="form-control" :maxlength="push ? 100 : 200" v-model="subject" :disabled="busy" required /></div>
    <div><label for="message-body" class="form-label">Message</label><textarea id="message-body" class="form-control" rows="5" :maxlength="push ? 500 : email ? 10000 : 1600" v-model="body" :disabled="busy" required></textarea><small>{{ body.length }}/{{ push ? 500 : email ? 10000 : 1600 }} characters</small></div>
    <div v-if="push"><label for="message-destination" class="form-label">Destination</label><select id="message-destination" class="form-select" v-model="section" :disabled="busy"><option v-for="value in ['notifications', 'home', 'products', 'files', 'users', 'roles', 'configurationentries', 'auditlogentries']" :key="value" :value="value">{{ value }}</option></select></div>
    <div class="d-flex flex-wrap gap-2"><button type="submit" class="btn btn-primary" :disabled="busy || !info"><ActionIcon action="notification" />{{ busy ? 'Queueing…' : 'Queue message' }}</button><button type="button" class="btn btn-outline-secondary" :disabled="busy" @click="refresh"><ActionIcon action="refresh" />Refresh queue</button></div>
  </form>
  <h2 class="mt-4">Recent messages</h2><p v-if="info?.items.length === 0">No queued messages yet.</p>
  <div class="d-grid gap-3"><article v-for="item in info?.items" :key="item.id" class="border rounded p-3"><h3 class="h5">{{ item.title }}</h3><p v-if="!push">To: {{ item.recipient }}</p><p style="white-space:pre-wrap;overflow-wrap:anywhere">{{ item.body }}</p><p class="mb-0"><template v-if="push">Pending: {{ item.pending }} / Completed: {{ item.completed }} / Failed: {{ item.failed }}</template><template v-else>{{ item.status }} / Attempts: {{ item.attempts }}</template> / {{ new Date(item.createdAt).toLocaleString() }}</p></article></div>
</template>
<script setup lang="ts">
import { computed, ref, watch } from 'vue'
import axios from 'axios'
import env from '../../../environments'
import addAuthInterceptors from '../../auth/authInterceptors'
import ActionIcon from '../../components/ActionIcon.vue'
const props = defineProps<{ channel: string }>()
const push = computed(() => props.channel === 'mobilepush')
const path = computed(() => push.value ? 'playground' : 'playgrounds/' + props.channel)
const section = ref('notifications')
const name = computed(() => push.value ? 'Mobile Push' : email.value ? 'Email' : 'SMS')
const email = computed(() => props.channel === 'email')
const client = axios.create({ baseURL: env.ResourceServer.Endpoint + 'notifications/' })
addAuthInterceptors(client)
type Item = { pending?: number; completed?: number; failed?: number; id: string; title: string; body: string; recipient: string; createdAt: string; status: string; attempts: number }
const from = ref(''), recipient = ref(''), subject = ref(''), body = ref(''), error = ref(''), success = ref(''), busy = ref(false)
const info = ref<{ provider: string; deviceCount?: number; items: Item[] }>()
async function refresh() {
  try {
    info.value = (await client.get(path.value)).data
    error.value = ''
  } catch {
    error.value = 'Unable to load the queue. Please try again.'
  }
}
watch(() => props.channel, () => {
  from.value = recipient.value = subject.value = body.value = success.value = ''
  void refresh()
}, { immediate: true })
async function queue() {
  if (busy.value) {
    return
  }
  success.value = ''
  if ((!push.value && !recipient.value.trim()) || !body.value.trim() || ((email.value || push.value) && !subject.value.trim()) || (email.value && !from.value.trim())) {
    error.value = push.value ? 'Title and message are required.' : 'Complete all required fields.'
    return
  }
  const pattern = /^[^\s@;]+@[^\s@;]+\.[^\s@;]+$/
  if (!push.value && (email.value ? !pattern.test(from.value.trim()) || !pattern.test(recipient.value.trim()) : !/^\+[1-9][0-9]{7,14}$/.test(recipient.value.trim()))) {
    error.value = email.value ? 'Enter valid From and To email addresses.' : 'Enter a phone number in international format, for example +15551234567.'
    return
  }
  busy.value = true
  error.value = ''
  try {
    const result = await client.post(path.value, push.value ? { title: subject.value.trim(), body: body.value.trim(), section: section.value } : { from: from.value.trim(), recipient: recipient.value.trim(), subject: subject.value.trim(), body: body.value.trim() })
    success.value = `Message queued. ID: ${result.data.id}`
    await refresh()
  } catch {
    error.value = 'Unable to queue the message. Check the fields and try again.'
  } finally {
    busy.value = false
  }
}
</script>
