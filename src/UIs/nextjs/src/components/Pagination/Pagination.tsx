import { Pagination as BootstrapPagination } from "react-bootstrap";

const Pagination = (props) => {
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
      <BootstrapPagination.First disabled={currentPage === 1} onClick={() => pageSelected(1)} />
      <BootstrapPagination.Prev
        disabled={currentPage === 1}
        onClick={() => pageSelected(currentPage - 1)}
      />
      {pageItems}
      <BootstrapPagination.Next
        disabled={currentPage === totalPages}
        onClick={() => pageSelected(currentPage + 1)}
      />
      <BootstrapPagination.Last
        disabled={currentPage === totalPages}
        onClick={() => pageSelected(totalPages)}
      />
    </BootstrapPagination>
  );
};

export default Pagination;
