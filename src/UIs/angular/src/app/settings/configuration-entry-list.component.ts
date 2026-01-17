import { Component, OnInit, TemplateRef } from "@angular/core";
import { CommonModule } from "@angular/common";
import { FormsModule } from "@angular/forms";
import { RouterModule } from "@angular/router";
import { ConfigurationEntriesService } from "./configuration-entry.service";
import { IConfigurationEntry } from "./configuration-entry";
import { MatDialog, MatDialogRef } from "@angular/material/dialog";
import { NgForm } from "@angular/forms";
import { GuidEmpty } from "../shared/constants";
import { MatDialogModule } from "@angular/material/dialog";

@Component({
  selector: "app-configuration-entry-list",
  templateUrl: "./configuration-entry-list.component.html",
  styleUrls: ["./configuration-entry-list.component.css"],
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule, MatDialogModule],
})
export class ConfigurationEntryListComponent implements OnInit {
  GuidEmpty = GuidEmpty;
  configurationEntries: IConfigurationEntry[] = [];
  selectedEntry: IConfigurationEntry = null;
  addUpdateModalRef: MatDialogRef<any>;
  deleteModalRef: MatDialogRef<any>;
  importExcelModalRef: MatDialogRef<any>;
  errorMessage;
  importingFile;
  constructor(
    private configurationEntriesService: ConfigurationEntriesService,
    private dialog: MatDialog
  ) {}

  ngOnInit(): void {
    this.configurationEntriesService.getConfigurationEntries().subscribe({
      next: (configurationEntries) => {
        this.configurationEntries = configurationEntries;
      },
      error: (err) => (this.errorMessage = err),
    });
  }

  addEntry(template: TemplateRef<any>) {
    this.selectedEntry = {
      id: GuidEmpty,
      key: "",
      value: "",
      description: "",
      isSensitive: false,
      createdDateTime: new Date(),
    };
    this.addUpdateModalRef = this.dialog.open(template, {
      width: "700px",
    });
  }

  updateEntry(template: TemplateRef<any>, entry: IConfigurationEntry) {
    this.selectedEntry = {
      id: entry.id,
      key: entry.key,
      value: entry.isSensitive ? "" : entry.value,
      description: entry.description,
      isSensitive: entry.isSensitive,
      createdDateTime: new Date(),
    };
    this.addUpdateModalRef = this.dialog.open(template, {
      width: "700px",
    });
  }

  deleteEntry(template: TemplateRef<any>, entry: IConfigurationEntry) {
    this.selectedEntry = entry;
    this.deleteModalRef = this.dialog.open(template, {
      width: "400px",
    });
  }

  confirmAddUpdate(form: NgForm) {
    if (form.invalid) return;
    var request =
      this.selectedEntry.id == GuidEmpty
        ? this.configurationEntriesService.addConfigurationEntry(this.selectedEntry)
        : this.configurationEntriesService.updateConfigurationEntry(this.selectedEntry);

    request.subscribe({
      next: (rs) => {
        console.log(rs);
        this.addUpdateModalRef.close();
        this.ngOnInit();
      },
      error: (err) => (this.errorMessage = err),
    });
  }

  confirmDelete() {
    this.configurationEntriesService.deleteConfigurationEntry(this.selectedEntry).subscribe({
      next: (rs) => {
        console.log(rs);
        this.deleteModalRef.close();
        this.ngOnInit();
      },
      error: (err) => (this.errorMessage = err),
    });
  }

  cancelDelete() {
    this.deleteModalRef.close();
  }

  exportAsExcel() {
    this.configurationEntriesService.exportAsExcel().subscribe({
      next: (rs) => {
        const url = window.URL.createObjectURL(rs);
        const element = document.createElement("a");
        element.href = url;
        element.download = "Settings.xlsx";
        document.body.appendChild(element);
        element.click();
      },
      error: (err) => (this.errorMessage = err),
    });
  }

  openImportExcelModal(template: TemplateRef<any>) {
    this.importingFile = null;
    this.importExcelModalRef = this.dialog.open(template, {
      width: "500px",
    });
  }

  handleFileInput(files: FileList) {
    this.importingFile = files.item(0);
  }

  confirmImportExcelFile(form: NgForm) {
    if (form.invalid || !this.importingFile) {
      return;
    }

    var request = this.configurationEntriesService.importExcelFile(this.importingFile);

    request.subscribe({
      next: (rs) => {
        console.log(rs);
        this.importExcelModalRef.close();
        this.ngOnInit();
      },
      error: (err) => (this.errorMessage = err),
    });
  }
}
