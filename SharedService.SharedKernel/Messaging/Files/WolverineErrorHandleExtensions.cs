using Npgsql;
using SharedService.SharedKernel.Exceptions;
using Wolverine;
using Wolverine.ErrorHandling;

namespace SharedService.SharedKernel.Messaging.Files;

public static class WolverineErrorHandleExtensions
{
    public static void ConfigureStandardErrorPolicies(this WolverineOptions opts)
    {
        opts.Policies.OnException<NpgsqlException>(ex => ex.IsTransient)
            .RetryWithCooldown(TimeSpan.FromMilliseconds(50), TimeSpan.FromMilliseconds(100))
            .Then.ScheduleRetry(TimeSpan.FromSeconds(3))
            .Then.ScheduleRetry(TimeSpan.FromSeconds(10));

        opts.Policies.OnException<TimeoutException>()
            .RetryWithCooldown(TimeSpan.FromMilliseconds(50), TimeSpan.FromMilliseconds(100))
            .Then.ScheduleRetry(TimeSpan.FromSeconds(3))
            .Then.ScheduleRetry(TimeSpan.FromSeconds(15));

        opts.Policies.OnException<HttpRequestException>()
            .RetryWithCooldown(TimeSpan.FromMilliseconds(50), TimeSpan.FromMilliseconds(100))
            .Then.ScheduleRetry(TimeSpan.FromSeconds(3))
            .Then.ScheduleRetry(TimeSpan.FromSeconds(15));

        opts.Policies.OnException<TaskCanceledException>(ex => ex.InnerException is TimeoutException)
            .RetryWithCooldown(TimeSpan.FromMilliseconds(50), TimeSpan.FromMilliseconds(100))
            .Then.ScheduleRetry(TimeSpan.FromSeconds(3))
            .Then.ScheduleRetry(TimeSpan.FromSeconds(15));

        opts.Policies.OnException<IOException>()
            .ScheduleRetry(TimeSpan.FromSeconds(1))
            .Then.ScheduleRetry(TimeSpan.FromSeconds(5));

        opts.Policies.OnException<ValidationException>().MoveToErrorQueue();
        opts.Policies.OnException<ArgumentException>().MoveToErrorQueue();
        opts.Policies.OnException<InvalidCastException>().MoveToErrorQueue();
        opts.Policies.OnException<FailureException>().MoveToErrorQueue();
        opts.Policies.OnException<NotFoundException>().MoveToErrorQueue();
        opts.Policies.OnException<ConflictException>().MoveToErrorQueue();

        opts.Policies.OnException<Exception>()
            .RetryWithCooldown(TimeSpan.FromMilliseconds(100), TimeSpan.FromMilliseconds(250))
            .Then.ScheduleRetry(TimeSpan.FromSeconds(5))
            .Then.MoveToErrorQueue();
    }
}