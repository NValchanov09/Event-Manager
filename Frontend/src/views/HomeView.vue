<template>
	<div class="w-full h-full overflow-auto bg-grey text-white">
		<!-- Hero Section -->
		<div class="container mx-auto px-6 py-16">
			<div class="text-center mb-16">
				<h1 class="text-5xl md:text-6xl font-bold text-yellow mb-6">
					Добре дошли!
				</h1>
				<p
					class="text-xl md:text-2xl text-white/80 mb-8 max-w-3xl mx-auto">
					Открийте, присъединете се и управлявайте събития
					безпроблемно. Свържете се с вашата общност и не пропускайте
					нито едно важно събитие.
				</p>
				<div class="flex flex-col sm:flex-row gap-4 justify-center">
					<RouterLink
						to="/events"
						class="bg-yellow text-grey px-8 py-3 rounded-lg font-semibold hover:opacity-90 transition text-lg whitespace-nowrap">
						Разгледай събитията
					</RouterLink>
					<RouterLink
						to="/events/joined"
						class="border-2 border-yellow text-yellow px-8 py-3 rounded-lg font-semibold hover:bg-yellow hover:text-grey transition text-lg whitespace-nowrap">
						Присъединени събития
					</RouterLink>
				</div>
			</div>

			<!-- Features Section -->
			<div class="grid grid-cols-1 md:grid-cols-3 gap-8 mb-16">
				<div class="bg-dark-grey rounded-lg p-8 text-center">
					<div
						class="w-16 h-16 bg-yellow rounded-full flex items-center justify-center mx-auto mb-4">
						<svg
							class="w-8 h-8 text-grey"
							fill="currentColor"
							viewBox="0 0 20 20">
							<path
								fill-rule="evenodd"
								d="M6 2a1 1 0 00-1 1v1H4a2 2 0 00-2 2v10a2 2 0 002 2h12a2 2 0 002-2V6a2 2 0 00-2-2h-1V3a1 1 0 10-2 0v1H7V3a1 1 0 00-1-1zm0 5a1 1 0 000 2h8a1 1 0 100-2H6z"
								clip-rule="evenodd" />
						</svg>
					</div>
					<h3 class="text-xl font-semibold text-yellow mb-2">
						Лесно откриване на събития
					</h3>
					<p class="text-white/80">
						Разгледайте разнообразие от събития и намерете
						перфектните за вас.
					</p>
				</div>

				<div class="bg-dark-grey rounded-lg p-8 text-center">
					<div
						class="w-16 h-16 bg-yellow rounded-full flex items-center justify-center mx-auto mb-4">
						<svg
							class="w-8 h-8 text-grey"
							fill="currentColor"
							viewBox="0 0 20 20">
							<path
								d="M13 6a3 3 0 11-6 0 3 3 0 016 0zM18 8a2 2 0 11-4 0 2 2 0 014 0zM14 15a4 4 0 00-8 0v3h8v-3z" />
						</svg>
					</div>
					<h3 class="text-xl font-semibold text-yellow mb-2">
						Лесна регистрация
					</h3>
					<p class="text-white/80">
						Присъединете се към събития със само няколко клика и
						управлявайте регистрациите си без усилие.
					</p>
				</div>

				<div class="bg-dark-grey rounded-lg p-8 text-center">
					<div
						class="w-16 h-16 bg-yellow rounded-full flex items-center justify-center mx-auto mb-4">
						<svg
							class="w-8 h-8 text-grey"
							fill="currentColor"
							viewBox="0 0 20 20">
							<path
								fill-rule="evenodd"
								d="M3 4a1 1 0 011-1h12a1 1 0 110 2H4a1 1 0 01-1-1zm0 4a1 1 0 011-1h12a1 1 0 110 2H4a1 1 0 01-1-1zm0 4a1 1 0 011-1h12a1 1 0 110 2H4a1 1 0 01-1-1zm0 4a1 1 0 011-1h12a1 1 0 110 2H4a1 1 0 01-1-1z"
								clip-rule="evenodd" />
						</svg>
					</div>
					<h3 class="text-xl font-semibold text-yellow mb-2">
						Организирано табло
					</h3>
					<p class="text-white/80">
						Следете всички свои събития на едно място с нашето
						интуитивно табло.
					</p>
				</div>
			</div>

			<!-- Admin Features (if admin) -->
			<div
				v-if="userStore.isAdmin"
				class="bg-dark-grey rounded-lg p-8 mb-16">
				<div class="text-center mb-8">
					<h2 class="text-3xl font-bold text-yellow mb-4">
						Администраторски инструменти
					</h2>
					<p class="text-white/80 text-lg">
						Управлявайте събития и потребители с мощни
						административни функции.
					</p>
				</div>
				<div class="flex flex-col sm:flex-row gap-4 justify-center">
					<RouterLink
						to="/events/create"
						class="bg-yellow text-grey px-6 py-3 rounded-lg font-semibold hover:opacity-90 transition text-center whitespace-nowrap">
						Създаване на ново събитие
					</RouterLink>
					<RouterLink
						to="/users"
						class="border-2 border-yellow text-yellow px-6 py-3 rounded-lg font-semibold hover:bg-yellow hover:text-grey transition text-center whitespace-nowrap">
						Управление на потребители
					</RouterLink>
				</div>
			</div>

			<!-- Call to Action -->
			<div class="text-center">
				<h2 class="text-3xl font-bold text-yellow mb-4">
					Готови ли сте да започнете?
				</h2>
				<p class="text-white/80 text-lg mb-6">
					Присъединете се към общността и започнете да разглеждате
					събития днес.
				</p>
				<RouterLink
					to="/events"
					class="bg-yellow text-grey px-8 py-3 rounded-lg font-semibold hover:opacity-90 transition text-lg inline-block whitespace-nowrap">
					Разгледайте всички събития
				</RouterLink>
			</div>
		</div>
	</div>
</template>

<script setup lang="ts">
import { RouterLink } from "vue-router";
import { useUserStore } from "@/stores/userStore";

const userStore = useUserStore();
</script>
