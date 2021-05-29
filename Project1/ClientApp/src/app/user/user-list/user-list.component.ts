import { Component, OnInit } from '@angular/core';
import { MatDialog, MatSlideToggleChange } from '@angular/material';
import { Observable, Subject } from 'rxjs';
import { map, switchMap, takeUntil, tap } from 'rxjs/operators';
import { UserProfile } from 'src/app/models/login.model';
import { ApiService } from 'src/app/services/api.service';
import { AddEditUserComponent, UserData } from './add-edit-user/add-edit-user.component';

@Component({
  selector: 'app-user-list',
  templateUrl: './user-list.component.html',
  styleUrls: ['./user-list.component.css']
})
export class UserListComponent implements OnInit {
  displayedColumns: string[] = ['firstName', 'lastName', 'email', 'roles','lockoutEnabled', 'id'];
  dataSource:UserProfile[] = [];
  ngUnscubscribe = new Subject<void>();
  getUserList = new Subject<string>();
  getUserList$: Observable<string>;
  updateUserStatus = new Subject<{user:UserProfile,lockoutEnabled:boolean}>();
  updateUserStatus$: Observable<string>;
  constructor(private service: ApiService, public dialog: MatDialog) { }

  ngOnInit() {
    this.getUserList$ = this.getUserList.pipe(
      takeUntil(this.ngUnscubscribe),
      switchMap(action => {
       return this.service.getUseList(action).pipe(map(val=> {
        this.dataSource = val;
        return action;
      }));
    }))
    this.getUserList$.subscribe();
    this.getUserList.next();

    this.updateUserStatus$ = this.updateUserStatus.pipe(switchMap( action => {
      return this.service.activateUser({id:action.user.id, lockoutEnabled: action.lockoutEnabled}).pipe(tap(
        success => {
          action.user.lockoutEnabled = action.lockoutEnabled;
        },
        err => {
          action.user.lockoutEnabled = !action.lockoutEnabled;
        }
      ));
    }))
    this.updateUserStatus$.subscribe();
  }

  openAddEditUserDialog(user:UserProfile = null): void {
    var title = user == null ? "Add User" : "Edit User";
    const dialogRef = this.dialog.open(AddEditUserComponent, {
      width: '500px',
      data: {title: title, userData: user}
    });
    dialogRef.afterClosed().subscribe(result => {
      if(result && result.succeeded) {
        this.getUserList.next();
      }
    });
  }

  onHideDeactivatedUsers(event:MatSlideToggleChange) {
    if(event.checked) 
      this.getUserList.next('hide');
    else
      this.getUserList.next();
  }

  onUpdateUserStatus(user: UserProfile, event: MatSlideToggleChange) {
    this.updateUserStatus.next({user:user,lockoutEnabled:event.checked})
  }
  ngOnDestroy(): void {
    this.ngUnscubscribe.next();
    this.ngUnscubscribe.complete();
  }
}

