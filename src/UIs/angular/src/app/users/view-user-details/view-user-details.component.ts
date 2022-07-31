import { Component, OnInit, TemplateRef } from "@angular/core";
import { IUser } from "../user";
import { UserService } from "../user.service";
import { Router, ActivatedRoute } from "@angular/router";
import { BsModalService, BsModalRef } from "ngx-bootstrap/modal";
import { NgForm } from "@angular/forms";

@Component({
  selector: "app-view-user-details",
  templateUrl: "./view-user-details.component.html",
  styleUrls: ["./view-user-details.component.css"],
})
export class ViewUserDetailsComponent implements OnInit {
  user: IUser = null;
  setPasswordModel: any = {};
  passwordValidationErrors: any = [];
  postErrorMessage: string = "";
  setPasswordModalRef: BsModalRef;
  sendPasswordResetEmailModalRef: BsModalRef;
  sendEmailAddressConfirmationEmailModalRef: BsModalRef;
  constructor(
    private userService: UserService,
    private router: Router,
    private route: ActivatedRoute,
    private modalService: BsModalService
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
    this.setPasswordModalRef = this.modalService.show(template, {
      class: "modal-l",
    });
  }

  sendPasswordResetEmailModal(template: TemplateRef<any>) {
    this.sendPasswordResetEmailModalRef = this.modalService.show(template, {
      class: "modal-sm",
    });
  }

  confirmSendPasswordResetEmail() {
    this.userService.sendPasswordResetEmail(this.user.id).subscribe({
      next: () => {
        this.sendPasswordResetEmailModalRef.hide();
      },
      error: (err) => {
        this.postErrorMessage = err;
      },
    });
  }

  sendEmailAddressConfirmationEmailModal(template: TemplateRef<any>) {
    this.sendEmailAddressConfirmationEmailModalRef = this.modalService.show(
      template,
      {
        class: "modal-sm",
      }
    );
  }

  confirmSendEmailAddressConfirmationEmail() {
    this.userService.sendEmailAddressConfirmationEmail(this.user.id).subscribe({
      next: () => {
        this.sendEmailAddressConfirmationEmailModalRef.hide();
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
      this.userService
        .setPassword(this.user.id, this.setPasswordModel.confirmPassword)
        .subscribe({
          next: () => {
            setTimeout(() => {
              this.setPasswordModel = {};
            }, 1000);
            this.setPasswordModalRef.hide();
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
