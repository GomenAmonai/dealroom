import { Check } from "lucide-react";

const POINTS = [
  "One shared room per deal — no scattered email threads",
  "Documents reviewed and signed off by the counterparty",
  "Live chat and status, in sync for both sides",
];

export default function AuthBrand() {
  return (
    <aside className="relative hidden w-1/2 flex-col justify-between bg-accent p-12 text-paper lg:flex">
      <div className="flex items-center gap-2.5">
        <svg width="26" height="26" viewBox="0 0 24 24" aria-hidden role="img">
          <rect x="1.5" y="1.5" width="21" height="21" rx="6.5" fill="#f6f5f1" />
          <path d="M5.5 18.5 L18.5 5.5" stroke="#18553b" strokeWidth="1.5" strokeLinecap="round" />
          <circle cx="8.5" cy="8.5" r="2.1" fill="#18553b" />
          <circle cx="15.5" cy="15.5" r="2.1" fill="#18553b" />
        </svg>
        <span className="font-semibold tracking-tight">DealRoom</span>
      </div>

      <div>
        <h2 className="max-w-sm text-[2rem] font-semibold leading-[1.15] tracking-tight">
          One room for every deal between two companies.
        </h2>
        <ul className="mt-8 space-y-3">
          {POINTS.map((point) => (
            <li key={point} className="flex items-start gap-2.5 text-sm text-paper/85">
              <Check size={16} className="mt-0.5 shrink-0" aria-hidden />
              {point}
            </li>
          ))}
        </ul>
      </div>

      <p className="font-mono text-xs text-paper/55">Documents · Approvals · Chat · Status</p>
    </aside>
  );
}
