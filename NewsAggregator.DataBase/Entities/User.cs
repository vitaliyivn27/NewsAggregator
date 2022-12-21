﻿namespace NewsAggregator.DataBase.Entities;

public class User : IBaseEntity
{
    public Guid Id { get; set; }
    public string Email { get; set; }
    public string PasswordHash { get; set; }
    public DateTime RegistrationDate { get; set; }

    public Guid RoleId { get; set; }
    public Role Role { get; set; }

    public List<Comment> Comments { get; set; }
    public List<RefreshToken> RefreshTokens { get; set; }
}