import { Injectable } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import { Observable } from "rxjs";
import { environment } from "../../environments/environment";

export interface Role {
  id: string;
  name: string;
  normalizedName?: string;
}

@Injectable({ providedIn: "root" })
export class RoleService {
  private readonly url = environment.ResourceServer.Endpoint + "roles";
  constructor(private http: HttpClient) {}

  getRoles(): Observable<Role[]> { return this.http.get<Role[]>(this.url); }
  getRole(id: string): Observable<Role> { return this.http.get<Role>(`${this.url}/${id}`); }
  addRole(role: Role): Observable<Role> { return this.http.post<Role>(this.url, role); }
  updateRole(role: Role): Observable<Role> { return this.http.put<Role>(`${this.url}/${role.id}`, role); }
  deleteRole(id: string): Observable<void> { return this.http.delete<void>(`${this.url}/${id}`); }
}
