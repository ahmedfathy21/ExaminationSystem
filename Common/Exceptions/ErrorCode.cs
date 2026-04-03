namespace ExaminationSystem.Common.Exceptions;

public enum ErrorCode
{
// ── 4xx Client Errors ────────────────────────────────────────
    BadRequest          = 400,
    Unauthorized        = 401,
    Forbidden           = 403,
    NotFound            = 404,
    Conflict            = 409,
    ValidationError     = 422,
 
    // ── 5xx Server Errors ────────────────────────────────────────
    InternalServerError = 500
}
