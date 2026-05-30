"use client";

import { FormEvent, useState } from "react";
import { useRouter } from "next/navigation";
import Link from "next/link";
import { ApiError, authApi } from "@/lib/api";
import { setTokens } from "@/lib/auth";
import AuthBrand from "@/components/AuthBrand";

const inputClass =
  "w-full rounded-lg border border-line bg-surface px-3 py-2.5 text-sm text-ink placeholder-faint outline-none transition focus:border-accent focus:ring-2 focus:ring-accent/15";
const labelClass = "mb-1.5 block text-xs font-medium text-muted";

export default function RegisterPage() {
  const router = useRouter();
  const [name, setName] = useState("");
  const [organizationName, setOrganizationName] = useState("");
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [error, setError] = useState<string | null>(null);
  const [submitting, setSubmitting] = useState(false);

  async function handleSubmit(event: FormEvent) {
    event.preventDefault();
    setError(null);
    setSubmitting(true);
    try {
      setTokens(await authApi.register({ name, email, password, organizationName }));
      router.replace("/deals");
    } catch (err) {
      setError(err instanceof ApiError ? err.message : "Registration failed");
    } finally {
      setSubmitting(false);
    }
  }

  return (
    <div className="flex min-h-screen">
      <AuthBrand />
      <div className="flex flex-1 items-center justify-center px-6 py-12">
        <div className="w-full max-w-sm animate-fade-up">
          <h1 className="text-2xl font-semibold tracking-tight text-ink">Create workspace</h1>
          <p className="mt-1 text-sm text-muted">Register your organization and first user.</p>

          <form onSubmit={handleSubmit} className="mt-7 space-y-4">
            <div>
              <label htmlFor="organizationName" className={labelClass}>
                Organization name
              </label>
              <input
                id="organizationName"
                required
                value={organizationName}
                onChange={(e) => setOrganizationName(e.target.value)}
                className={inputClass}
              />
            </div>
            <div>
              <label htmlFor="name" className={labelClass}>
                Your name
              </label>
              <input
                id="name"
                required
                value={name}
                onChange={(e) => setName(e.target.value)}
                className={inputClass}
              />
            </div>
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
                minLength={8}
                value={password}
                onChange={(e) => setPassword(e.target.value)}
                className={inputClass}
              />
            </div>

            {error && <p className="text-sm text-rose-600">{error}</p>}

            <button
              type="submit"
              disabled={submitting}
              className="w-full rounded-lg bg-accent px-4 py-2.5 text-sm font-semibold text-paper transition hover:bg-accent-hover disabled:opacity-60"
            >
              {submitting ? "Creating…" : "Create account"}
            </button>
          </form>

          <p className="mt-6 text-sm text-muted">
            Already registered?{" "}
            <Link href="/login" className="font-medium text-accent hover:text-accent-hover">
              Sign in
            </Link>
          </p>
        </div>
      </div>
    </div>
  );
}
