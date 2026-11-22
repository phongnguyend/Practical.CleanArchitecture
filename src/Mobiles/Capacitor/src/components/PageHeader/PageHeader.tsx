import type { ReactNode } from "react";
import ActionIcon, { type ActionIconName } from "../ActionIcon/ActionIcon";
import ActionGroup from "../ActionGroup/ActionGroup";
export default function PageHeader({
  title,
  icon,
  onRefresh,
  children,
}: {
  title: string;
  icon: ActionIconName;
  onRefresh?: () => unknown;
  children?: ReactNode;
}) {
  return (
    <div className="card-header mobile-page-header">
      <div className="page-heading">
        <h1>
          <ActionIcon action={icon} />
          {title}
        </h1>
        {onRefresh && (
          <button
            type="button"
            className="btn btn-secondary"
            onClick={() => void onRefresh()}
          >
            <ActionIcon action="refresh" />
            Refresh
          </button>
        )}
      </div>
      {children && <ActionGroup>{children}</ActionGroup>}
    </div>
  );
}
