import type { DealStatus, DocumentStatus } from "@/lib/types";

const STYLES: Record<string, string> = {
  Draft: "bg-slate-100 text-slate-700",
  Negotiation: "bg-amber-100 text-amber-800",
  Active: "bg-brand-100 text-brand-700",
  Closed: "bg-emerald-100 text-emerald-800",
  Cancelled: "bg-rose-100 text-rose-700",
  Pending: "bg-amber-100 text-amber-800",
  Approved: "bg-emerald-100 text-emerald-800",
  Rejected: "bg-rose-100 text-rose-700",
};

export default function StatusBadge({ status }: { status: DealStatus | DocumentStatus }) {
  const style = STYLES[status] ?? "bg-slate-100 text-slate-700";
  return (
    <span className={`inline-flex rounded-full px-2.5 py-0.5 text-xs font-medium ${style}`}>
      {status}
    </span>
  );
}
