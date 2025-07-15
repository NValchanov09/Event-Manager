import { ref } from "vue";

export function useAsync<T>(fn: () => Promise<T>) {
	const loading = ref(false);
	const error = ref<Error | null>(null);
	const data = ref<T | null>(null);

	const execute = async () => {
		loading.value = true;
		error.value = null;

		try {
			data.value = await fn();
		} catch (e) {
			error.value = e as Error;
		} finally {
			loading.value = false;
		}
	};

	return { loading, error, data, execute };
}
