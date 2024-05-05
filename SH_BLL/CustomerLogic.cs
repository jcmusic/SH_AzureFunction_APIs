using Microsoft.Extensions.Logging;
using SH.Models.Customer;
using SH.Models.Models;

namespace SH.BLL
{
    public interface ICustomerLogic
    {
        Task<CustomerDto> AddCustomerAsync(CreateCustomerRequest request);
        Task<CustomerDto> GetCustomerByIdAsync(string customerId);
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

            //var newDto = new CustomerDto
            //{
            //    CustomerId = Guid.NewGuid(),
            //    FullName = request.FullName,
            //    DateOfBirth = DateOnly.FromDateTime(request.DateOfBirth)
            //};

            return await Task.FromResult<CustomerDto>(newDto);
        }

        public async Task<CustomerDto> GetCustomerByIdAsync(string customerId)
        {
            //return await _customerRepo.GetCustomerByIdAsync(customerId);
            var newDto = await _customerRepo.GetCustomerByIdAsync(customerId);
            return await Task.FromResult<CustomerDto>(newDto);
        }


        //try
        //{
        //    var newCustomer = await _customerRepo.AddCustomerAsync(dto);


        ////}
        ////catch (Exception)
        ////{
        ////    return HttpStatusCode.BadRequest;
        ////}
        //return newCustomer;
        //}


        //public async Task<IActionResult> Create(Movie movie)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return View(movie);
        //    }

        //    _context.Movies.Add(movie);
        //    await _context.SaveChangesAsync();

        //    return RedirectToAction(nameof(Index));
        //}
    }
}
