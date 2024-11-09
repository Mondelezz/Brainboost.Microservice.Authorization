using System.Runtime.CompilerServices;

namespace Application.Common.Exceptions;
public class EntityNotFoundException : Exception
{
    public EntityNotFoundException(long id, [CallerArgumentExpression(nameof(id))] string paramName = "")
        : base($"Entity {paramName}: {id} not found.")
    {
    }

    public EntityNotFoundException(string? id, [CallerArgumentExpression(nameof(id))] string paramName = "")
        : base($"Entity {paramName}: {id} not found")
    {
    }

    public EntityNotFoundException(
        long id,
        long id2,
        [CallerArgumentExpression(nameof(id))] string paramName = "",
        [CallerArgumentExpression(nameof(id2))] string paramName2 = "")
        : base($"Entity {paramName}: {id} {paramName2}: {id2} not found.")
    {
    }

    public EntityNotFoundException(
        long id,
        long id2,
        long id3,
        [CallerArgumentExpression(nameof(id))] string paramName = "",
        [CallerArgumentExpression(nameof(id2))] string paramName2 = "",
        [CallerArgumentExpression(nameof(id3))] string paramName3 = "")
        : base($"Entity {paramName}: {id} {paramName2}: {id2} {paramName3}: {id3} not found.")
    {
    }

    public EntityNotFoundException(string? message, Exception? innerException)
        : base(message, innerException)
    {
    }

    public EntityNotFoundException()
        : base()
    {
    }
}
