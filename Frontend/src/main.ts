import "./assets/main.css";

import { createApp } from "vue";
import { createPinia } from "pinia";

import { useUserStore } from "@/stores/userStore";
import App from "./App.vue";
import router from "./router";
const pinia = createPinia();
const app = createApp(App);

app.use(pinia);
app.use(router);

const userStore = useUserStore();

if (userStore.refreshToken) {
	await userStore.refreshAccessToken();

	if (userStore.accessToken) {
		await userStore.fetchUser();
	}
}

app.mount("#app");
