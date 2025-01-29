using CodeWash.MessageExchange.Api.Midllewares;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;

namespace CodeWash.MessageExchange.Tests;

[TestFixture]
public class ErrorHandlerMiddlewareTests
{
    private Mock<RequestDelegate> nextMock;
    private Mock<ILogger<ErrorHandlerMiddleware>> loggerMock;
    private ErrorHandlerMiddleware middleware;

    [SetUp]
    public void Setup()
    {
        nextMock = new Mock<RequestDelegate>();
        loggerMock = new Mock<ILogger<ErrorHandlerMiddleware>>();
        middleware = new ErrorHandlerMiddleware(nextMock.Object, loggerMock.Object);
    }

    [Test]
    public async Task Invoke_OperationCanceledException_ReturnsBadRequest()
    {
        // Arrange
        nextMock.Setup(x => x(It.IsAny<HttpContext>())).ThrowsAsync(new OperationCanceledException());
        DefaultHttpContext context = new();

        // Act
        await middleware.Invoke(context);

        // Assert
        Assert.That(context.Response.StatusCode, Is.EqualTo(StatusCodes.Status400BadRequest));
    }

    [Test]
    public async Task Invoke_UnhandledException_ReturnsInternalServerError()
    {
        // Arrange
        nextMock.Setup(x => x(It.IsAny<HttpContext>())).ThrowsAsync(new Exception("Test error"));
        DefaultHttpContext context = new();

        // Act
        await middleware.Invoke(context);

        // Assert
        Assert.That(context.Response.StatusCode, Is.EqualTo(StatusCodes.Status500InternalServerError));
    }
}
