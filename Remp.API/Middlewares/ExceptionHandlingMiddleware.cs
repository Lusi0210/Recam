using System;
using Remp.Common.Exceptions;
using Remp.Common.Responses;

namespace Remp.API.Middlewares;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    // 框架创建中间件时会自动注入这两个参数
    // _next：管道里的"下一步"（后面的中间件，最终到 Controller）
    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    // 每个请求进来都会执行这个方法
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            // 把请求交给后面的环节。后面任何地方抛出的异常都会冒泡到这里
            await _next(context);
        }
        catch (AppException ex)
        {
            // 我们自己抛的业务异常：预期内的错误，记 Warning 就够了
            _logger.LogWarning(ex, "Handled exception: {Message}", ex.Message);
            await WriteErrorResponseAsync(context, ex.StatusCode, ex.Message);
        }
        catch (Exception ex)
        {
            // 没预料到的异常：记 Error，但不把细节返回给前端
            _logger.LogError(ex, "Unhandled exception on {Method} {Path}",
                context.Request.Method, context.Request.Path);
            await WriteErrorResponseAsync(context, StatusCodes.Status500InternalServerError,
                "An unexpected error occurred.");
        }
    }

    // 两个 catch 都要写响应，抽成一个方法，避免重复代码
    private static async Task WriteErrorResponseAsync(HttpContext context, int statusCode, string message)
    {
        context.Response.StatusCode = statusCode;
        var response = ApiResponse<object>.FailureResponse(message);
        await context.Response.WriteAsJsonAsync(response);   // 会自动设置 Content-Type: application/json
    }
}
