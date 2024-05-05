using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SH.BLL;
using SH.Models.Customer;
using SQLitePCL;
using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace SafeHarborFunctionApp
{
    //public CustomerEndpoints(IHttpClientFactory httpClientFactory)
    //{
    //    this._client = httpClientFactory.CreateClient();
    //}

    //[FunctionName("MyHttpTrigger")]
    //public async Task<IActionResult> Run(
    //    [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
    //    ILogger log)
    //{
    //    var response = await _client.GetAsync("https://microsoft.com");
    //    var message = _service.GetMessage();

    //    return new OkObjectResult("Response from function with injected dependencies.");
    //}


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

            var customer = await _customerLogic.AddCustomerAsync(createCustomerRequest);

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

            return new OkObjectResult(customer);
        }

        [FunctionName("GetCustomerById")]
        public async Task<IActionResult> GetCustomerById(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "customers/{guid}")] HttpRequest req,
            ILogger log, string guid)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

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
    }
}
