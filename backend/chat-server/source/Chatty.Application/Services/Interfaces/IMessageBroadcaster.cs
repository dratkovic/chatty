using Chatty.Domain;
using ErrorOr;

namespace Chatty.Application.Services;

public interface IMessageBroadcaster
{
    Task<ErrorOr<Success>> BroadcastMessage(Message message, CancellationToken ct = default);
}