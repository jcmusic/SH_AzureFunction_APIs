using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Newtonsoft.Json;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using NSubstitute.ReturnsExtensions;
using SafeHarborFunctionApp;
using SH.BLL;
using SH.Models.Customer;
using System;
using System.Text;

namespace SH.Tests.UnitTests
{
    public class AzureFN_Customer_Tests
    {
        #region Ctor/Fields

        private readonly CustomerFunctions _sut;
        private readonly ILogger<CustomerFunctions> _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ICustomerLogic _customerLogic;

        private readonly Guid _customerId = Guid.NewGuid();
        private readonly DateTime _defaultDobDatetime = new DateTime(1999, 1, 1);
        private readonly string _defaultFullName = "John Doe";

        public CustomerDto ExpectedResponseCustomer
        {
            get
            {
                return new CustomerDto
                {
                    CustomerId = _customerId,
                    FullName = _defaultFullName,
                    DateOfBirth = DateOnly.FromDateTime(_defaultDobDatetime)
                };
            }
        }

        public AzureFN_Customer_Tests()
        {
            _logger = NullLogger<CustomerFunctions>.Instance; 
            _httpClientFactory = Substitute.For<IHttpClientFactory>();
            _customerLogic = Substitute.For<ICustomerLogic>();
            _sut = new CustomerFunctions(_customerLogic);
        }

        #endregion


        #region AddCustomer

        [Fact]
        public async Task Post_AddCust_ShouldReturnOkObjectResult_WhenCalledWithValidInput()
        {
            //Arrange
            var requestCustomer = new CreateCustomerRequest(_defaultFullName, _defaultDobDatetime);
            var postRequest = RequestFactory.PostHttpRequest(requestCustomer);
            var expectedResult = ExpectedResponseCustomer;

            //Mock
            _customerLogic.AddCustomerAsync(default)
                .ReturnsForAnyArgs(expectedResult);

            //Act
            var actualResponse = await _sut.AddCustomer(postRequest, _logger);

            //Assert
            actualResponse.Should().BeOfType<CreatedResult>();
            var actualValue = ((CreatedResult)actualResponse).Value;
            CustomerDto typedActualResult = (CustomerDto)actualValue;
            typedActualResult.CustomerId.Should().Be(expectedResult.CustomerId);
            typedActualResult.DateOfBirth.Should().Be(expectedResult.DateOfBirth);
            typedActualResult.FullName.Should().Be(expectedResult.FullName);
        }

        [Fact]
        public async Task Post_AddCust_ShouldReturnBadRequestObjectResult_WhenCalledWithInvalidName()
        {
            //Arrange
            var requestCustomer = new CreateCustomerRequest("Joe", _defaultDobDatetime);
            var postRequest = RequestFactory.PostHttpRequest(requestCustomer);

            //Act
            var actualResponse = await _sut.AddCustomer(postRequest, _logger);

            //Assert
            actualResponse.Should().BeOfType<BadRequestObjectResult>();
            var msg = actualResponse.As<BadRequestObjectResult>();
            msg.Equals("FullName minimum length is 4 characters");
        }

        [Fact]
        public async Task Post_AddCust_ShouldReturnBadRequestObjectResult_WhenCalledWithEmptyName()
        {
            //Arrange
            var requestCustomer = new CreateCustomerRequest(String.Empty, _defaultDobDatetime);
            var postRequest = RequestFactory.PostHttpRequest(requestCustomer);

            //Act
            var actualResponse = await _sut.AddCustomer(postRequest, _logger);

            //Assert
            actualResponse.Should().BeOfType<BadRequestObjectResult>();
            var msg = actualResponse.As<BadRequestObjectResult>();
            msg.Equals("FullName is required");
        }

