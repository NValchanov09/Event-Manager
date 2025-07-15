import { defineStore } from "pinia";

interface UIState {
	toastType: "success" | "info" | "warning" | "error";
	toastMessage: string;
	showToast: boolean;
}

export const useUIStore = defineStore("ui", {
	state: (): UIState => ({
		showToast: false,
		toastMessage: "",
		toastType: "info", // 'success', 'error', etc.
	}),
	actions: {
		triggerToast(message: string, type: "success" | "info" | "warning" | "error" = "info") {
			this.toastMessage = message;
			this.toastType = type;
			this.showToast = true;
		},
	},
});
