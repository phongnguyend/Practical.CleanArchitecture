<template>
  <div class="crop" :style="{width: starWidth + 'px'}" :title="rating" @click="onClick">
    <div style="width: 75px">
      <span class="fa fa-star"></span>
      <span class="fa fa-star"></span>
      <span class="fa fa-star"></span>
      <span class="fa fa-star"></span>
      <span class="fa fa-star"></span>
    </div>
  </div>
</template>
<script>
export default {
  props: ["rating"],
  data() {
    return {};
  },
  computed: {
    starWidth() {
      return (this.rating * 75) / 5;
    }
  },
  methods: {
    onClick() {
      this.$emit("ratingClicked", `The rating ${this.rating} was clicked!`);
    }
  }
};
</script>
<style scoped>
.crop {
  overflow: hidden;
}
div {
  cursor: pointer;
}
</style>