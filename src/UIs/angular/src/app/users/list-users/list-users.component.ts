import { Component, OnInit, TemplateRef } from "@angular/core";
import { CommonModule } from "@angular/common";
import { RouterModule } from "@angular/router";
import { IUser } from "../user";
import { IAuditLogEntry } from "src/app/auditlogs/audit-log";
import { MatDialog, MatDialogRef } from "@angular/material/dialog";
import { UserService } from "../user.service";
import { MatDialogModule } from "@angular/material/dialog";

@Component({
  selector: "app-list-users",
  templateUrl: "./list-users.component.html",
  styleUrls: ["./list-users.component.css"],
  standalone: true,
  imports: [CommonModule, RouterModule, MatDialogModule],
})
export class ListUsersComponent implements OnInit {
  users: IUser[] = [];
  selectedUser: IUser = null;
  modalRef: MatDialogRef<any>;
  errorMessage;
  auditLogs: IAuditLogEntry[];
  constructor(
    private userService: UserService,
    private dialog: MatDialog
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
