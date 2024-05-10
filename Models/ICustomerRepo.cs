using SH.Models.Customer;

namespace SH.Models.Models
{
    public interface ICustomerRepo
    {
        Task<CustomerDto> AddCustomerAsync(CreateCustomerDto dto);
        Task<CustomerDto?> GetCustomerByIdAsync(string id);
        Task<List<CustomerDto>> GetCustomersByAgeAsync(DateOnly beginDate, DateOnly endDate);
        Task PersistImageToCustomerDBAsync(Guid customerId, byte[] imageByteArray);

        //Task<(List<CustomerDto>, PaginationMetadata)> GetCustomersAsync(Predicate<int> agePredicate, int pageNumber, int pageSize);
    }
}