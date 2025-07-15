<script setup lang="ts">
import EventInformationFormComponent from "@/components/EventInformationFormComponent.vue";
import CreateForm from "@/components/FormCreator/CreateForm.vue";
import TwoPanelLayout from "@/components/TwoPanelLayout.vue";
import LoaderComponent from "@/components/LoaderComponent.vue";
import { createEvent } from "@/services/eventsService";
import { useCurrentEventStore } from "@/stores/currentEventStore";
import { useUIStore } from "@/stores/uiStore";
import { useAsync } from "@/composables/useAsync";
import { useTemplateRef, onUnmounted, computed } from "vue";
import { useRouter } from "vue-router";
import { extractErrorMessage } from "@/utils/errorHandling";
import type { CreateEventDto } from "@/utils/types";

const { currentEvent, clearCurrentEvent } = useCurrentEventStore();
const { triggerToast } = useUIStore();

const router = useRouter();

const createEventInformationRef = useTemplateRef<{
	getState: () => CreateEventDto;
	isValid: () => boolean;
}>("createEventInformationRef");
const createEventFormRef = useTemplateRef("createEventFormRef");

// Computed property to check if form is valid
const isFormValid = computed(() => {
	// Check if the event information form is valid
	const eventInfoValid = createEventInformationRef.value?.isValid?.() ?? true;
	return eventInfoValid;
});

// UseAsync for creating event
const {
	execute: executeCreateEvent,
	loading: isCreating,
	error: createError,
} = useAsync(async () => {
	const eventInformation = createEventInformationRef.value?.getState();
	const formFields = createEventFormRef.value?.getState();

	if (!eventInformation || !formFields) {
		throw new Error("Моля, попълнете всички задължителни полета");
	}

	const result = await createEvent({
		...eventInformation,
		fields: formFields,
	});

	triggerToast("Събитието беше създадено успешно!", "success");
	clearCurrentEvent(); // Clear the event data after successful creation
	router.push(`/events/${result.id}`);
	return result;
});

// Clear when leaving the page
onUnmounted(() => {
	clearCurrentEvent();
});

async function handleSubmit() {
	try {
		await executeCreateEvent();
	} catch (validationError) {
		// Handle client-side validation errors
		triggerToast(
			extractErrorMessage(validationError, "Моля, коригирайте грешките във формата"),
			"error"
		);
	}
}
</script>

<template>
	<div>
		<!-- Loading State -->
		<div v-if="isCreating" class="flex items-center justify-center min-h-screen">
			<LoaderComponent />
		</div>

		<!-- Error State -->
		<div v-else-if="createError" class="p-10 text-center text-red-500">
			<h2 class="text-xl font-semibold mb-4">Възникна грешка</h2>
			<p class="mb-4">
				{{
					extractErrorMessage(createError, "Неочаквана грешка при създаване на събитието")
				}}
			</p>
			<button
				@click="executeCreateEvent"
				class="bg-yellow text-dark-grey px-6 py-2 rounded-lg hover:opacity-90 transition">
				Опитай отново
			</button>
		</div>

		<!-- Main Form -->
		<form v-else @submit.prevent="handleSubmit">
			<TwoPanelLayout action-name="Създаване на ново събитие">
				<template #left>
					<EventInformationFormComponent
						ref="createEventInformationRef"
						v-bind="{ event: currentEvent ?? undefined }" />
				</template>
				<template #right>
					<CreateForm
						ref="createEventFormRef"
						v-bind="{ fields: currentEvent?.fields ?? [] }" />

					<button
						type="submit"
						class="w-full py-3 rounded-lg font-semibold transition-all cursor-pointer mt-6 bg-yellow text-dark-grey hover:opacity-90">
						{{ isCreating ? "Създава се..." : "Запази информацията" }}
					</button>
				</template>
			</TwoPanelLayout>
		</form>
	</div>
</template>
