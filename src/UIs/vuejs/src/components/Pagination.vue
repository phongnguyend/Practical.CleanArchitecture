<template>
  <ul class="pagination">
    <li
      class="page-item"
      :class="{
        disabled: currentPage <= 1,
      }"
    >
      <button type="button" class="page-link" :disabled="currentPage <= 1" @click="selectPage(1)" aria-label="First page" title="First page"><ActionIcon action="first" /><span class="page-nav-label">First</span></button>
    </li>
    <li
      class="page-item"
      :class="{
        disabled: currentPage <= 1,
      }"
    >
      <button type="button" class="page-link" :disabled="currentPage <= 1" @click="selectPage(currentPage - 1)" aria-label="Previous page" title="Previous page"><ActionIcon action="previous" /><span class="page-nav-label">Previous</span></button>
    </li>

    <li
      v-for="pageNumber in pageNumbers"
      :key="pageNumber"
      class="page-item"
      :class="{
        active: currentPage === pageNumber,
      }"
    >
      <button type="button" class="page-link" :aria-current="currentPage === pageNumber ? 'page' : undefined" @click="selectPage(pageNumber)">{{ pageNumber }}</button>
    </li>

    <li
      class="page-item"
      :class="{
        disabled: currentPage >= totalPages,
      }"
    >
      <button type="button" class="page-link" :disabled="currentPage >= totalPages" @click="selectPage(currentPage + 1)" aria-label="Next page" title="Next page"><ActionIcon action="next" /><span class="page-nav-label">Next</span></button>
    </li>
    <li
      class="page-item"
      :class="{
        disabled: currentPage >= totalPages,
      }"
    >
      <button type="button" class="page-link" :disabled="currentPage >= totalPages" @click="selectPage(totalPages)" aria-label="Last page" title="Last page"><ActionIcon action="last" /><span class="page-nav-label">Last</span></button>
    </li>
  </ul>
</template>

<script setup lang="ts">
import { computed } from 'vue'

interface Props {
  totalItems: number
  pageSize: number
  currentPage: number
}

const props = defineProps<Props>()
const emit = defineEmits<{
  pageSelected: [page: number]
}>()

const totalPages = computed(() => {
  return Math.ceil(props.totalItems / props.pageSize)
})

const pageNumbers = computed(() => {
  const total = Math.ceil(props.totalItems / props.pageSize)

  let startIndex = props.currentPage - 2
  let endIndex = props.currentPage + 2

  if (startIndex < 1) {
    endIndex = endIndex + (1 - startIndex)
    startIndex = 1
  }

  if (endIndex > total) {
    startIndex = Math.max(1, startIndex - (endIndex - total))
    endIndex = total
  }

  const pages = []
  for (let i = startIndex; i <= endIndex; i++) {
    pages.push(i)
  }

  return pages
})

const selectPage = (page: number): void => {
  if (page < 1 || page > totalPages.value || page === props.currentPage) {
    return
  }
  emit('pageSelected', page)
}
</script>

<style scoped>
a {
  cursor: pointer;
}

.page-item.disabled a {
  cursor: not-allowed;
}
</style>
