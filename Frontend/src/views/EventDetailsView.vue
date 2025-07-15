<script setup lang="ts">
import TwoPanelLayout from "@/components/TwoPanelLayout.vue";
import { deleteEvent, getEventById } from "@/services/eventsService";
import { useRoute, useRouter } from "vue-router";
import FormSubmit from "@/components/FormSubmit.vue";
import { useAsync } from "@/composables/useAsync";
import LoaderComponent from "@/components/LoaderComponent.vue";
import { onMounted, watch, ref } from "vue";
import { useUserStore } from "@/stores/userStore";
import { useCurrentEventStore } from "@/stores/currentEventStore";
import { useUIStore } from "@/stores/uiStore";
import CalendarIcon from "@/components/icons/CalendarIcon.vue";
import LocationIcon from "@/components/icons/LocationIcon.vue";
import ClockIcon from "@/components/icons/ClockIcon.vue";
import UserIcon from "@/components/icons/UserIcon.vue";
import DeleteIcon from "@/components/icons/DeleteIcon.vue";
import DuplicateIcon from "@/components/icons/DuplicateIcon.vue";
import EditIcon from "@/components/icons/EditIcon.vue";
import ViewSubmissionsIcon from "@/components/icons/ViewSubmissionsIcon.vue";
import ConfirmationComponent from "@/components/ConfirmationComponent.vue";
import { cancelSubmission } from "@/services/submissionService.ts";
import { formatDateTime } from "@/utils/date";
import { extractErrorMessage } from "@/utils/errorHandling";

const router = useRouter();
const userStore = useUserStore();
const { setEventForDuplication } = useCurrentEventStore();
const { triggerToast } = useUIStore();

// Confirmation dialog state
const showDeleteConfirm = ref(false);

const {
	execute,
	data: event,
	loading,
	error,
} = useAsync(() => {
	const route = useRoute();

	if (!route.params.id || typeof route.params.id !== "string") {
		throw new Error("Event ID is required");
	}

	return getEventById(route.params.id);
});

watch(event, newEvent => {
	if (newEvent === null) {
		router.push("/not-found");
	}
});

onMounted(execute);

// Function to duplicate the current event
const duplicateEvent = () => {
	if (event.value) {
		// Set the current event for duplication (without ID to create a new one)
		const eventToDuplicate = {
			name: `${event.value.name} (копие)`,
			description: event.value.description,
			date: "",
			signUpDeadline: "",
			location: event.value.location,
			fields: event.value.fields,
			peopleLimit: event.value.peopleLimit,
		};
		setEventForDuplication(eventToDuplicate);
		router.push("/events/create");
	}
};

// Function to show delete confirmation
const showDeleteConfirmation = () => {
	showDeleteConfirm.value = true;
};

const { execute: executeDeleteEvent, loading: deletingEvent } = useAsync(async () => {
	if (!event.value) {
		throw new Error("Event not found");
	}
	await deleteEvent(event.value.id.toString());
	triggerToast("Събитието беше изтрито успешно!", "success");
	await router.push("/events");
});

// Function to handle delete confirmation
const handleDeleteConfirmation = async (confirmed: boolean) => {
	showDeleteConfirm.value = false;

	if (confirmed && event.value) {
		try {
			await executeDeleteEvent();
		} catch (error) {
			console.error("Грешка при изтриване на събитието:", error);

			triggerToast(
				"Възникна грешка при изтриване на събитието. Моля, опитайте отново.",
				"error"
			);
		}
	}
};

// UseAsync for canceling submission
const {
	execute: executeCancelSubmission,
	loading: cancellingSubmission,
	error: cancelError,
} = useAsync(async () => {
	if (!event.value) {
		throw new Error("Event not found");
	}

	await cancelSubmission(event.value.id.toString());
	event.value.userSignedUp = false;
	triggerToast("Отписването беше успешно!", "success");
});

// UseAsync for simple registration
const {
	execute: executeSimpleRegistration,
	loading: registeringSubmission,
	error: registerError,
} = useAsync(async () => {
	if (!event.value) {
		throw new Error("Event not found");
	}

	const { createSubmission } = await import("@/services/submissionService");
	await createSubmission(event.value.id, []);
	event.value.userSignedUp = true;
	if (event.value.spotsLeft > 0) {
		event.value.spotsLeft--;
	}
	triggerToast("Записването беше успешно!", "success");
});

const cancelSubmissionButton = async () => {
	try {
		await executeCancelSubmission();
	} catch (error) {
		triggerToast(
			extractErrorMessage(
				error,
				"Възникна грешка при отписването ви. Моля, опитайте отново."
			),
			"error"
		);
	}
};

