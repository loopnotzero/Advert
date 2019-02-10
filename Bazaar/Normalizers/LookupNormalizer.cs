using Microsoft.AspNetCore.Identity;

namespace Bazaar.Normalizers
{
    public static class LookupNormalizer
    {
        public static string NormalizeKey(this ILookupNormalizer keyNormalizer, string key)
        {
            return keyNormalizer != null ? keyNormalizer.Normalize(key) : key;
        }
    }
}