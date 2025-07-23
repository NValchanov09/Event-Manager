<template>
	<div
		class="w-full h-full overflow-auto bg-grey text-white flex flex-col justify-center items-center px-6 py-12 text-center gap-y-8">
		<h1
			class="text-4xl sm:text-[120px] leading-none font-black text-yellow">
			404
		</h1>
		<h2 class="text-3xl md:text-4xl font-semibold">
			Страницата не беше намерена
		</h2>
		<p class="text-white/80 max-w-md">
			Страницата, която търсите, не съществува или е била преместена.
		</p>
		<RouterLink
			to="/"
			class="inline-block bg-yellow text-grey px-6 py-3 rounded-lg font-semibold hover:opacity-90 transition whitespace-nowrap">
			Към началната страница
		</RouterLink>
	</div>
</template>
