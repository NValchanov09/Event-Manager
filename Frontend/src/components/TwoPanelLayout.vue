<script setup lang="ts">
const { actionName } = defineProps<{
	actionName: string;
}>();
</script>
<template>
	<div class="w-full h-full overflow-auto bg-grey text-white flex flex-col">
		<!-- Middle Section (Left and Right Panels) - Shared Scroll -->
		<div class="flex-1 overflow-auto min-h-0">
			<h1 class="text-yellow text-3xl font-semibold text-center my-6">
				{{ actionName }}
			</h1>

			<div class="grid grid-cols-1 lg:grid-cols-2 min-h-full">
				<!-- Left Panel -->
				<div class="md:px-8 px-4 py-2 lg:border-r lg:border-white/20">
					<slot name="left">
						<div class="text-center text-white/60 py-8">
							<h3 class="text-xl font-semibold mb-2">Left Panel</h3>
							<p>Left content goes here.</p>
						</div>
					</slot>
				</div>

				<!-- Right Panel -->
				<div class="md:px-8 px-4 py-2 lg:border-l lg:border-white/20">
					<slot name="right">
						<div class="text-center text-white/60 py-8">
							<h3 class="text-xl font-semibold mb-2">Right Panel</h3>
							<p>Right content goes here.</p>
						</div>
					</slot>
				</div>
			</div>
		</div>
	</div>
</template>
