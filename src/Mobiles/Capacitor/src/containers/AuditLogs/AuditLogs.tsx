import AuditEntries from "../../components/AuditEntries/AuditEntries";
import PageHeader from "../../components/PageHeader/PageHeader";
import { useState, useEffect } from "react";
import Pagination from "../../components/Pagination/Pagination";
import axios from "./axios";

import "./AuditLogs.css";

const AuditLogs = () => {
  const [error, setError] = useState("");
  const [pageTitle] = useState("Audit Logs");
  const [currentPage, setCurrentPage] = useState(1);
  const [auditLogs, setAuditLogs] = useState<any[]>([]);
  const [totalItems, setTotalItems] = useState(0);
  const pageSize = 5;

  useEffect(() => {
    fetchAuditLogs(currentPage, pageSize);
  }, []);

  const fetchAuditLogs = async (currentPage: number, pageSize: number) => {
    try {
      setError("");
      const response = await axios.get(
        "paged?page=" + currentPage + "&pageSize=" + pageSize,
      );
      const data = response.data;
      setAuditLogs(data.items);
      setTotalItems(data.totalItems);
    } catch (error) {
      setError("Request failed. Please refresh the audit logs.");
    }
  };

  const pageSelected = async (page: number) => {
    setCurrentPage(page);
    await fetchAuditLogs(page, pageSize);
  };

  const pagination = (
    <div>
      <Pagination
        pageSelected={pageSelected}
        totalItems={totalItems}
        pageSize={pageSize}
        currentPage={currentPage}
      />
    </div>
  );

  return (
    <div className="card">
      <PageHeader
        title={pageTitle}
        icon="audit"
        onRefresh={() => fetchAuditLogs(currentPage, pageSize)}
      ></PageHeader>
      <div className="card-body">
        <AuditEntries entries={auditLogs ?? []} />
        {pagination}
        {error && <p role="alert">{error}</p>}
      </div>
    </div>
  );
};

export default AuditLogs;
