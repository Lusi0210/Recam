using System;

namespace Remp.Common.Exceptions;

public abstract class AppException : Exception
{
    public int StatusCode {get;}
    public List<string>? Errors { get; }
    protected AppException(string message, int statusCode,List<string>? errors = null): base(message)
    {
        StatusCode = statusCode;
        Errors = errors;
    }
}
