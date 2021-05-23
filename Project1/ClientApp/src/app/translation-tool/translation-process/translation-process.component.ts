import { Component, Inject, OnDestroy, OnInit } from "@angular/core";
import { FormControl } from "@angular/forms";
import { MatDialogRef, MAT_DIALOG_DATA } from "@angular/material";
import { TranslationQueue } from "src/app/models/file.model";
import { faCheck } from "@fortawesome/free-solid-svg-icons";
import { Observable, Subject } from "rxjs";
import { map, switchMap, takeUntil, tap } from "rxjs/operators";
import { ApiService } from "src/app/services/api.service";

export interface DialogData {
  translationQueues: TranslationQueue[];
  title: string;
}

@Component({
  selector: "app-translation-process",
  templateUrl: "./translation-process.component.html",
  styleUrls: ["./translation-process.component.css"],
})
export class TranslationProcessComponent implements OnInit, OnDestroy {
  faCheck = faCheck;
  progress = 0;
  private queueSubject = new Subject<number>();
  ngUnscubscribe: Subject<void> = new Subject<void>();
  queueSubject$: Observable<TranslationQueue>;
  constructor(
    public dialogRef: MatDialogRef<TranslationProcessComponent>,
    @Inject(MAT_DIALOG_DATA) public data: DialogData,
    private service: ApiService
  ) {
    dialogRef.disableClose = true;
  }
  ngOnDestroy(): void {
    this.ngUnscubscribe.next();
    this.ngUnscubscribe.complete();
  }
  ngOnInit(): void {
    this.queueSubject$ = this.queueSubject.pipe(takeUntil(this.ngUnscubscribe),switchMap(queueIndex=>{
      this.data.translationQueues[queueIndex].state = 'processing';
      return this.service.startTranslate(this.data.translationQueues[queueIndex]).pipe(tap(rs => {
        this.calculateProgress(queueIndex);
        this.data.translationQueues[queueIndex].state = 'complete';
        if(queueIndex < this.data.translationQueues.length -1) {
          this.queueSubject.next(queueIndex + 1);
        } else {
          this.data.title = 'Complete';
        }
      }))
    }));
    this.queueSubject$.subscribe();
    this.queueSubject.next(0);
  }
  calculateProgress(queueIndex: number) {
    this.progress = Math.round((100 * (queueIndex + 1)) / this.data.translationQueues.length);
  }
  onNoClick(): void {
    this.dialogRef.close();
  }
}
