import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable, Subject } from 'rxjs';
import { catchError, exhaustMap, map, share, shareReplay, switchMap } from 'rxjs/operators';
import { FileManagement, Languge, TranslationQueue } from '../models/file.model';

import { Token, UserProfile } from '../models/login.model';
import { UserFolder } from '../models/userFolder.model';
import { UserData } from '../user/user-list/add-edit-user/add-edit-user.component';

@Injectable()
export class ApiService {
  private callNavbarUpdate = new Subject<any>();
  callNavbarUpdate$ = this.callNavbarUpdate.asObservable();
  private userProfile = new BehaviorSubject<UserProfile>(new UserProfile());
  userProfile$ =  this.userProfile.asObservable();
  private languages = new BehaviorSubject<Languge[]>([]);
  languages$ = this.languages.asObservable();
  private defaultLanguages = new Subject<any>();
  defaultLanguages$: Observable<string[]>;

  constructor(private http: HttpClient) { 
    this.userProfile$ = this.http.get<UserProfile>("/api/UserProfile").pipe(shareReplay(1));
    this.languages$ = this.http.get<Languge[]>('/api/Translation/GetLanguages').pipe(shareReplay(1));
    this.defaultLanguages$ = this.http.get<string[]>('/api/Translation/GetDefaultLanguages');
  }
  getDefaultLanguages() {
    this.defaultLanguages.next();
  }
  updateDefaultLanguages(langagues:string[]) {
    return this.http.post<any>('/api/Translation/UpdateDefaultLanguages',{langagues: langagues}).pipe(map(val=>val.responseTxt));
  }
  getUserRoles() {
    return this.http.get<string[]>("/api/UserProfile/GetUserRoles");
  }
  getUserFolders() {
    return this.http.get<UserFolder[]>("/api/UserProfile/GetUserFolders");
  }
  getUseList() {
    return this.http.get<UserProfile[]>("/api/UserList");
  }
  checkAvailableUsername(paras:any) {
    return this.http.post<{foundUser:boolean}>("/api/ApplicationUser/CheckAvailableUserName",paras);
  }
  addEditUser(formData: UserData): Observable<any> {
    var url = formData.id == "" ? "/api/ApplicationUser/Register" : "/api/ApplicationUser/UpdateUser";
    return this.http.post(url, formData).pipe(map(val=>val),share());
  }
  uploadSRT(formData: any) {
    return this.http.post('/api/UploadFile', formData, {reportProgress: true, observe: 'events'});
  }
  downloadSRT(fileIds: number[]) {
    return this.http.post('/api/DownloadFIle',{filesIds: fileIds}, { responseType: 'blob'});
  }
  GetInputFiles() {
    return this.http.get('/api/UploadFile/GetInputFiles');
  }
  startTranslate(queue:TranslationQueue): Observable<FileManagement> {
    return this.http.post<FileManagement>('/api/Translation',queue);
  }
  emitNavbarUpdate() {
    this.callNavbarUpdate.next(1);
  }
}
