<script setup lang="ts">
import { onMounted, ref, watch } from "vue";
import type { CreateEventDto, FilledField } from "@/utils/types.ts";
import { useRoute } from "vue-router";
import { apiClient } from "@/utils/api.ts";
import { downloadFile } from "@/services/exportsService.ts";
import { getSubmissions } from "@/services/submissionService.ts";
import { useAsync } from "@/composables/useAsync";
import LoaderComponent from "@/components/LoaderComponent.vue";
import { formatDateTime } from "@/utils/date.ts";
import { useUIStore } from "@/stores/uiStore.ts";
import { extractErrorMessage } from "@/utils/errorHandling";

const uiStore = useUIStore();
const getSubmissionField = (
	submission: FilledField[],
	fieldId: number
): string => {
	return submission.find(s => s.id === fieldId)?.options.join(", ") || "-";
};

const route = useRoute();
const eventId = ref(route.params.id as string);

const fetchEvent = () =>
	apiClient.get<CreateEventDto>(`/events/${eventId.value}`);
const fetchSubmissions = () => getSubmissions(eventId.value);

const {
	loading: eventLoading,
	error: eventError,
	data: eventData,
	execute: loadEvent,
} = useAsync(fetchEvent);

const {
	loading: submissionsLoading,
	error: submissionsError,
	data: submissionsData,
	execute: loadSubmissions,
} = useAsync(fetchSubmissions);

onMounted(() => {
	loadEvent();
	loadSubmissions();
});

const exportToCSV = () => {
	downloadFile(
		"/csv/" + eventId.value,
		"submissions.csv",
		"text/csv;charset=utf-8,"
	);
};

const exportToXLSX = () => {
	downloadFile("/xlsx/" + eventId.value, "submissions.xlsx", "xlsx");
};

watch(submissionsError, () => {
	if (submissionsError.value != null) {
		uiStore.triggerToast(
			extractErrorMessage(submissionsError.value),
			"error"
		);
	}
});

watch(eventError, () => {
	if (eventError.value != null) {
		uiStore.triggerToast(extractErrorMessage(eventError.value), "error");
	}
});
</script>

<template>
	<div class="m-6">
		<div v-if="eventLoading || submissionsLoading">
			<LoaderComponent />
		</div>

		<div
			v-else
			class="flex flex-col lg:flex-row justify-between items-start lg:items-center mb-6 gap-4">
			<h1
				class="text-xl md:text-2xl font-bold text-yellow whitespace-nowrap">
				Попълнени формуляри
			</h1>
			<div class="flex flex-row gap-3 lg:gap-4">
				<button
					@click="exportToCSV"
					class="px-4 py-2 bg-cyan hover:bg-cyan-800 text-white rounded transition-colors whitespace-nowrap">
					Запази като CSV
				</button>
				<button
					@click="exportToXLSX"
					class="px-4 py-2 bg-cyan hover:bg-cyan-800 text-white rounded transition-colors whitespace-nowrap">
					Запази като XLSX
				</button>
			</div>
		</div>
		<div
			v-if="submissionsData?.length == 0"
			class="p-10 text-center text-lg text-white">
			Няма намерени попълвания.
		</div>
		<table
			v-else-if="eventData && submissionsData"
			class="min-w-4 divide-y divide-gray-200 overflow-auto">
			<thead class="bg-gray-800">
				<tr>
					<th
						scope="col"
						class="px-6 py-3 text-left text-xs font-medium text-yellow uppercase tracking-wider"
						data-tooltip="Submission Date">
						Дата на подаване
					</th>
					<th
						scope="col"
						class="px-6 py-3 text-left text-xs font-medium text-yellow uppercase tracking-wider"
						data-tooltip="User Email">
						Имейл на потребителя
					</th>
					<th
						v-for="field in eventData.fields"
						:key="field.id"
						scope="col"
						class="px-6 py-3 text-left text-xs font-medium text-yellow tracking-wider max-h-16"
						:data-tooltip="field.name">
						<div class="truncate max-w-[200px] uppercase">
							{{ field.name }}
						</div>
					</th>
				</tr>
			</thead>
			<tbody class="bg-gray-700 divide-y divide-gray-600">
				<tr
					v-for="submission in submissionsData"
					:key="submission.userId"
					class="transition-colors">
					<td
						class="px-6 py-4 whitespace-nowrap text-white"
						:data-tooltip="formatDateTime(submission.createdAt)">
						<div class="truncate max-w-[200px]">
							{{ formatDateTime(submission.createdAt) }}
						</div>
					</td>
					<td
						class="px-6 py-4 whitespace-nowrap text-white"
						:data-tooltip="submission.email">
						<div class="truncate max-w-[200px]">
							{{ submission.email }}
						</div>
					</td>
					<td
						v-for="field in eventData.fields"
						:key="field.id"
						class="px-6 py-4 whitespace-nowrap text-white"
						:data-tooltip="
							getSubmissionField(submission.answers, field.id)
						">
						<div class="truncate max-w-[200px]">
							{{
								getSubmissionField(
									submission.answers,
									field.id
								)
							}}
						</div>
					</td>
				</tr>
			</tbody>
		</table>
	</div>
</template>

<style scoped>
[data-tooltip] {
	position: relative;
}

[data-tooltip]:hover::after {
	content: attr(data-tooltip);
	position: absolute;
	top: 100%;
	left: 0;
	background-color: rgba(0, 0, 0, 0.8);
	color: white;
	padding: 0.5rem 0.75rem;
	border-radius: 0.25rem;
	font-size: 0.875rem;
	white-space: normal;
	max-width: 300px;
	z-index: 1000;
	word-break: break-word;
	opacity: 1;
	transition: opacity 300ms ease-in-out;
}
</style>
