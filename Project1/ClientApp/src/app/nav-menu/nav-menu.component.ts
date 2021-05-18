import { Component, OnInit } from '@angular/core';
import { Observable } from 'rxjs';
import { take } from 'rxjs/operators';
import { AuthGuard } from '../authentication/auth.guard';
import { UserProfile } from '../models/login.model';
import { ApiService } from '../services/api.service';

@Component({
  selector: 'app-nav-menu',
  templateUrl: './nav-menu.component.html',
  styleUrls: ['./nav-menu.component.css']
})
export class NavMenuComponent implements OnInit{
  isExpanded = false;

  constructor(private apiService: ApiService, private authGuard: AuthGuard) {

  }
  userProfile: UserProfile = new UserProfile();
  userFullName: string = '';
  showNavBar: boolean = false;
  ngOnInit(): void {
    this.apiService.callNavbarUpdate$.subscribe((val)=> {
      this.updateNavbar();
    });
    this.updateNavbar();
  }

  updateNavbar() {
    if(localStorage.getItem('token') != null) {
      this.showNavBar = true;
      const userProfile$: Observable<UserProfile>  = this.apiService.getUserProfile();
      userProfile$.subscribe(val => { 
        this.userProfile = val
        this.userFullName = `${val.firstName} ${val.lastName}`,
        take(1)
      })
    } else {
      this.showNavBar = false;
      this.userProfile = new UserProfile();
    }
  }

  collapse() {
    this.isExpanded = false;
  }

  toggle() {
    this.isExpanded = !this.isExpanded;
  }
  logout() {
    this.authGuard.logout();
  }
}
