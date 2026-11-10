import { auditSnapshot } from "../../shared/models/record";
import { RecordPage } from "../../shared/ui/RecordPage";
import * as service from "./api";
import { emptyRole, parseRole, validateRole } from "./model";
import { RoleDetails, RoleHistory, RoleSummary } from "./RoleDetails";
import { RoleForm } from "./RoleForm";
export default function RoleScreen() {
  return (
    <RecordPage
      title="Roles"
      repository={service.rolesRepository}
      empty={emptyRole}
      label={(value) => value.name}
      searchText={(value) => JSON.stringify(value)}
      validate={validateRole}
      Form={RoleForm}
      Details={RoleDetails}
      Summary={RoleSummary}
      History={(entry) => (
        <RoleHistory
          value={parseRole(auditSnapshot(entry))}
          changed={entry.highLight}
        />
      )}
    />
  );
}
