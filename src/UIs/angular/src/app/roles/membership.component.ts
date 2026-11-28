import { Component, Input, OnChanges, signal } from "@angular/core";
import { CommonModule } from "@angular/common";
import { FormsModule } from "@angular/forms";
import { HttpClient } from "@angular/common/http";
import { firstValueFrom } from "rxjs";
import { environment } from "../../environments/environment";
import { ActionIconComponent } from "../shared/action-icon.component";
import { confirmModal } from "../shared/modal/modalRuntime";

interface MembershipItem { id: string; name?: string; userName?: string; email?: string }
@Component({
  selector: "app-membership",
  standalone: true,
  imports: [CommonModule, FormsModule, ActionIconComponent],
  styles: [`
.membership-section { margin-top: 1.5rem; padding: 1.25rem 0 0; border-top: 1px solid var(--ui-border, var(--bs-border-color, #dce3ed)); }
.membership-section h2 { font-weight: 650; }
.membership-section .membership-description { color: var(--ui-muted, var(--bs-secondary-color, #64748b)); margin: 0 0 1rem; font-size: .9rem; }
.membership-assignment { display: grid; grid-template-columns: minmax(0, 1fr) minmax(0, 1fr) auto; grid-template-areas: "search-label select-label ." "search select assign"; gap: .5rem 1rem; align-items: end; margin-bottom: 1.25rem; padding: 0; }
.membership-assignment > label:first-child { grid-area: search-label; }
.membership-assignment > input { grid-area: search; }
.membership-assignment > label { grid-area: select-label; margin: 0; font-size: .875rem; font-weight: 600; }
.membership-assignment > select { grid-area: select; }
.membership-assignment > button { grid-area: assign; white-space: nowrap; }
.membership-assignment input, .membership-assignment select, .membership-assignment button { min-height: 42px; min-width: 0; }
.membership-section .list-group { padding-left: 0; list-style: none; margin-bottom: 0; }
.membership-section .list-group-item { padding: 1rem 0; gap: 1rem; border: 0; border-bottom: 1px solid var(--ui-border, var(--bs-border-color, #dce3ed)); border-radius: 0; background: transparent; }
.membership-section .list-group-item:last-child { border-bottom: 0; padding-bottom: 0; }
.membership-section small { color: var(--ui-muted, var(--bs-secondary-color, #64748b)); margin-top: .25rem; }
.membership-section .btn { display: inline-flex; align-items: center; justify-content: center; gap: .4rem; }
.membership-section .assigned-role { padding: .65rem 1rem; border: 1px solid var(--bs-border-color, #dce3ed); border-radius: 8px; }
.membership-section .assigned-roles { display: flex; flex-wrap: wrap; gap: .65rem; padding: 0; list-style: none; margin: 0; }
@media (max-width: 700px) {
  .membership-section { padding-top: 1rem; }
  .membership-assignment { grid-template-columns: minmax(0, 1fr); grid-template-areas: "search-label" "search" "select-label" "select" "assign"; gap: .65rem; }
  .membership-assignment > button { margin-top: .35rem; width: 100%; }
}
`],
  template: `
    <section class="membership-section" [attr.aria-label]="roleId ? 'Role users' : 'Assigned roles'">
      <div class="d-flex flex-wrap align-items-center justify-content-between gap-2 mb-3">
        <h2 class="h5 mb-0">{{ roleId ? 'Role users' : 'Assigned roles' }}</h2>
        <button class="btn btn-outline-secondary" [disabled]="busy()" (click)="load()"><app-action-icon action="refresh" />{{ roleId ? 'Refresh users' : 'Refresh roles' }}</button>
      </div>
      @if (roleId) {
        <p class="membership-description">Manage who belongs to this role. Search by username or email, then select a user to assign.</p>
      }
      @if (busy()) { <p role="status">Loading...</p> }
      @if (error()) { <p role="alert" class="alert alert-danger">{{ error() }}</p> }
      @if (roleId) {
        <div class="membership-assignment">
          <label for="role-user-search">Search users</label>
          <input id="role-user-search" placeholder="Username or email" class="form-control" [(ngModel)]="search" (ngModelChange)="selected = ''" />
          <label for="role-user-select">User to assign</label>
          <select id="role-user-select" class="form-select" [(ngModel)]="selected" [disabled]="busy()">
            <option value="">Select a user</option>
            @for (user of candidates; track user.id) { <option [value]="user.id">{{ user.userName }} ({{ user.email }})</option> }
          </select>
          <button class="btn btn-primary" [disabled]="busy() || !selected" (click)="assign()"><app-action-icon action="add" />Assign user</button>
        </div>
      }
      @if (!busy() && !error() && items().length === 0) { <p>{{ roleId ? 'No users assigned.' : 'No roles assigned.' }}</p> }
      <ul class="list-group">
        @for (item of items(); track item.id) {
          <li class="list-group-item d-flex flex-wrap align-items-center justify-content-between gap-3">
            <span style="overflow-wrap: anywhere">{{ item.userName || item.name }}<small class="d-block">{{ item.email }}</small></span>
            @if (roleId) { <button class="btn btn-outline-danger" [disabled]="busy()" [attr.aria-label]="'Remove ' + item.userName + ' from role'" (click)="change(item, false)"><app-action-icon action="delete" />Remove</button> }
          </li>
        }
      </ul>
    </section>`,
})
export class MembershipComponent implements OnChanges {
  @Input() roleId = "";
  @Input() userId = "";
  items = signal<MembershipItem[]>([]);
  users = signal<MembershipItem[]>([]);
  busy = signal(false);
  error = signal("");
  selected = "";
  search = "";
  private url = environment.ResourceServer.Endpoint;
  constructor(private http: HttpClient) {}
  ngOnChanges(): void {
    void this.load();
  }
  get candidates(): MembershipItem[] {
    return this.users().filter(user => !this.items().some(item => item.id === user.id)
      && `${user.userName} ${user.email || ""}`.toLowerCase().includes(this.search.toLowerCase()));
  }
  async load(): Promise<void> {
    this.busy.set(true);
    this.error.set("");
    try {
      if (this.roleId) {
        const [members, users] = await Promise.all([
          firstValueFrom(this.http.get<MembershipItem[]>(`${this.url}roles/${this.roleId}/users`)),
          firstValueFrom(this.http.get<MembershipItem[]>(`${this.url}users`)),
        ]);
        this.items.set(members);
        this.users.set(users);
      } else {
        this.items.set(await firstValueFrom(this.http.get<MembershipItem[]>(`${this.url}users/${this.userId}/roles`)));
      }
    } catch {
      this.error.set("Unable to load role membership. Please try again.");
    } finally {
      this.busy.set(false);
    }
  }
  async assign(): Promise<void> {
    const user = this.candidates.find(item => item.id === this.selected);
    if (user) {
      await this.change(user, true);
    }
  }
  async change(user: MembershipItem, assign: boolean): Promise<void> {
    if (this.busy()) {
      return;
    }
    if (!assign && !await confirmModal(`Remove ${user.userName} from this role?`, { title: "Remove assigned role", confirmText: "Remove" })) {
      return;
    }
    this.busy.set(true);
    this.error.set("");
    try {
      const path = `${this.url}roles/${this.roleId}/users/${user.id}`;
      await firstValueFrom(assign ? this.http.put(path, null) : this.http.delete(path));
      this.selected = "";
      await this.load();
    } catch {
      this.error.set("Unable to update role membership. Refresh and try again.");
    } finally {
      this.busy.set(false);
    }
  }
}
