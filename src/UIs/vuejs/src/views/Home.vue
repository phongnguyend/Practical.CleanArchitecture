<template>
  <div class="card">
    <div class="card-header">{{ appendVersion(pageTitle) }}
      <div style="float: right;color: green">
        <app-timer></app-timer>
      </div>
    </div>
    <div class="card-body">
      <div class="container-fluid">
        <div class="text-center">
          <img src="../assets/logo.png" class="img-responsive center-block"
            style="max-height:300px;padding-bottom:10px" />
        </div>

        <div class="text-center">Developed by:</div>
        <div class="text-center">
          <h3>Phong Nguyen</h3>
        </div>
        <div class="text-center">@phongnguyend</div>
        <div class="text-center">
          <a href="https://github.com/phongnguyend/Practical.CleanArchitecture">Practical.CleanArchitecture</a>
        </div>
        <div class="text-center">
          <strong>vuejs v{{ version }}</strong>
        </div>
        <div class="text-center">{{ appendCurrentDateTime("") }}</div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, version } from 'vue'
import AppTimer from '../components/Timer.vue'

const pageTitle = ref('Welcome ClassifiedAds Vue')

const appendVersion = (value: string) => {
  return value + ' ' + version
}

const appendCurrentDateTime = (value: string) => {
  return value + ' ' + new Date()
}
</script>
