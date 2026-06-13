import { createRouter, createWebHistory } from 'vue-router'
import { useAuthentificationStore } from '@/stores/authentification.store'

const router = createRouter({
    history: createWebHistory(),
    routes: [
        {
            path: '/',
            component: () => import('@/layouts/PublicLayout.vue'),
            children: [
                { path: '', name: 'home', component: () => import('@/views/HomeView.vue') },
                { path: 'authentification', name: 'authentification', component: () => import('@/views/authentification/Authentification.vue') },
                { path: 'dashboard', name: 'dashboard', component: () => import('@/views/DashboardView.vue') },

                // ── Client ──
                {
                    path: 'client/dashboard',
                    name: 'client-dashboard',
                    component: () => import('@/views/ClientDashboardView.vue'),
                    meta: { requiresAuthentification: true, requiredRole: 'Client' },
                },
                {
                    path: 'client/sell',
                    name: 'client-sell',
                    component: () => import('@/views/ClientSellPropertyView.vue'),
                    meta: { requiresAuthentification: true, requiredRole: 'Client' },
                },

                // ── Agent ──
                {
                    path: 'agent/dashboard',
                    name: 'agent-dashboard',
                    component: () => import('@/views/agent/AgentDashboardView.vue'),
                    meta: { requiresAuthentification: true, requiredRole: 'Agent' },
                },
                {
                    path: 'portfolio/new',
                    name: 'agent-property-create',
                    component: () => import('@/views/agent/PropertyFormView.vue'),
                    meta: { requiresAuthentification: true, requiredRole: 'Agent' },
                },
            ],
        },
    ],
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
