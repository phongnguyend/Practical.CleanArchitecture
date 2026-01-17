import { Component, OnInit, TemplateRef } from "@angular/core";
import { CommonModule } from "@angular/common";
import { RouterModule } from "@angular/router";
import { IFile } from "../file";
import { FileService } from "../file.service";
import { MatDialog, MatDialogRef } from "@angular/material/dialog";
import { IAuditLogEntry } from "src/app/auditlogs/audit-log";
import { MatDialogModule } from "@angular/material/dialog";

@Component({
  selector: "app-list-files",
  templateUrl: "./list-files.component.html",
  styleUrls: ["./list-files.component.css"],
  standalone: true,
  imports: [CommonModule, RouterModule, MatDialogModule],
})
export class ListFilesComponent implements OnInit {
  files: IFile[] = [];
  selectedFile: IFile = null;
  modalRef: MatDialogRef<any>;
  errorMessage;
  auditLogs: IAuditLogEntry[];
  constructor(
    private fileService: FileService,
    private dialog: MatDialog
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
    this.modalRef = this.dialog.open(template, { width: "400px" });
  }

  confirmDelete() {
    this.fileService.deleteFile(this.selectedFile).subscribe({
      next: (rs) => {
        console.log(rs);
        this.modalRef.close();
        this.ngOnInit();
      },
      error: (err) => (this.errorMessage = err),
    });
  }

  cancelDelete() {
    this.modalRef.close();
  }

  viewAuditLogs(template: TemplateRef<any>, file: IFile) {
    this.fileService.getAuditLogs(file.id).subscribe({
      next: (logs) => {
        this.auditLogs = logs;
        this.dialog.open(template, { width: "90%", maxWidth: "1200px" });
      },
      error: (err) => (this.errorMessage = err),
    });
  }
}