        [Fact]
        public async Task Post_AddCust_ShouldReturnBadRequestObjectResult_WhenCalledWithFutureDoB()
        {
            //Arrange
            var requestCustomer = new CreateCustomerRequest("John Doe", DateTime.Today.AddDays(1));
            var postRequest = RequestFactory.PostHttpRequest(requestCustomer);

            //Act
            var actualResponse = await _sut.AddCustomer(postRequest, _logger);

            //Assert
            actualResponse.Should().BeOfType<BadRequestObjectResult>();
            var msg = actualResponse.As<BadRequestObjectResult>();
            msg.Equals("DateOfBirth may not be a future date");
        }

        #endregion

        #region CustomerById

        [Fact]
        public async Task Get_CustById_ShouldContainOkObjectResult_WhenCalledWithValidInput()
        {
            //Arrange
            var responseCustomer = ExpectedResponseCustomer;
            responseCustomer.CustomerId = _customerId;
            var getRequest = RequestFactory.GetHttpRequest(_customerId.ToString());
            var expectedResult = responseCustomer;

            //Mock
            _customerLogic.GetCustomerByIdAsync(_customerId.ToString())
                .ReturnsForAnyArgs(expectedResult);

            //Act
            var actualResponse = await _sut.GetCustomerById(getRequest, _logger, _customerId.ToString());

            //Assert
            actualResponse.Should().BeOfType<OkObjectResult>();
            var actualValue = ((OkObjectResult)actualResponse).Value;
            actualValue.Should().BeOfType<CustomerDto>();
            CustomerDto typedActualResult = (CustomerDto)actualValue;
            typedActualResult.CustomerId.Should().Be(expectedResult.CustomerId);
            typedActualResult.DateOfBirth.Should().Be(expectedResult.DateOfBirth);
            typedActualResult.FullName.Should().Be(expectedResult.FullName);
        }

