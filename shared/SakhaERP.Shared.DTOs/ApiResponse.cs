namespace SakhaERP.Shared.DTOs;
public record ApiResponse<T>(bool Success, T? Data, IReadOnlyList<ApiError>? Errors = null)
{
    public static ApiResponse<T> Ok(T data) => new(true, data);
    public static ApiResponse<T> Fail(params ApiError[] errors) => new(false, default, errors);
}

public record ApiError(string Type, string Title, int Status, string Detail, string? Field = null);
