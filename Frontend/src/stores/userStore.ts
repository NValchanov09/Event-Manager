import { getMe } from "@/services/usersService";
import { apiClient } from "@/utils/api";
import type { User } from "@/utils/types";
import { defineStore } from "pinia";

let refreshIntervalId: ReturnType<typeof setInterval> |null = null;

export const useUserStore = defineStore("user", {
	state: () => ({
		accessToken: null as string | null,
		profile: null as User | null,
		refreshToken: localStorage.getItem("refreshToken"),
		expiresIn: null as number | null,
	}),
	getters: {
		isAuthenticated: state => !!state.accessToken,
		isAdmin: state => {
			return state.profile?.roles.includes("Administrator") ?? false;
		},
	},
	actions: {
		async refreshTokens() {
			try {
				const res = await apiClient.post<{ accessToken: string, refreshToken: string }>(
					"/refresh",
					{
						refreshToken: this.refreshToken,
					},
					false
				);

				if (res.refreshToken) {
					this.setAccessToken(res.accessToken);
				}
				
				if (res.refreshToken) {
					this.setRefreshToken(res.refreshToken);
				}

				return true;
			} catch (err) {
				console.error("Failed to refresh token", err);

				this.logout();

				return false;
			}
		},
		setAccessToken(token: string) {
			this.accessToken = token;

			console.log("Access Token changed.");
		},
		setRefreshToken(token: string) {
			this.refreshToken = token;

			localStorage.setItem("refreshToken", token);

			console.log("Refresh Token changed.");
		},
		async logout() {
			this.stopRefreshTokenLoop();
			this.accessToken = null;
			this.profile = null;
			this.refreshToken = null;
			localStorage.removeItem("refreshToken");
		},
		async fetchUser() {
			try {
				this.profile = await getMe();
			} catch (err) {
				console.error("Failed to fetch user profile:", err);
				this.profile = null;
			}
		},
		startRefreshTokenLoop(intervalInMinutes: number = 14) {
			if (refreshIntervalId) clearInterval(refreshIntervalId); // prevent duplicates

			refreshIntervalId = setInterval(async () => {
				const success = await this.refreshTokens();
				if (!success) {
					this.stopRefreshTokenLoop(); // cleanup if refresh fails
				}
			}, intervalInMinutes * 60 * 1000);
		},
		stopRefreshTokenLoop() {
			if (refreshIntervalId) {
				clearInterval(refreshIntervalId);
				refreshIntervalId = null;
			}
		},
	},
});
