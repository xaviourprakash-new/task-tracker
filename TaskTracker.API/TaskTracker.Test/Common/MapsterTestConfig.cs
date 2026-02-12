using Mapster;
using TaskTracker.API.Application.Common.Mappings;

namespace TaskTracker.Test.Common;

public static class MapsterTestConfig
{
    private static bool _configured;

    public static void Configure()
    {
        if (_configured) return;

        var config = TypeAdapterConfig.GlobalSettings;
        config.Scan(typeof(TaskMappingConfig).Assembly);
        _configured = true;
    }
}
