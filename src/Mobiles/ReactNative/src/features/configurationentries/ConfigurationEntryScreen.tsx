import { auditSnapshot } from "../../shared/models/record";
import { ImportButton } from "../../shared/ui/ImportButton";
import { RecordPage } from "../../shared/ui/RecordPage";
import { Button } from "../../shared/ui/ui";
import {
  ConfigurationEntryDetails,
  ConfigurationEntryHistory,
  ConfigurationEntrySummary,
} from "./ConfigurationEntryDetails";
import { ConfigurationEntryForm } from "./ConfigurationEntryForm";
import * as service from "./api";
import {
  emptyConfigurationEntry,
  parseConfigurationEntry,
  validateConfigurationEntry,
} from "./model";
export default function ConfigurationEntryScreen() {
  return (
    <RecordPage
      title="Configuration"
      repository={service.configurationentriesRepository}
      empty={emptyConfigurationEntry}
      label={(value) => value.key}
      searchText={(value) => JSON.stringify(value)}
      validate={validateConfigurationEntry}
      Form={ConfigurationEntryForm}
      Details={ConfigurationEntryDetails}
      Summary={ConfigurationEntrySummary}
      History={(entry) => (
        <ConfigurationEntryHistory
          value={parseConfigurationEntry(auditSnapshot(entry))}
          changed={entry.highLight}
        />
      )}
      toolbar={(actions) => (
        <>
          <ImportButton
            suffix=".xlsx"
            submit={service.importRecords}
            done={async () => {
              await actions.reload();
              actions.notify("Import processed");
            }}
          />
          <Button
            title="Export XLSX"
            onPress={() => void actions.run(service.ExportAsExcel)}
          />
        </>
      )}
    />
  );
}
