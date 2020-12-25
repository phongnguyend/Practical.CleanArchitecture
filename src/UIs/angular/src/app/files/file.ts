export interface IFile {
  id: string;
  name: string;
  description: string;
  size: number;
  uploadedTime: Date;
  fileName: string;
  formFile: File;
  encrypted: boolean;
}
