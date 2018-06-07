using System;

namespace Egghead.Common
{
    [Flags]
    public enum ResponseStatusCode
    {
        EmailNotFound = 0,
        PasswordDoNotMatch = 1,
        AuthorizationFailed = 2,
        EmailValidationFailed = 4,
        PasswordValidationFailed = 8
    }
}