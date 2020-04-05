import Vue from 'vue'
import VueRouter from 'vue-router'
import Home from '../views/Home.vue'
import OidcLoginRedirect from "../auth/OidcLoginRedirect.vue"

Vue.use(VueRouter)

const routes = [
  {
    path: '/',
    name: 'Home',
    component: Home
  },
  {
    path: '/oidc-login-redirect',
    name: 'OidcLoginRedirect',
    component: OidcLoginRedirect
  },
  {
    path: '/products',
    name: 'Product',
    component: () => import('../views/products/List.vue')
  },
  {
    path: '/products/add',
    name: 'AddProduct',
    component: () => import('../views/products/Edit.vue')
  },
  {
    path: '/products/edit/:id',
    name: 'EditProduct',
    component: () => import('../views/products/Edit.vue')
  }, {
    path: '/products/:id',
    name: 'ProductDetail',
    component: () => import('../views/products/Detail.vue')
  },
]

const router = new VueRouter({
  mode: 'history',
  base: process.env.BASE_URL,
  routes
})

export default router
