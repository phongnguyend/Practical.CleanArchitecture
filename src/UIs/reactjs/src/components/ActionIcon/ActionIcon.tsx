import {
  Check,
  ChevronLeft,
  ChevronRight,
  ChevronsLeft,
  ChevronsRight,
  ClipboardList,
  Download,
  ExternalLink,
  Eye,
  EyeOff,
  FileDown,
  FileUp,
  Files,
  History,
  House,
  KeyRound,
  LogIn,
  LogOut,
  Mail,
  Package,
  Pencil,
  Plus,
  Save,
  Settings,
  Trash2,
  Upload,
  Users,
  X,
  type LucideIcon,
} from "lucide-react";

export type ActionIconName = keyof typeof icons;

const icons = {
  add: Plus,
  audit: ClipboardList,
  back: ChevronLeft,
  cancel: X,
  close: X,
  confirm: Check,
  delete: Trash2,
  download: Download,
  edit: Pencil,
  export: FileDown,
  external: ExternalLink,
  files: Files,
  first: ChevronsLeft,
  history: History,
  home: House,
  import: FileUp,
  key: KeyRound,
  last: ChevronsRight,
  login: LogIn,
  logout: LogOut,
  mail: Mail,
  next: ChevronRight,
  previous: ChevronLeft,
  products: Package,
  save: Save,
  settings: Settings,
  show: Eye,
  hide: EyeOff,
  upload: Upload,
  users: Users,
  view: Eye,
} satisfies Record<string, LucideIcon>;

interface ActionIconProps {
  action: ActionIconName;
  className?: string;
}

const ActionIcon = ({ action, className = "me-1" }: ActionIconProps) => {
  const Icon = icons[action];
  return <Icon size={16} className={className} aria-hidden="true" />;
};

export default ActionIcon;
