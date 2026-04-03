using Microsoft.AspNetCore.Http.HttpResults;
using ExaminationSystem.Common.Exceptions;
namespace ExaminationSystem.Common.Exceptions;

public class AppException : Exception
{
    public ErrorCode StatusCode { get;  }
    public AppException(string message, ErrorCode statusCode) : base(message)
    {
        StatusCode = statusCode;
    }
}

public class NotFoundException : AppException
{
    public NotFoundException(string resource , object id) : base($"{resource} with id {id} not found", ErrorCode.NotFound) { }

    public NotFoundException(string message) : base(message, ErrorCode.NotFound)
    {
    }
    
}
public class UnauthorizedException : AppException
{
    public UnauthorizedException(string message = "Unauthorized.")
        : base(message, ErrorCode.Unauthorized) { }
}
public class ForbiddenException : AppException
{
    public ForbiddenException(string message = "You do not have permission to perform this action.")
        : base(message, ErrorCode.Forbidden) { }
}

public class ConflictException : AppException
{
    public ConflictException(string message = "Conflict.")
        : base(message, ErrorCode.Conflict) { }
}

public class ValidationException : AppException
{
    public List<string> ValidationErrors { get; }

    public ValidationException(List<string> errors) : base("Validation failed.", ErrorCode.ValidationError)
    {
        ValidationErrors = errors;
    }
}

