import { HttpClient, HttpEventType } from "@angular/common/http";
import { Component, OnInit } from "@angular/core";
import { combineLatest, Observable, Subject } from "rxjs";
import { delay, map, switchMap, tap, timeout } from "rxjs/operators";
import { FileManagement, Languge, TranslationQueue } from "../models/file.model";
import { ApiService } from "../services/api.service";
import { faChevronLeft, faChevronRight, faMinus, faTimes, faSync, faLanguage, faDownload, faUpload, faFolder } from '@fortawesome/free-solid-svg-icons';
import { FlatTreeControl } from "@angular/cdk/tree";
import { MatCheckboxChange, MatDialog, MatTreeFlatDataSource, MatTreeFlattener } from "@angular/material";
import { UserFolder } from "../models/userFolder.model";
import { UserProfile } from "../models/login.model";
import { saveAs } from 'file-saver';
import { TranslationConfigComponent } from "./translation-config/translation-config.component";
import { FormControl } from "@angular/forms";
import { TranslationProcessComponent } from "./translation-process/translation-process.component";

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
  displayedColumns = ['id', 'originalFileName', 'languageCode', 'createdDate'];

  public progress: number = 0;
  public inputFiles: FileManagement[];
  public selectedNodeId: string = "";
  public selectedFolder: string = "";
  public selectedFileIds: FileManagement[] = [];
  public allFileChecked: boolean = false;
  public userFolders: UserFolder[] = [];
  public translationLanguages: Languge[] = [];
  private userProfile: UserProfile;
  public translationQueues: TranslationQueue[] = [];

  constructor(private service: ApiService, public dialog: MatDialog) {
  }
  hasChild = (_: number, node: FileFlatNode) => node.expandable;
  getUserFolders: Subject<string> = new Subject<string>();
  getUserFolders$: Observable<UserFolder[]>;
  awesomeIcon = { faFolder: faFolder}
  

   menu = {
    back: {
      disabled: true,
      icon: faChevronLeft
    },
    forward: {
      disabled: true,
      icon: faChevronRight
    },
    refresh: {
      disabled: false,
      icon: faSync
    },
    upload: {
      disabled: false,
      icon: faUpload
    },
    translate: {
      disabled: true,
      icon: faLanguage
    },
    download: {
      disabled: true,
      icon: faDownload
    },
    close: {
      disable: false,
      icon: faTimes
    }
  }

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

    const combineFolderLanguage$ = combineLatest(this.getUserFolders$,this.service.languages$).pipe(switchMap(([userFolders,languages])=>{
      userFolders.map(root=> 
         root.children.map(folder => folder.files.map(file=> {
          file.language = languages.filter(val=>val.code == file.languageCode)[0].name;
          return file;
        }))
      )
      this.translationLanguages = languages;
      this.folderTreeSource.data = userFolders;
      return userFolders;
    }));

    combineFolderLanguage$.subscribe();
    this.getUserFolders.next();

  }
  
  resetTreeNode() {
    this.inputFiles = [];
    this.selectedNodeId = "";
    this.selectedFolder = "";
    this.treeControl.collapseAll();
  }
  onDownLoadFiles() {
    if(this.selectedFileIds.length > 0) {
      var selectFileIds = this.selectedFileIds.map(file=>file.id)
      this.service.downloadSRT(selectFileIds).subscribe(data => {
        saveAs(data,'srt_files.zip');
      });
    }   
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
  updateMenuButtonState() {
    var disabled = this.selectedFileIds.length == 0;
    this.menu.translate.disabled = disabled;
  }
  onClickCheckboxFile(event: MouseEvent,fileId: FileManagement) {
    event.preventDefault();
    event.stopPropagation();
    this.onSelectFile(fileId);
  }
  onClickCheckboxAllFile(event: MatCheckboxChange) {
    this.allFileChecked = event.checked;
    this.selectedFileIds = event.checked ? Object.assign([], this.inputFiles)  : [];

    this.updateMenuButtonState();
  }
  onSelectFile(fileId: FileManagement) {
    this.selectedFileIds = this.selectedFileIds.indexOf(fileId) == -1 ? [...this.selectedFileIds,fileId] : this.selectedFileIds.filter(val=>val!=fileId); 
    this.allFileChecked = this.inputFiles.length == this.selectedFileIds.length;
    this.updateMenuButtonState();

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

  languageCodeToName(languageCode: string) {
    return this.translationLanguages.filter(val=>val.code == languageCode)[0].name
  }
  
  openTranslateConfigDialog(): void {
    const dialogRef = this.dialog.open(TranslationConfigComponent, {
      width: '350px',
      data: {title: "Choose languages", translationLanguages: this.translationLanguages, defaultLanguages:["zh-CN","zh-TW"]}
    });
    dialogRef.afterClosed().subscribe(result => {
      if(result && result.length > 0) {
        this.translationQueues = [];
        result.forEach((lang:string)=>{
          var languageName = this.languageCodeToName(lang);
          this.selectedFileIds.forEach(fileId => {
            this.translationQueues.push({
              fileId: fileId.id,
              originalFileName: fileId.originalFileName,
              from: fileId.language,
              to: languageName,
              toCode: lang,
              state: "pending",
            })
          })
        })
        this.openTranslateProcessDialog();
      }
    });
  }
  
  
  openTranslateProcessDialog(): void {
    const dialogRef = this.dialog.open(TranslationProcessComponent, {
      width: '750px',
      data: {title: "Translating...", translationQueues: this.translationQueues}
    });
    dialogRef.afterClosed().subscribe(result => {
      if(result && result.length > 0) {
        console.log('The dialog was closed',result)
      }
    });
  }
}
