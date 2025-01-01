using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Sensei.Core.Application.Common.Persistance;
using Sensei.Core.Domain;
using Sensei.Core.Infrastructure.Common.Persistence;

namespace Sensei.Core.Infrastructure.Common.Middleware;

public class EventualConsistencyMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context, IPublisher publisher, IAppDbContext dbContext)
    {
        var strategy = dbContext.Database.CreateExecutionStrategy();

        await strategy.ExecuteAsync(async () =>
        {
            await using var transaction = await dbContext.Database.BeginTransactionAsync();
            // This is a queue of domain events that will be published after the response is sent to the client
            // Usually this is done in DbContext on save. In that case user is waiting for the response until
            // all the domain events are published and handled. I like this approach better because it doesn't
            // block the user from getting the response. Consequence if failure occur is that user will not get
            // that error response immediately but it will be handled in the background.
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

                await transaction.CommitAsync();
            }
            catch (Exception)
            {
                // TODO Implement resiliency.. Send an email / Add to some queue for reprocessing...
            }
        });
        });

        await next(context);
    }
}