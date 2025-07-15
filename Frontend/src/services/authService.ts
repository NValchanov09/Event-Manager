import { apiClient } from "../utils/api";
export const authApi = {
  login: (email: string, password: string) =>
    apiClient.post<{ accessToken: string, refreshToken: string }>("/login", { email, password }),
  register: (email: string, password: string) =>
    apiClient.post<{ accessToken: string, refreshToken: string }>("/register", { email, password }),
};
