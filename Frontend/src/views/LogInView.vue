<script setup lang="ts">
import { ref, reactive } from "vue";
import { useRouter } from "vue-router";
import { authApi } from "@/services/authService";
import { useUserStore } from "@/stores/userStore";
import { useUIStore } from "@/stores/uiStore";
import { useAsync } from "@/composables/useAsync";
const isLogin = ref(true);
const form = reactive({
	email: "",
	password: "",
	confirmPassword: "",
});
const errors = reactive<{
	email?: string;
	password?: string;
	confirmPassword?: string;
}>({});
const router = useRouter();
const userStore = useUserStore();
const { triggerToast } = useUIStore();

const { execute, data, loading, error } = useAsync(() =>
	isLogin.value
		? authApi.login(form.email, form.password)
		: authApi.register(form.email, form.password)
);

function toggleMode() {
	isLogin.value = !isLogin.value;
	errors.email = errors.password = errors.confirmPassword = "";
}

function validateEmail(email: string): string | null {
	if (!email) return "Имейлът е задължителен.";
	const re = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
	if (!re.test(email)) return "Моля, въведете валиден имейл адрес.";
	return null;
}

function handleSubmit() {
	errors.email = validateEmail(form.email) || "";
	errors.password = form.password ? "" : "Паролата е задължителна.";
	if (form.password && form.password.length < 8) {
		errors.password = "Паролата трябва да e поне 8 символа.";
	} else if (form.password && form.password.includes(" ")) {
		errors.password = "Паролата не трябва да съдържа интервали.";
	} else if (form.password && !/[A-Z]/.test(form.password)) {
		errors.password = "Паролата трябва да съдържа поне една главна буква.";
	} else if (form.password && !/[a-z]/.test(form.password)) {
		errors.password = "Паролата трябва да съдържа поне една малка буква.";
	} else if (form.password && !/[0-9]/.test(form.password)) {
		errors.password = "Паролата трябва да съдържа поне една цифра.";
	} else if (form.password && !/[!@#$%^&*(),.?":{}|<>]/.test(form.password)) {
		errors.password =
			"Паролата трябва да съдържа поне един специален символ.";
	}
	if (!isLogin.value) {
		errors.confirmPassword = form.confirmPassword
			? form.password === form.confirmPassword
				? ""
				: "Паролите не съвпадат."
			: "Полето за потвърждение на парола е задължително.";
	}

	const hasError = Object.values(errors).some(msg => msg);
	if (hasError) {
		return;
	}

	execute().then(() => {
		if (isLogin.value) {
			// Handle login success
			if (data.value?.accessToken && data.value?.refreshToken) {
				userStore.setAccessToken(data.value.accessToken);
				userStore.setRefreshToken(data.value.refreshToken);
				triggerToast("Успешно влизане!", "success");
				router.push("/events");
			}
		} else {
			// Handle registration success
			triggerToast(
				"Регистрацията беше успешна! Моля, влезте в профила си.",
				"success"
			);
			// Clear the form
			form.email = "";
			form.password = "";
			form.confirmPassword = "";
			// Switch to login mode
			isLogin.value = true;
		}
	});
}
</script>
<template>
	<div class="min-h-screen flex items-center justify-center bg-grey">
		<form
			@submit.prevent="handleSubmit"
			novalidate
			class="bg-dark-grey p-8 rounded-lg shadow-md w-full max-w-sm">
			<h2 class="text-yellow text-2xl font-semibold text-center mb-6">
				{{ isLogin ? "Вписване" : "Регистрация" }}
			</h2>

			<!-- API Error Display -->
			<div
				v-if="error"
				class="bg-red/20 border border-red text-white px-4 py-3 rounded mb-4">
				{{
					typeof error === "string"
						? error
						: "Възникна грешка при автентикацията"
				}}
			</div>

			<div class="flex flex-col gap-4">
				<div>
					<label class="text-white mb-1" for="email">Имейл</label>
					<input
						id="email"
						type="email"
						v-model="form.email"
						required
						class="w-full px-3 py-2 border rounded focus:outline-none focus:border-white text-yellow" />
					<div v-if="errors.email" class="text-red text-sm mt-1">
						{{ errors.email }}
					</div>
				</div>

				<div>
					<label class="text-white mb-1" for="password">Парола</label>
					<input
						id="password"
						type="password"
						v-model="form.password"
						required
						class="w-full px-3 py-2 border rounded focus:outline-none focus:border-white text-yellow" />
					<div v-if="errors.password" class="text-red text-sm mt-1">
						{{ errors.password }}
					</div>
				</div>

				<div v-if="!isLogin">
					<label class="text-white mb-1" for="confirmPassword"
						>Потвърди парола</label
					>
					<input
						id="confirmPassword"
						type="password"
						v-model="form.confirmPassword"
						required
						class="w-full px-3 py-2 border rounded focus:outline-none focus:border-white text-yellow" />
					<div
						v-if="errors.confirmPassword"
						class="text-red-500 text-sm mt-1">
						{{ errors.confirmPassword }}
					</div>
				</div>
			</div>

			<button
				type="submit"
				:disabled="loading"
				class="mt-6 w-full bg-yellow text-dark-grey py-2 rounded hover:bg-yellow-dark cursor-pointer disabled:opacity-50 disabled:cursor-not-allowed">
				{{
					loading
						? "Обработва се..."
						: isLogin
							? "Впишете се"
							: "Регистрирайте се"
				}}
			</button>

			<div class="mt-4 text-center text-sm">
				<span class="text-white">{{
					isLogin ? "Нямате акаунт?" : "Имате акаунт?"
				}}</span>
				<button
					type="button"
					@click="toggleMode"
					class="text-yellow hover:underline ml-2 cursor-pointer">
					{{ isLogin ? "Регистрирайте се" : "Впишете се" }}
				</button>
			</div>
		</form>
	</div>
</template>
