import { useState } from "react";

interface CopyToClipboardProps {
  text: string;
  className?: string;
  title?: string;
}

const CopyToClipboard = ({
  text,
  className = "copy-icon fa fa-clipboard",
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
        <span className="copy-icon">{copyStatus}</span>
      ) : (
        <i className={className} title={title} onClick={handleCopy}></i>
      )}
    </>
  );
};

export default CopyToClipboard;
