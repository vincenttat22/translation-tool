import { HttpClient, HttpEventType } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { ApiService } from '../services/api.service';

@Component({
  selector: 'app-translation-tool',
  templateUrl: './translation-tool.component.html',
  styleUrls: ['./translation-tool.component.css']
})
export class TranslationToolComponent implements OnInit {
  public progress: number;
  public message: string;
  constructor(private service: ApiService) { }

  
  ngOnInit() {
  }
  uploadFile(files: File[]) {
    if (files.length === 0) {
      return;
    }
    let filesToUpload : File[] = files;
    const formData = new FormData();
      
    Array.from(filesToUpload).map((file, index) => {
      return formData.append('file'+index, file, file.name);
    });
    this.service.uploadSRT(formData)
      .subscribe(event => {
        if (event.type === HttpEventType.UploadProgress)
          this.progress = Math.round(100 * event.loaded / event.total);
        else if (event.type === HttpEventType.Response) {
          this.message = 'Upload success.';
        }
      });
  }
  onTranslate() {
    this.service.startTranslate().subscribe();
  }
}
