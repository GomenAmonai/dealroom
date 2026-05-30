"use client";

import { FormEvent, useEffect, useState } from "react";
import Link from "next/link";
import AppShell from "@/components/AppShell";
import StatusBadge from "@/components/StatusBadge";
import { ApiError, dealsApi } from "@/lib/api";
import type { DealResponse } from "@/lib/types";

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
      <h1 className="text-2xl font-semibold text-slate-900">Deals</h1>

      <form
        onSubmit={handleCreate}
        className="mt-6 grid gap-4 rounded-xl border border-slate-200 bg-white p-5 sm:grid-cols-[1fr_220px_auto] sm:items-end"
      >
        <div>
          <label htmlFor="title" className="block text-sm font-medium text-slate-700">
            Deal title
          </label>
          <input
            id="title"
            required
            value={title}
            onChange={(e) => setTitle(e.target.value)}
            placeholder="Supply contract"
            className="mt-1 w-full rounded-md border border-slate-300 px-3 py-2 text-sm focus:border-brand-500 focus:outline-none focus:ring-1 focus:ring-brand-500"
          />
        </div>
        <div>
          <label htmlFor="counterparty" className="block text-sm font-medium text-slate-700">
            Counterparty org ID
          </label>
          <input
            id="counterparty"
            type="number"
            required
            min={1}
            value={counterpartyId}
            onChange={(e) => setCounterpartyId(e.target.value)}
            placeholder="e.g. 2"
            className="mt-1 w-full rounded-md border border-slate-300 px-3 py-2 text-sm focus:border-brand-500 focus:outline-none focus:ring-1 focus:ring-brand-500"
          />
        </div>
        <button
          type="submit"
          disabled={creating}
          className="h-[38px] rounded-md bg-brand-600 px-4 text-sm font-medium text-white hover:bg-brand-700 disabled:opacity-60"
        >
          {creating ? "Creating…" : "Create deal"}
        </button>
        {createError && <p className="text-sm text-rose-600 sm:col-span-3">{createError}</p>}
      </form>

      <div className="mt-8">
        {loading && <p className="text-sm text-slate-500">Loading deals…</p>}
        {error && <p className="text-sm text-rose-600">{error}</p>}
        {!loading && !error && deals.length === 0 && (
          <p className="text-sm text-slate-500">No deals yet. Create your first one above.</p>
        )}

        <ul className="space-y-3">
          {deals.map((deal) => (
            <li key={deal.id}>
              <Link
                href={`/deals/${deal.id}`}
                className="flex items-center justify-between rounded-xl border border-slate-200 bg-white p-4 transition hover:border-brand-300 hover:shadow-sm"
              >
                <div>
                  <p className="font-medium text-slate-900">{deal.title}</p>
                  <p className="mt-0.5 text-sm text-slate-500">
                    {deal.initiatorOrganizationName} ↔ {deal.counterpartyOrganizationName}
                  </p>
                </div>
                <StatusBadge status={deal.status} />
              </Link>
            </li>
          ))}
        </ul>
      </div>
    </AppShell>
  );
}
