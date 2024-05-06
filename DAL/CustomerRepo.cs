using AutoMapper;
using SH.DAL.Entities;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using SH.Models;
using SH.Models.Customer;
using SH.Models.Models;

namespace SH.DAL
{
    public class CustomerRepo : ICustomerRepo
    {
        #region Fields/Ctor

        private readonly CustomerDbContext _customerDbContext;
        private readonly IMapper _mapper;

        public CustomerRepo(CustomerDbContext customerDbContext, IMapper mapper)
        {
            _customerDbContext = customerDbContext;
            _mapper = mapper;
        }

        #endregion

        public async Task<CustomerDto> AddCustomerAsync(CreateCustomerDto customerDto)
        {
            var entity = _mapper.Map<Customer>(customerDto);

            await _customerDbContext.AddAsync<Customer>(entity);

            await SaveChangesAsync();

            return _mapper.Map<CustomerDto>(entity);
        }

        public async Task<CustomerDto> GetCustomerByIdAsync(Guid id)
        {
            var customer = await _customerDbContext.Customers
                .FirstOrDefaultAsync(x => x.CustomerId == id);

            var mappedCustomer = _mapper.Map<CustomerDto>(customer);

            return mappedCustomer;
        }

        public async Task<CustomerDto> GetCustomerByIdAsync(string id)
        {
            var customerId = Guid.Parse(id);
            var customer = await _customerDbContext.Customers
                .FirstOrDefaultAsync(x => x.CustomerId == customerId);

            var mappedCustomer = _mapper.Map<CustomerDto>(customer);

            return mappedCustomer;
        }

        public async Task<(List<CustomerDto>, PaginationMetadata)> GetCustomersAsync(int age, int pageNumber = 0, int pageSize = -1)
        {
            throw new NotImplementedException();

            //var searchAgeStart = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(age * -1));
            //var searchAgeEnd = searchAgeStart.AddYears(1);

            //var predicate = PredicateBuilder.New<Customer>();
            //predicate.Start(x => searchAgeStart < x.DateOfBirth && x.DateOfBirth < searchAgeEnd);

            //var query = _customerDbContext.Customers
            //    .AsNoTracking()
            //    .Where(predicate);

            //var totalCount = await query.CountAsync();

            //if (pageSize != -1)
            //{
            //    query = query
            //    .Skip(pageNumber * pageSize)
            //    .Take(pageSize);
            //}

            //var customers = await query.ToListAsync();

            //var paginationMetadata = new PaginationMetadata(totalCount, pageSize, pageNumber);
            //var mappedCustomerList = _mapper.Map<List<CustomerDto>>(customers);

            //return (mappedCustomerList, paginationMetadata);
        }

        public Task<List<CustomerDto>> GetCustomersByAgeAsync(DateOnly beginDate, DateOnly endDate)
        {
            throw new NotImplementedException();

            //var searchAgeStart = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(age * -1));
            //var searchAgeEnd = searchAgeStart.AddYears(1);

            //var customers = _customerDbContext.Customers
            //    .AsNoTracking()
            //    .Where(x => searchAgeStart < x.DateOfBirth)
            //    .Where(x => x.DateOfBirth < searchAgeEnd)
            //    .ToListAsync();

            //var mappedCustomerList = _mapper.Map<List<CustomerDto>>(customers);

            //return (mappedCustomerList);
        }

        public async Task<bool> SaveChangesAsync()
        {
            return (await _customerDbContext.SaveChangesAsync() >= 1);
        }
    }
}
