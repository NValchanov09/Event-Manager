<template>
	<div
		class="flex flex-col h-full py-4 pl-6 pr-10 gap-5 border-yellow rounded-lg border-2 border-solid bg-dark-grey transition delay-100 duration-300 ease-in-out hover:shadow-xl/30 cursor-pointer"
		@click="redirect"
		:title="`Кликнете за повече подробности за ${event.name}`">
		<h1
			class="w-full pt-3 text-yellow text-3xl font-semibold text-center break-words hyphens-auto leading-tight"
			:title="event.name">
			{{ event.name }}
		</h1>
		<div class="flex flex-col gap-3 flex-grow">
			<p
				class="flex pl-2 gap-3 items-start"
				:title="`Местоположение: ${event.location}`">
				<span><LocationIcon /></span>
				<span class="text-white text-lg break-words min-w-0 flex-1">{{
					event.location
				}}</span>
			</p>
			<p
				class="flex pl-2 gap-3 items-start"
				:title="`Дата на събитието: ${formatDateTime(event.date)}`">
				<span><CalendarIcon /></span>
				<span class="text-white text-lg break-words min-w-0 flex-1">{{
					formatDate(event.date)
				}}</span>
			</p>
			<p
				class="flex pl-2 gap-3 items-start"
				:title="`Краен срок за записване: ${formatDateTime(event.signUpDeadline)}`">
				<span><ClockIcon /></span>
				<span class="text-white text-lg break-words min-w-0 flex-1">{{
					formatDate(event.signUpDeadline)
				}}</span>
			</p>
			<p
				v-if="event.peopleLimit && event.peopleLimit > 0"
				class="flex pl-2 gap-3 items-start"
				:title="`Общо места: ${event.peopleLimit}, Заети: ${event.peopleLimit - event.spotsLeft}`">
				<span><UserIcon /></span>
				<span class="text-white text-lg break-words min-w-0 flex-1">
					{{
						event.spotsLeft === 0
							? "Няма свободни места"
							: `${event.spotsLeft} свободни места`
					}}
				</span>
			</p>
			<p
				class="flex pl-2 gap-3 items-start flex-grow"
				:title="event.description || 'Няма описание'">
				<span><InfoIcon /></span>
				<span class="text-white text-lg break-words min-w-0 flex-1">{{
					shortenedDescription
				}}</span>
			</p>
			<div class="mt-auto flex justify-end">
				<button
					v-if="!event.userSignedUp"
					@click.stop="redirect"
					:disabled="event.spotsLeft === 0"
					:title="
						event.spotsLeft === 0
							? 'Няма свободни места за това събитие'
							: 'Кликнете за повече подробности и записване в събитието'
					"
					:class="[
						'h-10 px-4 py-2 border-2 rounded-2xl border-solid transition-all duration-300 ease-in-out whitespace-nowrap flex items-center justify-center',
						event.spotsLeft === 0
							? 'border-grey-400 text-grey-400 cursor-not-allowed opacity-50'
							: 'text-white border-yellow-500 hover:text-white hover:border-transparent hover:bg-yellow-500 hover:scale-105 hover:shadow-lg',
					]">
					{{ event.spotsLeft === 0 ? "Няма места" : "Запиши се" }}
				</button>
				<button
					v-else
					@click.stop="redirect"
					title="Кликнете за повече подробности и управление на записването"
					class="h-10 px-4 py-2 border-2 text-white border-red-500 rounded-2xl border-solid transition-all duration-300 ease-in-out whitespace-nowrap flex items-center justify-center hover:text-white hover:border-transparent hover:bg-red-500 hover:scale-105 hover:shadow-lg">
					Отпиши се
				</button>
			</div>
		</div>
	</div>
</template>
<script setup lang="ts">
import CalendarIcon from "./icons/CalendarIcon.vue";
import type { Event } from "@/utils/types";
import LocationIcon from "./icons/LocationIcon.vue";
import ClockIcon from "./icons/ClockIcon.vue";
import InfoIcon from "./icons/InfoIcon.vue";
import UserIcon from "./icons/UserIcon.vue";
import { useRouter } from "vue-router";
import { formatDateTime } from "@/utils/date";

const { event } = defineProps<{
	event: Event;
}>();

const router = useRouter();
const shortenedDescription = event.description
	? event.description.slice(0, 50) +
		(event.description.length > 50 ? "..." : "")
	: "";

function formatDate(dateString: string) {
	if (!dateString) return "";
	// Use the shared formatDateTime function which handles UTC to local conversion
	const formatted = formatDateTime(dateString);
	// Make it shorter for card display
	return formatted.replace(/, \d{4}/, "").replace(/at /, "");
}

function redirect() {
	router.push(`/events/${event.id}`);
}
</script>
