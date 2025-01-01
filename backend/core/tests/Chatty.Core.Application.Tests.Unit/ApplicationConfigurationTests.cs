using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Chatty.Core.Application.Tests.Unit;

public class ApplicationConfigurationTests
{
    [Fact]
    public void AddCommonApplication_ShouldAddRequiredServices()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddCoreApplication<ApplicationConfigurationTests>();

        // Assert
        services.Should().Contain(sd => sd.ServiceType == typeof(AutoMapper.IConfigurationProvider));
        services.Should().Contain(sd => sd.ServiceType == typeof(MediatR.IMediator));
    }
}