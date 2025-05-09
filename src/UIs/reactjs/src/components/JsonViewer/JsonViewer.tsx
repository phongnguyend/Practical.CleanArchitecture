import { useState } from "react";
import { Modal, Button } from "react-bootstrap";
import { Prism as SyntaxHighlighter } from "react-syntax-highlighter";
import { tomorrow } from "react-syntax-highlighter/dist/esm/styles/prism";

interface JsonViewerProps {
  jsonData: string;
}

const JsonViewer = ({ jsonData }: JsonViewerProps) => {
  const [show, setShow] = useState(false);

  const handleShow = () => setShow(true);
  const handleClose = () => setShow(false);

  // Try to parse and format the JSON data
  const getParsedJson = () => {
    try {
      const parsedJson = JSON.parse(jsonData);
      return JSON.stringify(parsedJson, null, 2);
    } catch (error) {
      return jsonData; // Return as is if not valid JSON
    }
  };

  return (
    <>
      <i className="view-json-icon fa fa-eye" onClick={handleShow} title="View JSON"></i>
      <Modal show={show} onHide={handleClose} size="lg" centered>
        <Modal.Header closeButton>
          <Modal.Title>JSON Data</Modal.Title>
        </Modal.Header>
        <Modal.Body>
          <SyntaxHighlighter
            language="json"
            style={tomorrow}
            customStyle={{
              maxHeight: "70vh",
              overflowY: "auto",
              borderRadius: "5px",
            }}
          >
            {getParsedJson()}
          </SyntaxHighlighter>
        </Modal.Body>
        <Modal.Footer>
          <Button variant="secondary" onClick={handleClose}>
            Close
          </Button>
        </Modal.Footer>
      </Modal>
    </>
  );
};

export default JsonViewer;
