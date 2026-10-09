using System;

namespace Remp.Common.Exceptions;

public abstract class AppException : Exception
{
    public int StatusCode {get;}
    protected AppException(string message, int statusCode): base(message)
    {
        StatusCode = statusCode;
    }
}
