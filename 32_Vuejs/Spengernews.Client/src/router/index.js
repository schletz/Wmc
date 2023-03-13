import { createRouter, createWebHistory } from 'vue-router'
import store from '../store.js'
import HomeView from '../views/HomeView.vue'

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: '/',
      name: 'home',
      component: HomeView,
    },
    {
      path: '/news/:newsid',
      component: HomeView,
    },
    {
      path: '/addcategory',
      name: "addcategory",
      meta: { authorize: true },
      component: () => import('../views/AddCategoryView.vue')
    },    
    {
      path: '/about',
      name: 'about',
      // route level code-splitting
      // this generates a separate chunk (About.[hash].js) for this route
      // which is lazy-loaded when the route is visited.
      component: () => import('../views/AboutView.vue')
    },
    {
      path: '/write',
      name: 'write',
      // meta allows you to add custom information. We use authorize, if the route needs an
      // authorized user.
      meta: { authorize: true },
      component: () => import('../views/CreateArticleView.vue'),
    }
  ]
});

router.beforeEach((to, from, next) => {
  const authenticated = store.state.user.isLoggedIn;
  if (to.meta.authorize && !authenticated) {
    next("/");
    return;
  }
  next();
  return;
});

export default router;
