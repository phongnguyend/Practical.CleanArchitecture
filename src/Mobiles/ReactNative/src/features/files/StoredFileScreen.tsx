import { auditSnapshot } from "../../shared/models/record";
import { RecordPage } from "../../shared/ui/RecordPage";
import { Button } from "../../shared/ui/ui";
import * as service from "./api";
import { emptyStoredFile, parseStoredFile, validateStoredFile } from "./model";
import {
  StoredFileDetails,
  StoredFileHistory,
  StoredFileSummary,
} from "./StoredFileDetails";
import { StoredFileForm } from "./StoredFileForm";
export default function StoredFileScreen() {
  return (
    <RecordPage
      title="Files"
      repository={service.filesRepository}
      empty={emptyStoredFile}
      label={(value) => value.name}
      searchText={(value) => JSON.stringify(value)}
      validate={validateStoredFile}
      Form={StoredFileForm}
      Details={StoredFileDetails}
      Summary={StoredFileSummary}
      History={(entry) => (
        <StoredFileHistory
          value={parseStoredFile(auditSnapshot(entry))}
          changed={entry.highLight}
        />
      )}
      createTitle="Upload file"
      actions={({ value, actions }) => (
        <Button
          title="Download"
          onPress={() => void actions.run(() => service.downloadFile(value))}
        />
      )}
    />
  );
}
