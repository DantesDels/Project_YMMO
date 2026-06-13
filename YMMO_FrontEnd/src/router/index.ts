import { createRouter, createWebHistory } from 'vue-router'
import { useAuthentificationStore } from '@/stores/authentification.store'
import PublicLayout from '@/layouts/PublicLayout.vue'

const router = createRouter({
    history: createWebHistory(),
    scrollBehavior: () => ({ top: 0 }),

    routes: [
        {
            path: '/',
            component: PublicLayout,
            children: [
                { path: '', name: 'home', component: () => import('@/views/HomeView.vue') },

                // ── Auth ──
                { path: 'authentification', name: 'authentification', component: () => import('@/views/authentification/Authentification.vue') },

                // ── Header pages ──
                { path: 'catalog', name: 'catalog', component: () => import('@/views/headerLayout/CatalogView.vue') },
                { path: 'informations', name: 'informations', component: () => import('@/views/headerLayout/InformationsView.vue') },
                {
                    path: 'profile',
                    name: 'profile',
                    component: () => import('@/views/headerLayout/UserView.vue'),
                    meta: { requiresAuthentification: true },
                },
                {
                    path: 'dashboard',
                    name: 'dashboard',
                    component: () => import('@/views/DashboardView.vue'),
                    meta: { requiresAuthentification: true },
                },

                // ── Footer pages ──
                { path: 'help', name: 'help', component: () => import('@/views/footerLayout/HelpView.vue') },
                { path: 'cgu', name: 'cgu', component: () => import('@/views/footerLayout/CguView.vue') },
                { path: 'legal-mentions', name: 'legal-mentions', component: () => import('@/views/footerLayout/LegalMentionsView.vue') },
                { path: 'confidentiality', name: 'confidentiality', component: () => import('@/views/footerLayout/ConfidentialityView.vue') },

                // ── Property detail ──
                {
                    path: 'property/:id',
                    name: 'property-detail',
                    component: () => import('@/views/PropertyDetailView.vue'),
                },

                // ── Support ──
                {
                    path: 'support',
                    name: 'support',
                    component: () => import('@/views/SupportTicketFormView.vue'),
                    meta: { requiresAuthentification: true },
                },

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

                // ── Fallback 404 ──
                {
                    path: ':pathMatch(.*)*',
                    name: 'not-found',
                    component: () => import('@/views/NotFoundView.vue'),
                },
            ],
        },
    ],
})

router.beforeEach((to, from, next) => {
    const authStore = useAuthentificationStore()
    const token = localStorage.getItem('token')

    if (to.meta.requiresAuthentification && !token) {
        next({ name: 'authentification', query: { redirect: to.fullPath } })
        return
    }

    if (to.meta.requiredRole && authStore.user?.role !== to.meta.requiredRole) {
        next({ name: 'home' })
        return
    }

    next()
})

export default router
