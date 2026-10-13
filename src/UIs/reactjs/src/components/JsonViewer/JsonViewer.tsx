"use client";
import { useEffect, useRef, useState } from "react";
import Modal from "../Modal/Modal";
import { mountJsonViewer } from "./jsonRuntime";
import "./json.css";

export function JsonContent({ jsonData }: { jsonData: unknown }) {
  const host = useRef<HTMLDivElement>(null);
  useEffect(() => {
    if (host.current) return mountJsonViewer(host.current, jsonData);
  }, [jsonData]);
  return <div ref={host} />;
}

export default function JsonViewer({ jsonData }: { jsonData: unknown }) {
  const [show, setShow] = useState(false);
  return (
    <>
      <button
        type="button"
        className="app-json-trigger"
        title="View JSON"
        aria-label="View JSON"
        onClick={() => setShow(true)}
      >
        {"{ }"}
      </button>
      <Modal show={show} onHide={() => setShow(false)} size="lg">
        <Modal.Header>
          <Modal.Title>JSON Data</Modal.Title>
        </Modal.Header>
        <Modal.Body>
          <JsonContent jsonData={jsonData} />
        </Modal.Body>
        <Modal.Footer>
          <button type="button" className="btn btn-secondary" onClick={() => setShow(false)}>
            Close
          </button>
        </Modal.Footer>
      </Modal>
    </>
  );
}
