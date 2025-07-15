import { getMe } from "@/services/usersService";
import { apiClient } from "@/utils/api";
import type { User } from "@/utils/types";
import { defineStore } from "pinia";

export const useUserStore = defineStore("user", {
	state: () => ({
		accessToken: null as string | null,
		profile: null as User | null,
		refreshToken: localStorage.getItem("refreshToken"),
	}),
	getters: {
		isAuthenticated: state => !!state.accessToken,
		isAdmin: state => {
			return state.profile?.roles.includes("Administrator") ?? false;
		},
	},
	actions: {
		async refreshAccessToken() {
			try {
				const res = await apiClient.post<{ accessToken: string }>(
					"/refresh",
					{
						refreshToken: this.refreshToken,
					},
					false
				);

				this.setAccessToken(res.accessToken);

				return true;
			} catch (err) {
				console.error("Failed to refresh token", err);

				this.logout();

				return false;
			}
		},
		setAccessToken(token: string) {
			this.accessToken = token;
		},
		setRefreshToken(token: string) {
			this.refreshToken = token;

			localStorage.setItem("refreshToken", token);
		},
		async logout() {
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
	},
});
