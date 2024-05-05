using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using SH.BLL;
using SH.Models.Customer;
using SH.Models.Models;

namespace SH.Tests.UnitTests
{

    //public class CollectionName_Tests
    //{
    //    [Fact]
    //    public void TestName()
    //    {
    //        //Arrange


    //        //Act


    //        //Assert
    //    }
    //}

    public class Customer_Logic_Tests
    {
        private readonly ILogger<CustomerLogic> _logger;
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
            _bll = new CustomerLogic(_logger, _customerRepo);
        }

        [Fact]
        public async Task AddCustomer_ShouldReturnCustomerDto_WhenCalledWithValidInput()
        {
            //Arrange
            var createCustomerRequest = new CreateCustomerRequest(_defaultFullName, _defaultDobDatetime);
            var expectedResult = ExpectedResponseCustomer;

            //Mocks
            _customerRepo.AddCustomerAsync(default)
                .ReturnsForAnyArgs(expectedResult);

            //Act
            var actual = await _bll.AddCustomerAsync(createCustomerRequest);

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
    }
}