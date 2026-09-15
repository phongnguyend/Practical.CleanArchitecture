import { ChangeDetectionStrategy, Component, Input } from "@angular/core";
import { NgIcon } from "@ng-icons/core";

export type ActionIconName = keyof typeof iconNames;

const iconNames = {
  add: "lucidePlus",
  audit: "lucideClipboardList",
  back: "lucideChevronLeft",
  cancel: "lucideX",
  close: "lucideX",
  confirm: "lucideCheck",
  delete: "lucideTrash2",
  download: "lucideDownload",
  edit: "lucidePencil",
  export: "lucideFileDown",
  external: "lucideExternalLink",
  files: "lucideFiles",
  first: "lucideChevronsLeft",
  history: "lucideHistory",
  home: "lucideHouse",
  import: "lucideFileUp",
  key: "lucideKeyRound",
  last: "lucideChevronsRight",
  login: "lucideLogIn",
  logout: "lucideLogOut",
  mail: "lucideMail",
  next: "lucideChevronRight",
  previous: "lucideChevronLeft",
  products: "lucidePackage",
  save: "lucideSave",
  settings: "lucideSettings",
  show: "lucideEye",
  hide: "lucideEyeOff",
  upload: "lucideUpload",
  users: "lucideUsers",
  view: "lucideEye",
} as const;

@Component({
  selector: "app-action-icon",
  standalone: true,
  imports: [NgIcon],
  template: `<ng-icon [name]="iconName" size="16" class="me-1" aria-hidden="true" />`,
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ActionIconComponent {
  @Input({ required: true }) action: ActionIconName;

  get iconName(): (typeof iconNames)[ActionIconName] {
    return iconNames[this.action];
  }
}
