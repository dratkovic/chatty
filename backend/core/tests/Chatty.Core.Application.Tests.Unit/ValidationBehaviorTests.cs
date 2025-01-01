using Chatty.Core.Application.Behaviors;
using ErrorOr;
using FluentAssertions;
using FluentValidation;
using MediatR;
using NSubstitute;
using Xunit;

namespace Chatty.Core.Application.Tests.Unit;

public class ValidationBehaviorTests
{
    private readonly IValidator<TestRequest> _validator;
    private readonly ValidationBehavior<TestRequest, ErrorOr<string>> _behavior;

    public ValidationBehaviorTests()
    {
        _validator = Substitute.For<IValidator<TestRequest>>();
        _behavior = new ValidationBehavior<TestRequest, ErrorOr<string>>(_validator);
    }

    [Fact]
    public async Task Handle_WithValidRequest_ShouldCallNext()
    {
        // Arrange
        var request = new TestRequest();
        var next = Substitute.For<RequestHandlerDelegate<ErrorOr<string>>>();
        next().Returns("Success");

        _validator.ValidateAsync(request, Arg.Any<CancellationToken>())
            .Returns(new FluentValidation.Results.ValidationResult());

        // Act
        var result = await _behavior.Handle(request, next, CancellationToken.None);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Should().Be("Success");
        await next.Received(1).Invoke();
    }

    [Fact]
    public async Task Handle_WithInvalidRequest_ShouldReturnValidationErrors()
    {
        // Arrange
        var request = new TestRequest();
        var next = Substitute.For<RequestHandlerDelegate<ErrorOr<string>>>();

        var validationFailures = new List<FluentValidation.Results.ValidationFailure>
        {
            new("PropertyName", "Error message")
        };
        _validator.ValidateAsync(request, Arg.Any<CancellationToken>())
            .Returns(new FluentValidation.Results.ValidationResult(validationFailures));

        // Act
        var result = await _behavior.Handle(request, next, CancellationToken.None);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Type.Should().Be(ErrorType.Validation);
        result.FirstError.Code.Should().Be("PropertyName");
        result.FirstError.Description.Should().Be("Error message");
        await next.DidNotReceive().Invoke();
    }

    public class TestRequest : IRequest<ErrorOr<string>> { }
}