namespace Aro.UI.Application.Exceptions;

public class AccountLockedException : Exception
{
    public DateTime? LockoutEnd { get; }

    public int RemainingMinutes => LockoutEnd.HasValue && LockoutEnd.Value > DateTime.UtcNow
        ? (int)Math.Ceiling((LockoutEnd.Value - DateTime.UtcNow).TotalMinutes)
        : 0;

    public AccountLockedException(string message, DateTime? lockoutEnd = null)
        : base(message)
    {
        LockoutEnd = lockoutEnd;
    }

    public AccountLockedException(string message, DateTime? lockoutEnd, Exception? innerException)
        : base(message, innerException)
    {
        LockoutEnd = lockoutEnd;
    }
}
