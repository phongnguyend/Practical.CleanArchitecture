import { auditSnapshot } from "../../shared/models/record";
import { RecordPage } from "../../shared/ui/RecordPage";
import * as service from "./api";
import { emptyUser, parseUser, validateUser } from "./model";
import { UserActions } from "./UserActions";
import { UserDetails, UserHistory, UserSummary } from "./UserDetails";
import { UserForm } from "./UserForm";
export default function UserScreen() {
  return (
    <RecordPage
      title="Users"
      repository={service.usersRepository}
      empty={emptyUser}
      label={(value) => value.userName}
      searchText={(value) => JSON.stringify(value)}
      validate={validateUser}
      Form={UserForm}
      Details={UserDetails}
      Summary={UserSummary}
      History={(entry) => (
        <UserHistory
          value={parseUser(auditSnapshot(entry))}
          changed={entry.highLight}
        />
      )}
      actions={({ value, actions }) => (
        <UserActions value={value} notify={actions.notify} />
      )}
    />
  );
}
