import { ModalRef, ModalModule, ModalService } from "../../shared/modal/modal";
import { Component, OnInit, TemplateRef, ChangeDetectionStrategy } from "@angular/core";
import { CommonModule } from "@angular/common";
import { RouterModule } from "@angular/router";
import { IUser } from "../user";
import { IAuditLogEntry } from "../../auditlogs/audit-log";
import { UserService } from "../user.service";
import { ActionIconComponent } from "../../shared/action-icon.component";

@Component({
  selector: "app-list-users",
  templateUrl: "./list-users.component.html",
  styleUrls: ["./list-users.component.css"],
  standalone: true,
  changeDetection: ChangeDetectionStrategy.Eager,
  imports: [CommonModule, RouterModule, ModalModule, ActionIconComponent],
})
export class ListUsersComponent implements OnInit {
  users: IUser[] = [];
  selectedUser: IUser = null;
  modalRef: ModalRef<any>;
  errorMessage;
  auditLogs: IAuditLogEntry[];
  constructor(
    private userService: UserService,
    private dialog: ModalService
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
    this.modalRef = this.dialog.open(template, { width: "400px" });
  }

  confirmDelete() {
    this.userService.deleteUser(this.selectedUser).subscribe({
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

  viewAuditLogs(template: TemplateRef<any>, file: IUser) {
    this.userService.getAuditLogs(file.id).subscribe({
      next: (logs) => {
        this.auditLogs = logs;
        this.dialog.open(template, { width: "90%", maxWidth: "1200px" });
      },
      error: (err) => (this.errorMessage = err),
    });
  }
}
