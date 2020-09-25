import { Component, OnInit } from "@angular/core";
import { IUser } from "../user";
import { UserService } from "../user.service";
import { Router, ActivatedRoute } from "@angular/router";
import { NgModel, NgForm } from "@angular/forms";

@Component({
  selector: "app-add-edit-user",
  templateUrl: "./add-edit-user.component.html",
  styleUrls: ["./add-edit-user.component.css"],
})
export class AddEditUserComponent implements OnInit {
  formMode: string = "add";
  user: IUser = {
    id: "00000000-0000-0000-0000-000000000000",
    userName: null,
    email: null,
    emailConfirmed: false,
    phoneNumber: null,
    phoneNumberConfirmed: false,
    twoFactorEnabled: false,
    lockoutEnabled: false,
    lockoutEnd: null,
    accessFailedCount: 0,
  };
  postErrorMessage: string = "";
  postError = false;
  isDirty = false;

  constructor(
    private userService: UserService,
    private router: Router,
    private route: ActivatedRoute
  ) {}

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get("id");
    if (id) {
      this.formMode = "edit";
      this.userService.getUser(id).subscribe({
        next: (user) => {
          this.user = user;
        },
        error: (err) => (this.postErrorMessage = err),
      });
    }
  }

  onBlur(field: NgModel) {
    if (field.dirty) {
      this.isDirty = true;
    }
    console.log("in onBlur: ", field.valid);
  }

  onSubmit(form: NgForm) {
    console.log("in onSubmit: ", form.value, this.user);

    if (form.valid) {
      const id = this.route.snapshot.paramMap.get("id");
      if (id) {
        this.userService.updateUser(this.user).subscribe(
          (result) => {
            console.log("success: ", result);
            this.isDirty = false;
            this.router.navigate(["/users", id]);
          },
          (error) => this.onHttpError(error)
        );
      } else {
        this.userService.addUser(this.user).subscribe(
          (result) => {
            console.log("success: ", result);
            this.isDirty = false;
            this.router.navigate(["/users", result.id]);
          },
          (error) => this.onHttpError(error)
        );
      }
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
