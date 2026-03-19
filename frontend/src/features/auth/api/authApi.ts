import type { AuthResponse, CurrentUser, LoginRequest, RegisterRequest } from "../../../shared/types/api";
import { apiRequest } from "../../../shared/api/httpClient";

export const authApi = {
  register: (payload: RegisterRequest) =>
    apiRequest<AuthResponse>("/auth/register", {
      method: "POST",
      body: JSON.stringify(payload),
    }),

  login: (payload: LoginRequest) =>
    apiRequest<AuthResponse>("/auth/login", {
      method: "POST",
      body: JSON.stringify(payload),
    }),

  me: (token: string) =>
    apiRequest<CurrentUser>("/auth/me", {
      method: "GET",
      token,
    }),
};
