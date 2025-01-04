import { useState, useEffect } from "react";
import Pagination from "../../components/Pagination/Pagination";
import axios from "./axios";

const AuditLogs = () => {
  const [pageTitle] = useState("Audit Logs");
  const [currentPage, setCurrentPage] = useState(1);
  const [auditLogs, setAuditLogs] = useState([]);
  const [totalItems, setTotalItems] = useState(0);
  const pageSize = 5;

  useEffect(() => {
    fetchAuditLogs(currentPage, pageSize);
  }, []);

  const fetchAuditLogs = async (currentPage: number, pageSize: number) => {
    try {
      const response = await axios.get("paged?page=" + currentPage + "&pageSize=" + pageSize);
      const data = response.data;
      setAuditLogs(data.items);
      setTotalItems(data.totalItems);
    } catch (error) {
      //
    }
  };

  const formatDateTime = (value: string) => {
    if (!value) return value;
    const date = new Date(value);
    return date.toLocaleDateString() + " " + date.toLocaleTimeString();
  };

  const pageSelected = async (page: number) => {
    setCurrentPage(page);
    await fetchAuditLogs(page, pageSize);
  };

  const rows = auditLogs?.map((auditLog) => (
    <tr key={auditLog.id}>
      <td>{formatDateTime(auditLog.createdDateTime)}</td>
      <td>{auditLog.userName}</td>
      <td>{auditLog.action}</td>
      <td>{auditLog.log}</td>
    </tr>
  ));

  const table = auditLogs ? (
    <table className="table">
      <thead>
        <tr>
          <th>Date Time</th>
          <th>User Name</th>
          <th>Action</th>
          <th>Data</th>
        </tr>
      </thead>
      <tbody>{rows}</tbody>
    </table>
  ) : null;

  const pagination = (
    <div style={{ float: "right" }}>
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
      <div className="card-header">{pageTitle}</div>
      <div className="card-body">
        {pagination}
        <div className="table-responsive" style={{ width: "100%" }}>
          {table}
        </div>
        {pagination}
      </div>
    </div>
  );
};

export default AuditLogs;
