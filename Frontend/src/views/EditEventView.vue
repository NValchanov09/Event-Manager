<script setup lang="ts">
import EventInformationFormComponent from "@/components/EventInformationFormComponent.vue";
import CreateForm from "@/components/FormCreator/CreateForm.vue";
import TwoPanelLayout from "@/components/TwoPanelLayout.vue";
import LoaderComponent from "@/components/LoaderComponent.vue";
import ConfirmationComponent from "@/components/ConfirmationComponent.vue";
import { updateEvent, getEventById } from "@/services/eventsService";
import { useCurrentEventStore } from "@/stores/currentEventStore";
import { useUIStore } from "@/stores/uiStore";
import { useAsync } from "@/composables/useAsync";
import { useTemplateRef, onMounted, onUnmounted, ref, computed } from "vue";
import { useRouter, useRoute } from "vue-router";
import { extractErrorMessage } from "@/utils/errorHandling";
import type { UpdateEventDto } from "@/utils/types";

const { setCurrentEvent, clearCurrentEvent } = useCurrentEventStore();
const { triggerToast } = useUIStore();

const router = useRouter();

const route = useRoute();

const editEventInformationRef = useTemplateRef<{
	getState: () => UpdateEventDto;
	isValid: () => boolean;
}>("editEventInformationRef");
const editEventFormRef = useTemplateRef("editEventFormRef");

const showConfirmDialog = ref(false);

// Computed property to check if form is valid
const isFormValid = computed(() => {
	// Check if the event information form is valid
	const eventInfoValid = editEventInformationRef.value?.isValid?.() ?? true;
	return eventInfoValid;
});

const {
	execute: loadEvent,
	data: currentEvent,
	loading: isLoading,
	error: loadError,
} = useAsync(async () => {
	const eventId = route.params.id as string;

	if (!eventId || typeof eventId !== "string") {
		throw new Error("Event ID is required");
	}

	const event = await getEventById(eventId);
	setCurrentEvent(event);
	return event;
});

onMounted(async () => {
	try {
		await loadEvent();
	} catch (error) {
		console.error("Грешка при зареждане на събитието:", error);

		triggerToast(
			extractErrorMessage(error, "Възникна грешка при зареждане на събитието."),
			"error"
		);

		router.push("/events");
	}
});

// Clear current event when leaving the edit page
onUnmounted(() => {
	clearCurrentEvent();
});

// UseAsync for updating event
const {
	execute: executeUpdateEvent,
	loading: isUpdating,
	error: updateError,
} = useAsync(async () => {
	const eventInformation = editEventInformationRef.value?.getState();
	const formFields = editEventFormRef.value?.getState();

	if (!eventInformation || !formFields) {
		throw new Error("Моля, попълнете всички задължителни полета");
	}

	await updateEvent(route.params.id as string, {
		...eventInformation,
		fields: formFields,
	});

	triggerToast("Събитието беше обновено успешно!", "success");
	router.push(`/events/${route.params.id}`);
	clearCurrentEvent();
});

function handleSubmit() {
	// Show confirmation dialog before proceeding
	showConfirmDialog.value = true;
}

async function confirmSubmit() {
	showConfirmDialog.value = false;

	try {
		await executeUpdateEvent();
	} catch (validationError) {
		triggerToast(
			extractErrorMessage(validationError, "Моля, коригирайте грешките във формата"),
			"error"
		);
	}
}

function cancelSubmit() {
	showConfirmDialog.value = false;
}
</script>

<template>
	<form @submit.prevent="handleSubmit">
		<TwoPanelLayout action-name="Редактиране на събитие">
			<template #left>
				<div v-if="isUpdating">
					<LoaderComponent />
				</div>
				<div v-if="isLoading">
					<LoaderComponent />
				</div>
				<div v-else-if="loadError" class="p-10 text-center text-red-500">
					Възникна грешка: {{ loadError }}
				</div>
				<EventInformationFormComponent
					v-else
					ref="editEventInformationRef"
					v-bind="{ event: currentEvent ?? undefined }" />
			</template>
			<template #right>
				<div v-if="isLoading">
					<LoaderComponent />
				</div>
				<div v-else-if="loadError" class="p-10 text-center text-red-500">
					Възникна грешка: {{ loadError }}
				</div>
				<template v-else>
					<div
						v-if="updateError"
						class="bg-red/20 border border-red text-white px-4 py-3 rounded mb-4">
						{{ extractErrorMessage(updateError, "Грешка при обновяване на събитието") }}
					</div>

					<CreateForm
						ref="editEventFormRef"
						v-bind="{ fields: currentEvent?.fields ?? [] }" />

					<button
						type="submit"
						class="w-full py-3 rounded-lg font-semibold transition-all cursor-pointer mt-6 bg-yellow text-dark-grey hover:opacity-90">
						{{ isUpdating ? "Запазва се..." : "Запази промените" }}
					</button>
				</template>
			</template>
		</TwoPanelLayout>
	</form>
	<!-- Confirmation Dialog -->
	<ConfirmationComponent
		v-if="showConfirmDialog"
		title="Потвърдете редактирането"
		message="Редактирането на това събитие ще изтрие всички съществуващи регистрации. Тази операция не може да бъде отменена. Сигурни ли сте, че искате да продължите?"
		confirm-text="Редактирай"
		cancel-text="Откажи"
		@confirm="confirmSubmit"
		@cancel="cancelSubmit" />
</template>
