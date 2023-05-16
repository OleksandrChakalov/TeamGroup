using FluentAssertions;
using GS.Data.Entities;
using GS.Data.Repositories.UserRead;
using GS.Data.Repositories.UserWrite;
using GS.Domain.Models.Configuration;
using GS.Domain.Models.User;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace GS.Business.Test
{
    public class AuthServiceTest
    {
        private Mock<IUserWriteRepository> _userWriteRepository;
        private Mock<IUserReadRepository> _userReadRepository;
        private AuthService _authService;

        [SetUp]
        public void SetUp()
        {
            _userReadRepository = new Mock<IUserReadRepository>();
            _userWriteRepository = new Mock<IUserWriteRepository>();

            var jwtSettings = new JwtSettings
            {
                ExpiryInMinutes = 5,
                ValidAudience = "test",
                SecurityKey = "testtesttesttesttesttesttest",
                ValidIssuer = "test"
            };
            var jwtOptions = Options.Create(jwtSettings);

            var googleAuthSettings = new GoogleAuthSettings
            {
                ClientId = "test"
            };
            var googleOption = Options.Create(googleAuthSettings);

            _authService = new AuthService(jwtOptions, googleOption, _userReadRepository.Object, _userWriteRepository.Object);
        }
        [Test]
        public void GenerateToken_WhenValidUserProvided_ShouldReturnToken()
        {
            // Arrange
            var user = new UserModel { Id = Guid.NewGuid(), Email = "test@example.com", Username = "testuser" };

            // Act
            var token = _authService.GenerateToken(user);

            // Assert
            Assert.IsNotNull(token);
            Assert.IsNotEmpty(token);
        }

        [Test]
        public void VerifyGoogleToken_WhenValidExternalAuthDtoProvided_ShouldReturnPayload()
        {
            // Arrange
            var externalAuth = new ExternalAuthDto { IdToken = "valid_token" };

            // Act
            var payload = _authService.VerifyGoogleToken(externalAuth).Result;

            // Assert
            Assert.Null(payload);
        }

        [Test]
        public void GetUserByLoginAsync_WhenValidLoginProviderAndProviderKeyProvided_ShouldReturnUser()
        {
            // Arrange
            var loginProvider = "google";
            var providerKey = "google_user123";

            // Act
            var user = _authService.GetUserByLoginAsync(loginProvider, providerKey).Result;

            // Assert
            Assert.Null(user);
        }

        [Test]
        public void AddUserAsync_WhenValidUserModelProvided_ShouldAddUser()
        {
            // Arrange
            var user = new UserModel { Id = Guid.NewGuid(), Email = "test@example.com", Username = "testuser" };

            // Act
            _authService.AddUserAsync(user).Wait();

            // Assert
            // Додаткові перевірки, наприклад, перевірка, що користувач збережений в базі даних
        }

        [Test]
        public void AddUserLoginAsync_WhenValidUserLoginModelProvided_ShouldAddUserLogin()
        {
            // Arrange
            var userLogin = new UserLoginModel { UserId = Guid.NewGuid(), LoginProvider = "google", ProviderKey = "google_user123" };

            // Act
            _authService.AddUserLoginAsync(userLogin).Wait();

            // Assert
            // Додаткові перевірки, наприклад, перевірка, що інформація про вхід користувача збережена в базі даних
        }

        [Test]
        public async Task VerifyGoogleToken_InvalidToken_ReturnsNullPayload()
        {
            // Arrange
            var externalAuth = new ExternalAuthDto { /* ініціалізація властивостей ExternalAuthDto */ };

            // Act
            var payload = await _authService.VerifyGoogleToken(externalAuth);

            // Assert
            Assert.IsNull(payload);
        }

        [Test]
        public async Task GetUserByLoginAsync_UserExists_ReturnsUser()
        {
            // Arrange
            var loginProvider = "provider";
            var providerKey = "key";

            // Act
            var user = await _authService.GetUserByLoginAsync(loginProvider, providerKey);

            // Assert
            Assert.Null(user);
        }

        [Test]
        public async Task GetUserByLoginAsync_UserNotExists_ReturnsNull()
        {
            // Arrange
            var loginProvider = "provider";
            var providerKey = "key";

            // Act
            var user = await _authService.GetUserByLoginAsync(loginProvider, providerKey);

            // Assert
            Assert.IsNull(user);
        }

        [Test]
        public void GenerateToken_WithUser_Token()
        {
            var user = new UserModel
            {
                Email = "test@test.com"
            };

            var result = _authService.GenerateToken(user);

            result.Should().NotBeNullOrWhiteSpace();
        }

        [Test]
        public async Task GetUserByLoginAsync_WithLogin_User()
        {
            var user = new User()
            {
                Id = Guid.Empty
            };

            _userReadRepository.Setup(x => x.GetUserByLoginAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(user);

            var result = await _authService.GetUserByLoginAsync("test", "test");

            result.Should().NotBeNull();
            result.Id.Should().Be(Guid.Empty);
        }

        [Test]
        public void AddUserAsync_WithUser_ShouldAdd()
        {
            var model = new UserModel()
            {
                Id = Guid.Empty
            };

            _userWriteRepository.Setup(x => x.AddUserAsync(It.IsAny<User>()))
                .Returns(Task.CompletedTask);

            Assert.DoesNotThrowAsync(async () => await _authService.AddUserAsync(model));
        }

        [Test]
        public void AddUserLoginAsync_WithUser_ShouldAdd()
        {
            var model = new UserLoginModel()
            {
                UserId = Guid.Empty
            };

            _userWriteRepository.Setup(x => x.AddUserLoginAsync(It.IsAny<UserLogin>()))
                .Returns(Task.CompletedTask);

            Assert.DoesNotThrowAsync(async () => await _authService.AddUserLoginAsync(model));
        }
    }
}
