using System;

namespace Remp.Common.Exceptions;

public class ForbiddenException : AppException
{
    public ForbiddenException(string message) : base(message, 403)
    {
        
    }
}
