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
    <section className="flex h-[28rem] flex-col rounded-xl border border-slate-200 bg-white">
      <h2 className="border-b border-slate-200 px-4 py-3 text-sm font-semibold text-slate-700">Chat</h2>

      <div className="flex-1 space-y-3 overflow-y-auto px-4 py-3">
        {messages.length === 0 && (
          <p className="text-sm text-slate-400">No messages yet. Say hello.</p>
        )}
        {messages.map((message) => {
          const mine = message.senderOrganizationId === myOrgId;
          return (
            <div key={message.id} className={`flex ${mine ? "justify-end" : "justify-start"}`}>
              <div
                className={`max-w-[80%] rounded-lg px-3 py-2 text-sm ${
                  mine ? "bg-brand-600 text-white" : "bg-slate-100 text-slate-800"
                }`}
              >
                <p className={`text-xs font-medium ${mine ? "text-brand-100" : "text-slate-500"}`}>
                  {message.senderName}
                </p>
                <p className="mt-0.5 whitespace-pre-wrap break-words">{message.content}</p>
                <p className={`mt-1 text-[10px] ${mine ? "text-brand-100" : "text-slate-400"}`}>
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

      <form onSubmit={handleSubmit} className="flex gap-2 border-t border-slate-200 p-3">
        <input
          aria-label="Message"
          value={content}
          onChange={(e) => setContent(e.target.value)}
          placeholder="Type a message…"
          className="flex-1 rounded-md border border-slate-300 px-3 py-2 text-sm focus:border-brand-500 focus:outline-none focus:ring-1 focus:ring-brand-500"
        />
        <button
          type="submit"
          disabled={sending}
          className="rounded-md bg-brand-600 px-4 py-2 text-sm font-medium text-white hover:bg-brand-700 disabled:opacity-60"
        >
          Send
        </button>
      </form>
    </section>
  );
}
