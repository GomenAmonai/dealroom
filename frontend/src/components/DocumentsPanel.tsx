"use client";

import { ChangeEvent, useRef, useState } from "react";
import { Check, Download, FileText, Upload, X } from "lucide-react";
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
    <section className="flex h-[30rem] flex-col rounded-xl border border-line bg-surface shadow-card">
      <div className="flex items-center justify-between border-b border-line px-5 py-3">
        <h2 className="flex items-center gap-2 text-sm font-semibold text-ink">
          <FileText size={15} className="text-muted" aria-hidden />
          Documents
        </h2>
        <label className="inline-flex cursor-pointer items-center gap-1.5 rounded-lg border border-line px-3 py-1.5 text-xs font-medium text-ink transition hover:border-accent hover:text-accent">
          <Upload size={13} aria-hidden />
          {uploading ? "Uploading…" : "Upload"}
          <input ref={inputRef} type="file" className="hidden" disabled={uploading} onChange={handleFile} />
        </label>
      </div>

      <div className="scroll-slim flex-1 overflow-y-auto px-5 py-4">
        {error && <p className="mb-2 text-sm text-rose-600">{error}</p>}
        {documents.length === 0 && (
          <div className="flex flex-col items-center gap-2 py-12 text-center">
            <FileText size={20} className="text-faint" aria-hidden />
            <p className="text-sm text-muted">No documents yet.</p>
          </div>
        )}

        <ul className="space-y-2.5">
          {documents.map((document) => {
            const canReview =
              document.status === "Pending" && document.uploadedByOrganizationId !== myOrgId;
            return (
              <li key={document.id} className="rounded-lg border border-line p-3">
                <div className="flex items-start justify-between gap-2">
                  <div className="flex min-w-0 items-start gap-2.5">
                    <FileText size={16} className="mt-0.5 shrink-0 text-faint" aria-hidden />
                    <div className="min-w-0">
                      <p className="truncate text-sm font-medium text-ink">{document.fileName}</p>
                      <p className="mt-0.5 font-mono text-xs text-faint">{formatBytes(document.sizeBytes)}</p>
                    </div>
                  </div>
                  <StatusBadge status={document.status} />
                </div>
                <div className="mt-2.5 flex flex-wrap gap-2 pl-[26px]">
                  <button
                    type="button"
                    onClick={() => onDownload(document)}
                    className="inline-flex items-center gap-1.5 rounded-lg border border-line px-2.5 py-1 text-xs text-muted transition hover:border-accent hover:text-accent"
                  >
                    <Download size={13} aria-hidden />
                    Download
                  </button>
                  {canReview && (
                    <>
                      <button
                        type="button"
                        onClick={() => onApprove(document.id)}
                        className="inline-flex items-center gap-1.5 rounded-lg bg-accent px-2.5 py-1 text-xs font-medium text-paper transition hover:bg-accent-hover"
                      >
                        <Check size={13} aria-hidden />
                        Approve
                      </button>
                      <button
                        type="button"
                        onClick={() => onReject(document.id)}
                        className="inline-flex items-center gap-1.5 rounded-lg border border-rose-200 bg-rose-50 px-2.5 py-1 text-xs font-medium text-rose-700 transition hover:bg-rose-100"
                      >
                        <X size={13} aria-hidden />
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
