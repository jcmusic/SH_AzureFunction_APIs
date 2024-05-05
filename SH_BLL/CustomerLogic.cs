using Microsoft.Extensions.Logging;
using SH.Models.Customer;
using SH.Models.Models;

namespace SH.BLL
{
    public interface ICustomerLogic
    {
        Task<CustomerDto> AddCustomerAsync(CreateCustomerRequest request);
        Task<CustomerDto> GetCustomerByIdAsync(string customerId);
        Task<List<CustomerDto>> GetCustomersByAgeAsync(int age);
    }
    public class CustomerLogic : ICustomerLogic
    {
        private readonly ILogger<CustomerLogic> _logger;
        private readonly ICustomerRepo _customerRepo;

        public CustomerLogic(ILogger<CustomerLogic> logger, ICustomerRepo customerRepo)
        {
            _logger = logger;
            _customerRepo = customerRepo;
        }

        public async Task<CustomerDto> AddCustomerAsync(CreateCustomerRequest request)
        {
            var dto = CreateCustomerDto.New(request);
            var newDto = await _customerRepo.AddCustomerAsync(dto);

            return await Task.FromResult<CustomerDto>(newDto);
        }

        public async Task<CustomerDto> GetCustomerByIdAsync(string customerId)
        {
            //return await _customerRepo.GetCustomerByIdAsync(customerId);
            var newDto = await _customerRepo.GetCustomerByIdAsync(customerId);
            return await Task.FromResult<CustomerDto>(newDto);
        }

        public async Task<List<CustomerDto>> GetCustomersByAgeAsync(int age)
        {
            var dtoList = await _customerRepo.GetCustomersByAgeAsync(age);
            return await Task.FromResult<List<CustomerDto>>(dtoList);
        }
    }
}
