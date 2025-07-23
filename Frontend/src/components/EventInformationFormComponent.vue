<script setup lang="ts">
import type { CreateEventDto } from "@/utils/types";
import { ref, toRaw, computed, watch } from "vue";
import { utcToLocal, localToUtc, getCurrentLocalDateTime } from "@/utils/date";

const props = withDefaults(
	defineProps<{
		event?: Omit<CreateEventDto, "fields">;
	}>(),
	{
		event: () => ({
			name: "",
			date: "",
			location: "",
			signUpDeadline: "",
			description: "",
			peopleLimit: 0,
			imageUrl: ""
		}),
	}
);

const widget = window.cloudinary.createUploadWidget(
	{
		cloud_name: import.meta.env.VITE_CLOUD_NAME,
		upload_preset: import.meta.env.VITE_UPLOAD_PRESET
	},

	(error, result) => {
	if(!error && result && result.event === "success") {
		console.log("Result URL : ", result.info.url);
		props.event.imageUrl = result.info.url;
	}
        
	}
)


function openUploadWidget() {
	widget.open();
}


defineExpose({
	getState: () => {
		const rawData = toRaw(formData.value);

		// Validate form before returning
		if (!isFormValid.value) {
			// if (!areRequiredFieldsValid.value) {
			// 	throw new Error("Моля, попълнете всички задължителни полета");
			// }
			// if (!isSignUpDeadlineValid.value) {
			// 	throw new Error(
			// 		"Крайният срок за записване не може да бъде след датата на събитието"
			// 	);
			// }
			return;
		}

		return {
			...rawData,
			// Convert local times back to UTC for server
			imageUrl: props.event.imageUrl,
			date: rawData.date ? localToUtc(rawData.date) : "",
			signUpDeadline: rawData.signUpDeadline ? localToUtc(rawData.signUpDeadline) : undefined,
		};
	},
	isValid: () => isFormValid.value,
});

// Initialize form data with converted dates from UTC to local
const formData = ref<Omit<CreateEventDto, "fields">>({
	...props.event,
	// Convert UTC dates to local for datetime-local inputs
	date: props.event?.date ? utcToLocal(props.event.date) : "",
	signUpDeadline: props.event?.signUpDeadline ? utcToLocal(props.event.signUpDeadline) : "",
});

// Computed properties for min/max attributes
const currentLocalDateTime = computed(() => getCurrentLocalDateTime());
const maxSignUpDeadline = computed(() => formData.value.date || "");

// Validation computed properties
// const isSignUpDeadlineValid = computed(() => {
// 	if (!formData.value.signUpDeadline || !formData.value.date) {
// 		return true; // No validation needed if either is empty
// 	}
// 	return new Date(formData.value.signUpDeadline) <= new Date(formData.value.date);
// });

const areRequiredFieldsValid = computed(() => {
	return !!(
		formData.value.name?.trim() &&
		formData.value.date &&
		formData.value.location?.trim() &&
		formData.value.description?.trim()
	);
});

const isFormValid = computed(() => {
	return areRequiredFieldsValid.value; // && isSignUpDeadlineValid.value;
});

// const signUpDeadlineError = computed(() => {
// 	if (!isSignUpDeadlineValid.value) {
// 		return "Крайният срок за записване не може да бъде след датата на събитието";
// 	}
// 	return "";
// });

watch(
	() => formData.value.date,
	newDate => {
		if (
			formData.value.signUpDeadline &&
			new Date(newDate) <= new Date(formData.value.signUpDeadline)
		) {
			formData.value.signUpDeadline = undefined;
		}
	}
);
</script>

