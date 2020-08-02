import { Component, OnInit, TemplateRef } from "@angular/core";
import { FileService } from "../file.service";
import { Router, ActivatedRoute } from "@angular/router";
import { IFile } from "../file";
import { NgModel, NgForm } from "@angular/forms";
import { IAuditLogEntry } from "src/app/auditlogs/audit-log";
import { BsModalService } from "ngx-bootstrap";

@Component({
  selector: "app-edit-file",
  templateUrl: "./edit-file.component.html",
  styleUrls: ["./edit-file.component.css"],
})
export class EditFileComponent implements OnInit {
  file: IFile = null;
  postErrorMessage: string = "";
  postError = false;
  isDirty = false;
  auditLogs: IAuditLogEntry[];
  constructor(
    private fileService: FileService,
    private router: Router,
    private route: ActivatedRoute,
    private modalService: BsModalService
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
        this.modalService.show(template, { class: "modal-xl" });
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
