import type { DealStatus, DocumentStatus } from "@/lib/types";

const STYLES: Record<string, { dot: string; pill: string }> = {
  Draft: { dot: "bg-slate-400", pill: "text-slate-300 ring-slate-400/20 bg-slate-400/10" },
  Negotiation: { dot: "bg-amber-400", pill: "text-amber-200 ring-amber-400/25 bg-amber-400/10" },
  Active: { dot: "bg-sky-400", pill: "text-sky-200 ring-sky-400/25 bg-sky-400/10" },
  Closed: { dot: "bg-emerald-400", pill: "text-emerald-200 ring-emerald-400/25 bg-emerald-400/10" },
  Cancelled: { dot: "bg-rose-400", pill: "text-rose-200 ring-rose-400/25 bg-rose-400/10" },
  Pending: { dot: "bg-amber-400", pill: "text-amber-200 ring-amber-400/25 bg-amber-400/10" },
  Approved: { dot: "bg-emerald-400", pill: "text-emerald-200 ring-emerald-400/25 bg-emerald-400/10" },
  Rejected: { dot: "bg-rose-400", pill: "text-rose-200 ring-rose-400/25 bg-rose-400/10" },
};

export default function StatusBadge({ status }: { status: DealStatus | DocumentStatus }) {
  const style = STYLES[status] ?? STYLES.Draft;
  return (
    <span
      className={`inline-flex items-center gap-1.5 rounded-full px-2.5 py-1 font-mono text-[11px] font-medium uppercase tracking-wider ring-1 ${style.pill}`}
    >
      <span className={`h-1.5 w-1.5 rounded-full ${style.dot}`} aria-hidden />
      {status}
    </span>
  );
}
