import type { DealStatus, DocumentStatus } from "@/lib/types";

const PILL: Record<string, string> = {
  Draft: "bg-slate-100 text-slate-600 ring-slate-200",
  Negotiation: "bg-amber-50 text-amber-700 ring-amber-200",
  Active: "bg-sky-50 text-sky-700 ring-sky-200",
  Closed: "bg-emerald-50 text-emerald-700 ring-emerald-200",
  Cancelled: "bg-rose-50 text-rose-700 ring-rose-200",
  Pending: "bg-amber-50 text-amber-700 ring-amber-200",
  Approved: "bg-emerald-50 text-emerald-700 ring-emerald-200",
  Rejected: "bg-rose-50 text-rose-700 ring-rose-200",
};

const DOT: Record<string, string> = {
  Draft: "bg-slate-400",
  Negotiation: "bg-amber-500",
  Active: "bg-sky-500",
  Closed: "bg-emerald-500",
  Cancelled: "bg-rose-500",
  Pending: "bg-amber-500",
  Approved: "bg-emerald-500",
  Rejected: "bg-rose-500",
};

export default function StatusBadge({ status }: { status: DealStatus | DocumentStatus }) {
  return (
    <span
      className={`inline-flex items-center gap-1.5 rounded-full px-2 py-0.5 text-xs font-medium ring-1 ${PILL[status] ?? PILL.Draft}`}
    >
      <span className={`h-1.5 w-1.5 rounded-full ${DOT[status] ?? DOT.Draft}`} aria-hidden />
      {status}
    </span>
  );
}
