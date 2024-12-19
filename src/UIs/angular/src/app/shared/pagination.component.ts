import { Component, EventEmitter, Input, OnChanges, Output } from "@angular/core";

@Component({
    selector: "app-pagination",
    templateUrl: "./pagination.component.html",
    styleUrls: ["./pagination.component.css"],
    standalone: false
})
export class PaginationComponent implements OnChanges {
  pageNumbers: Array<number> = [];
  totalPages: number;
  constructor() {}

  @Input() totalItems: number;
  @Input() currentPage: number;
  @Input() pageSize: number;
  @Output() pageSelected = new EventEmitter<number>();

  selectPage = (page: number) => {
    this.pageSelected.emit(page);
  };

  ngOnChanges(): void {
    this.totalPages = Math.ceil(this.totalItems / this.pageSize);
    let startIndex = this.currentPage - 2;
    let endIndex = this.currentPage + 2;

    if (startIndex < 1) {
      endIndex = endIndex + (1 - startIndex);
      startIndex = 1;
    }

    if (endIndex > this.totalPages) {
      startIndex = startIndex - (endIndex - this.totalPages);
      endIndex = this.totalPages;
    }

    startIndex = Math.max(startIndex, 1);
    endIndex = Math.min(endIndex, this.totalPages);

    this.pageNumbers = [];

    for (let i = startIndex; i <= endIndex; i++) {
      this.pageNumbers.push(i);
    }
  }
}
