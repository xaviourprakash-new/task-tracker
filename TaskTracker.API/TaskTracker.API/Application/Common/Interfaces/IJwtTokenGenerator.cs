using TaskTracker.API.Domain.Entities;

namespace TaskTracker.API.Application.Common.Interfaces;

public interface IJwtTokenGenerator
{
    (string Token, DateTime ExpiresAt) GenerateToken(User user);
}
