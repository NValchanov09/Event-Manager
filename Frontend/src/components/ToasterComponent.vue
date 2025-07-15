<template>
	<div
		v-if="ui.showToast"
		@mouseenter="pauseTimer"
		@mouseleave="resumeTimer"
		@click.right.prevent="dismissToast"
		:class="[
			'fixed bottom-4 right-4 bg-dark-grey  p-4 rounded-lg shadow-lg z-10 border-2 select-none',
			'max-w-xl',
			toastBgClass,
		]">
		{{ ui.toastMessage }}
		<div class="mt-4 h-1 bg-white rounded-full overflow-hidden">
			<div
				:key="animationKey"
				:style="{
					animationDuration: `${duration}ms`,
					animationPlayState: isPaused ? 'paused' : 'running',
				}"
				:class="[
					'h-full rounded-full animate-progress',
					progressColor,
				]"></div>
		</div>
	</div>
</template>

<script lang="ts" setup>
import { ref, watch, computed } from "vue";
import { useUIStore } from "@/stores/uiStore";

const ui = useUIStore();
const animationKey = ref(0);
const duration = 3000; // milliseconds
const isPaused = ref(false);
let remainingTime = duration;
let startTime: number | null = null;
const progressColor = computed(() => {
	switch (ui.toastType) {
		case "success":
			return "bg-cyan";
		case "error":
			return "bg-red";
		case "warning":
			return "bg-yellow";
		case "info":
		default:
			return "bg-cyan";
	}
});
const toastBgClass = computed(() => {
	switch (ui.toastType) {
		case "success":
			return "text-cyan border-cyan";
		case "error":
			return "text-red border-red";
		case "warning":
			return "text-yellow border-yellow";
		case "info":
		default:
			throw new Error("Unknown toast type");
	}
});

let timeoutId: ReturnType<typeof setTimeout> | null = null;

const pauseTimer = () => {
	if (timeoutId) {
		clearTimeout(timeoutId);
		isPaused.value = true;
		if (startTime) {
			remainingTime -= Date.now() - startTime;
		}
	}
};

const resumeTimer = () => {
	isPaused.value = false;
	startTime = Date.now();
	if (remainingTime > 0) {
		timeoutId = setTimeout(() => {
			ui.showToast = false;
			ui.toastMessage = "";
			timeoutId = null;
		}, remainingTime);
	}
};

const dismissToast = () => {
	if (timeoutId) {
		clearTimeout(timeoutId);
	}
	ui.showToast = false;
	ui.toastMessage = "";
	timeoutId = null;
	isPaused.value = false;
};

watch(
	() => ui.toastMessage,
	newVal => {
		if (newVal) {
			ui.showToast = true;
			animationKey.value++;
			remainingTime = duration;
			startTime = Date.now();

			if (timeoutId) {
				clearTimeout(timeoutId);
			}

			timeoutId = setTimeout(() => {
				ui.showToast = false;
				ui.toastMessage = "";
				timeoutId = null;
			}, duration);
		}
	}
);
</script>

<style scoped>
@keyframes progress {
	from {
		transform: translateX(-100%);
	}
	to {
		transform: translateX(0);
	}
}

.animate-progress {
	animation-name: progress;
	animation-timing-function: linear;
	animation-fill-mode: forwards;
	animation-direction: normal;
}
</style>
