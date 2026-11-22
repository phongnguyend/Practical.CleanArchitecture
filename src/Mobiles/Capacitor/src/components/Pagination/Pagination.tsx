import ActionIcon from "../ActionIcon/ActionIcon";
interface Props {
  totalItems: number;
  currentPage: number;
  pageSize: number;
  pageSelected: (page: number) => void;
}
export default function Pagination({
  totalItems,
  currentPage,
  pageSize,
  pageSelected,
}: Props) {
  return (
    <div className="mobile-pagination">
      <p>
        Page {currentPage} &middot; {totalItems} records
      </p>
      <div>
        <button
          className="btn btn-secondary"
          disabled={currentPage <= 1}
          onClick={() => pageSelected(currentPage - 1)}
        >
          <ActionIcon action="previous" />
          Previous
        </button>
        <button
          className="btn btn-secondary"
          disabled={currentPage * pageSize >= totalItems}
          onClick={() => pageSelected(currentPage + 1)}
        >
          <ActionIcon action="next" />
          Next
        </button>
      </div>
    </div>
  );
}
