using Microsoft.AspNetCore.Mvc;

namespace ExaminationSystem.Common.Wrappers;

public class ApiResponse<T>
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public T? Data { get; set; }
    public List<string>? Errors { get; set; }
    
    public static ApiResponse<T> Ok(T data) => new() { Success = true, Data = data };
    
    public static ApiResponse<T> Fail(string message, List<string>? errors = null)
        => new() { Success = false, Message = message, Errors = errors };
    
    
}

public class ApiResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public List<string>? Errors { get; set; }
    
    public static ApiResponse Ok() => new() { Success = true ,Message = "Success"};
    
    public static ApiResponse Fail(string message, List<string>? errors = null)
    => new() { Success = false, Message = message, Errors = errors };
}

public class PagedResponse<T>
{
    public bool Success { get; set; } = true;
    public string Message { get; set; } = "Success";
    public IEnumerable<T> Data { get; set; } = Enumerable.Empty<T>();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    public bool HasNextPage => Page < TotalPages;
    public bool HasPreviousPage => Page > 1;
}