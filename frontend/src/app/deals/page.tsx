"use client";

import { FormEvent, useEffect, useState } from "react";
import Link from "next/link";
import { ArrowRight, Inbox, Plus } from "lucide-react";
import AppShell from "@/components/AppShell";
import StatusBadge from "@/components/StatusBadge";
import { ApiError, dealsApi } from "@/lib/api";
import type { DealResponse } from "@/lib/types";

const inputClass =
  "w-full rounded-lg border border-line bg-surface px-3 py-2.5 text-sm text-ink placeholder-faint outline-none transition focus:border-accent focus:ring-2 focus:ring-accent/15";
const labelClass = "mb-1.5 block text-xs font-medium text-muted";

function formatDate(value: string): string {
  return new Date(value).toLocaleDateString(undefined, { month: "short", day: "numeric" });
}

export default function DealsPage() {
  const [deals, setDeals] = useState<DealResponse[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  const [showForm, setShowForm] = useState(false);
  const [title, setTitle] = useState("");
  const [counterpartyId, setCounterpartyId] = useState("");
  const [creating, setCreating] = useState(false);
  const [createError, setCreateError] = useState<string | null>(null);

  useEffect(() => {
    dealsApi
      .list()
      .then(setDeals)
      .catch((err) => setError(err instanceof ApiError ? err.message : "Failed to load deals"))
      .finally(() => setLoading(false));
  }, []);

  async function handleCreate(event: FormEvent) {
    event.preventDefault();
    setCreateError(null);
    setCreating(true);
    try {
      const deal = await dealsApi.create({
        title,
        counterpartyOrganizationId: Number(counterpartyId),
      });
      setDeals((prev) => [deal, ...prev]);
      setTitle("");
      setCounterpartyId("");
      setShowForm(false);
    } catch (err) {
      setCreateError(err instanceof ApiError ? err.message : "Failed to create deal");
    } finally {
      setCreating(false);
    }
  }

  return (
    <AppShell>
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-2xl font-semibold tracking-tight text-ink">Deals</h1>
          <p className="mt-0.5 text-sm text-muted">
            {deals.length} {deals.length === 1 ? "deal" : "deals"}
          </p>
        </div>
        <button
          type="button"
          onClick={() => setShowForm((v) => !v)}
          className="inline-flex items-center gap-1.5 rounded-lg bg-accent px-3.5 py-2 text-sm font-semibold text-paper transition hover:bg-accent-hover"
        >
          <Plus size={16} aria-hidden />
          New deal
        </button>
      </div>

      {showForm && (
        <form
          onSubmit={handleCreate}
          className="mt-5 grid animate-fade-up gap-3 rounded-xl border border-line bg-surface p-4 shadow-card sm:grid-cols-[1fr_180px_auto] sm:items-end"
        >
          <div>
            <label htmlFor="title" className={labelClass}>
              Deal title
            </label>
            <input
              id="title"
              required
              value={title}
              onChange={(e) => setTitle(e.target.value)}
              placeholder="Supply contract"
              className={inputClass}
            />
          </div>
          <div>
            <label htmlFor="counterparty" className={labelClass}>
              Counterparty ID
            </label>
            <input
              id="counterparty"
              type="number"
              required
              min={1}
              value={counterpartyId}
              onChange={(e) => setCounterpartyId(e.target.value)}
              placeholder="e.g. 2"
              className={`${inputClass} font-mono`}
            />
          </div>
          <button
            type="submit"
            disabled={creating}
            className="h-[42px] rounded-lg bg-accent px-5 text-sm font-semibold text-paper transition hover:bg-accent-hover disabled:opacity-60"
          >
            {creating ? "Creating…" : "Create"}
          </button>
          {createError && <p className="text-sm text-rose-600 sm:col-span-3">{createError}</p>}
        </form>
      )}

      <div className="mt-6 overflow-hidden rounded-xl border border-line bg-surface shadow-card">
        <div className="grid grid-cols-[minmax(0,1fr)_minmax(0,1.2fr)_auto_auto] gap-4 border-b border-line px-5 py-2.5 text-[11px] font-medium uppercase tracking-wide text-faint">
          <span>Deal</span>
          <span>Parties</span>
          <span>Status</span>
          <span className="text-right">Updated</span>
        </div>

        {loading && <p className="px-5 py-10 text-sm text-muted">Loading…</p>}
        {error && <p className="px-5 py-10 text-sm text-rose-600">{error}</p>}
        {!loading && !error && deals.length === 0 && (
          <div className="flex flex-col items-center gap-2 px-5 py-16 text-center">
            <Inbox size={22} className="text-faint" aria-hidden />
            <p className="text-sm text-muted">No deals yet — create your first one.</p>
          </div>
        )}

        <ul className="divide-y divide-line">
          {deals.map((deal) => (
            <li key={deal.id}>
              <Link
                href={`/deals/${deal.id}`}
                className="grid grid-cols-[minmax(0,1fr)_minmax(0,1.2fr)_auto_auto] items-center gap-4 px-5 py-3.5 transition hover:bg-paper"
              >
                <span className="truncate font-medium text-ink">{deal.title}</span>
                <span className="truncate font-mono text-xs text-muted">
                  {deal.initiatorOrganizationName} ↔ {deal.counterpartyOrganizationName}
                </span>
                <StatusBadge status={deal.status} />
                <span className="flex items-center justify-end gap-2 font-mono text-xs text-faint">
                  {formatDate(deal.updatedAt ?? deal.createdAt)}
                  <ArrowRight size={14} aria-hidden />
                </span>
              </Link>
            </li>
          ))}
        </ul>
      </div>
    </AppShell>
  );
}