        [Fact]
        public async Task Get_CustById_ShouldReturn404NotFoundObjectResult_WhenCalledWithNonExistantValidCustomerId()
        {
            //Arrange
            var customerId = "11111111-1111-1111-1111-111111111111";
            var getRequest = RequestFactory.GetHttpRequest(customerId);

            //Mock
            _customerLogic.GetCustomerByIdAsync(customerId).ReturnsNull();

            //Act
            var actualResponse = await _sut.GetCustomerById(getRequest, _logger, customerId);

            //Assert
            actualResponse.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task Get_CustById_ShouldReturnBadRequest_WhenCalledWithEmptyGuid()
        {
            //Arrange
            var customerId = "00000000-0000-0000-0000-000000000000"; //Empty Guid
            var getRequest = RequestFactory.GetHttpRequest(customerId);

            //Act
            var actualResponse = await _sut.GetCustomerById(getRequest, _logger, customerId);

            //Assert
            actualResponse.Should().BeOfType<BadRequestObjectResult>();
            var msg = actualResponse.As<BadRequestObjectResult>();
            msg.Equals("CustomerId may not be an empty Guid.");
        }

        [Fact]
        public async Task Get_CustById_ShouldReturnBadRequest_WhenCalledWithInvalidCustomerId()
        {
            //Arrange
            var customerId = "11111111-1111-1111-1111-1111111"; //Not a valid Guid
            var getRequest = RequestFactory.GetHttpRequest(customerId);

            //Act
            var actualResponse = await _sut.GetCustomerById(getRequest, _logger, customerId);

            //Assert
            actualResponse.Should().BeOfType<BadRequestObjectResult>();
            var msg = actualResponse.As<BadRequestObjectResult>();
            msg.Equals("CustomerId is not a valid Guid.");
        }

        [Fact]
        public async Task Get_CustById_ShouldReturnBadRequest_WhenCalledWithNullCustomerId()
        {
            //Arrange
            string? customerId = null;
            var getRequest = RequestFactory.GetHttpRequest();

            //Act
            var actualResponse = await _sut.GetCustomerById(getRequest, _logger, customerId);

            //Assert
            actualResponse.Should().BeOfType<BadRequestObjectResult>();
            var msg = actualResponse.As<BadRequestObjectResult>();
            msg.Equals("CustomerId may not be null.");
        }

        #endregion

        #region CustomersByAge

        [Fact]
        public async Task Get_CustomersByAge_ShouldReturnEmptyOkObjectResult_WhenCalledWithValidInput()
        {
            //Arrange
            var age = 13;
            var getRequest = RequestFactory.GetHttpRequest();
            var expectedResult = new List<CustomerDto>();

            //Mock
            _customerLogic.GetCustomersByAgeAsync(age)
                .Returns<List<CustomerDto>>(new List<CustomerDto>());

            //Act
            var actualResponse = await _sut.GetCustomersByAge(getRequest, _logger, age);

            //Assert
            actualResponse.Should().BeOfType<OkObjectResult>();
            var actualValue = ((OkObjectResult)actualResponse).Value;
            actualValue.Should().BeOfType<List<CustomerDto>>();
            List<CustomerDto> typedActualResult = (List<CustomerDto>)actualValue;
            Assert.Empty(typedActualResult);
        }

        [Fact]
        public async Task Get_CustomersByAge_ShouldReturnPopulatedOkObjectResult_WhenCalledWithValidInput()
        {
            //Arrange
            var age = 24;
            var getRequest = RequestFactory.GetHttpRequest();
            var expectedResult = new List<CustomerDto>();
            expectedResult.Add(new CustomerDto
            {
                FullName = "Person1",
                CustomerId = Guid.NewGuid(),
                DateOfBirth = new DateOnly(1999, 12, 31)
            });
            expectedResult.Add(new CustomerDto
            {
                FullName = "Person2",
                CustomerId = Guid.NewGuid(),
                DateOfBirth = new DateOnly(2000, 1, 1)
            });

            //Mock
            _customerLogic.GetCustomersByAgeAsync(age)
                .Returns<List<CustomerDto>>(expectedResult);

            //Act
            var actualResponse = await _sut.GetCustomersByAge(getRequest, _logger, age);

            //Assert
            actualResponse.Should().BeOfType<OkObjectResult>();
            var actualValue = ((OkObjectResult)actualResponse).Value;
            actualValue.Should().BeOfType<List<CustomerDto>>();
            List<CustomerDto> typedActualResult = (List<CustomerDto>)actualValue;
            Assert.NotEmpty(typedActualResult);
            Assert.Equal(2, typedActualResult.Count);
        }

        [Fact]
        public async Task Get_CustomersByAge_ShouldBadRequestObjectResult_WhenCalledWithInvalidInput()
        {
            //Arrange
            var age = -1;
            var getRequest = RequestFactory.GetHttpRequest();

            //Mock
            _customerLogic.GetCustomersByAgeAsync(age)
                .ThrowsAsync(new ArgumentException("Age may not be less than 0"));

            //Act
            var actualResponse = await _sut.GetCustomersByAge(getRequest, _logger, age);

            //Assert
            actualResponse.Should().BeOfType<BadRequestObjectResult>();
            var msg = actualResponse.As<BadRequestObjectResult>();
            msg.Equals("Age may not be less than 0");
        }

        #endregion
    }

    public static class RequestFactory
    {
        public static HttpRequest GetHttpRequest(string customerId)
        {
            var request = new DefaultHttpContext().Request;
            request.Query = new QueryCollection(new Dictionary<string, Microsoft.Extensions.Primitives.StringValues>
            {
                { "customerId", customerId.ToString() }
            });

            return request;
        }
        public static HttpRequest GetHttpRequest()
        {
            var request = new DefaultHttpContext().Request;
            return request;
        }
        public static HttpRequest PostHttpRequest(CreateCustomerRequest dto)
        {
            var request = new DefaultHttpContext().Request;
            var dtoString = JsonConvert.SerializeObject(dto);
            var ms = new MemoryStream(Encoding.UTF8.GetBytes(dtoString));
            request.Body = ms; 
            request.Headers.Add("Content-Length", ms.Length.ToString());
            request.ContentType = "application/json";
            return request;
        }
    }
}