import { Component, Input, OnChanges, signal } from "@angular/core";
import { CommonModule } from "@angular/common";
import { FormsModule } from "@angular/forms";
import { HttpClient } from "@angular/common/http";
import { firstValueFrom } from "rxjs";
import { environment } from "../../environments/environment";
import { ActionIconComponent } from "../shared/action-icon.component";

interface QueueItem { pending?: number; completed?: number; failed?: number; id: string; title: string; body: string; recipient: string; status: string; attempts: number; createdAt: string; }
@Component({
  selector: "app-notification-playgrounds",
  standalone: true,
  imports: [CommonModule, FormsModule, ActionIconComponent],
  templateUrl: "./notification-playgrounds.component.html",
})
export class NotificationPlaygroundsComponent implements OnChanges {
  @Input() channel = "email";
  section = "notifications";
  readonly destinations = ["notifications", "home", "products", "files", "users", "roles", "configurationentries", "auditlogentries"];
  get push(): boolean {
    return this.channel === "mobilepush";
  }
  get name(): string {
    return this.push ? "Mobile Push" : this.email ? "Email" : "SMS";
  }
  from = "";
  recipient = "";
  subject = "";
  body = "";
  info = signal<{ provider: string; deviceCount?: number; items: QueueItem[] } | null>(null);
  busy = signal(false);
  error = signal("");
  success = signal("");
  constructor(private http: HttpClient) {}
  get email(): boolean {
    return this.channel === "email";
  }
  get url(): string {
    return environment.ResourceServer.Endpoint + "notifications/" + (this.push ? "playground" : "playgrounds/" + this.channel);
  }
  ngOnChanges(): void {
    this.from = this.recipient = this.subject = this.body = "";
    this.success.set("");
    void this.refresh();
  }
  async refresh(): Promise<void> {
    try {
      this.info.set(await firstValueFrom(this.http.get<{ provider: string; deviceCount?: number; items: QueueItem[] }>(this.url)));
      this.error.set("");
    } catch {
      this.error.set("Unable to load the queue. Please try again.");
    }
  }
  async queue(): Promise<void> {
    if (this.busy()) {
      return;
    }
    this.success.set("");
    if ((!this.push && !this.recipient.trim()) || !this.body.trim() || ((this.email || this.push) && !this.subject.trim()) || (this.email && !this.from.trim())) {
      this.error.set(this.push ? "Title and message are required." : "Complete all required fields.");
      return;
    }
    const emailPattern = /^[^\s@;]+@[^\s@;]+\.[^\s@;]+$/;
    if (!this.push && (this.email ? !emailPattern.test(this.from.trim()) || !emailPattern.test(this.recipient.trim()) : !/^\+[1-9][0-9]{7,14}$/.test(this.recipient.trim()))) {
      this.error.set(this.email ? "Enter valid From and To email addresses." : "Enter a phone number in international format, for example +15551234567.");
      return;
    }
    this.busy.set(true);
    this.error.set("");
    try {
      const result = await firstValueFrom(this.http.post<{ id: string }>(this.url, this.push ? { title: this.subject.trim(), body: this.body.trim(), section: this.section } : { from: this.from.trim(), recipient: this.recipient.trim(), subject: this.subject.trim(), body: this.body.trim() }));
      this.success.set(`Message queued. ID: ${result.id}`);
      await this.refresh();
    } catch {
      this.error.set("Unable to queue the message. Check the fields and try again.");
    } finally {
      this.busy.set(false);
    }
  }
}
