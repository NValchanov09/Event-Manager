<script setup lang="ts">
import { ref, toRaw } from "vue";
import InputField from "@/components/FormCreator/InputField.vue";
import FieldAdder from "@/components/FormCreator/FieldAdder.vue";
import type { FormField } from "@/utils/types.ts";

const props = withDefaults(
	defineProps<{
		fields?: FormField[];
	}>(),
	{
		fields: () => [],
	}
);

defineExpose({
	getState: () => toRaw(fields.value),
});

const fields = ref<FormField[]>(props.fields || []);

const addField = (fieldType: "text" | "checkbox" | "radio") => {
	const newField: FormField = {
		id: (fields.value[fields.value.length - 1]?.id ?? 0) + 1,
		type: fieldType,
		name: "Ново поле",
		required: false,
		options: [],
	};

	if (fieldType === "checkbox" || fieldType === "radio") {
		newField.options.push("Опция 1");
	}

	fields.value.push(newField);
};
</script>

<template>
	<div
		class="p-6 bg-dark-grey shadow-lg rounded-lg max-w-4xl mx-auto space-y-6">
		<h1 class="text-2xl font-semibold text-white text-center">
			Конфигурация на формата
		</h1>
		<div
			v-for="(field, index) in fields"
			:key="field.id"
			class="bg-grey-800 p-4 rounded-lg shadow-md border-l-4 border-yellow transition-transform duration-200 ease-in-out hover:scale-[1.03] hover:shadow-xl/30 hover:z-10">
			<!-- Field Name Input with Remove Button -->
			<div class="flex items-center gap-3 mb-3">
				<div class="flex-1">
					<label
						:for="`field-${field.id}`"
						class="block text-lg font-medium text-white">
						<InputField
							required
							class="w-full"
							:id="`field-${field.id}`"
							v-model="field.name"></InputField>
					</label>
				</div>
				<button
					class="w-8 h-8 flex items-center justify-center rounded-full hover:bg-red-500/20 transition-colors cursor-pointer text-red"
					@click="fields.splice(index, 1)"
					title="Премахни поле">
					<svg
						class="w-5 h-5"
						fill="currentColor"
						viewBox="0 0 20 20">
						<path
							fill-rule="evenodd"
							d="M4.293 4.293a1 1 0 011.414 0L10 8.586l4.293-4.293a1 1 0 111.414 1.414L11.414 10l4.293 4.293a1 1 0 01-1.414 1.414L10 11.414l-4.293 4.293a1 1 0 01-1.414-1.414L8.586 10 4.293 5.707a1 1 0 010-1.414z"
							clip-rule="evenodd" />
					</svg>
				</button>
			</div>

			<!-- Required Toggle -->
			<div class="mb-4">
				<label
					:for="`${field.id}-required`"
					class="relative inline-flex items-center cursor-pointer">
					<input
						v-model="field.required"
						:id="`${field.id}-required`"
						type="checkbox"
						class="sr-only peer" />
					<div
						class="w-11 h-6 bg-grey-400 rounded-full peer peer-focus:ring-3 peer-focus:ring-yellow peer-checked:after:translate-x-full peer-checked:after:border-white after:content-[''] after:absolute after:top-0.5 after:left-[2px] after:bg-white after:border-grey-300 after:border after:rounded-full after:h-5 after:w-5 after:transition-all peer-checked:bg-yellow"></div>
					<span class="ml-3 text-sm font-medium text-white-900"
						>Задължително</span
					>
				</label>
			</div>

			<div v-if="field.type === 'text'">
				<input
					type="text"
					disabled
					class="text-white mt-1 block w-full px-4 py-2 border-b border-outline bg-dark-grey rounded-t-md shadow-sm focus:border-primary focus:ring-yellow sm:text-base transition duration-150 ease-in-out"
					:placeholder="`Въведи ${field.name.toLowerCase()}`" />
			</div>

			<div
				v-else-if="field.type === 'checkbox' || field.type === 'radio'"
				class="space-y-2">
				<div
					v-for="(_, index) in field.options"
					:key="`field-${field.id}-${index}`"
					class="flex items-center">
					<input
						:id="`${field.id}-${index}`"
						:type="field.type"
						:name="`field-${field.id}`"
						class="h-5 w-5 text-white border-outline focus:ring-yellow rounded-sm cursor-pointer" />
					<label
						:for="`${field.id}-${index}`"
						class="ml-3 text-base text-white">
						<InputField
							required
							:id="`${field.id}-${index}`"
							v-model="field.options[index]"></InputField>
					</label>
					<button
						class="ml-3 w-8 h-8 flex-shrink-0 flex items-center justify-center rounded-full hover:bg-red-500/20 transition-colors cursor-pointer text-red"
						@click.prevent="field.options.splice(index, 1)"
						title="Премахни опция">
						<!-- Use minus symbol for option removal -->
						<svg
							class="w-4 h-4"
							fill="currentColor"
							viewBox="0 0 20 20">
							<path
								fill-rule="evenodd"
								d="M3 10a1 1 0 011-1h12a1 1 0 110 2H4a1 1 0 01-1-1z"
								clip-rule="evenodd" />
						</svg>
					</button>
				</div>
				<button
					class="mt-2 text-yellow-400 hover:bg-yellow-400/20 text-sm font-medium rounded-md px-3 py-1.5 transition-colors cursor-pointer"
					@click.prevent="
						field.options.push(`Oпция ${field.options.length + 1}`)
					">
					Добави опция
				</button>
			</div>
		</div>

		<FieldAdder @add-field="fieldType => addField(fieldType)" />
	</div>
</template>

<style>
input[type="text"]:focus,
select:focus {
	outline: none;
}

input[type="checkbox"] {
	accent-color: var(--color-yellow);
}

input[type="radio"] {
	accent-color: var(--color-yellow);
}

input:focus,
button:focus,
select:focus {
	outline: none;
	box-shadow: none;
}
</style>
