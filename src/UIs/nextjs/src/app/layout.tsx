import type { Metadata } from "next";
import { Geist, Geist_Mono } from "next/font/google";
import "./globals.css";
import "bootstrap/dist/css/bootstrap.min.css";
import "../workspace.css";
import ToastContainer from "@/components/Toast/ToastContainer";
import NavServer from "@/components/Nav/NavServer";

const geistSans = Geist({
  variable: "--font-geist-sans",
  subsets: ["latin"],
});

const geistMono = Geist_Mono({
  variable: "--font-geist-mono",
  subsets: ["latin"],
});

export const metadata: Metadata = {
  title: "ClassifiedAds.NextJs",
  description: "Manage products, files, users, and application settings.",
};

export default function RootLayout({
  children,
}: Readonly<{
  children: React.ReactNode;
}>) {
  return (
    <html lang="en" suppressHydrationWarning>
      <head>
        <script
          dangerouslySetInnerHTML={{
            __html: `
          (() => {
            let choice = 'system';
            try { choice = localStorage.getItem('classifiedads-theme') || 'system'; } catch {}
            const dark = choice === 'dark' || (choice !== 'light' && matchMedia('(prefers-color-scheme: dark)').matches);
            document.documentElement.setAttribute('data-bs-theme', dark ? 'dark' : 'light');
          })();
        `,
          }}
        />
      </head>
      <body className={`${geistSans.variable} ${geistMono.variable}`}>
        <ToastContainer />
        <div className="container ui-shell">
          <a className="skip-link" href="#main-content">Skip to content</a>
          <NavServer />
          <main id="main-content" tabIndex={-1}>{children}</main>
        </div>
      </body>
    </html>
  );
}
