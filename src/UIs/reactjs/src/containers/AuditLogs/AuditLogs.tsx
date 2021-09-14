import { useState, useEffect } from "react";
import { useDispatch, useSelector } from "react-redux";
import * as actions from "./actions";

const AuditLogs = () => {
  const [pageTitle] = useState("Audit Logs");
  const dispatch = useDispatch();

  const auditLogs = useSelector((state) => state.auditLog.auditLogs);

  useEffect(() => {
    dispatch(actions.fetchAuditLogs());
  }, [dispatch]);

  const formatDateTime = (value) => {
    if (!value) return value;
    var date = new Date(value);
    return date.toLocaleDateString() + " " + date.toLocaleTimeString();
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

  return (
    <div className="card">
      <div className="card-header">{pageTitle}</div>
      <div className="card-body">
        <div className="table-responsive">{table}</div>
      </div>
    </div>
  );
};

export default AuditLogs;
