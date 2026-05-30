export default function Logo({ withWordmark = true }: { withWordmark?: boolean }) {
  return (
    <span className="inline-flex items-center gap-2.5">
      <svg width="24" height="24" viewBox="0 0 24 24" aria-hidden role="img">
        <rect x="1.5" y="1.5" width="21" height="21" rx="6.5" fill="#18553b" />
        <path d="M5.5 18.5 L18.5 5.5" stroke="#f6f5f1" strokeWidth="1.5" strokeLinecap="round" />
        <circle cx="8.5" cy="8.5" r="2.1" fill="#f6f5f1" />
        <circle cx="15.5" cy="15.5" r="2.1" fill="#f6f5f1" />
      </svg>
      {withWordmark && (
        <span className="text-[15px] font-semibold tracking-tight text-ink">DealRoom</span>
      )}
    </span>
  );
}
