<script setup lang="ts">
import CardComponent from "@/components/CardComponent.vue";
import {
	getJoinedEvents,
	getJoinedEventsWithFilters,
} from "@/services/eventsService";
import type { EventsFilterParams } from "@/services/eventsService";
import { onMounted, ref } from "vue";
import FilterComponent from "@/components/FilterComponent.vue";
import LoaderComponent from "@/components/LoaderComponent.vue";
import type { Event } from "@/utils/types";
import { useAsync } from "@/composables/useAsync";

const { execute, data, error, loading } = useAsync(getJoinedEvents);
const events = ref<Event[]>([]);
const searchedEvents = ref<Event[]>([]);

// Loading and error states that combine both operations
const isLoading = ref(false);
const errorMessage = ref<string | null>(null);

onMounted(() => {
	execute().then(() => {
		events.value = data.value ?? [];
	});
});

const handleFilter = async (params: EventsFilterParams) => {
	isLoading.value = true;
	errorMessage.value = null;

	try {
		const result = await getJoinedEventsWithFilters(params);
		events.value = result ?? [];
		searchedEvents.value = []; // Clear search when filtering
	} catch (err) {
		errorMessage.value =
			err instanceof Error
				? err.message
				: "Възникна грешка при филтрирането";
	} finally {
		isLoading.value = false;
	}
};

const handleClearFilters = async () => {
	isLoading.value = true;
	errorMessage.value = null;

	try {
		const result = await getJoinedEvents();
		events.value = result ?? [];
		searchedEvents.value = []; // Clear search when clearing filters
	} catch (err) {
		errorMessage.value =
			err instanceof Error
				? err.message
				: "Възникна грешка при зареждането";
	} finally {
		isLoading.value = false;
	}
};
</script>

<template>
	<h2 class="text-3xl md:text-4xl pl-10 pt-6 font-bold text-yellow">
		Присъединени събития
	</h2>

	<FilterComponent
		v-model:events="events"
		v-model:searched-events="searchedEvents"
		@filter="handleFilter"
		@clear-filters="handleClearFilters" />

	<div v-if="loading || isLoading"><LoaderComponent /></div>
	<div v-else-if="error || errorMessage" class="p-10 text-center text-red">
		Възникна грешка: {{ error ?? errorMessage }}
	</div>
	<div
		v-else-if="!events || events.length === 0"
		class="p-10 text-center text-white">
		Няма намерени присъединени събития
	</div>
	<div v-else>
		<div
			class="md:p-10 p-5 pt-7 grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-6 overflow-y-auto auto-rows-fr">
			<CardComponent
				class="w-full"
				v-for="event in searchedEvents.length === 0
					? events
					: searchedEvents"
				:event="event"
				:key="event.id" />
		</div>
	</div>
</template>

<style>
input[type="checkbox"] {
	accent-color: var(--color-yellow);
}
</style>
