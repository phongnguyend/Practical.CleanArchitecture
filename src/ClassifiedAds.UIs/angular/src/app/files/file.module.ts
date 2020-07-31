import { NgModule } from "@angular/core";
import { UploadFileComponent } from "./upload-file/upload-file.component";
import { DeleteFileComponent } from "./delete-file/delete-file.component";
import { ListFileComponent } from "./list-file/list-file.component";
import { RouterModule } from "@angular/router";
import { SharedModule } from "../shared/shared.module";
import { ModalModule } from "ngx-bootstrap";
import { EditFileComponent } from "./edit-file/edit-file.component";

@NgModule({
  declarations: [
    UploadFileComponent,
    DeleteFileComponent,
    ListFileComponent,
    EditFileComponent,
  ],
  imports: [
    RouterModule.forChild([
      { path: "files", component: ListFileComponent },
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
