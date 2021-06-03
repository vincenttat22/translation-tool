import { Component, Inject, OnInit, ViewChild } from '@angular/core';
import { MatDialog, MatDialogRef, MatSelectionList, MatSnackBar, MatSnackBarRef, MAT_DIALOG_DATA } from '@angular/material';
import { Observable, Subject } from 'rxjs';
import { catchError, map, switchMap, tap } from 'rxjs/operators';
import { ConfirmationDialog } from 'src/app/confirmation-dialog/confirmation-dialog.component';
import { Permission, RolePermissions } from 'src/app/models/user.model';
import { ApiService } from 'src/app/services/api.service';

@Component({
  selector: 'app-user-roles',
  templateUrl: './user-roles.component.html',
  styleUrls: ['./user-roles.component.css']
})
export class UserRolesComponent implements OnInit {
  selectedOptions: {Home: string[], User: string[]} = {Home:[], User:[]};
  roleList: string[] = [];
  getUserRoles = new Subject<void>();
  getUserRoles$: Observable<void>;
  getRolePermissions = new Subject<string>();
  getRolePermissions$: Observable<void>;
  updateRolePermissions = new Subject<Permission>();
  updateRolePermissions$: Observable<void>;
  editUserRole = new Subject<void>();
  editUserRole$: Observable<void>;
  deleteUserRole = new Subject<string>();
  deleteUserRole$: Observable<void>;
  selectedRole = "";
  selectedEditRole = "";
  constructor(private service: ApiService, private dialog: MatDialog, private snackBar: MatSnackBar) { }

  ngOnInit() {
    this.getUserRoles$ = this.getUserRoles.pipe(switchMap(val=> {
      return this.service.getUserRoles().pipe(map(val=> {
        this.roleList = val;
      }))
    }))
    this.getUserRoles$.subscribe();
    this.getUserRoles.next();  

    this.getRolePermissions$ = this.getRolePermissions.pipe(switchMap(val=> {
      return this.service.getRolePermissions(val).pipe(map(permissions=> {
        permissions.forEach(x=> {
          var claimType = x.claimType;
          this.selectedOptions[claimType] = x.claimValues;
        })
      }))
    }))
    this.getRolePermissions$.subscribe();

    this.updateRolePermissions$ = this.updateRolePermissions.pipe(switchMap(val=> {
      return this.service.updateRolePermissions({roleName:this.selectedRole,...val}).pipe(map(rs=> {
        if(!rs.succeeded) {
          this.snackBar.open(rs.errors[0].description, '', {
            duration: 3000
          });
        }
      }))
    }))
    this.updateRolePermissions$.subscribe();

    this.editUserRole$ = this.editUserRole.pipe(switchMap(_=> 
      this.service.editUserRole({name: this.selectedRole, newName: this.selectedEditRole}).pipe(map(val=>{
        if(val.succeeded) {
          this.roleList = this.roleList.map(val => (this.selectedRole == val ? this.selectedEditRole : val));
          this.selectedRole = this.selectedEditRole;
        } else {
          this.snackBar.open(val.errors[0].description, '', {
            duration: 3000
          });
        } 
      })))
    );
    this.editUserRole$.subscribe();

    this.deleteUserRole$ = this.deleteUserRole.pipe(switchMap(roleName=> 
      this.service.deleteUserRole(roleName).pipe(map(val=>{
        if(val.succeeded) {
          this.getUserRoles.next();
        } else {
          this.snackBar.open(val.errors[0].description, '', {
            duration: 3000
          });
        } 
      })))
    );
    this.deleteUserRole$.subscribe();
  }
  openAddRole() {
    const dialogRef = this.dialog.open(AddRoleDialog, {
      width: '250px'
    });

    dialogRef.afterClosed().subscribe((result:string) => {
      if(result != "") {
        this.getUserRoles.next(); 
      }
    });
  }
  onSelectRole(roleName:string) {
    this.selectedRole = roleName;
    this.selectedEditRole = roleName;
    this.getRolePermissions.next(roleName);
  }

  onDeleteRole(roleName:string) {
    const message = `Are you sure you want to delte this role?`;
    const dialogRef = this.dialog.open(ConfirmationDialog, {
      width: '350px',
      data:  { title: 'Confirmation',message: message }
    });

    dialogRef.afterClosed().subscribe((result) => {
      if(result) {
        this.deleteUserRole.next(roleName);
      }
    });
  }
  onSaveRole() {
    this.editUserRole.next();
  }
  onPermissionChange(event: string[], claimType: string) {
    var Permissions = [{
      claimType: claimType,
      claimValues: event,
      }]
    this.updateRolePermissions.next({claimType: claimType,claimValues: event})
  }
}


@Component({
  selector: 'add-role-dialog',
  template: `<h1 mat-dialog-title>Add role</h1>
  <form #form='ngForm' class="" novalidate (ngSubmit)="onSubmitAddRole(form)" autocomplete="off">
    <mat-form-field style="width: 100%;">
      <input name="roleName2" #roleName2="ngModel" [(ngModel)]="data.roleName" matInput required value="" placeholder="Enter your new role">
    </mat-form-field>
    <div class="mat-dialog-buttons" style="width:100%; text-align: right;">
      <button type="button" mat-button (click)="onNoClick()">Cancel</button>
      <button type="submit" mat-button [disabled]="form.invalid" cdkFocusInitial >Save</button>
    </div>
  </form>`
})
export class AddRoleDialog implements OnInit{

  constructor(public dialogRef: MatDialogRef<AddRoleDialog>,private service: ApiService, private snackBar: MatSnackBar) {}
  
  data = {roleName: "" };
  addUserRole = new Subject<void>();
  addUserRole$: Observable<void>;

  ngOnInit(): void {
    this.addUserRole$ = this.addUserRole.pipe(switchMap(val=> 
      this.service.addUserRole({name: this.data.roleName}).pipe(map(val=>{
        if(val.succeeded) {
          this.dialogRef.close(this.data.roleName);
        } else {
          this.snackBar.open(val.errors[0].description, '', {
            duration: 3000
        });
      }      
    }))));
    this.addUserRole$.subscribe();
  }
  onNoClick(): void {
    this.dialogRef.close();
  }
  
  onSubmitAddRole() {
    this.addUserRole.next();
  }
}
