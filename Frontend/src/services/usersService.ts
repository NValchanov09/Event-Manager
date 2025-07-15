import { apiClient } from "@/utils/api";
import type { User } from "@/utils/types";

export function getUsers() {
	return apiClient.get<User[]>("/users");
}

export function getMe() {
	return apiClient.get<User>("/users/me");
}

export function addAdmin(userId: string) {
	return apiClient.post(`/users/admin/${userId}`);
}

export function removeAdmin(userId: string) {
	return apiClient.delete(`/users/admin/${userId}`);
}
