using AutoMapper;
using SH.DAL.Entities;
using SH.Models.Customer;

namespace SH.DAL.Automapper
{
    public class AutoMapProfiles : Profile
    {
        public AutoMapProfiles()
        {
            CreateMap<Customer, CustomerDto>().ReverseMap();
            CreateMap<CreateCustomerDto, Customer>();
            CreateMap<CreateCustomerRequest, CreateCustomerDto>();
        }
    }
}