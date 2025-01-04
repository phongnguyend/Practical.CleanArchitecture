<script>
import { HubConnectionBuilder } from '@microsoft/signalr'
import { useToast } from 'vue-toastification'

import authService from '../auth/authService'
import env from '../../environments'

export default {
  setup() {
    const toast = useToast()
    return { toast }
  },
  mounted() {
    const connection = new HubConnectionBuilder()
      .withUrl(env.ResourceServer.NotificationEndpoint, {
        accessTokenFactory: () => authService.getAccessToken(),
      })
      .withAutomaticReconnect()
      .build()

    let vm = this

    connection.start().then(
      function () {
        // console.log("Connected to NotificationHub");
        vm.toast.success('Connected to NotificationHub', {
          timeout: 2000,
        })
      },
      function () {
        // console.log(
        //   "Cannot connect to NotificationHub: " +
        //     env.ResourceServer.NotificationEndpoint
        // );
        vm.toast.error(
          'Cannot connect to NotificationHub: ' + env.ResourceServer.NotificationEndpoint,
        )
      },
    )

    connection.on('ReceiveMessage', (message) => {
      // console.log("Received Message from NotificationHub: " + message);
      vm.toast.info('Received Message from NotificationHub: ' + message)
    })
  },
}
</script>
