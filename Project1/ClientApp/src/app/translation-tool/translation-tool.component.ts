import { HttpClient, HttpEventType } from "@angular/common/http";
import { Component, OnInit } from "@angular/core";
import { Observable, Subject } from "rxjs";
import { map, switchMap } from "rxjs/operators";
import { FileManagement } from "../models/file.model";
import { ApiService } from "../services/api.service";
import { faChevronLeft, faChevronRight, faMinus, faTimes, faSync, faLanguage, faCog, faDownload, faUpload, faFolder } from '@fortawesome/free-solid-svg-icons';
import { FlatTreeControl } from "@angular/cdk/tree";
import { MatTreeFlatDataSource, MatTreeFlattener } from "@angular/material";
import { UserFolder } from "../models/userFolder.model";


/** Flat node with expandable and level information */
interface FileFlatNode {
  expandable: boolean;
  name: string;
  id: string;
  level: number;
}

@Component({
  selector: "app-translation-tool",
  templateUrl: "./translation-tool.component.html",
  styleUrls: ["./translation-tool.component.css"],
})

export class TranslationToolComponent implements OnInit {
  private _transformer = (node: UserFolder, level: number) => {
    return {
      expandable: !!node.children && node.children.length > 0,
      name: node.name,
      id: node.id,
      level: level,
    };
  }

  treeControl = new FlatTreeControl<FileFlatNode>(
      node => node.level, node => node.expandable);

  treeFlattener = new MatTreeFlattener(
      this._transformer, node => node.level, node => node.expandable, node => node.children);

  folderTreeSource = new MatTreeFlatDataSource(this.treeControl, this.treeFlattener);

  public progress: number;
  public uploadDone: boolean = false;
  public inputFiles: FileManagement[];
  public selectedNodeId: string = "";
  public selectedFolder: string = "";
  public userFolders: UserFolder[] = [];
  displayedColumns = ['id', 'originalFileName', 'languageCode', 'createdDate'];
  
  constructor(private service: ApiService) {
  }
  hasChild = (_: number, node: FileFlatNode) => node.expandable;
  requestUploadSRT: Subject<string> = new Subject<string>();
  requestUploadSRT$ = this.requestUploadSRT.asObservable();
  awesomeIcon = {faChevronLeft:faChevronLeft, faChevronRight: faChevronRight, faMinus: faMinus, faTimes: faTimes, faSync: faSync, faLanguage: faLanguage, faCog: faCog, faDownload: faDownload, faUpload: faUpload, faFolder: faFolder}
  
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
    const getUserFolders$: Observable<UserFolder[]> = this.service.getUserFolders().pipe(map(val=>{
      this.folderTreeSource.data = val;
      return val;
    }));
    getUserFolders$.subscribe();
  }
  uploadFile(files: File[]) {
    if (files.length === 0) {
      return;
    }
    let filesToUpload: File[] = files;
    const formData = new FormData();
    this.uploadDone = false;
    Array.from(filesToUpload).map((file, index) => {
      return formData.append("file" + index, file, file.name);
    });
    this.service.uploadSRT(formData).subscribe((event) => {
      if (event.type === HttpEventType.UploadProgress)
        this.progress = Math.round((100 * event.loaded) / event.total);
      else if (event.type === HttpEventType.Response) {
        this.uploadDone = true;
        this.requestUploadSRT.next("input");
      }
    });
  }
  onSelectNode(node:FileFlatNode) {
    this.selectedNodeId = node.id;
    if(node.level == 0) {
      this.userFolders = this.getNodeChildren(node)[0].children;
    }
  }
  onDbclickItemFolder(txt:string) {
    var nodeIndex = this.getNodeIndex();
    if(nodeIndex > -1) {
      this.treeControl.expand(this.treeControl.dataNodes[nodeIndex]);
      this.selectedNodeId += "-"+txt;
    }
  }
  getNodeChildren(node:FileFlatNode) {
    return this.folderTreeSource.data.map((val)=> {
      if(val.id == node.id) {
        return val;
      }
    })
  }
  getNodeIndex() {
    return this.treeControl.dataNodes.findIndex((val)=>val.id == this.selectedNodeId);
  }
  onTranslate() {
    this.service.startTranslate().subscribe();
  }
}
