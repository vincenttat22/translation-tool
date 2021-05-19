import { HttpClient, HttpEventType } from "@angular/common/http";
import { Component, OnInit } from "@angular/core";
import { Subject } from "rxjs";
import { map, switchMap } from "rxjs/operators";
import { FileManagement } from "../models/file.model";
import { ApiService } from "../services/api.service";
import { faChevronLeft, faChevronRight, faMinus, faTimes, faSync, faLanguage, faCog, faDownload, faUpload } from '@fortawesome/free-solid-svg-icons';
@Component({
  selector: "app-translation-tool",
  templateUrl: "./translation-tool.component.html",
  styleUrls: ["./translation-tool.component.css"],
})
export class TranslationToolComponent implements OnInit {
  public progress: number;
  public message: string;
  public inputFiles: FileManagement[];
  displayedColumns = ['id', 'originalFileName', 'languageCode', 'createdDate'];
  
  constructor(private service: ApiService) {}

  requestUploadSRT: Subject<string> = new Subject<string>();
  requestUploadSRT$ = this.requestUploadSRT.asObservable();
  awesomeIcon = {faChevronLeft:faChevronLeft, faChevronRight: faChevronRight, faMinus: faMinus, faTimes: faTimes, faSync: faSync, faLanguage: faLanguage, faCog: faCog, faDownload: faDownload, faUpload: faUpload}
  ngOnInit() {
    this.requestUploadSRT$ = this.requestUploadSRT.pipe(
      switchMap((val) => {
        switch (val) {
          case "input":
            return this.service.GetInputFiles().pipe(
              map((files: FileManagement[]) => {
                this.inputFiles = files;
                console.log(this.inputFiles)
                return val;
              })
            );
        }
      })
    );
    this.requestUploadSRT$.subscribe();
    this.requestUploadSRT.next("input");
  }
  uploadFile(files: File[]) {
    if (files.length === 0) {
      return;
    }
    let filesToUpload: File[] = files;
    const formData = new FormData();

    Array.from(filesToUpload).map((file, index) => {
      return formData.append("file" + index, file, file.name);
    });
    this.service.uploadSRT(formData).subscribe((event) => {
      if (event.type === HttpEventType.UploadProgress)
        this.progress = Math.round((100 * event.loaded) / event.total);
      else if (event.type === HttpEventType.Response) {
        this.message = "Upload success.";
        this.requestUploadSRT.next("input");
      }
    });
  }
  onTranslate() {
    this.service.startTranslate().subscribe();
  }
}
