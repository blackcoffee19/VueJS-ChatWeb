import { createRouter, createWebHistory } from 'vue-router'
const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: '/',
      component: ()=> import("@/layouts/HomeLayout.vue"),
      children:[
        {
          path: "/",
          name: "home",
          component: ()=> import("@/views/HomeView.vue")
        },
        {
          path: "/search",
          name: "search",
          component: () => import("@/views/SearchView.vue")
        },
        {
          path: '/notification',
          name: 'notification',
          component: () => import('@/views/NotificationsView.vue'),
        },
        {
          path: '/videocall',
          name: 'video call',
          component: () => import('@/views/VideoCallView.vue')
        },
        {
          path: '/profile',
          name: "profile",
          component: () => import('@/views/ProfileView.vue')
        }
      ]
    },
    {
      path: '/about',
      name: 'about',
      // route level code-splitting
      // this generates a separate chunk (About.[hash].js) for this route
      // which is lazy-loaded when the route is visited.
      component: () => import('../views/AboutView.vue'),
    },
    {
      path: "/login",
      component: ()=> import("@/views/LoginView.vue")
    }
  ],
})

export default router
