const API_BASE_URL = import.meta.env.VITE_API_URL ?? "https://localhost:5001/api";

export class ApiError extends Error {
  status: number;
  payload?: unknown;

  constructor(message: string, status: number, payload?: unknown) {
    super(message);
    this.name = "ApiError";
    this.status = status;
    this.payload = payload;
  }
}

interface RequestOptions extends RequestInit {
  token?: string | null;
}

export async function apiRequest<T>(path: string, options: RequestOptions = {}): Promise<T> {
  const { token, headers, ...rest } = options;

  let response: Response;
  try {
    response = await fetch(`${API_BASE_URL}${path}`, {
      ...rest,
      headers: {
        "Content-Type": "application/json",
        ...(token ? { Authorization: `Bearer ${token}` } : {}),
        ...headers,
      },
    });
  } catch {
    throw new ApiError(
      `Unable to reach API at ${API_BASE_URL}. Make sure the backend is running and the URL is correct.`,
      0,
    );
  }

  const text = await response.text();
  const payload = text ? JSON.parse(text) : null;

  if (!response.ok) {
    const validationMessage =
      payload &&
      typeof payload === "object" &&
      "errors" in payload &&
      payload.errors &&
      typeof payload.errors === "object"
        ? Object.values(payload.errors as Record<string, unknown[]>)
            .flat()
            .find((value) => typeof value === "string")
        : null;

    const message =
      (validationMessage as string | undefined) ||
      (payload && typeof payload === "object" && "message" in payload && (payload as { message?: string }).message) ||
      `Request failed with status ${response.status}`;
    throw new ApiError(message ?? "Request failed", response.status, payload);
  }

  return payload as T;
}
