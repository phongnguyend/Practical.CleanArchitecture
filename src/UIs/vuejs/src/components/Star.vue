<template>
  <div class="crop" :style="{width: starWidth + 'px'}" :title="`${rating}`" @click="onClick">
    <div style="width: 75px">
      <Star v-for="index in 5" :key="index" :size="15" fill="currentColor" aria-hidden="true" />
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue'
import { Star } from 'lucide-vue-next'

interface Props {
  rating: number
}

const props = defineProps<Props>()
const emit = defineEmits<{
  ratingClicked: [message: string]
}>()

const starWidth = computed(() => {
  return (props.rating * 75) / 5
})

const onClick = () => {
  emit('ratingClicked', `The rating ${props.rating} was clicked!`)
}
</script>

<style scoped>
.crop {
  overflow: hidden;
}
div {
  cursor: pointer;
}
</style>
