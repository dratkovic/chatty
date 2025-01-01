using AutoMapper;
using Chatty.Core.Application.Mapping;
using FluentAssertions;
using Xunit;

namespace Chatty.Core.Application.Tests.Unit;

public class MappingProfileTests
{
    [Fact]
    public void MappingProfile_ShouldCreateValidConfiguration()
    {
        // Arrange
        var config = new MapperConfiguration(cfg => 
            cfg.AddProfile(new MappingProfile(typeof(MappingProfileTests).Assembly)));

        // Act & Assert
        config.AssertConfigurationIsValid();
    }

    [Fact]
    public void MappingProfile_ShouldMapTestClassCorrectly()
    {
        // Arrange
        var config = new MapperConfiguration(cfg => 
            cfg.AddProfile(new MappingProfile(typeof(MappingProfileTests).Assembly)));
        var mapper = config.CreateMapper();

        var source = new TestSource { Id = 1, Name = "Test" };

        // Act
        var result = mapper.Map<TestDestination>(source);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(source.Id);
        result.Name.Should().Be(source.Name);
    }

    public class TestSource
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    public class TestDestination : IMapFrom<TestSource>
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}