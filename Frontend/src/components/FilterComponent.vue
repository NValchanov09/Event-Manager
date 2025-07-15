<script setup lang="ts">
import SearchIcon from "@/components/icons/SearchIcon.vue";
import { ref, watch } from "vue";
import { TransitionRoot, TransitionChild } from "@headlessui/vue";
import DatePicker from "@vuepic/vue-datepicker";
import "@vuepic/vue-datepicker/dist/main.css";
import ChevronDownIcon from "@/components/icons/ChevronDownIcon.vue";
import ChevronUp from "@/components/icons/ChevronUp.vue";
import FilterIcon from "./icons/FilterIcon.vue";
import SortIcon from "./icons/SortIcon.vue";
import type { Event } from "@/utils/types";
import { onClickOutside } from "@vueuse/core";
import CancelIcon from "./icons/CancelIcon.vue";
import type { EventsFilterParams } from "@/services/eventsService";

const search = ref("");
const filtersOpen = ref(false);
const date = ref<[Date, Date] | null>(null);
const activeEvent = ref<boolean | null>(null);
const sortOpened = ref(false);
const sort = ref<"new" | "old" | "az" | "za" | null>(null);
const sortMenu = ref(null);
const filterMenu = ref(null);
const mobileFilterMenu = ref(null);
const datePickerOpen = ref(false);

const events = defineModel<Event[]>("events", { required: true });
const searchedEvents = defineModel<Event[]>("searched-events", {
	required: true,
});

// Events for parent components to handle
const emit = defineEmits<{
	filter: [params: EventsFilterParams];
	clearFilters: [];
}>();

function closeSortMenu(event: PointerEvent) {
	// Check if click is inside the sort menu itself or on the sort button
	const target = event.target as HTMLElement;
	if (
		target &&
		(target.closest(".sort-dropdown-content") ||
			target.closest(".sort-button"))
	) {
		return;
	}

	sortOpened.value = false;
}

function closeFilterMenu(event: PointerEvent) {
	// Don't close if DatePicker is open
	if (datePickerOpen.value) {
		return;
	}

	// Check if click is inside the filter menu itself or on the filter button
	const target = event.target as HTMLElement;
	if (
		target &&
		(target.closest(".filter-dropdown-content") ||
			target.closest(".filter-button") ||
			target.closest(".dp__menu") ||
			target.closest(".dp__calendar") ||
			target.closest(".dp__overlay"))
	) {
		return;
	}

	// Apply filters when closing the menu
	if (filtersOpen.value) {
		filterBy();
	}

	filtersOpen.value = false;
}

onClickOutside(sortMenu, closeSortMenu);
onClickOutside(filterMenu, closeFilterMenu);
onClickOutside(mobileFilterMenu, closeFilterMenu);

const textInputOptions = {
	format: "dd.MM.yyyy",
};

async function sortBy(text: string) {
	const keywords = text
		.toLowerCase()
		.split(" ")
		.filter(word => word.trim().length > 0);
	searchedEvents.value =
		events.value.filter(currentEvent =>
			keywords.every(
				key =>
					currentEvent.name.toLowerCase().includes(key) ||
					currentEvent.description.toLowerCase().includes(key)
			)
		) ?? null;
}
watch(search, async changedValue => {
	if (changedValue) {
		await sortBy(changedValue);
	} else {
		searchedEvents.value = [];
	}
});

async function deleteFilters() {
	date.value = null;
	activeEvent.value = null;
	sort.value = null;
	searchedEvents.value = [];
	emit("clearFilters");
}

