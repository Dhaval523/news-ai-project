using System;
using Microsoft.AspNetCore.Mvc;
namespace Server.Helpers ;
public class ApiResponse<T>
{
    public bool Success { get; set; }         // Status: true or false
    public string Message { get; set; }       // Message for client
    public T Data { get; set; }               // Generic data payload

    public ApiResponse() { }

    public ApiResponse(bool success, string message, T data)
    {
        Success = success;
        Message = message;
        Data = data;
    }

    // Static helper methods for clean usage
    public static ApiResponse<T> SuccessResponse(T data, string message = "Request successful")
    {
        return new ApiResponse<T>(true, message, data);
    }

    public static ApiResponse<T> FailureResponse(string message)
    {
        return new ApiResponse<T>(false, message, default!);
    }
}
