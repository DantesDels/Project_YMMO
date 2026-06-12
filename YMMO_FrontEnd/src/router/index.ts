import { createRouter, createWebHistory } from 'vue-router'
import { useAuthentificationStore } from '@/stores/authentification.store';
import PublicLayout from '@/layouts/PublicLayout.vue'
import HomeView from '@/views/HomeView.vue'
import AuthentificationView from '@/views/authentification/Authentification.vue'
import DashboardView from '@/views/DashboardView.vue'

const router = createRouter({
    history: createWebHistory(),
    routes: [
        {
            path: '/',
            component: PublicLayout,
            children: [
                { path: '', name: 'home', component: HomeView },
                { path: 'home', redirect: '/' },
                { path: 'authentification', component: AuthentificationView },
                { path: 'dashboard', name: 'dashboard', component: DashboardView },
                {
                    path: 'portfolio/new',
                    name: 'agent-property-create',
                    component: () => import('@/views/agent/PropertyFormView.vue'),
                    meta: {
                        requiresAuthentification: true,
                        requiredRole: 'Agent'
                    }
                }
            ]
        }
    ]
})

router.beforeEach((to, from, next) => {
    const authStore = useAuthentificationStore();
    const token = localStorage.getItem('token');

    if (to.meta.requiresAuthentification && !token) {
        next({ name: 'authentification' });
    }
    else if (to.meta.requiredRole && authStore.user?.role !== to.meta.requiredRole) {
        next({ name: 'home' });
    }
    else {
        next();
    }
});

export default router