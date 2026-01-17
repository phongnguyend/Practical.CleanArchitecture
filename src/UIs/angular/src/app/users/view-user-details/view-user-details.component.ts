import { Component, OnInit, TemplateRef } from "@angular/core";

import { FormsModule } from "@angular/forms";
import { RouterModule } from "@angular/router";
import { IUser } from "../user";
import { UserService } from "../user.service";
import { Router, ActivatedRoute } from "@angular/router";
import { MatDialog, MatDialogRef } from "@angular/material/dialog";
import { NgForm } from "@angular/forms";
import { MatDialogModule } from "@angular/material/dialog";

@Component({
  selector: "app-view-user-details",
  templateUrl: "./view-user-details.component.html",
  styleUrls: ["./view-user-details.component.css"],
  standalone: true,
  imports: [FormsModule, RouterModule, MatDialogModule],
})
export class ViewUserDetailsComponent implements OnInit {
  user: IUser = null;
  setPasswordModel: any = {};
  passwordValidationErrors: any = [];
  postErrorMessage: string = "";
  setPasswordModalRef: MatDialogRef<any>;
  sendPasswordResetEmailModalRef: MatDialogRef<any>;
  sendEmailAddressConfirmationEmailModalRef: MatDialogRef<any>;
  constructor(
    private userService: UserService,
    private router: Router,
    private route: ActivatedRoute,
    private dialog: MatDialog
  ) {}

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get("id");
    if (id) {
      this.userService.getUser(id).subscribe({
        next: (user) => {
          this.user = user;
        },
        error: (err) => (this.postErrorMessage = err),
      });
    }
  }

  setPasswordModal(template: TemplateRef<any>) {
    this.setPasswordModel = {};
    this.passwordValidationErrors = [];
    this.setPasswordModalRef = this.dialog.open(template, {
      width: "600px",
    });
  }

  sendPasswordResetEmailModal(template: TemplateRef<any>) {
    this.sendPasswordResetEmailModalRef = this.dialog.open(template, {
      width: "400px",
    });
  }

  confirmSendPasswordResetEmail() {
    this.userService.sendPasswordResetEmail(this.user.id).subscribe({
      next: () => {
        this.sendPasswordResetEmailModalRef.close();
      },
      error: (err) => {
        this.postErrorMessage = err;
      },
    });
  }

  sendEmailAddressConfirmationEmailModal(template: TemplateRef<any>) {
    this.sendEmailAddressConfirmationEmailModalRef = this.dialog.open(template, {
      width: "400px",
    });
  }

  confirmSendEmailAddressConfirmationEmail() {
    this.userService.sendEmailAddressConfirmationEmail(this.user.id).subscribe({
      next: () => {
        this.sendEmailAddressConfirmationEmailModalRef.close();
      },
      error: (err) => {
        this.postErrorMessage = err;
      },
    });
  }

  confirmSetPassword(form: NgForm) {
    if (
      this.setPasswordModel.password &&
      this.setPasswordModel.password == this.setPasswordModel.confirmPassword
    ) {
      this.userService.setPassword(this.user.id, this.setPasswordModel.confirmPassword).subscribe({
        next: () => {
          setTimeout(() => {
            this.setPasswordModel = {};
          }, 1000);
          this.setPasswordModalRef.close();
        },
        error: (err) => {
          if (Array.isArray(err)) {
            this.passwordValidationErrors = err;
          } else {
            this.postErrorMessage = err;
          }
        },
      });
    }
  }
}
