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
using SH.Models.Models;
using System.Text;

namespace SH.Tests.IntegrationTests
{
    public class Customer_Logic_Factory
    {
        private readonly ILogger<CustomerLogic> _logger = NullLogger<CustomerLogic>.Instance;
        private readonly ICustomerLogic _customerLogic = Substitute.For<ICustomerLogic>();
        private readonly ICustomerRepo _customerRepo = Substitute.For<ICustomerRepo>();
        public CustomerLogic GetCustomerLogic()
        {
            return new CustomerLogic(_logger, _customerRepo);
        }
    }

    public class AzureFN_Customer_Tests
    {
        private readonly ICustomerLogic _customerLogic = new Customer_Logic_Factory().GetCustomerLogic();
        private readonly CustomerFunctions _sut;
        private readonly ILogger<CustomerFunctions> _logger = NullLogger<CustomerFunctions>.Instance;
        private readonly Guid _customerId = Guid.NewGuid();
        private readonly Guid _emptyGuid = new Guid();
        private readonly DateTime _defaultDobDatetime = new DateTime(1999, 1, 1);
        private readonly string _defaultFullName = "John Doe";

        public CustomerDto ResponseCustomer
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

        public AzureFN_Customer_Tests()
        {
            _sut = new CustomerFunctions(_customerLogic);
        }

        [Fact]
        public async Task Get_CustById_ShouldContainOkObjectResult_WhenCalledWithValidInput()
        {
            //Arrange
            var responseCustomer = ResponseCustomer;
            responseCustomer.CustomerId = _customerId;
            var getRequest = RequestFactory.GetHttpRequest(_customerId);
            var expectedResult = responseCustomer;

            //Act
            var response = await _sut.GetCustomerById(getRequest, _logger, _customerId.ToString());

            //Assert
            response.Should().BeOfType<OkObjectResult>();
            var actualValue = ((OkObjectResult)response).Value;
            actualValue.Should().BeOfType<CustomerDto>();
            CustomerDto typedActualResult = (CustomerDto)actualValue;
            typedActualResult.CustomerId.Should().NotBe(_emptyGuid);
            typedActualResult.DateOfBirth.Should().Be(expectedResult.DateOfBirth);
            typedActualResult.FullName.Should().Be(expectedResult.FullName);
        }

        [Fact]
        public async Task Post_AddCust_ShouldContainOkObjectResult_WhenCalledWithValidInput()
        {
            //Arrange
            var requestCustomer = new CreateCustomerRequest(ResponseCustomer.FullName, _defaultDobDatetime);
            var postRequest = RequestFactory.PostHttpRequest(requestCustomer);
            var expectedResult = ResponseCustomer;

            //Act
            var response = await _sut.AddCustomer(postRequest, _logger);

            //Assert
            response.Should().BeOfType<OkObjectResult>();
            var actualValue = ((OkObjectResult)response).Value;
            CustomerDto typedActualResult = (CustomerDto)actualValue;
            typedActualResult.CustomerId.Should().NotBe(_emptyGuid);
            typedActualResult.DateOfBirth.Should().Be(expectedResult.DateOfBirth);
            typedActualResult.FullName.Should().Be(expectedResult.FullName);
        }

        //[Fact]
        //public async Task Get_CustsByAge_ShouldContainOkObjectResult_WhenCalledWithValidInput()
        //{
        //    //Arrange
        //    var responseCustomer = ExpectedResponseCustomer;
        //    responseCustomer.CustomerId = _customerId;
        //    var getRequest = RequestFactory.GetHttpRequest(_customerId);
        //    var expectedResult = responseCustomer;

        //    //Act
        //    var response = await CustomerFunctions.GetCustomerByIdAsync(getRequest, _logger, _customerId);

        //    //Assert
        //    response.Should().BeOfType<OkObjectResult>();
        //    var actualValue = ((OkObjectResult)response).Value;
        //    actualValue.Should().BeOfType<CustomerDto>();
        //    CustomerDto typedActualResult = (CustomerDto)actualValue;
        //    typedActualResult.CustomerId.Should().NotBe(_emptyGuid);
        //    typedActualResult.DateOfBirth.Should().Be(expectedResult.DateOfBirth);
        //    typedActualResult.FullName.Should().Be(expectedResult.FullName);
        //}
    }

    public static class RequestFactory
    {
        public static HttpRequest GetHttpRequest(Guid customerId)
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