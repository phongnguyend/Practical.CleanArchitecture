import { NgModule } from "@angular/core";
import { RouterModule, Routes } from "@angular/router";
import { WelcomeComponent } from "./core/welcome.component";
import { OidcLoginRedirect } from './auth/oidc-login-redirect.component';

const routes: Routes = [
  { path: "welcome", component: WelcomeComponent },
  { path: "oidc-login-redirect", component: OidcLoginRedirect },
  { path: "", redirectTo: "welcome", pathMatch: "full" },
  { path: "**", redirectTo: "welcome", pathMatch: "full" }
];

@NgModule({
  imports: [RouterModule.forRoot(routes, {})],
  exports: [RouterModule]
})
export class AppRoutingModule { }
