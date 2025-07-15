<template>
	<div
		class="fixed top-0 left-0 w-screen h-screen flex justify-center items-center pointer-events-none bg-transparent z-[9999]">
		<div class="w-12 h-12 relative">
			<!-- Inner spinning dots (orange) - behind -->
			<div
				class="absolute inset-1 animate-spin z-0"
				style="animation-duration: 1s">
				<div
					class="absolute top-0 left-1/2 w-2 h-2 bg-orange rounded-full transform -translate-x-1/2"></div>
				<div
					class="absolute bottom-0 left-1/2 w-2 h-2 bg-orange rounded-full transform -translate-x-1/2"></div>
				<div
					class="absolute right-0 top-1/2 w-2 h-2 bg-orange rounded-full transform -translate-y-1/2"></div>
				<div
					class="absolute left-0 top-1/2 w-2 h-2 bg-orange rounded-full transform -translate-y-1/2"></div>
			</div>
			<!-- Outer spinning dots (yellow) - in front -->
			<div
				class="absolute inset-0 animate-spin z-10"
				style="animation-duration: 2s">
				<div
					class="absolute top-0 left-1/2 w-3 h-3 bg-yellow rounded-full transform -translate-x-1/2"></div>
				<div
					class="absolute bottom-0 left-1/2 w-3 h-3 bg-yellow rounded-full transform -translate-x-1/2"></div>
				<div
					class="absolute right-0 top-1/2 w-3 h-3 bg-yellow rounded-full transform -translate-y-1/2"></div>
				<div
					class="absolute left-0 top-1/2 w-3 h-3 bg-yellow rounded-full transform -translate-y-1/2"></div>
			</div>
		</div>
	</div>
</template>

<script lang="ts" setup>
// No logic needed
</script>
