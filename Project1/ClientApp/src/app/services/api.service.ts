import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable, Subject } from 'rxjs';
import { catchError, shareReplay } from 'rxjs/operators';
import { Languge, TranslationQueue } from '../models/file.model';

import { Token, UserProfile } from '../models/login.model';
import { UserFolder } from '../models/userFolder.model';

@Injectable()
export class ApiService {
  private callNavbarUpdate = new Subject<any>();
  callNavbarUpdate$ = this.callNavbarUpdate.asObservable();
  private userProfile = new BehaviorSubject<UserProfile>(new UserProfile());
  userProfile$ =  this.userProfile.asObservable();
  private languages = new BehaviorSubject<Languge[]>([]);
  languages$ = this.languages.asObservable();

  constructor(private http: HttpClient) { 
    this.userProfile$ = this.http.get<UserProfile>("/api/UserProfile").pipe(shareReplay(1));
    this.languages$ = this.http.get<Languge[]>('/api/Translation/GetLanguages').pipe(shareReplay(1));
  }
  getUserFolders(): Observable<UserFolder[]> {
    return this.http.get<UserFolder[]>("/api/UserProfile/GetUserFolders");
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
  startTranslate(queue:TranslationQueue): Observable<TranslationQueue> {
    return this.http.post<TranslationQueue>('/api/Translation',queue);
  }
  emitNavbarUpdate() {
    this.callNavbarUpdate.next(1);
  }
}
