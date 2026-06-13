import { createApp } from 'vue'
import { createPinia } from 'pinia'
import App    from './App.vue'
import router from './router'
import './assets/css/style.css'
import { useAuthentificationStore } from './stores/authentification.store'

const app = createApp(App)

const pinia = createPinia()
app.use(pinia)

useAuthentificationStore().restoreSession()

app.use(router)
app.mount('#app')
