import { confirmModal } from "../shared/modal/modalRuntime";
import { ModalRef, ModalModule, ModalService } from "../shared/modal/modal";
import { Component, OnInit, TemplateRef, signal } from "@angular/core";
import { CommonModule } from "@angular/common";
import { FormsModule } from "@angular/forms";
import { ActivatedRoute, Router, RouterModule } from "@angular/router";
import { Role, RoleService } from "./role.service";
import { ActionIconComponent } from "../shared/action-icon.component";

@Component({
  selector: "app-roles",
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule, ActionIconComponent, ModalModule],
  templateUrl: "./roles.component.html",
})
export class RolesComponent implements OnInit {
  roles = signal<Role[]>([]);
  role = signal<Role>({ id: "", name: "" });
  error = signal("");
  saving = false;
  adding = false;
  newRoleName = "";
  addDialog: ModalRef<any>;
  editDialog: ModalRef<any>;
  editingRole: Role | null = null;
  editName = "";
  id: string | null = null;
  mode: "list" | "view" = "list";

  constructor(private service: RoleService, private route: ActivatedRoute, private router: Router, private dialog: ModalService) {}

  ngOnInit(): void {
    this.id = this.route.snapshot.paramMap.get("id");
    this.mode = this.router.url === "/roles" ? "list" : "view";
    this.load();
  }

  load(): void {
    this.error.set("");
    if (this.mode === "list") {
      this.service.getRoles().subscribe({
        next: roles => this.roles.set(roles),
        error: () => this.error.set("Unable to load roles. Please try again."),
      });
    } else if (this.id) {
      this.service.getRole(this.id).subscribe({
        next: role => this.role.set(role),
        error: () => this.error.set("Unable to load the role. Please try again."),
      });
    }
  }

  save(): void {
    if (!this.editingRole || !this.editName.trim()) { this.error.set("Enter a role name."); return; }
    this.saving = true;
    this.error.set("");
    this.service.updateRole({ ...this.editingRole, name: this.editName.trim() }).subscribe({
      next: () => { this.saving = false; this.editDialog.close(); this.editingRole = null; this.load(); },
      error: () => { this.saving = false; this.error.set("Unable to save the role. Please try again."); },
    });
  }

  openEdit(template: TemplateRef<any>, role: Role): void {
    this.error.set("");
    this.editingRole = role;
    this.editName = role.name;
    this.editDialog = this.dialog.open(template, { width: "400px" });
  }

  async remove(role: Role): Promise<void> {
    if (!await confirmModal(`Delete role ${role.name}?`, { title: "Delete Role", confirmText: "Delete" })) return;
    this.service.deleteRole(role.id).subscribe({
      next: () => this.load(),
      error: () => this.error.set("Unable to delete the role. Please try again."),
    });
  }

  openAdd(template: TemplateRef<any>): void {
    this.error.set("");
    this.newRoleName = "";
    this.addDialog = this.dialog.open(template, { width: "400px" });
  }

  add(): void {
    const name = this.newRoleName.trim();
    if (!name) { this.error.set("Enter a role name."); return; }
    this.adding = true;
    this.error.set("");
    this.service.addRole({ id: "", name }).subscribe({
      next: () => { this.adding = false; this.addDialog.close(); this.load(); },
      error: () => { this.adding = false; this.error.set("Unable to add the role. Please try again."); },
    });
  }
}
