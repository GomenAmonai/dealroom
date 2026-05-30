"use client";

import { FormEvent, useEffect, useRef, useState } from "react";
import { MessageSquare, Send } from "lucide-react";
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
    <section className="flex h-[30rem] flex-col rounded-xl border border-line bg-surface shadow-card">
      <h2 className="flex items-center gap-2 border-b border-line px-5 py-3 text-sm font-semibold text-ink">
        <MessageSquare size={15} className="text-muted" aria-hidden />
        Chat
      </h2>

      <div className="scroll-slim flex-1 space-y-3 overflow-y-auto px-5 py-4">
        {messages.length === 0 && <p className="text-sm text-faint">No messages yet.</p>}
        {messages.map((message) => {
          const mine = message.senderOrganizationId === myOrgId;
          return (
            <div key={message.id} className={`flex ${mine ? "justify-end" : "justify-start"}`}>
              <div
                className={`max-w-[82%] rounded-xl px-3.5 py-2 text-sm ${
                  mine ? "bg-accent text-paper" : "border border-line bg-paper text-ink"
                }`}
              >
                <p className={`text-xs font-medium ${mine ? "text-paper/75" : "text-muted"}`}>
                  {message.senderName}
                </p>
                <p className="mt-0.5 whitespace-pre-wrap break-words">{message.content}</p>
                <p className={`mt-1 font-mono text-[10px] ${mine ? "text-paper/65" : "text-faint"}`}>
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
          className="flex-1 rounded-lg border border-line bg-surface px-3 py-2.5 text-sm text-ink placeholder-faint outline-none transition focus:border-accent focus:ring-2 focus:ring-accent/15"
        />
        <button
          type="submit"
          disabled={sending}
          aria-label="Send message"
          className="inline-flex items-center justify-center rounded-lg bg-accent px-3.5 text-paper transition hover:bg-accent-hover disabled:opacity-60"
        >
          <Send size={16} aria-hidden />
        </button>
      </form>
    </section>
  );
}
