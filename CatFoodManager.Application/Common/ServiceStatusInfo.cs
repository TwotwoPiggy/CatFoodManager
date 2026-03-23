namespace CatFoodManager.Application.Common;

public record ServiceStatusInfo(
    bool IsRunning,
    bool IsPaused,
    int RunningTaskCount,
    int QueueLength,
    DateTimeOffset? StartedAt,
    DateTimeOffset? LastActivityAt
);
