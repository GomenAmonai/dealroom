"use client";

import { FormEvent, useEffect, useState } from "react";
import Link from "next/link";
import AppShell from "@/components/AppShell";
import StatusBadge from "@/components/StatusBadge";
import { ApiError, dealsApi } from "@/lib/api";
import type { DealResponse } from "@/lib/types";

const inputClass =
  "w-full rounded-lg border border-line bg-surface-input px-3.5 py-2.5 text-sm text-fg placeholder-faint outline-none transition focus:border-gold/50 focus:ring-2 focus:ring-gold/15";
const labelClass = "mb-1.5 block text-xs font-medium uppercase tracking-wide text-muted";

export default function DealsPage() {
  const [deals, setDeals] = useState<DealResponse[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

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
    } catch (err) {
      setCreateError(err instanceof ApiError ? err.message : "Failed to create deal");
    } finally {
      setCreating(false);
    }
  }

  return (
    <AppShell>
      <div className="animate-fade-up">
        <p className="font-mono text-[11px] uppercase tracking-[0.2em] text-gold">Your workspace</p>
        <h1 className="mt-2 font-display text-4xl font-semibold tracking-tight text-fg">Deals</h1>
      </div>

      <form
        onSubmit={handleCreate}
        className="mt-7 grid animate-fade-up gap-4 rounded-2xl border border-line bg-surface/70 p-5 shadow-card sm:grid-cols-[1fr_200px_auto] sm:items-end"
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
          className="h-[42px] rounded-lg bg-gold px-5 text-sm font-semibold text-ink transition hover:bg-gold-soft disabled:opacity-60"
        >
          {creating ? "Creating…" : "Create"}
        </button>
        {createError && <p className="text-sm text-rose-300 sm:col-span-3">{createError}</p>}
      </form>

      <div className="mt-8">
        {loading && <p className="text-sm text-muted">Loading deals…</p>}
        {error && <p className="text-sm text-rose-300">{error}</p>}
        {!loading && !error && deals.length === 0 && (
          <p className="rounded-2xl border border-dashed border-line bg-surface/40 px-5 py-10 text-center text-sm text-muted">
            No deals yet. Open your first one above.
          </p>
        )}

        <ul className="space-y-3">
          {deals.map((deal) => (
            <li key={deal.id}>
              <Link
                href={`/deals/${deal.id}`}
                className="group flex items-center justify-between rounded-xl border border-line bg-surface/60 p-4 transition hover:border-gold/30 hover:bg-surface"
              >
                <div className="min-w-0">
                  <p className="truncate font-medium text-fg">{deal.title}</p>
                  <p className="mt-1 truncate font-mono text-xs text-muted">
                    {deal.initiatorOrganizationName} <span className="text-gold">↔</span>{" "}
                    {deal.counterpartyOrganizationName}
                  </p>
                </div>
                <div className="flex items-center gap-4 pl-4">
                  <StatusBadge status={deal.status} />
                  <span className="text-muted transition group-hover:translate-x-0.5 group-hover:text-gold" aria-hidden>
                    →
                  </span>
                </div>
              </Link>
            </li>
          ))}
        </ul>
      </div>
    </AppShell>
  );
}
