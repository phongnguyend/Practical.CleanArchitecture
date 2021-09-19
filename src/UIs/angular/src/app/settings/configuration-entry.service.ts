import { Injectable } from "@angular/core";
import { HttpClient } from "@angular/common/http";

import { IConfigurationEntry } from "./configuration-entry";
import { environment } from "src/environments/environment";
import { Observable } from "rxjs";

@Injectable({
  providedIn: "root",
})
export class ConfigurationEntriesService {
  private url = environment.ResourceServer.Endpoint + "ConfigurationEntries";

  constructor(private http: HttpClient) {}

  getConfigurationEntries(): Observable<IConfigurationEntry[]> {
    return this.http.get<IConfigurationEntry[]>(this.url);
  }

  getConfigurationEntry(
    id: string
  ): Observable<IConfigurationEntry | undefined> {
    return this.http.get<IConfigurationEntry>(this.url + "/" + id);
  }

  addConfigurationEntry(
    entry: IConfigurationEntry
  ): Observable<IConfigurationEntry | undefined> {
    return this.http.post<IConfigurationEntry>(this.url, entry);
  }

  updateConfigurationEntry(
    entry: IConfigurationEntry
  ): Observable<IConfigurationEntry | undefined> {
    return this.http.put<IConfigurationEntry>(this.url + "/" + entry.id, entry);
  }

  deleteConfigurationEntry(
    entry: IConfigurationEntry
  ): Observable<IConfigurationEntry | undefined> {
    return this.http.delete<IConfigurationEntry>(this.url + "/" + entry.id);
  }

  exportAsExcel() {
    return this.http.get(this.url + "/ExportAsExcel", { responseType: "blob" });
  }

  importExcelFile(file: File): Observable<IConfigurationEntry[] | undefined> {
    const formData: FormData = new FormData();
    formData.append("formFile", file);
    return this.http.post<IConfigurationEntry[]>(
      this.url + "/ImportExcel",
      formData
    );
  }
}
