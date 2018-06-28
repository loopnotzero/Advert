using System;

namespace Egghead.Utils
{
    public static class AccountValidation
    {
        public static bool IsEmailSyntacticallyValid(string email)
        {
            //todo: Add regex validation
            return !string.IsNullOrEmpty(email);
        }

        public static bool IsPasswordSyntacticallyValid(string password)
        {
            //todo: Add char, symbol, number validation
            return !string.IsNullOrEmpty(password);
        }

        public static bool IsFisrtNameValid(string firstName)
        {
            return !string.IsNullOrEmpty(firstName);
        }

        public static bool IsLastNameValid(string lastName)
        {
            return !string.IsNullOrEmpty(lastName);
        }
    }
}