async function filterBy() {
	const params: EventsFilterParams = {};

	if (date.value?.[0] != null && date.value?.[1] != null) {
		const startDate = new Date(date.value[0]);
		startDate.setHours(3, 0, 0, 0);
		params.fromDate = startDate.toISOString();

		const endDate = new Date(date.value[1]);
		endDate.setHours(23, 59, 59, 999);
		params.toDate = endDate.toISOString();
	}

	if (activeEvent.value != null) {
		params.activeOnly = activeEvent.value;
	}

	switch (sort.value) {
		case "new":
			params.sortDescending = false;
			break;
		case "old":
			params.sortDescending = true;
			break;
		case "az":
			params.alphabetical = true;
			params.sortDescending = false;
			break;
		case "za":
			params.alphabetical = true;
			params.sortDescending = true;
			break;
	}

	emit("filter", params);
}
</script>
<template>
	<div class="hidden md:flex justify-between items-center pt-10 px-10 gap-4">
		<!-- Search section on the left -->
		<div class="flex items-center flex-shrink-0">
			<div class="relative">
				<input
					v-model="search"
					placeholder="Потърси..."
					class="w-60 lg:w-80 h-12 pl-12 pr-10 border rounded focus:outline-none focus:border-white text-yellow cursor-pointer" />
				<div
					class="absolute left-0 top-0 h-full w-10 flex items-center justify-center border-r border-yellow rounded-l">
					<SearchIcon
						class="w-6 h-6 text-yellow pointer-events-none" />
				</div>
				<button
					v-if="search"
					@click="search = ''"
					class="absolute right-2 top-1/2 transform -translate-y-1/2 w-6 h-6 flex items-center justify-center text-yellow hover:text-white transition-colors cursor-pointer">
					<CancelIcon class="w-4 h-4" />
				</button>
			</div>
		</div>

		<!-- Filter controls on the right -->
		<div class="flex items-center gap-2 lg:gap-5 flex-shrink-0">
			<div class="relative inline-block text-left" ref="filterMenu">
				<button
					type="button"
					@click="
						if (filtersOpen) {
							filterBy();
						}
						filtersOpen = !filtersOpen;
					"
					class="filter-button flex px-3 lg:px-4 py-2 justify-between items-center min-w-0 border rounded border-yellow-500 text-yellow transition delay-100 duration-300 hover:ease-in-out hover:border-transparent hover:text-white/100 hover:bg-yellow cursor-pointer whitespace-nowrap h-12">
					<span class="hidden lg:inline">Филтрирай</span>
					<span class="lg:hidden">Филтър</span>
					<ChevronDownIcon
						v-if="filtersOpen"
						class="ml-2 flex-shrink-0" />
					<ChevronUp v-else class="ml-2 flex-shrink-0" />
				</button>
				<TransitionRoot
					as="template"
					:show="filtersOpen && sortOpened === false">
					<TransitionChild
						as="template"
						enter="transition-opacity ease-in-out duration-300"
						enter-from="opacity-0"
						enter-to="opacity-100"
						leave="transition-opacity ease-in-out duration-300"
						leave-from="opacity-100"
						leave-to="opacity-0">
						<div
							class="filter-dropdown-content absolute mt-1 left-0 flex flex-col w-72 lg:w-90 rounded origin-top-left border-2 border-yellow bg-dark-grey shadow-lg z-50 gap-4 p-4">
							<div
								class="flex flex-col lg:flex-row gap-3 lg:gap-5 text-yellow">
								<span
									class="text-white text-base lg:text-lg self-start lg:self-center">
									Период:
								</span>
								<DatePicker
									class="w-full lg:w-md"
									v-model="date"
									placeholder="Период"
									range
									:text-input="textInputOptions"
									:enable-time-picker="false"
									:inputClass="'filterCalendar'"
									position="center"
									circle
									lang="bg"
									dark
									@open="datePickerOpen = true"
									@closed="datePickerOpen = false" />
							</div>
							<div
								class="flex flex-col lg:flex-row justify-between items-start lg:items-center gap-3">
								<span class="flex gap-2 items-center">
									<input
										type="checkbox"
										name="active"
										id="active"
										v-model="activeEvent"
										class="w-4 h-4 lg:w-5 lg:h-5" />
									<label
										for="active"
										class="text-base lg:text-lg text-white">
										Активно събитие
									</label>
								</span>
								<button
									@click="
										filtersOpen = false;
										filterBy();
									"
									class="border rounded-2xl px-3 py-1.5 min-w-0 bg-yellow text-black hover:opacity-90 transition-colors cursor-pointer whitespace-nowrap text-sm lg:text-base">
									Запази
								</button>
							</div>
						</div>
					</TransitionChild>
				</TransitionRoot>
			</div>
			<div class="relative inline-block text-left">
				<button
					type="button"
					@click="sortOpened = !sortOpened"
					class="sort-button flex items-center justify-center md:w-12 md:h-12 w-8 h-8 text-yellow transition-all duration-300 hover:text-white hover:scale-110 cursor-pointer">
					<SortIcon class="w-8 h-8" />
				</button>
				<TransitionRoot as="template" :show="sortOpened" ref="sortMenu">
					<TransitionChild
						as="template"
						enter="transition-opacity ease-in-out duration-300"
						enter-from="opacity-0"
						enter-to="opacity-100"
						leave="transition-opacity ease-in-out duration-300"
						leave-from="opacity-100"
						leave-to="opacity-0">
						<div
							class="sort-dropdown-content absolute mt-1 left-0 flex flex-col w-48 rounded origin-top-left border-2 border-yellow bg-dark-grey shadow-lg z-50 py-2">
							<button
								class="text-white hover:text-yellow hover:bg-yellow/10 active:text-yellow cursor-pointer px-4 py-2 text-left transition-colors duration-200"
								@click="
									sort = 'new';
									sortOpened = false;
									filterBy();
								">
								Най-нови
							</button>
							<button
								class="text-white hover:text-yellow hover:bg-yellow/10 active:text-yellow cursor-pointer px-4 py-2 text-left transition-colors duration-200"
								@click="
									sort = 'old';
									sortOpened = false;
									filterBy();
								">
								Най-стари
							</button>
							<button
								class="text-white hover:text-yellow hover:bg-yellow/10 active:text-yellow cursor-pointer px-4 py-2 text-left transition-colors duration-200"
								@click="
									sort = 'az';
									sortOpened = false;
									filterBy();
								">
								Азбучен ред: А до Я
							</button>
							<button
								class="text-white hover:text-yellow hover:bg-yellow/10 cursor-pointer px-4 py-2 text-left transition-colors duration-200"
								@click="
									sort = 'za';
									sortOpened = false;
									filterBy();
								">
								Азбучен ред: Я до А
							</button>
						</div>
					</TransitionChild>
				</TransitionRoot>
			</div>
			<div>
				<button
					@click="deleteFilters()"
					class="flex items-center justify-center px-3 lg:px-4 py-2 min-w-0 border rounded border-yellow-500 text-yellow transition-all duration-300 hover:border-transparent hover:text-black hover:bg-yellow hover:scale-105 cursor-pointer whitespace-nowrap md:h-12 h-8">
					<span class="hidden lg:inline">Изчисти филтрите</span>
					<span class="lg:hidden">Изчисти</span>
				</button>
			</div>
		</div>
	</div>
	<div class="md:hidden flex justify-between items-center pt-10 sm:px-10 px-5">
		<!-- Mobile search section on the left -->
		<div class="flex items-center">
			<div class="relative">
				<input
					v-model="search"
					placeholder="Потърси..."
					class="w-48 md:h-12 h-8 pl-12 pr-10 border rounded focus:outline-none focus:border-white text-yellow cursor-pointer" />
				<div
					class="absolute left-0 top-0 h-full w-10 flex items-center justify-center border-r border-yellow rounded-l">
					<SearchIcon
						class="w-5 h-5 text-yellow pointer-events-none" />
				</div>
				<button
					v-if="search"
					@click="search = ''"
					class="absolute right-2 top-1/2 transform -translate-y-1/2 w-6 h-6 flex items-center justify-center text-yellow hover:text-white transition-colors cursor-pointer">
					<CancelIcon class="w-4 h-4" />
				</button>
			</div>
		</div>

		<!-- Mobile filter controls on the right -->
		<div class="flex flex-row items-center gap-2">
			<div class="relative inline-block text-left" ref="mobileFilterMenu">
				<button
					type="button"
					@click="
						if (filtersOpen) {
							filterBy();
						}
						filtersOpen = !filtersOpen;
					"
					class="filter-button flex items-center justify-center md:h-12 h-8 text-yellow hover:text-white transition-all duration-300 hover:scale-110 cursor-pointer">
					<FilterIcon class="w-8 h-8" />
				</button>
				<TransitionRoot
					as="template"
					:show="filtersOpen && sortOpened === false">
					<TransitionChild
						as="template"
						enter="transition-opacity ease-in-out duration-200"
						enter-from="opacity-0"
						enter-to="opacity-100"
						leave="transition-opacity ease-in-out duration-200"
						leave-from="opacity-100"
						leave-to="opacity-0">
						<div
							class="filter-dropdown-content absolute mt-1 left-1/2 transform -translate-x-1/2 flex flex-col gap-4 p-4 w-64 max-w-[calc(100vw-2rem)] rounded origin-top border-2 border-yellow bg-dark-grey shadow-lg z-50">
							<div class="flex flex-col gap-2 text-yellow">
								<span class="text-white text-sm text-center">
									Период:
								</span>
								<DatePicker
									class="w-full text-xs"
									v-model="date"
									placeholder="Период"
									range
									:text-input="textInputOptions"
									:enable-time-picker="false"
									:inputClass="'filterCalendar'"
									position="center"
									circle
									lang="bg"
									dark
									@open="datePickerOpen = true"
									@closed="datePickerOpen = false" />
							</div>
							<div class="flex justify-between items-center">
								<span class="flex gap-2 items-center">
									<input
										type="checkbox"
										name="active"
										id="active-mobile"
										v-model="activeEvent"
										class="w-3 h-3" />
									<label
										for="active-mobile"
										class="text-sm text-white"
										>Активно събитие</label
									>
								</span>
								<button
									@click="
										filtersOpen = false;
										filterBy();
									"
									class="border rounded-xl px-3 py-1.5 min-w-0 bg-yellow text-black hover:opacity-90 transition-colors cursor-pointer whitespace-nowrap text-xs">
									Запази
								</button>
							</div>
						</div>
					</TransitionChild>
				</TransitionRoot>
			</div>
			<div class="relative inline-block text-left">
				<button
					type="button"
					@click="sortOpened = !sortOpened"
					class="sort-button flex items-center justify-center md:w-12 w-8 md:h-12 h-8 text-yellow transition-all duration-300 hover:text-white hover:scale-110 cursor-pointer">
					<SortIcon class="w-8 h-8" />
				</button>
				<TransitionRoot
					as="template"
					:show="sortOpened && filtersOpen === false"
					ref="sortMenu">
					<TransitionChild
						as="template"
						enter="transition-opacity ease-in-out duration-200"
						enter-from="opacity-0"
						enter-to="opacity-100"
						leave="transition-opacity ease-in-out duration-200"
						leave-from="opacity-100"
						leave-to="opacity-0">
						<div
							class="sort-dropdown-content absolute mt-1 right-0 mr-2 flex flex-col w-48 max-w-[calc(100vw-1rem)] rounded origin-top-right border-2 border-yellow bg-dark-grey shadow-lg z-50 py-2">
							<button
								class="text-white hover:text-yellow hover:bg-yellow/10 active:text-yellow cursor-pointer px-4 py-2 text-left transition-colors duration-200"
								@click="
									sort = 'new';
									sortOpened = false;
									filterBy();
								">
								Най-нови
							</button>
							<button
								class="text-white hover:text-yellow hover:bg-yellow/10 active:text-yellow cursor-pointer px-4 py-2 text-left transition-colors duration-200"
								@click="
									sort = 'old';
									sortOpened = false;
									filterBy();
								">
								Най-стари
							</button>
							<button
								class="text-white hover:text-yellow hover:bg-yellow/10 active:text-yellow cursor-pointer px-4 py-2 text-left transition-colors duration-200"
								@click="
									sort = 'az';
									sortOpened = false;
									filterBy();
								">
								Азбучен ред: А до Я
							</button>
							<button
								class="text-white hover:text-yellow hover:bg-yellow/10 cursor-pointer px-4 py-2 text-left transition-colors duration-200"
								@click="
									sort = 'za';
									sortOpened = false;
									filterBy();
								">
								Азбучен ред: Я до А
							</button>
						</div>
					</TransitionChild>
				</TransitionRoot>
			</div>
			<div>
				<button
					@click="deleteFilters"
					class="flex items-center justify-center w-12 md:h-12 h-8 text-yellow hover:text-yellow-400 transition-all duration-300 hover:scale-110 cursor-pointer">
					<CancelIcon class="w-8 h-8" />
				</button>
			</div>
		</div>
	</div>
</template>
