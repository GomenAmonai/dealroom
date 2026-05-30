"use client";

import { FormEvent, useEffect, useRef, useState } from "react";
import type { MessageResponse } from "@/lib/types";

interface Props {
  messages: MessageResponse[];
  myOrgId: number | null;
  onSend: (content: string) => Promise<void>;
}

export default function DealChat({ messages, myOrgId, onSend }: Props) {
  const [content, setContent] = useState("");
  const [sending, setSending] = useState(false);
  const endRef = useRef<HTMLDivElement>(null);

  useEffect(() => {
    endRef.current?.scrollIntoView({ behavior: "smooth" });
  }, [messages]);

  async function handleSubmit(event: FormEvent) {
    event.preventDefault();
    const trimmed = content.trim();
    if (!trimmed) return;
    setSending(true);
    try {
      await onSend(trimmed);
      setContent("");
    } finally {
      setSending(false);
    }
  }

  return (
    <section className="flex h-[30rem] flex-col rounded-2xl border border-line bg-surface/70 shadow-card">
      <h2 className="flex items-center gap-2 border-b border-line px-5 py-3.5 font-mono text-[11px] uppercase tracking-[0.2em] text-muted">
        <span className="h-1.5 w-1.5 rounded-full bg-emerald-400" aria-hidden />
        Chat
      </h2>

      <div className="scroll-slim flex-1 space-y-3 overflow-y-auto px-5 py-4">
        {messages.length === 0 && (
          <p className="text-sm text-faint">No messages yet. Open the conversation.</p>
        )}
        {messages.map((message) => {
          const mine = message.senderOrganizationId === myOrgId;
          return (
            <div key={message.id} className={`flex ${mine ? "justify-end" : "justify-start"}`}>
              <div
                className={`max-w-[80%] rounded-xl px-3.5 py-2 text-sm ring-1 ${
                  mine ? "bg-gold/15 text-fg ring-gold/25" : "bg-surface-raised text-fg ring-line"
                }`}
              >
                <p className={`text-xs font-medium ${mine ? "text-gold/90" : "text-muted"}`}>
                  {message.senderName}
                </p>
                <p className="mt-0.5 whitespace-pre-wrap break-words">{message.content}</p>
                <p className="mt-1 font-mono text-[10px] text-faint">
                  {new Date(message.sentAt).toLocaleTimeString([], {
                    hour: "2-digit",
                    minute: "2-digit",
                  })}
                </p>
              </div>
            </div>
          );
        })}
        <div ref={endRef} />
      </div>

      <form onSubmit={handleSubmit} className="flex gap-2 border-t border-line p-3">
        <input
          aria-label="Message"
          value={content}
          onChange={(e) => setContent(e.target.value)}
          placeholder="Type a message…"
          className="flex-1 rounded-lg border border-line bg-surface-input px-3.5 py-2.5 text-sm text-fg placeholder-faint outline-none transition focus:border-gold/50 focus:ring-2 focus:ring-gold/15"
        />
        <button
          type="submit"
          disabled={sending}
          className="rounded-lg bg-gold px-4 py-2.5 text-sm font-semibold text-ink transition hover:bg-gold-soft disabled:opacity-60"
        >
          Send
        </button>
      </form>
    </section>
  );
}
