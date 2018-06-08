using System;

namespace Egghead.Common
{
    [Flags]
    public enum ResponseStatusCode
    {
        //Email
        ThatEmailIsTaken = 0,
        EmailValidationError = 1,
        CouldNotFindYourEmail = 2,

        //Password
        PasswordDidNotMatch = 10,           
        PasswordValidationError = 11,
       
        //Account
        CouldNotAthorizeYourAccount = 20,
        CouldNotRegisterYourAccount = 21
    }
}