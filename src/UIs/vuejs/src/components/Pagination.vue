<template>
  <ul class="pagination">
    <li
      class="page-item"
      :class="{
        disabled: currentPage === 1,
      }"
    >
      <a class="page-link" @click="selectPage(1)">First</a>
    </li>
    <li
      class="page-item"
      :class="{
        disabled: currentPage === 1,
      }"
    >
      <a class="page-link" @click="selectPage(currentPage - 1)">Previous</a>
    </li>

    <li
      v-for="pageNumber in pageNumbers"
      :key="pageNumber"
      class="page-item"
      :class="{
        active: currentPage === pageNumber,
      }"
    >
      <a class="page-link" @click="selectPage(pageNumber)">{{ pageNumber }}</a>
    </li>

    <li
      class="page-item"
      :class="{
        disabled: currentPage === totalPages,
      }"
    >
      <a class="page-link" @click="selectPage(currentPage + 1)">Next</a>
    </li>
    <li
      class="page-item"
      :class="{
        disabled: currentPage === totalPages,
      }"
    >
      <a class="page-link" @click="selectPage(totalPages)">Last</a>
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
