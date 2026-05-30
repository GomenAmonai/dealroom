"use client";

import { FormEvent, useState } from "react";
import { useRouter } from "next/navigation";
import Link from "next/link";
import { ApiError, authApi } from "@/lib/api";
import { setTokens } from "@/lib/auth";

const inputClass =
  "w-full rounded-lg border border-line bg-surface-input px-3.5 py-2.5 text-sm text-fg placeholder-faint outline-none transition focus:border-gold/50 focus:ring-2 focus:ring-gold/15";
const labelClass = "mb-1.5 block text-xs font-medium uppercase tracking-wide text-muted";

export default function LoginPage() {
  const router = useRouter();
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [error, setError] = useState<string | null>(null);
  const [submitting, setSubmitting] = useState(false);

  async function handleSubmit(event: FormEvent) {
    event.preventDefault();
    setError(null);
    setSubmitting(true);
    try {
      setTokens(await authApi.login({ email, password }));
      router.replace("/deals");
    } catch (err) {
      setError(err instanceof ApiError ? err.message : "Login failed");
    } finally {
      setSubmitting(false);
    }
  }

  return (
    <div className="flex min-h-screen items-center justify-center px-4">
      <div className="w-full max-w-sm animate-fade-up rounded-2xl border border-line bg-surface/80 p-8 shadow-card backdrop-blur">
        <span className="h-2.5 w-2.5 rotate-45 bg-gold shadow-glow" aria-hidden />
        <p className="mt-5 font-mono text-[11px] uppercase tracking-[0.2em] text-gold">Private deal room</p>
        <h1 className="mt-2 font-display text-3xl font-semibold tracking-tight text-fg">Welcome back</h1>
        <p className="mt-1.5 text-sm text-muted">Sign in to continue your deals.</p>

        <form onSubmit={handleSubmit} className="mt-7 space-y-4">
          <div>
            <label htmlFor="email" className={labelClass}>
              Email
            </label>
            <input
              id="email"
              type="email"
              required
              value={email}
              onChange={(e) => setEmail(e.target.value)}
              className={inputClass}
            />
          </div>
          <div>
            <label htmlFor="password" className={labelClass}>
              Password
            </label>
            <input
              id="password"
              type="password"
              required
              value={password}
              onChange={(e) => setPassword(e.target.value)}
              className={inputClass}
            />
          </div>

          {error && <p className="text-sm text-rose-300">{error}</p>}

          <button
            type="submit"
            disabled={submitting}
            className="w-full rounded-lg bg-gold px-4 py-2.5 text-sm font-semibold text-ink transition hover:bg-gold-soft disabled:opacity-60"
          >
            {submitting ? "Signing in…" : "Sign in"}
          </button>
        </form>

        <p className="mt-6 text-center text-sm text-muted">
          No account?{" "}
          <Link href="/register" className="font-medium text-gold hover:text-gold-soft">
            Create one
          </Link>
        </p>
      </div>
    </div>
  );
}
