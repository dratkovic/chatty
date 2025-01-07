using Chatty.Core.Application.Common.Persistance;
using Chatty.Core.Domain;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Chatty.Core.Infrastructure.Common.Middleware;

public class EventualConsistencyMiddleware(
    RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context,
        IPublisher publisher,
        IAppDbContext dbContext,
        ILogger<EventualConsistencyMiddleware> _logger)
    {
        var strategy = dbContext.Database.CreateExecutionStrategy();
        
        // This is a queue of domain events that will be published after the response is sent to the client
        // Usually this is done in DbContext on save. In that case user is waiting for the response until
        // all the domain events are published and handled. I like this approach better because it doesn't
        // block the user from getting the response. Consequence if failure occur is that user will not get
        // that error response immediately but it will be handled in the background.
        await strategy.ExecuteAsync(async () =>
        {
            await using var transaction = await dbContext.Database.BeginTransactionAsync();
            try
            {
                context.Response.OnCompleted(async () =>
                {
                    try
                    {
                        if (context.Items.TryGetValue("DomainEventsQueue", out var value) &&
                            value is Queue<IDomainEvent> domainEventsQueue)
                        {
                            while (domainEventsQueue!.TryDequeue(out var domainEvent))
                            {
                                await publisher.Publish(domainEvent);
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        _logger.LogError(e, "Error occurred while publishing domain events");
                        // TODO Implement resiliency.. Send an email / Add to some queue for reprocessing...
                    }
                });

                await next(context);
                await transaction.CommitAsync();
            }
            catch (Exception e)
            {
                _logger.LogCritical(e, "Error occurred while publishing domain events");
                await transaction.RollbackAsync();
                throw;
            }
        });
    }
}