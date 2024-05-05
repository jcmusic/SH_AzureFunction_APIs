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
using System.Net.Http;
using System.Threading.Tasks;

namespace SafeHarborFunctionApp
{
    public class CustomerFunctions
    {
        private readonly HttpClient _client;
        //private readonly IImageService _imageService;
        private readonly ICustomerLogic _customerLogic;

        public CustomerFunctions(IHttpClientFactory httpClientFactory,
            ICustomerLogic customerLogic) //, IImageService imageService
        {
            this._client = httpClientFactory.CreateClient();
            this._customerLogic = customerLogic;
            //this._imageService = imageService;
        }

        [FunctionName("AddCustomer")]
        public async Task<IActionResult> AddCustomer(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "customers")] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("'AddCustomer' HTTP trigger function - begin");

            //
            //var response = await _client.GetAsync("https://microsoft.com");
            //


            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var createCustomerRequest = JsonConvert.DeserializeObject<CreateCustomerRequest>(requestBody);

            var newCustomer = await _customerLogic.AddCustomerAsync(createCustomerRequest);

            // validate the createCustomerRequest object


            //if (!createCustomerRequest.IsValid())
            //{

            //}
            //try
            //{
            //    var dto = CreateCustomerDto.New(createCustomerRequest);
            //}
            //catch (Exception)
            //{
            //    return new BadRequestObjectResult()
            //}

            var uri = $"{req.HttpContext.Request.Scheme}://{req.HttpContext.Request.Host.Value}/api/customers/GUID/{newCustomer.CustomerId}";

            return new CreatedResult(uri, newCustomer);
        }

        [FunctionName("GetCustomerById")]
        public async Task<IActionResult> GetCustomerById(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "customers/GUID/{guid}")] HttpRequest req,
            ILogger log, string guid)
        {
            log.LogInformation("'GetCustomerById' HTTP trigger function - begin");

            try
            {
                var customer = await _customerLogic.GetCustomerByIdAsync(guid);
                if (customer == null)
                {
                    return new NotFoundResult();
                }
                return new OkObjectResult(customer);
            }
            catch (Exception e)
            {
                var msg = $"Error getting customer by id: {guid}";
                log.LogError(e, "msg");
                return new OkObjectResult(msg);
            }
        }

        [FunctionName("GetCustomerByAge")]
        public async Task<IActionResult> GetCustomerByAge(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "customers/int/{age}")] HttpRequest req,
            ILogger log, int age)
        {
            log.LogInformation("'GetCustomerByAge' HTTP trigger function - begin");

            try
            {
                var customerList = await _customerLogic.GetCustomersByAgeAsync(age);
                //if (customers == null)
                //{
                //    return new NotFoundResult();
                //}
                return new OkObjectResult(customerList);
            }
            catch (Exception e)
            {
                var msg = $"Error getting customers by age: {age}";
                log.LogError(e, "msg");
                return new OkObjectResult(msg);
            }
        }
    }
}


//public async Task<IActionResult> Create()
//{
//    if (!ModelState.IsValid)
//    {
//        return View(movie);
//    }

//    _context.Movies.Add(movie);
//    await _context.SaveChangesAsync();

//    return RedirectToAction(nameof(Index));
//}