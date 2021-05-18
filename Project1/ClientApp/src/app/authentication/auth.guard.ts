import { HttpClient, HttpErrorResponse } from "@angular/common/http";
import { Injectable } from "@angular/core";
import {
  ActivatedRouteSnapshot,
  CanActivate,
  Router,
  RouterStateSnapshot,
} from "@angular/router";
import { Observable } from "rxjs";
import { ApiService } from "../services/api.service";

@Injectable({
  providedIn: "root",
})
export class AuthGuard implements CanActivate {
  constructor(private router: Router, private http: HttpClient, private apiService: ApiService) {}

  canActivate(
    route: ActivatedRouteSnapshot,
    state: RouterStateSnapshot
  ): Observable<boolean> | Promise<boolean> | boolean {
    if (localStorage.getItem("token") == null) {
      this.router.navigate(["/login"]);
    }
    return true;
  }

  login(formData: {}) {
    return this.http.post("/api/ApplicationUser/Login", formData);
  }

  logout() {
    localStorage.removeItem('token');
    this.router.navigateByUrl('/login');
    this.apiService.emitNavbarUpdate();
  }

  handleError(error: HttpErrorResponse) {
    console.log(error)
    if (
      error.status !== 200 &&
      error.status !== 502 &&
      error.name === "HttpErrorResponse"
    ) {
      this.logout();
      return error;
    }
  }
}
