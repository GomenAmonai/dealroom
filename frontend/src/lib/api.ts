import { clearTokens, getAccessToken, getRefreshToken, setTokens } from "./auth";
import type {
  AuthResponse,
  CreateDealRequest,
  DealResponse,
  DealStatus,
  DocumentResponse,
  LoginRequest,
  MessageResponse,
  OrganizationResponse,
  RegisterRequest,
} from "./types";

export const API_URL = process.env.NEXT_PUBLIC_API_URL ?? "http://localhost:8080";

export class ApiError extends Error {
  status: number;

  constructor(status: number, message: string) {
    super(message);
    this.name = "ApiError";
    this.status = status;
  }
}

async function parseError(res: Response): Promise<ApiError> {
  let message = res.statusText || "Request failed";
  try {
    const body = await res.json();
    if (body?.error) message = body.error;
  } catch {
    // response had no JSON body
  }
  return new ApiError(res.status, message);
}

async function refreshTokens(): Promise<boolean> {
  const refreshToken = getRefreshToken();
  if (!refreshToken) return false;

  const res = await fetch(`${API_URL}/auth/refresh`, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify({ refreshToken }),
  });

  if (!res.ok) {
    clearTokens();
    return false;
  }

  setTokens((await res.json()) as AuthResponse);
  return true;
}

interface RequestOptions {
  method?: string;
  body?: unknown;
  auth?: boolean;
}

async function request<T>(path: string, options: RequestOptions = {}, retry = true): Promise<T> {
  const { method = "GET", body, auth = true } = options;
  const isForm = body instanceof FormData;

  const headers: Record<string, string> = {};
  if (body !== undefined && !isForm) headers["Content-Type"] = "application/json";
  if (auth) {
    const token = getAccessToken();
    if (token) headers["Authorization"] = `Bearer ${token}`;
  }

  const res = await fetch(`${API_URL}${path}`, {
    method,
    headers,
    body: body === undefined ? undefined : isForm ? (body as FormData) : JSON.stringify(body),
  });

  if (res.status === 401 && retry && auth && getRefreshToken()) {
    if (await refreshTokens()) return request<T>(path, options, false);
  }

  if (!res.ok) throw await parseError(res);
  if (res.status === 204) return undefined as T;

  const contentType = res.headers.get("content-type") ?? "";
  if (contentType.includes("application/json")) return (await res.json()) as T;
  return undefined as T;
}

export const authApi = {
  register: (body: RegisterRequest) =>
    request<AuthResponse>("/auth/register", { method: "POST", body, auth: false }),
  login: (body: LoginRequest) =>
    request<AuthResponse>("/auth/login", { method: "POST", body, auth: false }),
};

export const organizationsApi = {
  me: () => request<OrganizationResponse>("/organizations/me"),
};

export const dealsApi = {
  list: () => request<DealResponse[]>("/deals"),
  get: (id: number) => request<DealResponse>(`/deals/${id}`),
  create: (body: CreateDealRequest) => request<DealResponse>("/deals", { method: "POST", body }),
  updateStatus: (id: number, status: DealStatus) =>
    request<DealResponse>(`/deals/${id}/status`, { method: "PATCH", body: { status } }),
};

export const documentsApi = {
  list: (dealId: number) => request<DocumentResponse[]>(`/deals/${dealId}/documents`),
  upload: (dealId: number, file: File) => {
    const form = new FormData();
    form.append("file", file);
    return request<DocumentResponse>(`/deals/${dealId}/documents`, { method: "POST", body: form });
  },
  approve: (id: number) => request<DocumentResponse>(`/documents/${id}/approve`, { method: "POST" }),
  reject: (id: number) => request<DocumentResponse>(`/documents/${id}/reject`, { method: "POST" }),
};

export const messagesApi = {
  list: (dealId: number) => request<MessageResponse[]>(`/deals/${dealId}/messages`),
  send: (dealId: number, content: string) =>
    request<MessageResponse>(`/deals/${dealId}/messages`, { method: "POST", body: { content } }),
};

// Document content needs the Authorization header, so it is fetched as a blob
// rather than linked to directly.
export async function downloadDocument(id: number, fileName: string): Promise<void> {
  const token = getAccessToken();
  const res = await fetch(`${API_URL}/documents/${id}/content`, {
    headers: token ? { Authorization: `Bearer ${token}` } : {},
  });
  if (!res.ok) throw await parseError(res);

  const blob = await res.blob();
  const url = URL.createObjectURL(blob);
  const link = document.createElement("a");
  link.href = url;
  link.download = fileName;
  link.click();
  URL.revokeObjectURL(url);
}
