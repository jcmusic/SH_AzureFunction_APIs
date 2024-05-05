﻿using SH.Models.Customer;

namespace SH.Models.Models
{
    public interface ICustomerRepo
    {
        Task<CustomerDto> AddCustomerAsync(CreateCustomerDto dto);
        Task<CustomerDto> GetCustomerByIdAsync(string id);
        Task<List<CustomerDto>> GetCustomersByAgeAsync(int age);
        //Task<(List<CustomerDto>, PaginationMetadata)> GetCustomersAsync(Predicate<int> agePredicate, int pageNumber, int pageSize);
    }
}