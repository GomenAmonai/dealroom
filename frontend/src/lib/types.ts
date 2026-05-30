export type DealStatus = "Draft" | "Negotiation" | "Active" | "Closed" | "Cancelled";
export type DocumentStatus = "Pending" | "Approved" | "Rejected";

export interface AuthResponse {
  accessToken: string;
  refreshToken: string;
}

export interface RegisterRequest {
  name: string;
  email: string;
  password: string;
  organizationName: string;
}

export interface LoginRequest {
  email: string;
  password: string;
}

export interface OrganizationResponse {
  id: number;
  name: string;
  createdAt: string;
}

export interface CreateDealRequest {
  title: string;
  counterpartyOrganizationId: number;
}

export interface DealResponse {
  id: number;
  title: string;
  status: DealStatus;
  initiatorOrganizationName: string;
  counterpartyOrganizationName: string;
  createdAt: string;
  updatedAt: string | null;
}

export interface DocumentResponse {
  id: number;
  dealId: number;
  fileName: string;
  contentType: string;
  sizeBytes: number;
  status: DocumentStatus;
  uploadedByOrganizationId: number;
  uploadedAt: string;
  reviewedAt: string | null;
}

export interface MessageResponse {
  id: number;
  dealId: number;
  senderUserId: number;
  senderOrganizationId: number;
  senderName: string;
  content: string;
  sentAt: string;
}

export const ALLOWED_TRANSITIONS: Record<DealStatus, DealStatus[]> = {
  Draft: ["Negotiation", "Active", "Cancelled"],
  Negotiation: ["Active", "Cancelled"],
  Active: ["Closed", "Cancelled"],
  Closed: [],
  Cancelled: [],
};
