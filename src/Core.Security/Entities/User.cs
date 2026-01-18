using Core.Persistence.Abstractions.Repositories;
using Core.Security.Enums;

namespace Core.Security.Entities;

public class User<TId> : Entity<TId>
{
    public string Email { get; protected set; }      
    public byte[] PasswordSalt { get; protected set; }      
    public byte[] PasswordHash { get; protected set; }      
    public AuthenticatorType AuthenticatorType { get; protected set; } 
    
    public User()
    {
        Email = string.Empty;
        PasswordHash = [];
        PasswordSalt = [];
    }

    public User(string email, byte[] passwordSalt, byte[] passwordHash, AuthenticatorType authenticatorType)
    {
        Email = email;
        PasswordSalt = passwordSalt;
        PasswordHash = passwordHash;
        AuthenticatorType = authenticatorType;
    }

    public User(TId id, string email, byte[] passwordSalt, byte[] passwordHash, AuthenticatorType authenticatorType)
        : base(id)
    {
        Email = email;
        PasswordSalt = passwordSalt;
        PasswordHash = passwordHash;
        AuthenticatorType = authenticatorType;
    }
}