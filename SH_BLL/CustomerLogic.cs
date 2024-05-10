using Microsoft.Extensions.Logging;
using SH.Models.Customer;
using SH.Models.Models;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("SH.Tests")]

namespace SH.BLL
{
    public interface ICustomerLogic
    {
        Task<CustomerDto> AddCustomerAsync(CreateCustomerDto dto);
        Task GetAndPersistCustomerProfileImage(CustomerDto dto);
        Task<CustomerDto?> GetCustomerByIdAsync(string customerId);
        Task<List<CustomerDto>> GetCustomersByAgeAsync(int age);
        (DateOnly beginDate, DateOnly endDate) GetDatesTupleForAge(int age);
    }

    public class CustomerLogic : ICustomerLogic
    {
        #region Ctor/Fields

        private readonly ILogger<CustomerLogic> _logger;
        private readonly ICustomerRepo _customerRepo;
        private readonly HttpClient _httpClient;

        public CustomerLogic(ILogger<CustomerLogic> logger, 
            ICustomerRepo customerRepo, HttpClient httpClient)
        {
            _logger = logger;
            _customerRepo = customerRepo;
            _httpClient = httpClient;
        }

        #endregion

        public async Task<CustomerDto> AddCustomerAsync(CreateCustomerDto dto)
        {
            var newDto = await _customerRepo.AddCustomerAsync(dto);

            if (newDto is not null && newDto.ProfileImage == null)
            {
                await SendRequestToPopulateImageAsync(newDto);
            }

            return await Task.FromResult<CustomerDto>(newDto);
        }

        /// <summary>
        /// Update to place a message in a msgQueue and update the Az function trigger to queue
        /// </summary>
        /// <param name="newDto"></param>
        /// <returns></returns>
        private async Task SendRequestToPopulateImageAsync(CustomerDto? newDto)
        {
            var uriBuilder = new UriBuilder
            {
                Scheme = "http",
                Host = "localhost",
                Port = 7079,
                Path = $"api/customers/populateprofileImage/{newDto.CustomerId}"
            };

            await _httpClient.GetAsync(uriBuilder.ToString());
        }

        public async Task GetAndPersistCustomerProfileImage(CustomerDto dto)
        {
            var imageByteArray = await _httpClient.GetByteArrayAsync("https://ui-avatars.com/api/?John+Doe&format=svg");
            await _customerRepo.PersistImageToCustomerDBAsync(dto.CustomerId, imageByteArray);
        }

        public async Task<CustomerDto?> GetCustomerByIdAsync(string customerId)
        {
            var newDto = await _customerRepo.GetCustomerByIdAsync(customerId);
            return await Task.FromResult(newDto);
        }

        public async Task<List<CustomerDto>> GetCustomersByAgeAsync(int age)
        {
            if(age < 0) throw new ArgumentException("Age may not be less than 0");

            (DateOnly beginDate, DateOnly endDate) agePredicate = GetDatesTupleForAge(age);

            var dtoList = await _customerRepo.GetCustomersByAgeAsync(agePredicate.beginDate, agePredicate.endDate);
            return await Task.FromResult<List<CustomerDto>>(dtoList);
        }

        /// <summary>
        /// Derives UTC-based beginning and ending birth dates for a given age (int) input.
        /// Tried to keep this internal w/ [assembly: InternalsVisibleTo("SH.Tests")], but didn't work.
        /// </summary>
        /// <param name="age"></param>
        /// <returns></returns>
        public (DateOnly beginDate, DateOnly endDate) GetDatesTupleForAge(int age)
        {
            DateOnly todayUtc = DateOnly.FromDateTime(DateTime.UtcNow);
            DateOnly endDate_birthdayTodayUtcBirthDate = todayUtc.AddYears(age * -1);
            DateOnly beginDate_birthdayTomorrowUtcBirthDate = todayUtc.AddYears(age * -1).AddYears(-1).AddDays(1);
            return (beginDate_birthdayTomorrowUtcBirthDate, endDate_birthdayTodayUtcBirthDate);
        }
    }
}
