using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using NSubstitute.ReturnsExtensions;
using SH.BLL;
using SH.Models.Customer;
using SH.Models.Models;
using System.Net.Http;

namespace SH.Tests.UnitTests
{

    public class Customer_Logic_Tests
    {
        #region Ctor/Fields

        private readonly ILogger<CustomerLogic> _logger;
        private readonly HttpClient _httpClient;
        private readonly ICustomerLogic _bll;
        private readonly ICustomerRepo _customerRepo;
        private readonly Guid _customerId = Guid.NewGuid();
        private readonly Guid _emptyGuid = new Guid();
        private readonly DateTime _defaultDobDatetime = new DateTime(1999, 1, 1);
        private readonly string _defaultFullName = "John Doe";

        public CustomerDto ExpectedResponseCustomer
        {
            get
            {
                return new CustomerDto
                {
                    CustomerId = _emptyGuid,
                    FullName = _defaultFullName,
                    DateOfBirth = DateOnly.FromDateTime(_defaultDobDatetime)
                };
            }
        }

        public Customer_Logic_Tests()
        {
            _logger = NullLogger<CustomerLogic>.Instance;
            _customerRepo = Substitute.For<ICustomerRepo>();
            _httpClient = Substitute.For<HttpClient>();
            _bll = new CustomerLogic(_logger, _customerRepo, _httpClient);
        }

        #endregion

        [Fact]
        public async Task AddCustomer_ShouldReturnCustomerDto_WhenCalledWithValidInput()
        {
            //Arrange
            CreateCustomerDto requestDto = new CreateCustomerDto
            {
                FullName = _defaultFullName,
                DateOfBirth = DateOnly.FromDateTime(_defaultDobDatetime)
            };
            var expectedResult = ExpectedResponseCustomer;

            //Mocks
            _customerRepo.AddCustomerAsync(default)
                .ReturnsForAnyArgs(expectedResult);

            //Act
            var actual = await _bll.AddCustomerAsync(requestDto);

            //Assert
            actual.Should().BeOfType<CustomerDto>();
            actual.CustomerId.Should().Be(expectedResult.CustomerId);
            actual.FullName.Should().Be(expectedResult.FullName);
            actual.DateOfBirth.Should().Be(expectedResult.DateOfBirth);
        }

        [Fact]
        public async Task GetCustomerById_ShouldReturnCustomerDto_WhenCalledWithValidInput()
        {
            //Arrange
            var expectedResult = ExpectedResponseCustomer;

            //Mocks
            _customerRepo.GetCustomerByIdAsync(_customerId.ToString())
                .ReturnsForAnyArgs(expectedResult);

            //Act
            var actual = await _bll.GetCustomerByIdAsync(_customerId.ToString());

            //Assert
            actual.Should().BeOfType<CustomerDto>();
            actual.CustomerId.Should().Be(expectedResult.CustomerId);
            actual.FullName.Should().Be(expectedResult.FullName);
            actual.DateOfBirth.Should().Be(expectedResult.DateOfBirth);
        }

        [Theory]
        [InlineData("00000000-0000-0000-0000-000000000000")]
        [InlineData("11111111-1111-1111-1111-111111111111")]
        public async Task GetCustomerById_ShouldReturnNull_WhenCalledWithValidInput(string guid)
        {
            //Arrange

            //Mocks
            _customerRepo.GetCustomerByIdAsync(guid).ReturnsNull();

            //Act
            var actual = await _bll.GetCustomerByIdAsync(guid);

            //Assert
            Assert.Null(actual);
        }

        [Fact]
        public void GetDatesTuple_ShouldReturnTuple_WhenCalledWithValidInput()
        {
            //Arrange
            var age = 24;
            DateOnly todayUtc  = DateOnly.FromDateTime(DateTime.UtcNow);
            DateOnly birthdayTodayUtcBirthDate = todayUtc.AddYears(age * -1);
            DateOnly birthdayTomorrowUtcBirthDate = todayUtc.AddYears(age * -1).AddYears(-1).AddDays(1);

            var expectedResult = (birthdayTomorrowUtcBirthDate, birthdayTodayUtcBirthDate);

            //Act
            var actual = _bll.GetDatesTupleForAge(age);

            //Assert
            actual.Should().BeOfType<(DateOnly, DateOnly)>();
            actual.beginDate.Should().Be(expectedResult.birthdayTomorrowUtcBirthDate);
            actual.endDate.Should().Be(expectedResult.birthdayTodayUtcBirthDate);
        }

        [Fact]
        public async Task GetCustomersByAge_ShouldThrow_WhenCalledWithInvalidInput()
        {
            //Arrange
            var age = -1;

            //Act
            var exception = await Record.ExceptionAsync(() => _bll.GetCustomersByAgeAsync(age));

            Assert.IsType<ArgumentException>(exception);
        }
    }
}