import { Component, OnInit, TemplateRef } from "@angular/core";
import { CommonModule } from "@angular/common";
import { FormsModule } from "@angular/forms";
import { RouterModule } from "@angular/router";
import { FileService } from "../file.service";
import { Router, ActivatedRoute } from "@angular/router";
import { IFile } from "../file";
import { NgModel, NgForm } from "@angular/forms";
import { IAuditLogEntry } from "src/app/auditlogs/audit-log";
import { MatDialog } from "@angular/material/dialog";
import { GuidEmpty } from "src/app/shared/constants";
import { MatDialogModule } from "@angular/material/dialog";

@Component({
  selector: "app-edit-file",
  templateUrl: "./edit-file.component.html",
  styleUrls: ["./edit-file.component.css"],
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule, MatDialogModule],
})
export class EditFileComponent implements OnInit {
  file: IFile = {
    id: GuidEmpty,
    name: null,
    description: null,
    uploadedTime: null,
    fileName: null,
    size: 0,
    formFile: null,
    encrypted: false,
  };
  postErrorMessage: string = "";
  postError = false;
  isDirty = false;
  auditLogs: IAuditLogEntry[];
  constructor(
    private fileService: FileService,
    private router: Router,
    private route: ActivatedRoute,
    private dialog: MatDialog
  ) {}

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get("id");
    this.fileService.getFile(id).subscribe({
      next: (file) => {
        this.file = file;
      },
      //error: err => this.errorMessage = err
    });
  }

  onBlur(field: NgModel) {
    if (field.dirty) {
      this.isDirty = true;
    }
    console.log("in onBlur: ", field.valid);
  }

  onSubmit(form: NgForm) {
    console.log("in onSubmit: ", form.value, this.file);
    if (form.valid) {
      this.fileService.updateFile(this.file).subscribe(
        (result) => {
          console.log("success: ", result);
          this.isDirty = false;
          this.router.navigate(["/files"]);
        },
        (error) => this.onHttpError(error)
      );
    } else {
      this.postError = true;
      this.postErrorMessage = "Please fix the errors";
    }
  }

  viewAuditLogs(template: TemplateRef<any>) {
    this.fileService.getAuditLogs(this.file.id).subscribe({
      next: (logs) => {
        this.auditLogs = logs;
        this.dialog.open(template, { width: "90%", maxWidth: "1200px" });
      },
      error: (error) => this.onHttpError(error),
    });
  }

  onHttpError(errorResponse: any) {
    console.log("error: ", errorResponse);
    this.postError = true;
    this.postErrorMessage = errorResponse.error.errorMessage;
  }
}
