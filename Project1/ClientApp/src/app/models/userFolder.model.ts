import { FileManagement } from "./file.model";

export class UserFolder {
    id: string;
    name: string;
    children?: UserFolder[];
    files: FileManagement[];
  }