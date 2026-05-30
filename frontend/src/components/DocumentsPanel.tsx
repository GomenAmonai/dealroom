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
    <section className="flex h-[28rem] flex-col rounded-xl border border-slate-200 bg-white">
      <div className="flex items-center justify-between border-b border-slate-200 px-4 py-3">
        <h2 className="text-sm font-semibold text-slate-700">Documents</h2>
        <label className="cursor-pointer rounded-md border border-slate-300 px-3 py-1.5 text-xs font-medium text-slate-700 hover:bg-slate-50">
          {uploading ? "Uploading…" : "Upload"}
          <input
            ref={inputRef}
            type="file"
            className="hidden"
            disabled={uploading}
            onChange={handleFile}
          />
        </label>
      </div>

      <div className="flex-1 overflow-y-auto px-4 py-3">
        {error && <p className="mb-2 text-sm text-rose-600">{error}</p>}
        {documents.length === 0 && <p className="text-sm text-slate-400">No documents uploaded yet.</p>}

        <ul className="space-y-3">
          {documents.map((document) => {
            const canReview = document.status === "Pending" && document.uploadedByOrganizationId !== myOrgId;
            return (
              <li key={document.id} className="rounded-lg border border-slate-200 p-3">
                <div className="flex items-start justify-between gap-2">
                  <div className="min-w-0">
                    <p className="truncate text-sm font-medium text-slate-800">{document.fileName}</p>
                    <p className="text-xs text-slate-400">{formatBytes(document.sizeBytes)}</p>
                  </div>
                  <StatusBadge status={document.status} />
                </div>
                <div className="mt-2 flex flex-wrap gap-2">
                  <button
                    type="button"
                    onClick={() => onDownload(document)}
                    className="rounded-md border border-slate-300 px-2.5 py-1 text-xs text-slate-700 hover:bg-slate-50"
                  >
                    Download
                  </button>
                  {canReview && (
                    <>
                      <button
                        type="button"
                        onClick={() => onApprove(document.id)}
                        className="rounded-md bg-emerald-600 px-2.5 py-1 text-xs font-medium text-white hover:bg-emerald-700"
                      >
                        Approve
                      </button>
                      <button
                        type="button"
                        onClick={() => onReject(document.id)}
                        className="rounded-md bg-rose-600 px-2.5 py-1 text-xs font-medium text-white hover:bg-rose-700"
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
