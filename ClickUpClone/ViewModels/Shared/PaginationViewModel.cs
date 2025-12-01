namespace ClickUpClone.ViewModels.Shared
{
    public class PaginationViewModel
    {
        public int CurrentPage { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int TotalCount { get; set; }

        public int TotalPages => TotalCount > 0 ? (int)Math.Ceiling((double)TotalCount / PageSize) : 1;
        public bool HasPreviousPage => CurrentPage > 1;
        public bool HasNextPage => CurrentPage < TotalPages;

        public string GetPageUrl(int page, string baseUrl)
        {
            var separator = baseUrl.Contains("?") ? "&" : "?";
            return $"{baseUrl}{separator}page={page}&pageSize={PageSize}";
        }

        public IEnumerable<int> GetPageNumbers(int windowSize = 5)
        {
            var startPage = Math.Max(1, CurrentPage - (windowSize / 2));
            var endPage = Math.Min(TotalPages, startPage + windowSize - 1);

            // Adjust if we're near the end
            if (endPage - startPage + 1 < windowSize)
            {
                startPage = Math.Max(1, endPage - windowSize + 1);
            }

            return Enumerable.Range(startPage, endPage - startPage + 1);
        }
    }

    public class FilterViewModel
    {
        public string? SearchTerm { get; set; }
        public string? Status { get; set; }
        public string? Priority { get; set; }
        public string? AssignedTo { get; set; }
        public DateTime? DueDateFrom { get; set; }
        public DateTime? DueDateTo { get; set; }
        public string? SortBy { get; set; } = "CreatedAt";
        public bool SortDescending { get; set; } = true;

        public Dictionary<string, string> ToQueryString()
        {
            var dict = new Dictionary<string, string>();

            if (!string.IsNullOrEmpty(SearchTerm))
                dict["search"] = SearchTerm;
            
            if (!string.IsNullOrEmpty(Status))
                dict["status"] = Status;
            
            if (!string.IsNullOrEmpty(Priority))
                dict["priority"] = Priority;
            
            if (!string.IsNullOrEmpty(AssignedTo))
                dict["assignedTo"] = AssignedTo;
            
            if (DueDateFrom.HasValue)
                dict["dueDateFrom"] = DueDateFrom.Value.ToString("yyyy-MM-dd");
            
            if (DueDateTo.HasValue)
                dict["dueDateTo"] = DueDateTo.Value.ToString("yyyy-MM-dd");
            
            if (!string.IsNullOrEmpty(SortBy))
                dict["sortBy"] = SortBy;
            
            if (SortDescending)
                dict["sortDesc"] = "true";

            return dict;
        }
    }
}
