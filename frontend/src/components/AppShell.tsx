"use client";

import { useEffect, useState } from "react";
import { usePathname, useRouter } from "next/navigation";
import Link from "next/link";
import { Briefcase, LogOut } from "lucide-react";
import Logo from "@/components/Logo";
import { clearTokens, isAuthenticated } from "@/lib/auth";
import { organizationsApi } from "@/lib/api";

export default function AppShell({ children }: { children: React.ReactNode }) {
  const router = useRouter();
  const pathname = usePathname();
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

  const navActive = pathname.startsWith("/deals");

  return (
    <div className="flex min-h-screen">
      <aside className="sticky top-0 hidden h-screen w-60 shrink-0 flex-col border-r border-line bg-surface md:flex">
        <div className="px-5 py-5">
          <Link href="/deals">
            <Logo />
          </Link>
        </div>
        <nav className="flex-1 px-3">
          <Link
            href="/deals"
            className={`flex items-center gap-2.5 rounded-lg px-3 py-2 text-sm font-medium transition ${
              navActive ? "bg-accent-soft text-accent" : "text-muted hover:bg-paper hover:text-ink"
            }`}
          >
            <Briefcase size={16} aria-hidden />
            Deals
          </Link>
        </nav>
        <div className="border-t border-line p-3">
          {orgName && (
            <div className="mb-2 px-2">
              <p className="text-[11px] uppercase tracking-wide text-faint">Organization</p>
              <p className="truncate text-sm font-medium text-ink">{orgName}</p>
            </div>
          )}
          <button
            type="button"
            onClick={logout}
            className="flex w-full items-center gap-2.5 rounded-lg px-3 py-2 text-sm text-muted transition hover:bg-paper hover:text-ink"
          >
            <LogOut size={16} aria-hidden />
            Log out
          </button>
        </div>
      </aside>

      <div className="flex min-w-0 flex-1 flex-col">
        <header className="flex items-center justify-between border-b border-line bg-surface px-4 py-3 md:hidden">
          <Link href="/deals">
            <Logo />
          </Link>
          <button type="button" onClick={logout} aria-label="Log out" className="text-muted">
            <LogOut size={18} aria-hidden />
          </button>
        </header>
        <main className="mx-auto w-full max-w-5xl flex-1 px-6 py-9">{children}</main>
      </div>
    </div>
  );
}
