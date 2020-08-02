import React, { Component } from "react";
import { connect } from "react-redux";
import * as actions from "./actions";

class AuditLogs extends Component {
  state = {
    pageTitle: "Audit Logs",
  };

  formatDateTime = (value) => {
    if (!value) return value;
    var date = new Date(value);
    return date.toLocaleDateString() + " " + date.toLocaleTimeString();
  };

  componentDidMount() {
    this.props.fetchAuditLogs();
  }

  render() {
    const rows = this.props.auditLogs?.map((auditLog) => (
      <tr key={auditLog.id}>
        <td>{this.formatDateTime(auditLog.createdDateTime)}</td>
        <td>{auditLog.userName}</td>
        <td>{auditLog.action}</td>
        <td>{auditLog.log}</td>
      </tr>
    ));

    const table = this.props.auditLogs ? (
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
        <div className="card-header">Audit Logs</div>
        <div className="card-body">
          <div className="table-responsive">{table}</div>
        </div>
      </div>
    );
  }
}

const mapStateToProps = (state) => {
  return {
    auditLogs: state.auditLog.auditLogs,
  };
};

const mapDispatchToProps = (dispatch) => {
  return {
    fetchAuditLogs: () => dispatch(actions.fetchAuditLogs()),
  };
};

export default connect(mapStateToProps, mapDispatchToProps)(AuditLogs);
