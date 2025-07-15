<template>
	<button class="text-white hover:text-yellow transition-colors p-2">
		<svg
			class="w-6 h-6"
			viewBox="0 0 24 24"
			fill="none"
			stroke="currentColor"
			stroke-width="2"
			stroke-linecap="round"
			stroke-linejoin="round">
			<line
				x1="4"
				y1="12"
				x2="20"
				y2="12"
				class="transition-transform duration-300"
				:class="isMobileMenuOpen ? 'rotate-45 translate-y-0' : ''" />
			<line
				x1="4"
				y1="6"
				x2="20"
				y2="6"
				class="transition-all duration-300"
				:class="isMobileMenuOpen ? 'opacity-0 translate-x-3' : ''" />
			<line
				x1="4"
				y1="18"
				x2="20"
				y2="18"
				class="transition-transform duration-300"
				:class="isMobileMenuOpen ? '-rotate-45 -translate-1' : ''" />
		</svg>
	</button>
</template>
<script setup lang="ts">
defineProps({ isMobileMenuOpen: Boolean });
</script>
<style scoped>
.transition-all {
	transition-property: all;
}

svg line {
	transform-origin: center;
}
</style>
