using Moq;
using CodeWash.MessageExchange.Api.Controllers;
using CodeWash.MessageExchange.DataAccess.Contracts;
using CodeWash.MessageExchange.Dtos.ApiDtos.AuthDtos;
using CodeWash.MessageExchange.Dtos.QueryDtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using CodeWash.MessageExchange.DataAccess.StoredProcedures.Commands;
using CodeWash.MessageExchange.DataAccess.StoredProcedures.Queries;

namespace CodeWash.MessageExchange.Tests.ApiTests;

[TestFixture]
public class AuthControllerTests
{
    private Mock<IDbConnector> _dbConnectorMock;
    private Mock<IConfiguration> _configurationMock;
    private AuthController _authController;

    [SetUp]
    public void SetUp()
    {
        _dbConnectorMock = new Mock<IDbConnector>();
        _configurationMock = new Mock<IConfiguration>();

        // Mock JWT Configuration
        _configurationMock.Setup(config => config["Jwt:Key"]).Returns("SuperSecretTestKey_SuperSecretTestKey");
        _configurationMock.Setup(config => config["Jwt:Issuer"]).Returns("CodeWash.MessageExchange.Api");
        _configurationMock.Setup(config => config["Jwt:Audience"]).Returns("CodeWash.MessageExchange.Client");

        _authController = new AuthController(_dbConnectorMock.Object, _configurationMock.Object);
    }

    [Test]
    public async Task Login_ShouldReturnToken_WhenCredentialsAreValid()
    {
        // Arrange
        var loginRequest = new LoginRequestDto("test@example.com", "password123");
        var user = new GetUserByEmailVM(Guid.NewGuid(), loginRequest.Email, BCrypt.Net.BCrypt.HashPassword(loginRequest.Password));

        _dbConnectorMock.Setup(db => db.ExecuteQueryTop1Async(It.IsAny<GetUserByEmailSP>(), It.IsAny<CancellationToken>()))
                        .ReturnsAsync(user);

        // Act
        var result = await _authController.Login(loginRequest, CancellationToken.None);
        var okResult = result as OkObjectResult;

        Assert.Multiple(() =>
        {
            // Assert
            Assert.That(okResult, Is.Not.Null);
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
        });
        Assert.That(okResult.Value, Is.InstanceOf<string>());
    }

    [Test]
    public async Task Login_ShouldReturnUnauthorized_WhenCredentialsAreInvalid()
    {
        // Arrange
        var loginRequest = new LoginRequestDto("test@example.com", "wrongpassword");

        _dbConnectorMock.Setup(db => db.ExecuteQueryTop1Async(It.IsAny<GetUserByEmailSP>(), It.IsAny<CancellationToken>()))
                        .ReturnsAsync((GetUserByEmailVM?)null);

        // Act
        var result = await _authController.Login(loginRequest, CancellationToken.None);

        // Assert
        Assert.That(result, Is.InstanceOf<UnauthorizedObjectResult>());
    }

    [Test]
    public async Task Register_ShouldReturnOk_WhenUserIsCreated()
    {
        // Arrange
        var registerRequest = new RegisterRequestDto("newuser@example.com", "securepassword");

        _dbConnectorMock.Setup(db => db.ExecuteQueryAsync(It.IsAny<GetUserByEmailSP>(), It.IsAny<CancellationToken>()))
                        .ReturnsAsync([]);

        _dbConnectorMock.Setup(db => db.ExecuteCommandAsync(It.IsAny<CreateUserSP>(), It.IsAny<CancellationToken>()))
                        .ReturnsAsync(1);

        // Act
        var result = await _authController.Register(registerRequest, CancellationToken.None);
        var okResult = result as OkObjectResult;

        // Assert
        Assert.That(okResult, Is.Not.Null);
        Assert.That(okResult.StatusCode, Is.EqualTo(200));
    }
}
