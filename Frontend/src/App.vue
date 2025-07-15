<script setup lang="ts">
import { RouterView } from "vue-router";
import NavbarComponent from "./components/NavbarComponent.vue";
import ToasterComponent from "./components/ToasterComponent.vue";
</script>

<template>
	<div class="h-screen flex flex-col bg-grey text-white font-roboto">
		<ToasterComponent />

		<header>
			<NavbarComponent />
		</header>

		<main class="flex-1 overflow-auto">
			<RouterView />
		</main>
	</div>
</template>
