"use client";

interface PaginationProps {
    currentPage: number;
    totalPages: number;
    onPageChange: (page: number) => void
}

export function Pagination({currentPage, totalPages, onPageChange}: PaginationProps) {
    if(totalPages <= 1) return null;

    return (
    <div className="mt-4 flex items-center justify-between text-sm">
      <button
        onClick={() => onPageChange(currentPage - 1)}
        disabled={currentPage <= 1}
        className="rounded-md border border-gray-300 px-3 py-1.5 disabled:cursor-not-allowed disabled:opacity-40 hover:bg-gray-50">
        Previous
      </button>
      <span className="text-gray-600">
        Page {currentPage} of {totalPages}
      </span>
      <button
        onClick={() => onPageChange(currentPage + 1)}
        disabled={currentPage >= totalPages}
        className="rounded-md border border-gray-300 px-3 py-1.5 disabled:cursor-not-allowed disabled:opacity-40 hover:bg-gray-50"
      >
        Next
      </button>
    </div>
  );
};