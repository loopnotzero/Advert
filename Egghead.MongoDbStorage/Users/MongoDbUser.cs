using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Egghead.MongoDbStorage.Users
{
    [BsonIgnoreExtraElements]
    public class MongoDbUser : IEquatable<MongoDbUser>
    {
        public MongoDbUser()
        {
            Id = ObjectId.GenerateNewId().ToString();
        }

        public MongoDbUser(string userName, string normalizedUserName) : this()
        {
            UserName = userName;
            NormalizedUserName = normalizedUserName;
        }

        public string Id { get; set; }
        public string UserName { get; set; }
        public string NormalizedUserName { get; set; }
        public string FirstName { get; set; }
        public string NormalizedFirstName { get; set; }
        public string LastName { get; set; }
        public string NormalizedLastName { get; set; }
        public string Email { get; set; }
        public string NormalizedEmail { get; set; }
        public bool EmailConfirmed { get; set; }
        public string PasswordHash { get; set; }
        public string SecurityStamp { get; set; }
        public string ConcurrencyStamp { get; set; }
        public string PhoneNumber { get; set; }
        public bool PhoneNumberConfirmed { get; set; }
        public bool TwoFactorEnabled { get; set; }
        public DateTimeOffset? LockoutEnd { get; set; }
        public bool LockoutEnabled { get; set; }
        public int AccessFailedCount { get; set; }
        
        public bool Equals(MongoDbUser other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(Id, other.Id) && string.Equals(UserName, other.UserName) && string.Equals(NormalizedUserName, other.NormalizedUserName) && string.Equals(Email, other.Email) && string.Equals(NormalizedEmail, other.NormalizedEmail) && EmailConfirmed == other.EmailConfirmed && string.Equals(PasswordHash, other.PasswordHash) && string.Equals(SecurityStamp, other.SecurityStamp) && string.Equals(ConcurrencyStamp, other.ConcurrencyStamp) && string.Equals(PhoneNumber, other.PhoneNumber) && PhoneNumberConfirmed == other.PhoneNumberConfirmed && TwoFactorEnabled == other.TwoFactorEnabled && LockoutEnd.Equals(other.LockoutEnd) && LockoutEnabled == other.LockoutEnabled && AccessFailedCount == other.AccessFailedCount;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((MongoDbUser) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (Id != null ? Id.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (UserName != null ? UserName.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (NormalizedUserName != null ? NormalizedUserName.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (FirstName != null ? FirstName.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (NormalizedFirstName != null ? NormalizedFirstName.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (LastName != null ? LastName.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (NormalizedLastName != null ? NormalizedLastName.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Email != null ? Email.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (NormalizedEmail != null ? NormalizedEmail.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ EmailConfirmed.GetHashCode();
                hashCode = (hashCode * 397) ^ (PasswordHash != null ? PasswordHash.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (SecurityStamp != null ? SecurityStamp.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (ConcurrencyStamp != null ? ConcurrencyStamp.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (PhoneNumber != null ? PhoneNumber.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ PhoneNumberConfirmed.GetHashCode();
                hashCode = (hashCode * 397) ^ TwoFactorEnabled.GetHashCode();
                hashCode = (hashCode * 397) ^ LockoutEnd.GetHashCode();
                hashCode = (hashCode * 397) ^ LockoutEnabled.GetHashCode();
                hashCode = (hashCode * 397) ^ AccessFailedCount;
                return hashCode;
            }
        }
    }
}