"use client";

import { useEffect, useState } from "react";
import { useRouter } from "next/navigation";
import Link from "next/link";
import { clearTokens, isAuthenticated } from "@/lib/auth";
import { organizationsApi } from "@/lib/api";

export default function AppShell({ children }: { children: React.ReactNode }) {
  const router = useRouter();
  const [orgName, setOrgName] = useState<string | null>(null);
  const [ready, setReady] = useState(false);

  useEffect(() => {
    if (!isAuthenticated()) {
      router.replace("/login");
      return;
    }
    setReady(true);
    organizationsApi
      .me()
      .then((org) => setOrgName(org.name))
      .catch(() => undefined);
  }, [router]);

  function logout() {
    clearTokens();
    router.replace("/login");
  }

  if (!ready) return null;

  return (
    <div className="min-h-screen">
      <header className="sticky top-0 z-20 border-b border-line bg-ink/70 backdrop-blur-md">
        <div className="mx-auto flex max-w-5xl items-center justify-between px-5 py-4">
          <Link href="/deals" className="flex items-center gap-2.5">
            <span className="h-2.5 w-2.5 rotate-45 bg-gold shadow-glow" aria-hidden />
            <span className="font-display text-xl font-semibold tracking-tight text-fg">DealRoom</span>
          </Link>
          <div className="flex items-center gap-3 text-sm">
            {orgName && (
              <span className="hidden rounded-full border border-line bg-surface px-3 py-1 font-mono text-xs text-muted sm:inline">
                {orgName}
              </span>
            )}
            <button
              type="button"
              onClick={logout}
              className="rounded-lg border border-line px-3 py-1.5 text-muted transition hover:border-gold/40 hover:text-fg"
            >
              Log out
            </button>
          </div>
        </div>
      </header>
      <main className="mx-auto max-w-5xl px-5 py-10">{children}</main>
    </div>
  );
}
