import type { Config } from "tailwindcss";

const config: Config = {
  content: ["./src/**/*.{ts,tsx}"],
  theme: {
    extend: {
      colors: {
        ink: "#17160f",
        paper: "#f6f5f1",
        surface: "#ffffff",
        line: "#e6e3db",
        accent: {
          DEFAULT: "#18553b",
          hover: "#1c6444",
          soft: "#e8efe9",
        },
        muted: "#6b6a61",
        faint: "#9a988d",
      },
      fontFamily: {
        sans: ["var(--font-sans)", "system-ui", "sans-serif"],
        mono: ["var(--font-mono)", "ui-monospace", "monospace"],
      },
      boxShadow: {
        card: "0 1px 2px rgba(23, 22, 15, 0.04), 0 1px 1px rgba(23, 22, 15, 0.03)",
        pop: "0 8px 28px -10px rgba(23, 22, 15, 0.18)",
      },
      borderRadius: {
        lg: "0.6rem",
        xl: "0.85rem",
      },
      keyframes: {
        "fade-up": {
          "0%": { opacity: "0", transform: "translateY(6px)" },
          "100%": { opacity: "1", transform: "translateY(0)" },
        },
      },
      animation: {
        "fade-up": "fade-up 0.4s cubic-bezier(0.22,1,0.36,1) both",
      },
    },
  },
  plugins: [],
};

export default config;
