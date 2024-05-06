using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SH.BLL;
using SH.Models.Customer;
using System;
using System.IO;
using System.Threading.Tasks;

namespace SafeHarborFunctionApp
{
    public class CustomerFunctions
    {
        private readonly ICustomerLogic _customerLogic;

        public CustomerFunctions(ICustomerLogic customerLogic)
        {
            this._customerLogic = customerLogic;
        }

        [FunctionName("AddCustomer")]
        public async Task<IActionResult> AddCustomer(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "customers")] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("'AddCustomer' HTTP trigger function - begin");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var createCustomerRequest = JsonConvert.DeserializeObject<CreateCustomerRequest>(requestBody);

            CreateCustomerRequestValidator validator = new CreateCustomerRequestValidator();

            ValidationResult result = validator.Validate(createCustomerRequest);
            if(result.Errors.Count > 0)
            {
                return new BadRequestObjectResult(result);
            }

            try
            {
                var newCustomer = await _customerLogic.AddCustomerAsync(createCustomerRequest.ToCreateCustomerDto());
                var uri = $"{req.HttpContext.Request.Scheme}://{req.HttpContext.Request.Host.Value}/api/customers/GUID/{newCustomer.CustomerId}";
                return new CreatedResult(uri, newCustomer);
            }
            catch (Exception e)
            {
                log.LogError(e, e.Message);
                var resultObj = new ObjectResult("Internal Server Error");
                resultObj.StatusCode = StatusCodes.Status500InternalServerError;
                return resultObj;
            }
        }

        [FunctionName("GetCustomerById")]
        public async Task<IActionResult> GetCustomerById(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "customers/GUID/{guid}")] HttpRequest req,
            ILogger log, string guid)
        {
            log.LogInformation("'GetCustomerById' HTTP trigger function - begin");

            try
            {
                if (guid is null)
                {
                    return new BadRequestObjectResult("CustomerId may not be null.");
                }
                if (guid == new Guid().ToString())
                {
                    return new BadRequestObjectResult("CustomerId may not be an empty Guid.");
                }

                try
                {
                    var g = new Guid(guid);
                }
                catch (Exception)
                {
                    return new BadRequestObjectResult("CustomerId is not a valid Guid.");
                }

                var customer = await _customerLogic.GetCustomerByIdAsync(guid);
                if (customer == null)
                {
                    return new NotFoundResult();
                }
                return new OkObjectResult(customer);
            }
            catch (Exception e)
            {
                var msg = $"Internal Server Error while retreiving customerId: {guid}";
                log.LogError(e, msg);
                var resultObj = new ObjectResult(msg);
                resultObj.StatusCode = StatusCodes.Status500InternalServerError;
                return resultObj;
            }
        }

        [FunctionName("GetCustomersByAge")]
        public async Task<IActionResult> GetCustomersByAge(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "customers/int/{age}")] HttpRequest req,
            ILogger log, int age)
        {
            log.LogInformation("'GetCustomerByAge' HTTP trigger function - begin");

            try
            {
                var customerList = await _customerLogic.GetCustomersByAgeAsync(age);
                return new OkObjectResult(customerList);
            }
            catch (ArgumentException e)
            {
                return new BadRequestObjectResult(e.Message);
            }
            catch (Exception e)
            {
                var msg = $"Internal Server Error while retreiving  customers by age: {age}";
                log.LogError(e, "msg");
                var resultObj = new ObjectResult(msg);
                resultObj.StatusCode = StatusCodes.Status500InternalServerError;
                return resultObj;
            }
        }
    }
}