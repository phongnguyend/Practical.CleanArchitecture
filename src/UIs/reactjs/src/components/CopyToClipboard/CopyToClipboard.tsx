import { useEffect, useRef, useState } from "react";
import { Clipboard } from "lucide-react";

interface CopyToClipboardProps {
  text: string;
  className?: string;
  title?: string;
}

export default function CopyToClipboard({ text, className = "", title = "Copy Data" }: CopyToClipboardProps) {
  const [copyStatus, setCopyStatus] = useState("");
  const timer = useRef<ReturnType<typeof setTimeout> | undefined>(undefined);
  useEffect(() => () => clearTimeout(timer.current), []);

  const handleCopy = async () => {
    clearTimeout(timer.current);
    try {
      await navigator.clipboard.writeText(text);
      setCopyStatus("Copied to clipboard");
    } catch {
      setCopyStatus("Unable to copy");
    }
    timer.current = setTimeout(() => setCopyStatus(""), 2000);
  };

  return (
    <span className="app-copy-control">
      <button type="button" className={`app-copy-trigger ${className}`} title={title} aria-label={title} onClick={handleCopy}>
        <Clipboard size={18} aria-hidden="true" />
      </button>
      <span className="app-copy-feedback" role="status">{copyStatus}</span>
    </span>
  );
}
