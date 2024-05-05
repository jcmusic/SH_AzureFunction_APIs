using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Newtonsoft.Json;
using NSubstitute;
using SafeHarborFunctionApp;
using SH.BLL;
using SH.Models.Customer;
using System.Text;

namespace SH.Tests.UnitTests
{
    public class AzureFN_Customer_Tests
    {
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
            _sut = new CustomerFunctions(_httpClientFactory, _customerLogic);
        }

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
        public async Task Post_AddCust_ShouldContainOkObjectResult_WhenCalledWithValidInput()
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
            actualResponse.Should().BeOfType<OkObjectResult>();
            var actualValue = ((OkObjectResult)actualResponse).Value;
            CustomerDto typedActualResult = (CustomerDto)actualValue;
            typedActualResult.CustomerId.Should().Be(expectedResult.CustomerId);
            typedActualResult.DateOfBirth.Should().Be(expectedResult.DateOfBirth);
            typedActualResult.FullName.Should().Be(expectedResult.FullName);
        }
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