// Simple registration for events without form fields
const handleSimpleRegistration = async () => {
	try {
		await executeSimpleRegistration();
	} catch (error) {
		triggerToast(
			extractErrorMessage(
				error,
				"Възникна грешка при записването ви. Моля, опитайте отново."
			),
			"error"
		);
	}
};
</script>

<template>
	<TwoPanelLayout action-name="">
		<template #left>
			<div v-if="loading || deletingEvent">
				<LoaderComponent />
			</div>
			<div v-else-if="error" class="p-10 text-center text-red">
				Възникна грешка: {{ error }}
			</div>
			<div v-if="event" class="space-y-6">
				<!-- Event Title -->
				<div class="border-b border-white/20 pb-4">
					<h1
						class="text-3xl font-bold text-yellow break-words hyphens-auto leading-tight">
						{{ event.name }}
					</h1>
				</div>

				<!-- Mobile Action Icons (only visible on mobile) -->
				<div v-if="userStore.isAdmin" class="flex justify-center gap-4 py-2 lg:hidden">
					<!-- Event Administration Icons -->
					<button
						@click="showDeleteConfirmation"
						class="w-10 h-10 rounded-full bg-red flex items-center justify-center flex-shrink-0 cursor-pointer hover:scale-110 transition-transform text-white"
						title="Изтрий събитие">
						<DeleteIcon />
					</button>
					<button
						@click="duplicateEvent"
						class="w-10 h-10 rounded-full bg-yellow flex items-center justify-center flex-shrink-0 cursor-pointer hover:scale-110 transition-transform text-dark-grey"
						title="Дублиране">
						<DuplicateIcon />
					</button>
					<RouterLink
						:to="{ name: 'edit-event', params: { id: event.id } }"
						class="w-10 h-10 rounded-full bg-orange flex items-center justify-center flex-shrink-0 cursor-pointer hover:scale-110 transition-transform text-white"
						title="Редактиране">
						<EditIcon />
					</RouterLink>
					<RouterLink
						v-if="userStore.isAdmin"
						:to="{ name: 'submissions', params: { id: event.id } }"
						class="w-10 h-10 rounded-full bg-cyan flex items-center justify-center flex-shrink-0 cursor-pointer hover:scale-110 transition-transform text-white"
						title="Попълвания">
						<ViewSubmissionsIcon />
					</RouterLink>
				</div>

				<!-- Event Information (without title) -->
				<div class="sticky top-6 space-y-6">
					<!-- Event Details Grid -->
					<div class="space-y-4">
						<!-- Date -->
						<div class="flex items-start gap-3">
							<CalendarIcon />
							<div class="min-w-0 flex-1">
								<h3 class="text-lg font-semibold text-white">
									Дата и час на събитие
								</h3>
								<p class="text-white/80 break-words">
									{{ formatDateTime(event.date) }}
								</p>
							</div>
						</div>

						<!-- Location -->
						<div class="flex items-start gap-3">
							<LocationIcon />
							<div class="min-w-0 flex-1">
								<h3 class="text-lg font-semibold text-white">Място</h3>
								<p class="text-white/80 break-words">
									{{ event.location }}
								</p>
							</div>
						</div>

						<!-- Sign Up Deadline -->
						<div class="flex items-start gap-3">
							<ClockIcon />
							<div class="min-w-0 flex-1">
								<h3 class="text-lg font-semibold text-white">
									Краен срок на записване
								</h3>
								<p class="text-white/80 break-words">
									{{ formatDateTime(event.signUpDeadline) }}
								</p>
							</div>
						</div>

						<!-- Availability -->
						<div class="flex items-start gap-3">
							<UserIcon />
							<div class="min-w-0 flex-1">
								<h3 class="text-lg font-semibold text-white">Места</h3>
								<div class="text-white/80 break-words space-y-1">
									<p v-if="event.peopleLimit && event.peopleLimit > 0">
										Лимит участници: {{ event.peopleLimit }}
									</p>
									<p v-else>Без лимит на участници</p>
									<p
										v-if="
											event.peopleLimit &&
											event.peopleLimit > 0 &&
											event.spotsLeft !== null &&
											event.spotsLeft !== undefined
										">
										Свободни места:
										{{
											event.spotsLeft === 0
												? "Няма свободни места"
												: event.spotsLeft
										}}
									</p>
								</div>
							</div>
						</div>
					</div>

					<!-- Event Description -->
					<div class="border-t border-white/20 pt-6">
						<h3 class="text-xl font-semibold text-yellow mb-3">За събитието</h3>
						<p class="text-white/90 leading-relaxed whitespace-pre-wrap break-words">
							{{ event.description }}
						</p>
					</div>
				</div>
			</div>
		</template>
		<template #right>
			<div v-if="loading">
				<LoaderComponent />
			</div>
			<div v-else-if="error" class="p-10 text-center text-red">
				Възникна грешка: {{ error }}
			</div>
			<div v-if="event" class="space-y-4">
				<!-- Cancellation Error Display -->
				<div
					v-if="cancelError"
					class="bg-red/20 border border-red text-white px-4 py-3 rounded mb-4">
					{{ extractErrorMessage(cancelError, "Грешка при отписване") }}
				</div>

				<!-- Admin Actions Section (only visible on desktop) -->
				<div
					v-if="userStore.isAdmin"
					class="hidden lg:flex justify-end gap-2 xl:gap-3 px-4 xl:px-6">
					<RouterLink
						:to="{ name: 'submissions', params: { id: event.id } }"
						class="bg-cyan text-white px-3 xl:px-4 py-2 rounded-md hover:bg-cyan-800 transition-colors font-medium cursor-pointer whitespace-nowrap">
						Попълвания
					</RouterLink>
					<button
						@click="duplicateEvent"
						class="bg-yellow text-dark-grey px-3 xl:px-4 py-2 rounded-md hover:bg-yellow-600 transition-colors font-medium cursor-pointer whitespace-nowrap">
						Дублиране
					</button>
					<RouterLink
						:to="{ name: 'edit-event', params: { id: event.id } }"
						class="bg-orange text-white px-3 xl:px-4 py-2 rounded-md hover:bg-orange-600 transition-colors font-medium cursor-pointer whitespace-nowrap">
						Редактиране
					</RouterLink>
					<button
						@click="showDeleteConfirmation"
						class="bg-red text-white px-3 xl:px-4 py-2 rounded-md hover:bg-red-600 transition-colors font-medium cursor-pointer whitespace-nowrap">
						Изтрий
					</button>
				</div>

				<!-- Submission Form or Simple Registration -->
				<FormSubmit
					v-if="event?.fields && event.fields.length > 0"
					:event-id="event?.id"
					:fields="event.fields"
					:user-signed-up="event.userSignedUp"
					:on-cancel="cancelSubmissionButton"
					:cancel-loading="cancellingSubmission"
					action-name="Запиши се"
					@signed-up="event.userSignedUp = true" />

				<!-- Simple Registration for events without form fields -->
				<div
					v-else-if="event"
					class="p-6 bg-dark-grey shadow-lg rounded-lg max-w-4xl mx-auto my-8">
					<!-- Cancellation Error Display -->
					<div
						v-if="cancelError"
						class="bg-red/20 border border-red text-white px-4 py-3 rounded mb-4">
						{{ extractErrorMessage(cancelError, "Грешка при отписване") }}
					</div>

					<!-- Registration Error Display -->
					<div
						v-if="registerError"
						class="bg-red/20 border border-red text-white px-4 py-3 rounded mb-4">
						{{ extractErrorMessage(registerError, "Грешка при записване") }}
					</div>

					<div v-if="event.userSignedUp" class="text-green-400 text-center mb-4 text-xl">
						Вече сте записани за това събитие.
					</div>

					<div class="pt-4 flex flex-wrap justify-center gap-4">
						<button
							v-if="!event.userSignedUp"
							@click="() => handleSimpleRegistration()"
							:disabled="event.spotsLeft === 0 || registeringSubmission"
							:class="[
								'cursor-pointer py-3 px-8 shadow-md text-base font-medium rounded-full transition duration-150 ease-in-out whitespace-nowrap',
								event.spotsLeft === 0 || registeringSubmission
									? 'bg-grey-400 text-grey-600 cursor-not-allowed opacity-50'
									: 'text-gray-900 bg-yellow hover:bg-yellow-900 disabled:opacity-50 disabled:cursor-not-allowed',
							]">
							{{
								registeringSubmission
									? "Записва се..."
									: event.spotsLeft === 0
										? "Няма свободни места"
										: "Запиши се"
							}}
						</button>
						<button
							v-if="event.userSignedUp"
							:disabled="cancellingSubmission"
							@click="cancelSubmissionButton"
							class="cursor-pointer py-3 px-8 shadow-md text-base font-medium rounded-full text-white bg-red hover:bg-red-800 transition duration-150 ease-in-out whitespace-nowrap disabled:opacity-50 disabled:cursor-not-allowed">
							{{ cancellingSubmission ? "Отписва се..." : "Отпиши се" }}
						</button>
					</div>
				</div>
			</div>
		</template>
	</TwoPanelLayout>

	<!-- Delete Confirmation Dialog -->
	<ConfirmationComponent
		v-if="showDeleteConfirm"
		title="Потвърдете изтриването"
		message="Сигурни ли сте, че искате да изтриете това събитие?"
		@confirm="handleDeleteConfirmation(true)"
		@cancel="handleDeleteConfirmation(false)"
		confirmText="Изтрий"
		cancelText="Откажи" />
</template>
