// EMS.Services/Helpers/ResponseHelper.cs
using System.Collections.Generic;

namespace EMS.Services.Helpers
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }
        public List<string> Errors { get; set; } = new List<string>();
        public int StatusCode { get; set; }
    }

    public class PaginatedResponse<T>
    {
        public List<T> Data { get; set; } = new List<T>();
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
    }

    public static class ResponseHelper
    {
        public static ApiResponse<T> SuccessResponse<T>(T data, string message = "Success", int statusCode = 200)
        {
            return new ApiResponse<T>
            {
                Success = true,
                Message = message,
                Data = data,
                StatusCode = statusCode,
                Errors = new List<string>()
            };
        }

        public static ApiResponse<object> ErrorResponse(string message, List<string> errors = null, int statusCode = 400)
        {
            return new ApiResponse<object>
            {
                Success = false,
                Message = message,
                Data = new object(),
                StatusCode = statusCode,
                Errors = errors ?? new List<string>()
            };
        }

        public static ApiResponse<object> ErrorResponse(string message, string error, int statusCode = 400)
        {
            return new ApiResponse<object>
            {
                Success = false,
                Message = message,
                Data = new object(),
                StatusCode = statusCode,
                Errors = new List<string> { error }
            };
        }

        public static ApiResponse<object> UnauthorizedResponse(string message = "Unauthorized")
        {
            return new ApiResponse<object>
            {
                Success = false,
                Message = message,
                StatusCode = 401,
                Data = new object(),
                Errors = new List<string> { "Invalid token" }
            };
        }

        public static ApiResponse<object> NotFoundResponse(string message = "Resource not found")
        {
            return new ApiResponse<object>
            {
                Success = false,
                Message = message,
                StatusCode = 404,
                Data = new object(),
                Errors = new List<string> { "Not found" }
            };
        }

        public static PaginatedResponse<T> PaginatedResponse<T>(List<T> data, int totalCount, int pageNumber, int pageSize)
        {
            var totalPages = (totalCount + pageSize - 1) / pageSize;
            
            return new PaginatedResponse<T>
            {
                Data = data,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalPages = totalPages
            };
        }
    }
}