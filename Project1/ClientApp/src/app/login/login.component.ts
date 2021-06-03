import { Component, OnInit } from "@angular/core";
import { NgForm } from "@angular/forms";
import { Router } from "@angular/router";
import { AuthGuard } from "../authentication/auth.guard";
import { ApiService } from "../services/api.service";

@Component({
  selector: "app-login",
  templateUrl: "./login.component.html",
  styleUrls: ["./login.component.css"],
})
export class LoginComponent implements OnInit {
  formModel = {
    UserName: "",
    Password: "",
  };
  errorMessage: string = '';
  waitingInitiateLogin = true;
  constructor(private authGuard: AuthGuard,private apiService: ApiService, private router: Router) {}

  ngOnInit() {
    this.authGuard.initiateLogin().subscribe(
      (res:any) => {
        this.waitingInitiateLogin = false;
      },
      (err) => {
        this.errorMessage = err.error.message;  
      }
    );
    if(this.authGuard.canActivate) {
      this.router.navigate(['/home']);
    }
  }
  onSubmit(form: NgForm) {
    this.authGuard.login(form.value).subscribe(
      (res: any) => {
        localStorage.setItem("token", res.token);
        window.location.href= "/home";
        // this.apiService.emitNavbarUpdate();
      },
      (err) => {
        if(err.status == 400) {
          this.errorMessage = err.error.message;        
        }
      }
    );
  }
}
