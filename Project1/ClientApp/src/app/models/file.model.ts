export class FileManagement {
  id: number = 0;
  userId: string = "";
  originalFileName: string = "";
  fileName: string = "";
  filePath: string = "";
  fileType: string = "";
  languageCode: string = "";
  language: string = "";
  createdDate: string = "";
  lastUpdated: string = "";
}

export class Languge {
  code: string = "";
  name: string = "";
}

export class TranslationQueue {
  fileId: number = 0;
  originalFileName: string = "";
  from: string = "";
  to: string = "";
  toCode: string = "";
  state: string = "";
}