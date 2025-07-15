<script setup lang="ts">
defineProps({
	title: {
		type: String,
		default: "Сигурни ли сте?",
	},
	message: {
		type: String,
		required: true,
	},
	confirmText: {
		type: String,
		default: "Потвърди",
	},
	cancelText: {
		type: String,
		default: "Откажи",
	},
});
const emit = defineEmits(["confirm", "cancel"]);
</script>
<template>
	<!-- Modal Backdrop -->
	<div
		class="fixed inset-0 bg-black/60 flex items-center justify-center z-[9999] backdrop-blur-sm"
		@click.self="emit('cancel')">
		<!-- Modal Content -->
		<div
			class="bg-dark-grey border-2 border-yellow rounded-lg p-6 max-w-md w-full mx-4 shadow-2xl transform transition-all duration-300 scale-100"
			@click.stop>
			<h3 class="text-xl font-semibold text-yellow mb-3">{{ title }}</h3>
			<p class="text-white mb-6 leading-relaxed">{{ message }}</p>
			<div class="flex justify-end gap-3">
				<button
					@click="emit('cancel')"
					class="px-6 py-2 text-white border-2 border-grey-400 rounded-lg hover:bg-grey-400 hover:text-yellow hover:border-grey-400 transition-all duration-200 cursor-pointer">
					{{ cancelText }}
				</button>
				<button
					@click="emit('confirm')"
					class="cursor-pointer px-6 py-2 bg-yellow text-dark-grey rounded-lg hover:bg-yellow-600 transition-all duration-200 font-semibold">
					{{ confirmText }}
				</button>
			</div>
		</div>
	</div>
</template>
