const API_BASE_URL = process.env.NEXT_PUBLIC_API_BASE_URL || "https://localhost:7072/api";

export class ApiError extends Error {
  constructor(public status: number, message: string) {
    super(message);
  }
}

interface RequestOptions extends RequestInit {
  tenantId: string;
}

export async function apiFetch<T>(path: string, options: RequestOptions): Promise<T> {
  const { tenantId, headers, ...rest } = options;

  const response = await fetch(`${API_BASE_URL}${path}`, {
    ...rest,
    headers: {
      "Content-Type": "application/json",
      "X-Tenant-Id": tenantId,
      ...headers,
    },
  });

  if (!response.ok) {
    const errorBody = await response.json().catch(() => ({ error: response.statusText }));
    throw new ApiError(response.status, errorBody.error || "Request failed");
  }

  // Handle empty responses (e.g., some PATCH/DELETE calls return no body)
  const text = await response.text();
  return text ? JSON.parse(text) : (undefined as T);
}