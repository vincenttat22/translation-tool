import { HttpClient, HttpEventType } from "@angular/common/http";
import { Component, OnInit } from "@angular/core";
import { Subject } from "rxjs";
import { map, switchMap } from "rxjs/operators";
import { FileManagement } from "../models/file.model";
import { ApiService } from "../services/api.service";

@Component({
  selector: "app-translation-tool",
  templateUrl: "./translation-tool.component.html",
  styleUrls: ["./translation-tool.component.css"],
})
export class TranslationToolComponent implements OnInit {
  public progress: number;
  public message: string;
  inputFiles: FileManagement[];
  public fileManagements:FileManagement[];
  displayedColumns = ['', 'OriginalFileName', 'LanguageCode', 'CreatedDate'];
  dataSource = ELEMENT_DATA;
  
  constructor(private service: ApiService) {}

  requestUploadSRT: Subject<string> = new Subject<string>();
  requestUploadSRT$ = this.requestUploadSRT.asObservable();
  ngOnInit() {
    this.requestUploadSRT$ = this.requestUploadSRT.pipe(
      switchMap((val) => {
        switch (val) {
          case "input":
            return this.service.GetInputFiles().pipe(
              map((files: FileManagement[]) => {
                this.inputFiles = files;
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
      }
    });
  }
  onTranslate() {
    this.service.startTranslate().subscribe();
  }
}

export interface PeriodicElement {
  name: string;
  position: number;
  weight: number;
  symbol: string;
}

const ELEMENT_DATA: PeriodicElement[] = [
  {position: 1, name: 'Hydrogen', weight: 1.0079, symbol: 'H'},
  {position: 2, name: 'Helium', weight: 4.0026, symbol: 'He'},
  {position: 3, name: 'Lithium', weight: 6.941, symbol: 'Li'},
  {position: 4, name: 'Beryllium', weight: 9.0122, symbol: 'Be'},
  {position: 5, name: 'Boron', weight: 10.811, symbol: 'B'},
  {position: 6, name: 'Carbon', weight: 12.0107, symbol: 'C'},
  {position: 7, name: 'Nitrogen', weight: 14.0067, symbol: 'N'},
  {position: 8, name: 'Oxygen', weight: 15.9994, symbol: 'O'},
  {position: 9, name: 'Fluorine', weight: 18.9984, symbol: 'F'},
  {position: 10, name: 'Neon', weight: 20.1797, symbol: 'Ne'},
];