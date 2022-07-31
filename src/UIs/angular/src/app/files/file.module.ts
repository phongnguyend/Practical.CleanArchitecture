import { NgModule } from "@angular/core";
import { UploadFileComponent } from "./upload-file/upload-file.component";
import { ListFilesComponent } from "./list-files/list-files.component";
import { RouterModule } from "@angular/router";
import { SharedModule } from "../shared/shared.module";
import { ModalModule } from "ngx-bootstrap/modal";
import { EditFileComponent } from "./edit-file/edit-file.component";

@NgModule({
  declarations: [UploadFileComponent, ListFilesComponent, EditFileComponent],
  imports: [
    RouterModule.forChild([
      { path: "files", component: ListFilesComponent },
      {
        path: "files/upload",
        component: UploadFileComponent,
      },
      {
        path: "files/edit/:id",
        component: EditFileComponent,
      },
    ]),
    ModalModule.forRoot(),
    SharedModule,
  ],
  exports: [],
})
export class FileModule {}
