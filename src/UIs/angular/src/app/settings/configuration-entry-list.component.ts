import { Component, OnInit, TemplateRef } from "@angular/core";
import { ConfigurationEntriesService } from "./configuration-entry.service";
import { IConfigurationEntry } from "./configuration-entry";
import { BsModalRef, BsModalService } from "ngx-bootstrap/modal";
import { NgForm } from "@angular/forms";
import { GuidEmpty } from "../shared/constants";

@Component({
  selector: "app-configuration-entry-list",
  templateUrl: "./configuration-entry-list.component.html",
  styleUrls: ["./configuration-entry-list.component.css"],
})
export class ConfigurationEntryListComponent implements OnInit {
  GuidEmpty = GuidEmpty;
  configurationEntries: IConfigurationEntry[] = [];
  selectedEntry: IConfigurationEntry = null;
  addUpdateModalRef: BsModalRef;
  deleteModalRef: BsModalRef;
  importExcelModalRef: BsModalRef;
  errorMessage;
  importingFile;
  constructor(
    private configurationEntriesService: ConfigurationEntriesService,
    private modalService: BsModalService
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
    this.addUpdateModalRef = this.modalService.show(template, {
      class: "modal-lg",
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
    this.addUpdateModalRef = this.modalService.show(template, {
      class: "modal-lg",
    });
  }

  deleteEntry(template: TemplateRef<any>, entry: IConfigurationEntry) {
    this.selectedEntry = entry;
    this.deleteModalRef = this.modalService.show(template, {
      class: "modal-sm",
    });
  }

  confirmAddUpdate(form: NgForm) {
    if (form.invalid) return;
    var request =
      this.selectedEntry.id == GuidEmpty
        ? this.configurationEntriesService.addConfigurationEntry(
            this.selectedEntry
          )
        : this.configurationEntriesService.updateConfigurationEntry(
            this.selectedEntry
          );

    request.subscribe({
      next: (rs) => {
        console.log(rs);
        this.addUpdateModalRef.hide();
        this.ngOnInit();
      },
      error: (err) => (this.errorMessage = err),
    });
  }

  confirmDelete() {
    this.configurationEntriesService
      .deleteConfigurationEntry(this.selectedEntry)
      .subscribe({
        next: (rs) => {
          console.log(rs);
          this.deleteModalRef.hide();
          this.ngOnInit();
        },
        error: (err) => (this.errorMessage = err),
      });
  }

  cancelDelete() {
    this.deleteModalRef.hide();
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
    this.importExcelModalRef = this.modalService.show(template, {
      class: "modal-sm",
    });
  }

  handleFileInput(files: FileList) {
    this.importingFile = files.item(0);
  }

  confirmImportExcelFile(form: NgForm) {
    if (form.invalid || !this.importingFile) {
      return;
    }

    var request = this.configurationEntriesService.importExcelFile(
      this.importingFile
    );

    request.subscribe({
      next: (rs) => {
        console.log(rs);
        this.importExcelModalRef.hide();
        this.ngOnInit();
      },
      error: (err) => (this.errorMessage = err),
    });
  }
}
