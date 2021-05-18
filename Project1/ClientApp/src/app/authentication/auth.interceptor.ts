import { HttpEvent, HttpHandler, HttpInterceptor, HttpRequest } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Router } from "@angular/router";
import { Observable } from "rxjs";
import { tap } from "rxjs/operators";
import { ApiService } from "../services/api.service";
import { AuthGuard } from "./auth.guard";

@Injectable()
export class AuthInterceptor implements HttpInterceptor {
    constructor(private router: Router, private authGuard: AuthGuard) {

    }
    intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
        var cloneReq = null;
        if(localStorage.getItem('token') != null) {
             cloneReq = req.clone({
                headers: req.headers.set('Authorization','Bearer '+localStorage.getItem('token'))
            })
            
        } else {
            cloneReq = req.clone();
        }
        return next.handle(cloneReq).pipe(
            tap(
                success => {},
                err => {
                    if(err.status == 401) {
                        this.authGuard.logout();
                    }
                }
            )
        );
    }
    
}