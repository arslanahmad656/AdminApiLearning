﻿namespace Aro.Admin.Domain.Shared.Exceptions;

public class AroUnauthorizedException(string errorCode, string message, Exception? innerException = null) : AroException(errorCode, message, innerException)
{
}
