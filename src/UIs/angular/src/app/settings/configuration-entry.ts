export interface IConfigurationEntry {
  id: string;
  key: string;
  value: string;
  description: string;
  isSensitive: boolean;
  createdDateTime: Date;
  updatedDateTime?: Date;
}
