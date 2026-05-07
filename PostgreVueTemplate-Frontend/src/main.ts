import App from "./App.vue";
import { createApp, markRaw } from "vue";
import { router } from "./router/index";
import { i18n } from "./i18n/index";
import { createPinia } from "pinia";
import "./themes/theme.css";
import "virtual:uno.css";
import { useRealTimeStore } from "./stores/realtimeStore";
import { useAuthStore } from "./stores/authStore";
import TooltipDirective from "./directives/vTooltip";

const app = createApp(App);

app.use(router);
app.use(i18n);
app.use(TooltipDirective);

const pinia = createPinia();
pinia.use(({ store }) =>
{
	store.router = markRaw(router);
});
app.use(pinia);

// const realtimeStore = useRealTimeStore();
// realtimeStore.startService();

const authStore = useAuthStore();
authStore.checkLogin();

app.mount("#app");
