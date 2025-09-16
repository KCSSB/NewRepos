using System.Net;
using API.DTO.Domain;
using API.Exceptions.Context;
using API.Exceptions.ContextCreator;
using API.Repositories.Interfaces;
using API.Repositories.Queries;
using API.Repositories.Uof;
using API.Services.Application.Implementations;
using API.Services.Helpers.Interfaces;
using DataBaseInfo.models;
using Microsoft.AspNetCore.Identity;
using Moq;

namespace API_tests.Services.Tests.UserServiceTests
{
    public class RegisterAsyncTests
    {
        private readonly Mock<IErrorContextCreatorFactory> _errFactoryMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IUserRepository> _userRepoMock;
        private readonly Mock<IJWTService> _jwtServiceMock;
        private readonly Mock<IQueries> _queryMock;
        private readonly Mock<IPasswordHasher<User>> _passHasherMock;
        private readonly UserService _service;
        public RegisterAsyncTests()
        {
            _errFactoryMock = new Mock<IErrorContextCreatorFactory>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _userRepoMock = new Mock<IUserRepository>();
            _jwtServiceMock = new Mock<IJWTService>();
            _queryMock = new Mock<IQueries>();
            _passHasherMock = new Mock<IPasswordHasher<User>>();
          
            _errFactoryMock.Setup(f => f.Create(It.IsAny<string>()))
                .Returns(new ErrorContextCreator("UserService"));


            _unitOfWorkMock.Setup(u => u.UserRepository)
                .Returns(_userRepoMock.Object);

            _service = new UserService(_jwtServiceMock.Object,_errFactoryMock.Object,_unitOfWorkMock.Object,_queryMock.Object,_passHasherMock.Object);
        }
        [Fact]
        public async Task RegisterAsync_ShouldThrow_WhenUserAlreadyExists()
        {
            //Arrange
            _userRepoMock.Setup(u => u.GetDbUserAsync("test@mail.ru"))
                .ReturnsAsync(new User { Id = 1, UserEmail = "test@mail.ru" });
            //Act && Assert
            await Assert.ThrowsAsync<AppException>( () => _service.RegisterAsync("test@mail.ru", "111111"));
           
        }
        [Fact]
        public async Task RegisterAsync_ShouldReturnUserId_WhenRegistrationSuccessful()
        {
            _passHasherMock.Setup(h => h.HashPassword(It.IsAny<User>(), It.IsAny<string>()))
               .Returns("hashed_password");
            // Arrange
            _userRepoMock.Setup(r => r.GetDbUserAsync("new@test.com"))
                .ReturnsAsync((User?)null);

            _userRepoMock.Setup(r => r.AddDbUserAsync(It.IsAny<User>()))
                .Callback<User>(u => u.Id = 123) // симулируем назначение Id
                .Returns(Task.CompletedTask);

            _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _service.RegisterAsync("new@test.com", "Password123!");

            // Assert
            Assert.Equal(123, result);
            _userRepoMock.Verify(r => r.AddDbUserAsync(It.IsAny<User>()), Times.Once);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }
        [Fact]
        public async Task RegisterAsync_ShouldThrow_WhenPasswordHashingFails()
        {
            // Arrange
            _userRepoMock.Setup(r => r.GetDbUserAsync("new@test.com"))
                .ReturnsAsync((User?)null);

            _passHasherMock.Setup(h => h.HashPassword(It.IsAny<User>(), It.IsAny<string>()))
                .Returns((string?)null);

            // Act
            Func<Task> act = async () =>
                await _service.RegisterAsync("new@test.com", "Password123!");

            // Assert
            var ex = await Assert.ThrowsAsync<AppException>(act);
            Assert.Contains("Ошибка во время хэширования пароля", ex.Message);

            // Проверяем, что методы сохранения не вызывались
            _userRepoMock.Verify(r => r.AddDbUserAsync(It.IsAny<User>()), Times.Never);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }
    }

}