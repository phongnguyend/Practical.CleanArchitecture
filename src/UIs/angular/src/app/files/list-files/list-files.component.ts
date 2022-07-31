import { Component, OnInit, TemplateRef } from "@angular/core";
import { IFile } from "../file";
import { FileService } from "../file.service";
import { BsModalService, BsModalRef } from "ngx-bootstrap/modal";
import { IAuditLogEntry } from "src/app/auditlogs/audit-log";

@Component({
  selector: "app-list-files",
  templateUrl: "./list-files.component.html",
  styleUrls: ["./list-files.component.css"],
})
export class ListFilesComponent implements OnInit {
  files: IFile[] = [];
  selectedFile: IFile = null;
  modalRef: BsModalRef;
  errorMessage;
  auditLogs: IAuditLogEntry[];
  constructor(
    private fileService: FileService,
    private modalService: BsModalService
  ) {}

  ngOnInit(): void {
    this.fileService.getFiles().subscribe({
      next: (files) => {
        this.files = files;
      },
      error: (err) => (this.errorMessage = err),
    });
  }

  download(file: IFile) {
    this.fileService.downloadFile(file).subscribe({
      next: (rs) => {
        const url = window.URL.createObjectURL(rs);
        const element = document.createElement("a");
        element.href = url;
        element.download = file.fileName;
        document.body.appendChild(element);
        element.click();
      },
      error: (err) => (this.errorMessage = err),
    });
  }

  deleteFile(template: TemplateRef<any>, file: IFile) {
    this.selectedFile = file;
    this.modalRef = this.modalService.show(template, { class: "modal-sm" });
  }

  confirmDelete() {
    this.fileService.deleteFile(this.selectedFile).subscribe({
      next: (rs) => {
        console.log(rs);
        this.modalRef.hide();
        this.ngOnInit();
      },
      error: (err) => (this.errorMessage = err),
    });
  }

  cancelDelete() {
    this.modalRef.hide();
  }

  viewAuditLogs(template: TemplateRef<any>, file: IFile) {
    this.fileService.getAuditLogs(file.id).subscribe({
      next: (logs) => {
        this.auditLogs = logs;
        this.modalService.show(template, { class: "modal-xl" });
      },
      error: (err) => (this.errorMessage = err),
    });
  }
}
