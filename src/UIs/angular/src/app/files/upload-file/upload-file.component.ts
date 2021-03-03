import { Component, OnInit } from "@angular/core";
import { IFile } from "../file";
import { Router, ActivatedRoute } from "@angular/router";
import { FileService } from "../file.service";
import { NgModel, NgForm } from "@angular/forms";
import { GuidEmpty } from "src/app/shared/constants";

@Component({
  selector: "app-upload-file",
  templateUrl: "./upload-file.component.html",
  styleUrls: ["./upload-file.component.css"],
})
export class UploadFileComponent implements OnInit {
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

  constructor(
    private fileService: FileService,
    private router: Router,
    private route: ActivatedRoute
  ) {}

  ngOnInit(): void {}

  onBlur(field: NgModel) {
    if (field.dirty) {
      this.isDirty = true;
    }
    console.log("in onBlur: ", field.valid);
  }

  handleFileInput(files: FileList) {
    this.file.formFile = files.item(0);
  }

  onSubmit(form: NgForm) {
    console.log("in onSubmit: ", form.value, this.file);
    if (form.valid && this.file.formFile) {
      this.fileService.uploadFile(this.file).subscribe(
        (result) => {
          console.log("success: ", result);
          this.isDirty = false;
          this.router.navigate(["/files/edit", result.id]);
        },
        (error) => this.onHttpError(error)
      );
    } else {
      this.postError = true;
      this.postErrorMessage = "Please fix the errors";
    }
  }

  onHttpError(errorResponse: any) {
    console.log("error: ", errorResponse);
    this.postError = true;
    this.postErrorMessage = errorResponse.error.errorMessage;
  }
}
