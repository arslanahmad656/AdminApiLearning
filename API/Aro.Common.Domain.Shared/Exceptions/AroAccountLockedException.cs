namespace Aro.Common.Domain.Shared.Exceptions;

[Serializable]
public class AroAccountLockedException : AroException
{
    public DateTime LockoutEnd { get; }

    public TimeSpan RemainingLockoutTime => LockoutEnd > DateTime.UtcNow
        ? LockoutEnd - DateTime.UtcNow
        : TimeSpan.Zero;

    public AroAccountLockedException(string errorCode, string message, DateTime lockoutEnd)
        : base(errorCode, message)
    {
        LockoutEnd = lockoutEnd;
    }

    public AroAccountLockedException(string errorCode, string message, DateTime lockoutEnd, Exception? innerException)
        : base(errorCode, message, innerException)
    {
        LockoutEnd = lockoutEnd;
    }
}
