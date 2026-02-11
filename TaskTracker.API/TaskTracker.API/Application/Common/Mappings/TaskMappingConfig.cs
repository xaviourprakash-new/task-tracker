using Mapster;
using TaskTracker.API.Application.DTOs;
using TaskTracker.API.Domain.Entities;

namespace TaskTracker.API.Application.Common.Mappings;

public class TaskMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<TaskItem, TaskItemDto>()
            .Map(dest => dest.Status, src => src.Status.ToString())
            .Map(dest => dest.Priority, src => src.Priority.ToString());
    }
}
