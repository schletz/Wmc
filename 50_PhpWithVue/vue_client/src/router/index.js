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
      path: '/login',
      name: 'login',
      // route level code-splitting
      // this generates a separate chunk (About.[hash].js) for this route
      // which is lazy-loaded when the route is visited.
      component: () => import('../views/LoginView.vue')
    },
    {
      path: '/logout',
      name: 'logout',
      meta: { authorize: true },
      component: () => import('../views/LogoutView.vue')
    },    
    {
      path: '/register',
      name: 'register',
      component: () => import('../views/RegisterView.vue')
    },        
    {
      path: '/profile',
      name: 'profile',
      // meta allows you to add custom information. We use authorize, if the route needs an
      // authorized user.
      meta: { authorize: true },
      component: () => import('../views/ProfileView.vue'),
    },
    {
      path: '/articles',
      name: 'articles',
      component: () => import('../views/ArticlesView.vue'),
    }       
  ]
});

router.beforeEach((to, from, next) => {
  const authenticated = store.state.user ? true : false;
  if (to.meta.authorize && !authenticated) {
    next("/");
    return;
  }
  next();
  return;
});

export default router;
