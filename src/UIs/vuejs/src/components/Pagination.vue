<template>
  <ul class="pagination">
    <li
      class="page-item"
      :class="{
        disabled: currentPage == 1,
      }"
    >
      <a class="page-link" @click="selectPage(1)">First</a>
    </li>
    <li
      class="page-item"
      :class="{
        disabled: currentPage == 1,
      }"
    >
      <a class="page-link" @click="selectPage(currentPage - 1)">Previous</a>
    </li>

    <li
      v-for="pageNumber of pageNumbers"
      :key="pageNumber"
      class="page-item"
      :class="{
        active: currentPage == pageNumber,
      }"
    >
      <a class="page-link" @click="selectPage(pageNumber)">{{ pageNumber }}</a>
    </li>

    <li
      class="page-item"
      :class="{
        disabled: currentPage == totalPages,
      }"
    >
      <a class="page-link" @click="selectPage(currentPage + 1)">Next</a>
    </li>
    <li
      class="page-item"
      :class="{
        disabled: currentPage == totalPages,
      }"
    >
      <a class="page-link" @click="selectPage(totalPages)">Last</a>
    </li>
  </ul>
</template>
<script>
export default {
  props: ["totalItems", "pageSize", "currentPage"],
  data() {
    return {};
  },
  computed: {
    totalPages() {
      return Math.ceil(this.totalItems / this.pageSize);
    },
    pageNumbers() {
      const totalPages = Math.ceil(this.totalItems / this.pageSize);

      let startIndex = this.currentPage - 2;
      let endIndex = this.currentPage + 2;

      if (startIndex < 1) {
        endIndex = endIndex + (1 - startIndex);
        startIndex = 1;
      }

      if (endIndex > totalPages) {
        startIndex = startIndex - (endIndex - totalPages);
        endIndex = totalPages;
      }

      startIndex = Math.max(startIndex, 1);
      endIndex = Math.min(endIndex, totalPages);

      const pageNumbers = [];

      for (let i = startIndex; i <= endIndex; i++) {
        pageNumbers.push(i);
      }
      return pageNumbers;
    },
  },
  methods: {
    selectPage(page) {
      this.$emit("pageSelected", page);
    },
  },
};
</script>
<style scoped>
a {
  cursor: pointer;
}
</style>
