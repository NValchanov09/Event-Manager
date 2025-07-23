<template>
	<div
		class="group relative overflow-hidden rounded-2xl bg-gradient-to-br from-slate-900 via-slate-800 to-slate-900 shadow-2xl hover:shadow-2xl transition-all duration-500 ease-out hover:-translate-y-2 cursor-pointer border border-slate-700/50"
		@click="redirect"
		:title="`Кликнете за повече подробности за ${event.name}`">
		
		<!-- Background gradient overlay -->
		<div class="absolute inset-0 bg-gradient-to-br from-yellow-500/5 via-transparent to-blue-500/5 opacity-0 group-hover:opacity-100 transition-opacity duration-500"></div>
		
		<!-- Image Section -->
		<div class="relative h-70 overflow-hidden">
			<img 
				:src="event.imageUrl" 
				:alt="event.name"
				class="w-full h-full object-cover transition-transform duration-700 group-hover:scale-110"
			/>
			<!-- Image overlay gradient -->
			<div class="absolute inset-0 bg-gradient-to-t from-black/60 via-transparent to-transparent"></div>
			
			<!-- Spots indicator -->
			<div class="absolute top-4 right-4">
				<div :class="[
					'px-3.5 py-1.5 rounded-full text-s text-white font-semibold backdrop-blur-sm shadow-lg/20 shadow-neutral',
					event.spotsLeft == null
						? 'bg-green-600/90'
						: event.spotsLeft === 0 
							? 'bg-red-500/90' 
							: event.spotsLeft <= 5 
								? 'bg-orange-500/90'
								: event.spotsLeft <= 10
									? 'bg-yellow-500/90'
									: 'bg-green-600/90'
				]">
					{{ event.spotsLeft == null ? 'Без лимит' : event.spotsLeft === 0 ? 'Пълно' : `${event.spotsLeft} места`}}
				</div>
			</div>
		</div>

		<!-- Content Section -->
		<div class="p-6 space-y-4">
			<!-- Event Title -->
			<h2 class="text-2xl font-bold text-white leading-tight line-clamp-2 group-hover:text-yellow-400 transition-colors duration-300">
				{{ event.name }}
			</h2>

			<!-- Event Details -->
			<div class="space-y-3">
				<!-- Location -->
				<div class="flex items-start gap-3 text-gray-300">
					<div class="flex-shrink-0">
						<LocationIcon class="w-7 h-7 text-yellow-400" />
					</div>
					<span class="text-sm leading-relaxed mt-1.5">{{ event.location }}</span>
				</div>

				<!-- Date -->
				<div class="flex items-start gap-3 text-gray-300">
					<div class="flex-shrink-0">
						<CalendarIcon class="w-7 h-7 text-yellow-400" />
					</div>
					<span class="text-sm leading-relaxed mt-1.5">{{ formatDate(event.date) }}</span>
				</div>

				<!-- Description -->
				<div class="flex items-start gap-3 text-gray-300">
					<div class="flex-shrink-0">
						<InfoIcon class="w-7 h-7 text-yellow-400" />
					</div>
					<p class="text-sm leading-relaxed line-clamp-3 mt-1.5">
						{{ shortenedDescription || 'Няма описание' }}
					</p>
				</div>
			</div>

			<!-- Action Button -->
			<div class="pt-4">
				<button
					v-if="!event.userSignedUp"
					@click.stop="redirect"
					:class="[
						'w-full py-3 px-4 rounded-xl font-semibold text-sm transition-all duration-300 transform',
						'bg-gradient-to-r from-yellow-500 to-yellow-600 hover:from-yellow-400 hover:to-yellow-600 text-black shadow-lg hover:shadow-yellow-500/25 hover:scale-105 active:scale-95'
					]"
					title="Кликнете за повече подробности и записване в събитието">
					Запиши се
				</button>
				<button
					v-else
					@click.stop="redirect"
					title="Кликнете за повече подробности и управление на записването"
					class="w-full py-3 px-4 bg-gradient-to-r from-red-500 to-red-600 hover:from-red-400 hover:to-red-600 text-white rounded-xl font-semibold text-sm transition-all duration-300 transform shadow-lg hover:shadow-red-500/25 hover:scale-105 active:scale-95">
					Отпиши се
				</button>
			</div>
		</div>

		<!-- Subtle border glow on hover -->
		<div class="absolute inset-0 rounded-2xl border-2 border-transparent group-hover:shadow-black -400/20 transition-all duration-500 pointer-events-none"></div>
	</div>
</template>

<script setup lang="ts">
import CalendarIcon from "./icons/CalendarIcon.vue";
import type { Event } from "@/utils/types";
import LocationIcon from "./icons/LocationIcon.vue";
import InfoIcon from "./icons/InfoIcon.vue";
import { useRouter } from "vue-router";
import { formatDateTime } from "@/utils/date";

const { event } = defineProps<{
	event: Event;
}>();

const router = useRouter();

const shortenedDescription = event.description
	? event.description.slice(0, 60) +
		(event.description.length > 60 ? "..." : "")
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

<style scoped>
.line-clamp-2 {
	display: -webkit-box;
	-webkit-line-clamp: 2;
	-webkit-box-orient: vertical;
	overflow: hidden;
}

.line-clamp-3 {
	display: -webkit-box;
	-webkit-line-clamp: 3;
	-webkit-box-orient: vertical;
	overflow: hidden;
}
</style>
