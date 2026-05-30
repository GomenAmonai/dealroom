"use client";

import { ChangeEvent, useRef, useState } from "react";
import StatusBadge from "@/components/StatusBadge";
import type { DocumentResponse } from "@/lib/types";

interface Props {
  documents: DocumentResponse[];
  myOrgId: number | null;
  onUpload: (file: File) => Promise<void>;
  onApprove: (id: number) => Promise<void>;
  onReject: (id: number) => Promise<void>;
  onDownload: (document: DocumentResponse) => Promise<void>;
}

function formatBytes(bytes: number): string {
  if (bytes < 1024) return `${bytes} B`;
  if (bytes < 1024 * 1024) return `${(bytes / 1024).toFixed(1)} KB`;
  return `${(bytes / (1024 * 1024)).toFixed(1)} MB`;
}

export default function DocumentsPanel({
  documents,
  myOrgId,
  onUpload,
  onApprove,
  onReject,
  onDownload,
}: Props) {
  const inputRef = useRef<HTMLInputElement>(null);
  const [uploading, setUploading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  async function handleFile(event: ChangeEvent<HTMLInputElement>) {
    const file = event.target.files?.[0];
    if (!file) return;
    setError(null);
    setUploading(true);
    try {
      await onUpload(file);
    } catch {
      setError("Upload failed");
    } finally {
      setUploading(false);
      if (inputRef.current) inputRef.current.value = "";
    }
  }

  return (
    <section className="flex h-[30rem] flex-col rounded-2xl border border-line bg-surface/70 shadow-card">
      <div className="flex items-center justify-between border-b border-line px-5 py-3">
        <h2 className="flex items-center gap-2 font-mono text-[11px] uppercase tracking-[0.2em] text-muted">
          <span className="h-1.5 w-1.5 rounded-full bg-gold" aria-hidden />
          Documents
        </h2>
        <label className="cursor-pointer rounded-lg border border-gold/40 bg-gold-dim px-3 py-1.5 text-xs font-medium text-gold transition hover:bg-gold/20">
          {uploading ? "Uploading…" : "+ Upload"}
          <input ref={inputRef} type="file" className="hidden" disabled={uploading} onChange={handleFile} />
        </label>
      </div>

      <div className="scroll-slim flex-1 overflow-y-auto px-5 py-4">
        {error && <p className="mb-2 text-sm text-rose-300">{error}</p>}
        {documents.length === 0 && <p className="text-sm text-faint">No documents uploaded yet.</p>}

        <ul className="space-y-3">
          {documents.map((document) => {
            const canReview =
              document.status === "Pending" && document.uploadedByOrganizationId !== myOrgId;
            return (
              <li key={document.id} className="rounded-xl border border-line bg-surface-raised/60 p-3.5">
                <div className="flex items-start justify-between gap-2">
                  <div className="min-w-0">
                    <p className="truncate text-sm font-medium text-fg">{document.fileName}</p>
                    <p className="mt-0.5 font-mono text-xs text-faint">{formatBytes(document.sizeBytes)}</p>
                  </div>
                  <StatusBadge status={document.status} />
                </div>
                <div className="mt-3 flex flex-wrap gap-2">
                  <button
                    type="button"
                    onClick={() => onDownload(document)}
                    className="rounded-lg border border-line px-2.5 py-1 text-xs text-muted transition hover:border-gold/40 hover:text-fg"
                  >
                    Download
                  </button>
                  {canReview && (
                    <>
                      <button
                        type="button"
                        onClick={() => onApprove(document.id)}
                        className="rounded-lg bg-emerald-500/90 px-2.5 py-1 text-xs font-medium text-ink transition hover:bg-emerald-400"
                      >
                        Approve
                      </button>
                      <button
                        type="button"
                        onClick={() => onReject(document.id)}
                        className="rounded-lg bg-rose-500/90 px-2.5 py-1 text-xs font-medium text-white transition hover:bg-rose-500"
                      >
                        Reject
                      </button>
                    </>
                  )}
                </div>
              </li>
            );
          })}
        </ul>
      </div>
    </section>
  );
}
