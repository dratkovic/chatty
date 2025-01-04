using Chatty.Application.Common;
using Chatty.Contracts.Responses;
using Chatty.Core.Application.Common.Persistance;
using Chatty.Domain;
using ErrorOr;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;

namespace Chatty.Application.Services;

public class MessageBroadcaster : IMessageBroadcaster
{
    private readonly IAppDbContext _dbContext;
    private readonly HybridCache _cache;
    private readonly IBus _bus;

    public MessageBroadcaster(IAppDbContext dbContext, HybridCache cache, IBus bus)
    {
        _dbContext = dbContext;
        _cache = cache;
        _bus = bus;
    }

    public async Task<ErrorOr<Success>> BroadcastMessage(Message message, CancellationToken ct = default)
    {
        // if recipient is set that means that we're handling 1:1 chat
        var defaultMessage = new MessageResponse(message.Id,
            message.SenderId,
            message.Sender.DisplayName,
            message.Content,
            message.RecipientId,
            message.GroupId,
            message.TimeStampUtc,
            message.Status.ToString());
        
        if (message.RecipientId != null)
        {
            await _bus.Publish(defaultMessage, ct);
            
            // TODO handle failure

            return new Success();
        }
        
        var recipients = message.Group != null
            ? await GetGroupUsers(message.GroupId!.Value, ct)
            : [(Guid)message.RecipientId!];

        var groupMessages = new List<MessageResponse>();
        foreach (var recipient in recipients)
        {
            var groupMessage = defaultMessage with {RecipientId = recipient};
            
            groupMessages.Add(groupMessage);
        }
        
        await _bus.PublishBatch(groupMessages, ct);
        
        // TODO handle failure

        return new Success();
    }
    
    private async Task<List<Guid>> GetGroupUsers(Guid messageGroupId, CancellationToken ct)
    {
        return await _cache.GetOrCreateAsync(CacheKeys.GroupUsers(messageGroupId), 
            async cancel => await GetGroupUsersFromDb(messageGroupId, cancel),
            cancellationToken: ct);
        
    }

    private async Task<List<Guid>> GetGroupUsersFromDb(Guid? messageGroupId, CancellationToken ct)
    {
        return await _dbContext.Set<Group>()
            .Where(x => x.Id == messageGroupId)
            .SelectMany(x => x.Participants.Select(y => y.Id))
            .ToListAsync(ct);
    }
}