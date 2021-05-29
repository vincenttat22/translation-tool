import { Component, Inject, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { FormControl, FormGroup, NgForm } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { Subject } from 'rxjs';
import { debounceTime, map, switchMap, takeUntil, tap } from 'rxjs/operators';
import { ApiService } from 'src/app/services/api.service';

@Component({
  selector: 'app-add-edit-user',
  templateUrl: './add-edit-user.component.html',
  styleUrls: ['./add-edit-user.component.css']
})
export class AddEditUserComponent implements OnInit, OnDestroy {
  @ViewChild('form', {static: false}) eForm: NgForm;
  formModel:UserData = { id: "",userName: "",firstName: "",lastName: "",password: "",email: "",roles: [] };
  roleList:string[] = [];
  keywordSubject: Subject<{val:string,from:string}> = new Subject();
  ngUnscubscribe: Subject<void> = new Subject<void>();
  constructor(
    public dialogRef: MatDialogRef<AddEditUserComponent>,
    @Inject(MAT_DIALOG_DATA) public data: DialogData, private service: ApiService) {
      dialogRef.disableClose = true;
      if(data.userData != null) {
        this.formModel = Object.assign({},data.userData);
      }
    }
  ngOnInit(): void {
    this.service.getUserRoles().subscribe(val=> {
      this.roleList = val;
    })
    this.keywordSubject.asObservable().pipe(
      takeUntil(this.ngUnscubscribe),
      debounceTime(500),
      switchMap(({val,from}) => {
        var para = from == "userName" ? {id: this.formModel.id, userName: val} : {id: this.formModel.id, email: val};
        return this.service.checkAvailableUsername(para).pipe(tap(val=>{
          if(this.eForm.form.controls[from] && val.foundUser) {
            this.eForm.form.controls[from].setErrors({'incorrect': true, 'message':'exists'});
          }
        }));
      })
    ).subscribe();
  }
  onNoClick(): void {
    this.dialogRef.close();
  }
  onSubmitUserData(form: NgForm) {
    this.service.addEditUser(form.value).subscribe(val=> {
      this.dialogRef.close(val);
      form.resetForm();
    });
  }

  onKeywordChange(val: any,from: string) {
    if(val && from)
      this.keywordSubject.next({val,from});
  }

  ngOnDestroy(): void {
    this.ngUnscubscribe.next();
    this.ngUnscubscribe.complete();
  }
}

export interface DialogData {
  title: string;
  userData: UserData;
}

export interface UserData {
  id: string;
  userName: string;
  firstName: string;
  lastName: string;
  password: string;
  email: string;
  roles: string[];
}