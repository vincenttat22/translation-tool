import { Component, Inject } from '@angular/core';
import { FormControl } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { Languge } from 'src/app/models/file.model';

export interface DialogData {
  defaultLanguages: string[];
  translationLanguages: Languge[];
  title: string;
}

@Component({
  selector: 'app-translation-config',
  templateUrl: './translation-config.component.html',
  styleUrls: ["./translation-config.component.css"],
})
export class TranslationConfigComponent {

  constructor(
    public dialogRef: MatDialogRef<TranslationConfigComponent>,
    @Inject(MAT_DIALOG_DATA) public data: DialogData) {
      dialogRef.disableClose = true;
    }
    selectedLanguages = new FormControl();
    onNoClick(): void {
      this.dialogRef.close();
    }
    onSelectChange() {
      console.log(this.data.defaultLanguages)
    }
}
