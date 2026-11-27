import { Pagination as BootstrapPagination } from "react-bootstrap";
import ActionIcon from "../ActionIcon/ActionIcon";

interface PaginationProps {
  totalItems: number;
  currentPage: number;
  pageSize: number;
  pageSelected: (page: number) => void;
}

const Pagination = (props: PaginationProps) => {
  const { totalItems, currentPage, pageSize } = props;
  const totalPages = Math.ceil(totalItems / pageSize);

  const pageNumbers: Array<number> = [];
  let startIndex = currentPage - 2;
  let endIndex = currentPage + 2;

  if (startIndex < 1) {
    endIndex = endIndex + (1 - startIndex);
    startIndex = 1;
  }

  if (endIndex > totalPages) {
    startIndex = startIndex - (endIndex - totalPages);
    endIndex = totalPages;
  }

  startIndex = Math.max(startIndex, 1);
  endIndex = Math.min(endIndex, totalPages);

  for (let i = startIndex; i <= endIndex; i++) {
    pageNumbers.push(i);
  }

  const pageSelected = (page: number) => {
    if (page < 1 || page > totalPages || page === currentPage) {
      return;
    }
    props.pageSelected(page);
  };

  const pageItems = pageNumbers.map((index) => (
    <BootstrapPagination.Item
      key={index}
      active={currentPage === index}
      onClick={() => pageSelected(index)}
    >
      {index}
    </BootstrapPagination.Item>
  ));

  return (
    <BootstrapPagination>
      <BootstrapPagination.Item disabled={currentPage <= 1} onClick={() => pageSelected(1)} aria-label="First page"><ActionIcon action="first" className="" /><span className="page-nav-label">First</span></BootstrapPagination.Item>
      <BootstrapPagination.Item disabled={currentPage <= 1} onClick={() => pageSelected(currentPage - 1)} aria-label="Previous page"><ActionIcon action="previous" className="" /><span className="page-nav-label">Previous</span></BootstrapPagination.Item>
      {pageItems}
      <BootstrapPagination.Item disabled={currentPage >= totalPages} onClick={() => pageSelected(currentPage + 1)} aria-label="Next page"><ActionIcon action="next" className="" /><span className="page-nav-label">Next</span></BootstrapPagination.Item>
      <BootstrapPagination.Item disabled={currentPage >= totalPages} onClick={() => pageSelected(totalPages)} aria-label="Last page"><ActionIcon action="last" className="" /><span className="page-nav-label">Last</span></BootstrapPagination.Item>
    </BootstrapPagination>
  );
};

export default Pagination;
