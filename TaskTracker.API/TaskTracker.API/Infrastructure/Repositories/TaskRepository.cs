using Microsoft.EntityFrameworkCore;
using TaskTracker.API.Application.Common.Interfaces;
using TaskTracker.API.Domain.Entities;
using TaskTracker.API.Domain.Enums;
using TaskTracker.API.Infrastructure.Data;

namespace TaskTracker.API.Infrastructure.Repositories;

public class TaskRepository : ITaskRepository
{
    private readonly AppDbContext _context;

    public TaskRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<TaskItem?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Tasks.FindAsync([id], cancellationToken);
    }

    public async Task<IEnumerable<TaskItem>> GetAllAsync(TaskItemStatus? status, TaskItemPriority? priority, CancellationToken cancellationToken = default)
    {
        var query = _context.Tasks.AsQueryable();

        if (status.HasValue)
            query = query.Where(t => t.Status == status.Value);

        if (priority.HasValue)
            query = query.Where(t => t.Priority == priority.Value);

        return await query.ToListAsync(cancellationToken);
    }

    public async Task<TaskItem> CreateAsync(TaskItem taskItem, CancellationToken cancellationToken = default)
    {
        _context.Tasks.Add(taskItem);
        await _context.SaveChangesAsync(cancellationToken);
        return taskItem;
    }

    public async Task<TaskItem> UpdateAsync(TaskItem taskItem, CancellationToken cancellationToken = default)
    {
        _context.Tasks.Update(taskItem);
        await _context.SaveChangesAsync(cancellationToken);
        return taskItem;
    }
}
