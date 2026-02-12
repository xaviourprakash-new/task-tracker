using TaskTracker.API.Domain.Entities;

namespace TaskTracker.API.Application.Common.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByEmailAsync(string email);
    Task<User> CreateAsync(User user);
}
