import { HttpClient, HttpEventType } from "@angular/common/http";
import { Component, OnInit } from "@angular/core";
import { combineLatest, Observable, Subject } from "rxjs";
import { map, switchMap } from "rxjs/operators";
import { FileManagement, Languge } from "../models/file.model";
import { ApiService } from "../services/api.service";
import { faChevronLeft, faChevronRight, faMinus, faTimes, faSync, faLanguage, faCog, faDownload, faUpload, faFolder } from '@fortawesome/free-solid-svg-icons';
import { FlatTreeControl } from "@angular/cdk/tree";
import { MatCheckboxChange, MatTreeFlatDataSource, MatTreeFlattener } from "@angular/material";
import { UserFolder } from "../models/userFolder.model";
import { notDeepEqual } from "assert";
import { UserProfile } from "../models/login.model";


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

  public progress: number = 0;
  public inputFiles: FileManagement[];
  public selectedNodeId: string = "";
  public selectedFolder: string = "";
  public selectedFileIds: number[] = [];
  public allFileChecked: boolean = false;
  public userFolders: UserFolder[] = [];
  public translationLanguages: Languge[] = [];
  private userProfile: UserProfile;
  displayedColumns = ['id', 'originalFileName', 'languageCode', 'createdDate'];
  
  constructor(private service: ApiService) {
  }
  hasChild = (_: number, node: FileFlatNode) => node.expandable;
  getUserFolders: Subject<string> = new Subject<string>();
  getUserFolders$: Observable<UserFolder[]>;
  awesomeIcon = {faChevronLeft:faChevronLeft, faChevronRight: faChevronRight, faMinus: faMinus, faTimes: faTimes, faSync: faSync, faLanguage: faLanguage, faCog: faCog, faDownload: faDownload, faUpload: faUpload, faFolder: faFolder}
  
  ngOnInit() {
    this.service.userProfile$.subscribe(val=> {
      this.userProfile = val;
    })
    this.getUserFolders$ = this.getUserFolders.pipe(
      switchMap((action) => {
        return this.service.getUserFolders().pipe(map(val=>{
          switch(action) {
            case "input":
            case "output":
              var nodeId = this.userProfile.id+"/"+action;
              this.selectedNodeId = nodeId;
              this.selectChildNode(nodeId);
              break;
            default:
              this.resetTreeNode();
          }
          return val;
        }));
      })
    );

    const combineFolderLanguge$ = combineLatest(this.getUserFolders$,this.service.languages$).pipe(switchMap(([userFolders,languages])=>{
      userFolders.map(root=> 
         root.children.map(folder => folder.files.map(file=> {
          file.lanague = languages.filter(val=>val.code == file.languageCode)[0].name;
          return file;
        }))
      )
      this.translationLanguages = languages;
      this.folderTreeSource.data = userFolders;
      return userFolders;
    }));

    combineFolderLanguge$.subscribe();
    this.getUserFolders.next();
    // this.getUserFolders$.subscribe();
    // this.getUserFolders.next();
    // this.service.languages$.subscribe(val=>{
    //   this.translationLanguages = val;
    // });
  }
  resetTreeNode() {
    this.inputFiles = [];
    this.selectedNodeId = "";
    this.selectedFolder = "";
    this.treeControl.collapseAll();
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
        this.progress = 0;
        this.getUserFolders.next("input");
      }
    });
  }
  onSelectNode(node:FileFlatNode) {
    this.selectedNodeId = node.id;
    if(node.level == 0) {
      this.userFolders = this.getNodeChildren(node)[0].children;
      this.selectedFolder = "";
    } else {
      this.selectChildNode(node.id);
    }
  }
  onClickCheckboxFile(event: MouseEvent,fileId: number) {
    event.preventDefault();
    event.stopPropagation();
    this.onSelectFile(fileId);
  }
  onClickCheckboxAllFile(event: MatCheckboxChange) {
    this.allFileChecked = event.checked;
    this.selectedFileIds = event.checked ? this.inputFiles.map(val=> val.id) : [];
  }
  onSelectFile(fileId: number) {
    if(this.selectedFileIds.indexOf(fileId) == -1) 
      this.selectedFileIds = [...this.selectedFileIds,fileId];
     else 
      this.selectedFileIds = this.selectedFileIds.filter(val=>val!=fileId);
    
    this.allFileChecked = this.inputFiles.length == this.selectedFileIds.length
  }
  selectChildNode(nodeId:string) {
    var nodeIdSpit = nodeId.split("/");
    var parentNodeId = nodeIdSpit[0];
    var thisFolder = this.folderTreeSource.data.filter(x=>x.id == parentNodeId);
    var thisFile = thisFolder[0].children.filter(x=> x.id == nodeId);
    this.inputFiles = thisFile[0].files;
    var nodeIndex = this.treeControl.dataNodes.findIndex((val)=>val.id == parentNodeId);
    this.treeControl.expand(this.treeControl.dataNodes[nodeIndex]);
  }
  onClickItemFolder(nodeId:string) {
    this.selectedFolder = nodeId;
  }
  onDbclickItemFolder(nodeId:string) {
    if(nodeId != null) {
      this.selectedNodeId = nodeId;
      this.selectChildNode(nodeId);
    }
  }
  getNodeChildren(node:FileFlatNode) {
    return this.folderTreeSource.data.filter((val)=> val.id == node.id)
  }

  onTranslate() {
    this.service.startTranslate().subscribe();
  }
  onDownLoadFiles() {
    this.service.userProfile$.subscribe(val=> {
      console.log(val);
    })
  }
}
