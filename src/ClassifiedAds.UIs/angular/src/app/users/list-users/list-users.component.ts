import { Component, OnInit, TemplateRef } from "@angular/core";
import { IUser } from "../user";
import { IAuditLogEntry } from "src/app/auditlogs/audit-log";
import { BsModalRef, BsModalService } from "ngx-bootstrap";
import { UserService } from "../user.service";

@Component({
  selector: "app-list-users",
  templateUrl: "./list-users.component.html",
  styleUrls: ["./list-users.component.css"],
})
export class ListUsersComponent implements OnInit {
  users: IUser[] = [];
  selectedUser: IUser = null;
  modalRef: BsModalRef;
  errorMessage;
  auditLogs: IAuditLogEntry[];
  constructor(
    private userService: UserService,
    private modalService: BsModalService
  ) {}

  ngOnInit(): void {
    this.userService.getUsers().subscribe({
      next: (users) => {
        this.users = users;
      },
      error: (err) => (this.errorMessage = err),
    });
  }

  deleteUser(template: TemplateRef<any>, user: IUser) {
    this.selectedUser = user;
    this.modalRef = this.modalService.show(template, { class: "modal-sm" });
  }

  confirmDelete() {
    this.userService.deleteUser(this.selectedUser).subscribe({
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

  viewAuditLogs(template: TemplateRef<any>, file: IUser) {
    this.userService.getAuditLogs(file.id).subscribe({
      next: (logs) => {
        this.auditLogs = logs;
        this.modalService.show(template, { class: "modal-xl" });
      },
      error: (err) => (this.errorMessage = err),
    });
  }
}
