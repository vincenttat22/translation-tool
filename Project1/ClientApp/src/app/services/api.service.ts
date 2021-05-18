import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, Subject } from 'rxjs';

import { Token, UserProfile } from '../models/login.model';

@Injectable()
export class ApiService {
  private callNavbarUpdate = new Subject<any>();
  callNavbarUpdate$ = this.callNavbarUpdate.asObservable();
  constructor(private http: HttpClient) { }
  getUserProfile(): Observable<UserProfile> {
    return this.http.get<UserProfile>("/api/UserProfile");
  }
  uploadSRT(formData: any) {
    return this.http.post('/api/UploadFile', formData, {reportProgress: true, observe: 'events'});
  }
  GetInputFiles() {
    return this.http.get('/api/UploadFile/GetInputFiles');
  }
  startTranslate() {
    return this.http.post('/api/Translation',{nothing:"for Now"});
  }
  emitNavbarUpdate() {
    console.log("UPDATE CALLED")
    this.callNavbarUpdate.next(1);
  }
}
