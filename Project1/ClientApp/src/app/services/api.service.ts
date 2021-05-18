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
  uploadSRT(formData) {
    return this.http.post('/api/UploadFile/Subtitile', formData, {reportProgress: true, observe: 'events'});
  }
  startTranslate() {
    return this.http.post('/api/Translation',{nothing:"for Now"});
  }
  emitNavbarUpdate() {
    console.log("UPDATE CALLED")
    this.callNavbarUpdate.next(1);
  }
}
