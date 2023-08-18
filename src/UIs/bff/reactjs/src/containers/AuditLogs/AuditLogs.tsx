import { useState, useEffect } from "react";
import { useDispatch, useSelector } from "react-redux";
import Pagination from "../../components/Pagination/Pagination";
import * as actions from "./actions";

const AuditLogs = () => {
  const [pageTitle] = useState("Audit Logs");
  const [currentPage, setCurrentPage] = useState(1);
  const pageSize = 5;
  const dispatch = useDispatch();

  const { auditLogs, totalItems } = useSelector((state: any) => state.auditLog);

  useEffect(() => {
    dispatch(actions.fetchAuditLogs(currentPage, pageSize));
  }, [dispatch]);

  const formatDateTime = (value) => {
    if (!value) return value;
    var date = new Date(value);
    return date.toLocaleDateString() + " " + date.toLocaleTimeString();
  };

  const pageSelected = (page) => {
    setCurrentPage(page);
    dispatch(actions.fetchAuditLogs(page, pageSize));
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
