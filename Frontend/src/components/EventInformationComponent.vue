<script setup lang="ts">
import type { Event } from "@/utils/types";

import CalendarIcon from "./icons/CalendarIcon.vue";
import LocationIcon from "./icons/LocationIcon.vue";
import ClockIcon from "./icons/ClockIcon.vue";
import { formatDateTime } from "@/utils/date.ts";

const { event } = defineProps<{
	event: Event;
}>();

// Helper function to format datetime
</script>

<template>
	<div class="sticky top-6 space-y-6">
		<!-- Event Title -->
		<div class="border-b border-white/20 pb-4">
			<h1
				class="text-3xl font-bold text-yellow break-words hyphens-auto leading-tight">
				{{ event.name }}
			</h1>
		</div>

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
		</div>

		<!-- Event Description -->
		<div class="border-t border-white/20 pt-6">
			<h3 class="text-xl font-semibold text-yellow mb-3">За събитието</h3>
			<p
				class="text-white/90 leading-relaxed whitespace-pre-wrap break-words">
				{{ event.description }}
			</p>
		</div>
	</div>
</template>
