<template>
  <div class="crop" :style="{width: starWidth + 'px'}" :title="`${rating}`" @click="onClick">
    <div style="width: 75px">
      <span class="fa fa-star"></span>
      <span class="fa fa-star"></span>
      <span class="fa fa-star"></span>
      <span class="fa fa-star"></span>
      <span class="fa fa-star"></span>
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue'

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