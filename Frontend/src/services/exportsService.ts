import { useUserStore } from "@/stores/userStore.ts";
import { baseUrl } from "@/utils/api.ts";

export const downloadFile = async (
	endpoint: string,
	filename: string,
	type: string
) => {
	try {
		const response = await fetch(baseUrl + endpoint, {
			method: "GET",
			headers: {
				"Authorization": `Bearer ${useUserStore().accessToken}`,
				"Content-Type": `application/${type}`,
			},
		});

		if (!response.ok) {
			const errorText = await response.text();
			console.error(
				`Error downloading file: ${response.status} - ${errorText}`
			);
			return;
		}

		const blob = await response.blob();

		const link = document.createElement("a");
		link.href = URL.createObjectURL(blob);
		link.download = filename;
		link.style.display = "none";

		document.body.appendChild(link);
		link.click();
		document.body.removeChild(link);
		URL.revokeObjectURL(link.href);
	} catch (error) {
		console.error("Network or fetch error during file download:", error);
		alert(
			"An error occurred during download. Please check your internet connection and try again."
		);
	}
};
