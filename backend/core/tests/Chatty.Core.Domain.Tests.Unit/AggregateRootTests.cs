using Chatty.Core.Domain;
using Chatty.Core.Domain.Models;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Chatty.Core.Domain.Tests.Unit;

public class AggregateRootTests
{
    [Fact]
    public void AddDomainEvent_ShouldNotAddNullEvent()
    {
        var aggregateRoot = Substitute.For<AggregateRoot>();

        aggregateRoot.AddDomainEvent(null!);

        aggregateRoot.PopDomainEvents().Should().BeEmpty();
    }

    [Fact]
    public void AddDomainEvent_ShouldNotAddDuplicateEvent()
    {
        var aggregateRoot = Substitute.For<AggregateRoot>();
        var domainEvent = Substitute.For<IDomainEvent>();

        aggregateRoot.AddDomainEvent(domainEvent);
        aggregateRoot.AddDomainEvent(domainEvent);

        aggregateRoot.PopDomainEvents().Should().ContainSingle().Which.Should().Be(domainEvent);
    }

    [Fact]
    public void PopDomainEvents_ShouldReturnEventsInOrderAdded()
    {
        var aggregateRoot = Substitute.For<AggregateRoot>();
        var domainEvent1 = Substitute.For<IDomainEvent>();
        var domainEvent2 = Substitute.For<IDomainEvent>();

        aggregateRoot.AddDomainEvent(domainEvent1);
        aggregateRoot.AddDomainEvent(domainEvent2);

        var events = aggregateRoot.PopDomainEvents();

        events.Should().HaveCount(2);
        events[0].Should().Be(domainEvent1);
        events[1].Should().Be(domainEvent2);
    }
}