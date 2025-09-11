using API.DTO.Domain;
using API.Exceptions.Context;
using API.Exceptions.ContextCreator;
using API.Repositories.Interfaces;
using API.Repositories.Queries;
using API.Repositories.Uof;
using API.Services.Application.Implementations;
using API.Services.Helpers.Interfaces;
using DataBaseInfo.models;
using Moq;

namespace API_tests.Services.Tests.UserServiceTests
{
    public class RegisterAsyncTests
    {
        private readonly Mock<IErrorContextCreatorFactory> _errFactoryMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IUserRepository> _userReposMock;
        private readonly Mock<IJWTService> _jwtServiceMock;
        private readonly Mock<IQueries> _queryMock;
        private readonly UserService _sut;
        public RegisterAsyncTests()
        {
            _errFactoryMock = new Mock<IErrorContextCreatorFactory>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _userReposMock = new Mock<IUserRepository>();
            _jwtServiceMock = new Mock<IJWTService>();
            _queryMock = new Mock<IQueries>();
           

            _errFactoryMock.Setup(f => f.Create(It.IsAny<string>()))
                .Returns(new ErrorContextCreator("UserService"));

            _unitOfWorkMock.Setup(u => u.UserRepository)
                .Returns(_userReposMock.Object);

            _sut = new UserService(_jwtServiceMock.Object,_errFactoryMock.Object,_unitOfWorkMock.Object,_queryMock.Object);
        }
        [Fact]
        public async Task RegisterAsync_ShouldThrow_WhenUserAlreadyExists()
        {
            //Arrange
            _userReposMock.Setup(u => u.GetDbUserAsync("test@mail.ru"))
                .ReturnsAsync(new User { Id = 1, UserEmail = "test@mail.ru" });
            //Act && Assert
            await Assert.ThrowsAsync<AppException>( () => _sut.RegisterAsync("test@mail.ru", "111111"));
           
        }
    }

}