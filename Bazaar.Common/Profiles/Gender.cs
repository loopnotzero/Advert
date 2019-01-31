using System;

namespace Bazaar.Common.Profiles
{
    public enum Gender
    {
        NotSpecified,
        Male,
        Female,
    }
    
    public static class GenderHumanizer {
        public static string Humanize(this Gender gender)
        {
            switch (gender)
            {
                case Gender.NotSpecified:
                    return "Not specified";
                case Gender.Male:
                    return "Male";
                case Gender.Female:
                    return "Female";
                default:
                    throw new ArgumentOutOfRangeException(nameof(gender), gender, null);
            }
        }
    }
}