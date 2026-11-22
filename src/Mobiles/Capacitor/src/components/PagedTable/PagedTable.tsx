import {
  Children,
  cloneElement,
  isValidElement,
  useState,
  type ReactNode,
  type ReactElement,
} from "react";
import ActionGroup from "../ActionGroup/ActionGroup";
import Pagination from "../Pagination/Pagination";
type Element = ReactElement<{
  children?: ReactNode;
  className?: string;
  "data-label"?: string;
}>;
function text(node: ReactNode): string {
  if (typeof node === "string" || typeof node === "number") {
    return String(node);
  }
  if (isValidElement(node)) {
    return text((node as Element).props.children);
  }
  return Children.toArray(node)
    .map((child) =>
      isValidElement(child)
        ? text((child as Element).props.children)
        : String(child),
    )
    .join(" ");
}
export default function PagedTable({
  children,
  searchable = true,
}: {
  children: ReactNode;
  className?: string;
  searchable?: boolean;
}) {
  const [filter, setFilter] = useState("");
  const [page, setPage] = useState(1);
  const sections = Children.toArray(children).filter(
    isValidElement,
  ) as Element[];
  const header = sections.find((section) => section.type === "thead");
  const body = sections.find((section) => section.type === "tbody");
  const headers = Children.toArray(
    (Children.toArray(header?.props.children)[0] as Element)?.props.children,
  ).map(text);
  const rows = Children.toArray(body?.props.children).filter((row) =>
    text(row).toLowerCase().includes(filter.toLowerCase()),
  );
  const current = Math.min(page, Math.max(1, Math.ceil(rows.length / 10)));
  return (
    <div className="record-list">
      {searchable && (
        <input
          className="form-control mb-3"
          aria-label="Filter records"
          placeholder="Search records"
          value={filter}
          onChange={(event) => {
            setFilter(event.target.value);
            setPage(1);
          }}
        />
      )}
      {rows.length === 0 && <p role="status">No records found</p>}
      <table className="table mobile-records">
        {header}
        <tbody>
          {rows.slice((current - 1) * 10, current * 10).map((row) => {
            if (!isValidElement(row)) {
              return row;
            }
            return cloneElement(
              row as Element,
              {},
              Children.toArray((row as Element).props.children).map(
                (cell, index) => {
                  if (!isValidElement(cell)) {
                    return cell;
                  }
                  const element = cell as Element;
                  const hasActions = Children.toArray(
                    element.props.children,
                  ).some(
                    (child) =>
                      isValidElement(child) &&
                      (child as Element).props.className
                        ?.split(" ")
                        .includes("btn"),
                  );
                  return cloneElement(
                    element,
                    { "data-label": headers[index] },
                    hasActions ? (
                      <ActionGroup>{element.props.children}</ActionGroup>
                    ) : (
                      element.props.children
                    ),
                  );
                },
              ),
            );
          })}
        </tbody>
      </table>
      <Pagination
        totalItems={rows.length}
        currentPage={current}
        pageSize={10}
        pageSelected={setPage}
      />
    </div>
  );
}
