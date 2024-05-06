using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;
using SH.Models.Customer;
using SH.Models.Models;

[assembly: InternalsVisibleTo("SH.Tests")]

namespace SH.BLL
{
    public interface ICustomerLogic
    {
        Task<CustomerDto> AddCustomerAsync(CreateCustomerDto dto);
        Task<CustomerDto> GetCustomerByIdAsync(string customerId);
        Task<List<CustomerDto>> GetCustomersByAgeAsync(int age);
        (DateOnly beginDate, DateOnly endDate) GetDatesTupleForAge(int age);
    }

    public class CustomerLogic : ICustomerLogic
    {
        #region Ctor/Fields

        private readonly ILogger<CustomerLogic> _logger;
        private readonly ICustomerRepo _customerRepo;

        public CustomerLogic(ILogger<CustomerLogic> logger, ICustomerRepo customerRepo)
        {
            _logger = logger;
            _customerRepo = customerRepo;
        }

        #endregion

        public async Task<CustomerDto> AddCustomerAsync(CreateCustomerDto dto)
        {
            var newDto = await _customerRepo.AddCustomerAsync(dto);

            return await Task.FromResult<CustomerDto>(newDto);
        }

        public async Task<CustomerDto?> GetCustomerByIdAsync(string customerId)
        {
            //return await _customerRepo.GetCustomerByIdAsync(customerId);
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
