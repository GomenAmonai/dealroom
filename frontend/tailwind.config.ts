import type { Config } from "tailwindcss";

const config: Config = {
  content: ["./src/**/*.{ts,tsx}"],
  theme: {
    extend: {
      colors: {
        ink: "#0a0c11",
        surface: {
          DEFAULT: "#13161f",
          raised: "#1a1e29",
          input: "#0e1117",
        },
        line: "rgba(255,255,255,0.08)",
        gold: {
          DEFAULT: "#e3b341",
          soft: "#f1d488",
          dim: "rgba(227,179,65,0.12)",
        },
        fg: "#e9eaee",
        muted: "#8a8f9c",
        faint: "#565b69",
      },
      fontFamily: {
        display: ["var(--font-display)", "Georgia", "serif"],
        sans: ["var(--font-sans)", "system-ui", "sans-serif"],
        mono: ["var(--font-mono)", "ui-monospace", "monospace"],
      },
      boxShadow: {
        card: "0 1px 0 0 rgba(255,255,255,0.04) inset, 0 24px 50px -28px rgba(0,0,0,0.8)",
        glow: "0 0 0 1px rgba(227,179,65,0.45), 0 10px 34px -10px rgba(227,179,65,0.3)",
      },
      borderRadius: {
        xl: "0.9rem",
        "2xl": "1.3rem",
      },
      keyframes: {
        "fade-up": {
          "0%": { opacity: "0", transform: "translateY(10px)" },
          "100%": { opacity: "1", transform: "translateY(0)" },
        },
      },
      animation: {
        "fade-up": "fade-up 0.55s cubic-bezier(0.22,1,0.36,1) both",
      },
    },
  },
  plugins: [],
};

export default config;
