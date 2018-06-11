using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Egghead.MongoDbStorage.Identities
{
    [BsonIgnoreExtraElements]
    public class MongoDbIdentityUser : IEquatable<MongoDbIdentityUser>
    {
        public MongoDbIdentityUser()
        {
            Id = ObjectId.GenerateNewId().ToString();
        }
        
        public MongoDbIdentityUser(string userName) : this()
        {
            UserName = userName;
        }
              
        public MongoDbIdentityUser(string userName, 
            string normalizedUserName) : this()
        {
            UserName = userName;
            NormalizedUserName = normalizedUserName;
        }
        
        public MongoDbIdentityUser(string userName, 
            string normalizedUserName,
            string email) : this()
        {
            UserName = userName;
            NormalizedUserName = normalizedUserName;
            Email = email;
        }
        
        public MongoDbIdentityUser(string userName, 
            string normalizedUserName, 
            string email, 
            bool emailConfirmed) : this()
        {
            UserName = userName;
            NormalizedUserName = normalizedUserName;
            Email = email;
            EmailConfirmed = emailConfirmed;
        }
        
        public MongoDbIdentityUser(string userName, 
            string normalizedUserName, 
            string email, 
            bool emailConfirmed, 
            string passwordHash) : this()
        {
            UserName = userName;
            NormalizedUserName = normalizedUserName;
            Email = email;
            EmailConfirmed = emailConfirmed;
            PasswordHash = passwordHash;
        }
        
        public MongoDbIdentityUser(string userName, 
            string normalizedUserName, 
            string email, 
            bool emailConfirmed, 
            string passwordHash, 
            string securityStamp) : this()
        {
            UserName = userName;
            NormalizedUserName = normalizedUserName;
            Email = email;
            EmailConfirmed = emailConfirmed;
            PasswordHash = passwordHash;
            SecurityStamp = securityStamp;
        }
        
        public MongoDbIdentityUser(string userName, 
            string normalizedUserName, 
            string email, 
            bool emailConfirmed, 
            string passwordHash, 
            string securityStamp, 
            string concurrencyStamp) : this()
        {
            UserName = userName;
            NormalizedUserName = normalizedUserName;
            Email = email;
            EmailConfirmed = emailConfirmed;
            PasswordHash = passwordHash;
            SecurityStamp = securityStamp;
            ConcurrencyStamp = concurrencyStamp;
        }
        
        public MongoDbIdentityUser(string userName,
            string normalizedUserName, 
            string email, 
            bool emailConfirmed, 
            string passwordHash, 
            string securityStamp, 
            string concurrencyStamp,
            string phoneNumber) : this()
        {
            UserName = userName;
            NormalizedUserName = normalizedUserName;
            Email = email;
            EmailConfirmed = emailConfirmed;
            PasswordHash = passwordHash;
            SecurityStamp = securityStamp;
            ConcurrencyStamp = concurrencyStamp;
            PhoneNumber = phoneNumber;
        }
        
        public MongoDbIdentityUser(string userName,
            string normalizedUserName, 
            string email, 
            bool emailConfirmed, 
            string passwordHash, 
            string securityStamp, 
            string concurrencyStamp,
            string phoneNumber,
            bool phoneNumberConfirmed) : this()
        {
            UserName = userName;
            NormalizedUserName = normalizedUserName;
            Email = email;
            EmailConfirmed = emailConfirmed;
            PasswordHash = passwordHash;
            SecurityStamp = securityStamp;
            ConcurrencyStamp = concurrencyStamp;
            PhoneNumber = phoneNumber;
            PhoneNumberConfirmed = phoneNumberConfirmed;
        }

        public MongoDbIdentityUser(string userName,
            string normalizedUserName, 
            string email, 
            bool emailConfirmed, 
            string passwordHash, 
            string securityStamp, 
            string concurrencyStamp,
            string phoneNumber,
            bool phoneNumberConfirmed,
            bool twoFactorEnabled) : this()
        {
            UserName = userName;
            NormalizedUserName = normalizedUserName;
            Email = email;
            EmailConfirmed = emailConfirmed;
            PasswordHash = passwordHash;
            SecurityStamp = securityStamp;
            ConcurrencyStamp = concurrencyStamp;
            PhoneNumber = phoneNumber;
            PhoneNumberConfirmed = phoneNumberConfirmed;
            TwoFactorEnabled = twoFactorEnabled;
        }
        
        public MongoDbIdentityUser(string userName,
            string normalizedUserName, 
            string email, 
            bool emailConfirmed, 
            string passwordHash, 
            string securityStamp, 
            string concurrencyStamp,
            string phoneNumber,
            bool phoneNumberConfirmed,
            bool twoFactorEnabled,
            DateTimeOffset? lockoutEnd) : this()
        {
            UserName = userName;
            NormalizedUserName = normalizedUserName;
            Email = email;
            EmailConfirmed = emailConfirmed;
            PasswordHash = passwordHash;
            SecurityStamp = securityStamp;
            ConcurrencyStamp = concurrencyStamp;
            PhoneNumber = phoneNumber;
            PhoneNumberConfirmed = phoneNumberConfirmed;
            TwoFactorEnabled = twoFactorEnabled;
            LockoutEnd = lockoutEnd;
        }
        
        public MongoDbIdentityUser(string userName,
            string normalizedUserName, 
            string email, 
            bool emailConfirmed, 
            string passwordHash, 
            string securityStamp, 
            string concurrencyStamp,
            string phoneNumber,
            bool phoneNumberConfirmed,
            bool twoFactorEnabled,
            DateTimeOffset? lockoutEnd,
            bool lockoutEnabled) : this()
        {
            UserName = userName;
            NormalizedUserName = normalizedUserName;
            Email = email;
            EmailConfirmed = emailConfirmed;
            PasswordHash = passwordHash;
            SecurityStamp = securityStamp;
            ConcurrencyStamp = concurrencyStamp;
            PhoneNumber = phoneNumber;
            PhoneNumberConfirmed = phoneNumberConfirmed;
            TwoFactorEnabled = twoFactorEnabled;
            LockoutEnd = lockoutEnd;
            LockoutEnabled = lockoutEnabled;
        }
        
        public MongoDbIdentityUser(string userName,
            string normalizedUserName, 
            string email, 
            bool emailConfirmed, 
            string passwordHash, 
            string securityStamp, 
            string concurrencyStamp,
            string phoneNumber,
            bool phoneNumberConfirmed,
            bool twoFactorEnabled,
            DateTimeOffset? lockoutEnd,
            bool lockoutEnabled,
            int accessFailedCount) : this()
        {
            UserName = userName;
            NormalizedUserName = normalizedUserName;
            Email = email;
            EmailConfirmed = emailConfirmed;
            PasswordHash = passwordHash;
            SecurityStamp = securityStamp;
            ConcurrencyStamp = concurrencyStamp;
            PhoneNumber = phoneNumber;
            PhoneNumberConfirmed = phoneNumberConfirmed;
            TwoFactorEnabled = twoFactorEnabled;
            LockoutEnd = lockoutEnd;
            LockoutEnabled = lockoutEnabled;
            AccessFailedCount = accessFailedCount;
        }
        
        public MongoDbIdentityUser(string userName,
            string normalizedUserName, 
            string email, 
            bool emailConfirmed, 
            string passwordHash, 
            string securityStamp, 
            string concurrencyStamp,
            string phoneNumber,
            bool phoneNumberConfirmed,
            bool twoFactorEnabled,
            DateTimeOffset? lockoutEnd,
            bool lockoutEnabled,
            int accessFailedCount,
            string firstName) : this()
        {
            UserName = userName;
            NormalizedUserName = normalizedUserName;
            Email = email;
            EmailConfirmed = emailConfirmed;
            PasswordHash = passwordHash;
            SecurityStamp = securityStamp;
            ConcurrencyStamp = concurrencyStamp;
            PhoneNumber = phoneNumber;
            PhoneNumberConfirmed = phoneNumberConfirmed;
            TwoFactorEnabled = twoFactorEnabled;
            LockoutEnd = lockoutEnd;
            LockoutEnabled = lockoutEnabled;
            AccessFailedCount = accessFailedCount;
            FirstName = firstName;
        }
        
        public MongoDbIdentityUser(string userName,
            string normalizedUserName, 
            string email, 
            bool emailConfirmed, 
            string passwordHash, 
            string securityStamp, 
            string concurrencyStamp,
            string phoneNumber,
            bool phoneNumberConfirmed,
            bool twoFactorEnabled,
            DateTimeOffset? lockoutEnd,
            bool lockoutEnabled,
            int accessFailedCount,
            string firstName,
            string lastName) : this()
        {
            UserName = userName;
            NormalizedUserName = normalizedUserName;
            Email = email;
            EmailConfirmed = emailConfirmed;
            PasswordHash = passwordHash;
            SecurityStamp = securityStamp;
            ConcurrencyStamp = concurrencyStamp;
            PhoneNumber = phoneNumber;
            PhoneNumberConfirmed = phoneNumberConfirmed;
            TwoFactorEnabled = twoFactorEnabled;
            LockoutEnd = lockoutEnd;
            LockoutEnabled = lockoutEnabled;
            AccessFailedCount = accessFailedCount;
            FirstName = firstName;
            LastName = lastName;
        }
        
        public string Id { get; set; }
        public string UserName { get; set; }
        public string NormalizedUserName { get; set; }
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
        
        //Egghead
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public bool Equals(MongoDbIdentityUser other)
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
            return Equals((MongoDbIdentityUser) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (Id != null ? Id.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (UserName != null ? UserName.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (NormalizedUserName != null ? NormalizedUserName.GetHashCode() : 0);
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