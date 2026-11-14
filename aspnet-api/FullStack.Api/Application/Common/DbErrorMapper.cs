using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace FullStack.Api.Application.Common;

public static class DbErrorMapper
{
    /// <summary>
    /// Maps common PostgreSQL constraint failures to API-friendly messages.
    /// </summary>
    public static (string Message, int StatusCode)? Map(DbUpdateException ex)
    {
        if (ex.InnerException is not PostgresException pg) return null;

        return pg.SqlState switch
        {
            PostgresErrorCodes.UniqueViolation =>
                ("A record with the same unique value already exists.", 409),
            PostgresErrorCodes.ForeignKeyViolation =>
                ("Related record is missing or still referenced.", 409),
            PostgresErrorCodes.StringDataRightTruncation or PostgresErrorCodes.NumericValueOutOfRange =>
                ("One or more values are invalid or too long.", 400),
            PostgresErrorCodes.NotNullViolation =>
                ("A required field is missing.", 400),
            _ => null
        };
    }
}
