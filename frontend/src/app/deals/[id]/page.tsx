"use client";

import { useCallback, useEffect, useRef, useState } from "react";
import Link from "next/link";
import type { HubConnection } from "@microsoft/signalr";
import AppShell from "@/components/AppShell";
import StatusBadge from "@/components/StatusBadge";
import DealChat from "@/components/DealChat";
import DocumentsPanel from "@/components/DocumentsPanel";
import {
  ApiError,
  dealsApi,
  documentsApi,
  downloadDocument,
  messagesApi,
  organizationsApi,
} from "@/lib/api";
import { createChatConnection } from "@/lib/signalr";
import { ALLOWED_TRANSITIONS } from "@/lib/types";
import type { DealResponse, DealStatus, DocumentResponse, MessageResponse } from "@/lib/types";

export default function DealPage({ params }: { params: { id: string } }) {
  const dealId = Number(params.id);
  const [deal, setDeal] = useState<DealResponse | null>(null);
  const [documents, setDocuments] = useState<DocumentResponse[]>([]);
  const [messages, setMessages] = useState<MessageResponse[]>([]);
  const [myOrgId, setMyOrgId] = useState<number | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [statusError, setStatusError] = useState<string | null>(null);
  const connectionRef = useRef<HubConnection | null>(null);

  useEffect(() => {
    let active = true;
    Promise.all([
      dealsApi.get(dealId),
      documentsApi.list(dealId),
      messagesApi.list(dealId),
      organizationsApi.me(),
    ])
      .then(([loadedDeal, docs, msgs, org]) => {
        if (!active) return;
        setDeal(loadedDeal);
        setDocuments(docs);
        setMessages(msgs);
        setMyOrgId(org.id);
      })
      .catch((err) => active && setError(err instanceof ApiError ? err.message : "Failed to load deal"))
      .finally(() => active && setLoading(false));
    return () => {
      active = false;
    };
  }, [dealId]);

  useEffect(() => {
    const connection = createChatConnection();
    connectionRef.current = connection;

    connection.on("ReceiveMessage", (message: MessageResponse) => {
      if (message.dealId !== dealId) return;
      setMessages((prev) => (prev.some((m) => m.id === message.id) ? prev : [...prev, message]));
    });
    connection.on("DealStatusChanged", (updated: DealResponse) => {
      if (updated.id === dealId) setDeal(updated);
    });

    connection
      .start()
      .then(() => connection.invoke("JoinDeal", dealId))
      .catch(() => undefined);

    return () => {
      void connection.stop();
      connectionRef.current = null;
    };
  }, [dealId]);

  const handleSend = useCallback(
    async (content: string) => {
      const message = await messagesApi.send(dealId, content);
      setMessages((prev) => (prev.some((m) => m.id === message.id) ? prev : [...prev, message]));
    },
    [dealId],
  );

  async function changeStatus(status: DealStatus) {
    setStatusError(null);
    try {
      setDeal(await dealsApi.updateStatus(dealId, status));
    } catch (err) {
      setStatusError(err instanceof ApiError ? err.message : "Failed to change status");
    }
  }

  async function handleUpload(file: File) {
    const document = await documentsApi.upload(dealId, file);
    setDocuments((prev) => [document, ...prev]);
  }

  async function handleApprove(id: number) {
    const document = await documentsApi.approve(id);
    setDocuments((prev) => prev.map((d) => (d.id === id ? document : d)));
  }

  async function handleReject(id: number) {
    const document = await documentsApi.reject(id);
    setDocuments((prev) => prev.map((d) => (d.id === id ? document : d)));
  }

  return (
    <AppShell>
      <Link href="/deals" className="text-sm text-brand-600 hover:underline">
        ← Back to deals
      </Link>

      {loading && <p className="mt-4 text-sm text-slate-500">Loading…</p>}
      {error && <p className="mt-4 text-sm text-rose-600">{error}</p>}

      {deal && (
        <>
          <div className="mt-4 rounded-xl border border-slate-200 bg-white p-5">
            <div className="flex items-start justify-between gap-4">
              <div>
                <h1 className="text-2xl font-semibold text-slate-900">{deal.title}</h1>
                <p className="mt-1 text-sm text-slate-500">
                  {deal.initiatorOrganizationName} ↔ {deal.counterpartyOrganizationName}
                </p>
              </div>
              <StatusBadge status={deal.status} />
            </div>

            <div className="mt-4 border-t border-slate-100 pt-4">
              <p className="text-xs font-medium uppercase tracking-wide text-slate-400">
                Change status
              </p>
              <div className="mt-2 flex flex-wrap gap-2">
                {ALLOWED_TRANSITIONS[deal.status].length === 0 && (
                  <span className="text-sm text-slate-400">This deal is in a final state.</span>
                )}
                {ALLOWED_TRANSITIONS[deal.status].map((status) => (
                  <button
                    key={status}
                    type="button"
                    onClick={() => changeStatus(status)}
                    className="rounded-md border border-slate-300 px-3 py-1.5 text-sm text-slate-700 hover:border-brand-300 hover:bg-brand-50"
                  >
                    {status}
                  </button>
                ))}
              </div>
              {statusError && <p className="mt-2 text-sm text-rose-600">{statusError}</p>}
            </div>
          </div>

          <div className="mt-6 grid gap-6 lg:grid-cols-2">
            <DocumentsPanel
              documents={documents}
              myOrgId={myOrgId}
              onUpload={handleUpload}
              onApprove={handleApprove}
              onReject={handleReject}
              onDownload={(document) => downloadDocument(document.id, document.fileName)}
            />
            <DealChat messages={messages} myOrgId={myOrgId} onSend={handleSend} />
          </div>
        </>
      )}
    </AppShell>
  );
}