<template>
	<div class="sticky top-6 p-6 bg-dark-grey shadow-lg rounded-lg max-w-4xl mx-auto space-y-6">
		<div class="flex flex-col gap-4">
			<h2 class="text-white text-2xl font-semibold text-center mb-2 ml-3 mr-3">
				Информация за събитието
			</h2>

			<div>
				<label class="text-white" for="name">
					Име <span class="text-red-500">*</span>
				</label>
				<input
					id="name"
					v-model="formData.name"
					required
					class="w-full bg-grey-400 mt-1 text-white rounded-t-lg not-focus:rounded-b-lg border-b border-grey-400 placeholder-grey-200 focus:border-yellow focus:border-b-2 focus:ring-0 focus:outline-none transition-colors duration-300 ease-in-out overflow-hidden leading-tight px-3 py-2" />
			</div>

			<div>
				<label class="text-white" for="location">
					Местоположение <span class="text-red-500">*</span>
				</label>
				<input
					id="location"
					v-model="formData.location"
					required
					class="w-full bg-grey-400 mt-1 text-white rounded-t-lg not-focus:rounded-b-lg border-b border-grey-400 placeholder-grey-200 focus:border-yellow focus:border-b-2 focus:ring-0 focus:outline-none transition-colors duration-300 ease-in-out overflow-hidden leading-tight px-3 py-2" />
			</div>

			<div>
				<label class="text-white" for="date">
					Дата <span class="text-red-500">*</span>
				</label>
				<input
					id="date"
					type="datetime-local"
					:min="currentLocalDateTime"
					v-model="formData.date"
					required
					class="w-full bg-grey-400 mt-1 text-white rounded-t-lg not-focus:rounded-b-lg border-b border-grey-400 placeholder-grey-200 focus:border-yellow focus:border-b-2 focus:ring-0 focus:outline-none transition-colors duration-300 ease-in-out overflow-hidden leading-tight px-3 py-2" />
			</div>

			<div>
				<label class="text-white mb-1" for="signUpDeadline">
					Краен срок за записване
				</label>
				<input
					id="signUpDeadline"
					type="datetime-local"
					:min="currentLocalDateTime"
					:max="maxSignUpDeadline"
					v-model="formData.signUpDeadline"
					:class="[
						'w-full bg-grey-400 text-white mt-1 rounded-t-lg not-focus:rounded-b-lg border-b placeholder-grey-200 focus:ring-0 focus:outline-none transition-colors duration-300 ease-in-out overflow-hidden leading-tight px-3 py-2 border-grey-400 focus:border-yellow focus:border-b-2',
					]" />
			</div>

			<div>
				<label class="text-white mb-1" for="peopleLimit">
					Максимален брой участници (0 = без лимит)
				</label>
				<input
					id="peopleLimit"
					type="number"
					min="0"
					v-model="formData.peopleLimit"
					class="w-full bg-grey-400 text-white mt-1 rounded-t-lg not-focus:rounded-b-lg border-b border-grey-400 placeholder-grey-200 focus:border-yellow focus:border-b-2 focus:ring-0 focus:outline-none transition-colors duration-300 ease-in-out overflow-hidden leading-tight px-3 py-2" />
			</div>

			<div>
				<label class="text-white mb-1" for="description">
					Описание <span class="text-red-500">*</span>
				</label>
				<textarea
					id="description"
					v-model="formData.description"
					required
					class="w-full overflow-y-scroll bg-grey-400 text-white mt-1 rounded-t-lg not-focus:rounded-b-lg border-b border-grey-400 placeholder-grey-200 focus:border-yellow focus:border-b-2 focus:ring-0 focus:outline-none transition-colors duration-300 ease-in-out overflow-hidden leading-tight px-3 py-2"
					rows="15"></textarea>
			</div>
			<div>
				<label class="text-white" for="imageUrl">
					Снимка
				</label>
				<button
				id="imageUrl"
				type="button"
				class="h-13 px-4 py-2 border-2 text-lg text-white mt-2 border-yellow-500 rounded-2xl border-solid transition-all duration-300 ease-in-out whitespace-nowrap flex items-center justify-center hover:text-black hover:border-transparent hover:bg-yellow-500 hover:scale-103 hover:shadow-lg cursor-pointer"
				@click="openUploadWidget()"
				>Добави снимка</button>
			</div>
		</div>
	</div>
</template>
