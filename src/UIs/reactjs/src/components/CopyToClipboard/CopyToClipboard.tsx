import { useState } from "react";
import { Clipboard } from "lucide-react";

interface CopyToClipboardProps {
  text: string;
  className?: string;
  title?: string;
}

const CopyToClipboard = ({
  text,
  className = "copy-icon",
  title = "Copy Data",
}: CopyToClipboardProps) => {
  const [copyStatus, setCopyStatus] = useState("");

  const handleCopy = () => {
    navigator.clipboard
      .writeText(text)
      .then(() => {
        setCopyStatus("✅ copied");
      })
      .catch(() => {
        setCopyStatus("❌ cannot copy");
      });

    setTimeout(() => {
      setCopyStatus("");
    }, 1000);
  };

  return (
    <>
      {copyStatus ? (
        <span className="copy-status">{copyStatus}</span>
      ) : (
        <button
          type="button"
          className={className}
          title={title}
          aria-label={title}
          onClick={handleCopy}
        >
          <Clipboard size={20} aria-hidden="true" />
        </button>
      )}
    </>
  );
};

export default CopyToClipboard;
