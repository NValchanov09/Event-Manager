<template>
	<textarea
		:id="id"
		:placeholder="placeholder"
		rows="1"
		:required="required ?? true"
		v-model="model"
		ref="textarea"
		class="w-full bg-grey-400 text-white rounded-t-lg not-focus:rounded-b-lg border-b border-grey-400 placeholder-grey-200 focus:border-yellow focus:border-b-2 focus:ring-0 focus:outline-none transition-colors duration-300 ease-in-out resize-none overflow-hidden leading-tight px-3 py-2"
		@input="resize"></textarea>
</template>

<script setup lang="ts">
import { onMounted, useTemplateRef } from "vue";

const input = useTemplateRef("textarea");
defineProps<{ id: string; placeholder?: string; required?: boolean }>();
const model = defineModel<string>();
const resize = () => {
	if (input.value != null) {
		input.value.style.height = "auto";
		input.value.style.height = `${input.value.scrollHeight}px`;
	}
};

onMounted(resize);
</script>
