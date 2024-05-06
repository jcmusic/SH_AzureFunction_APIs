using System.ComponentModel.DataAnnotations;

namespace SH.Models.Customer
{
    public class CreateCustomerRequest
    {
        public CreateCustomerRequest(string fullName, DateTime dateOfBirth)
        {
            FullName = fullName;
            DateOfBirth = dateOfBirth;
        }

        [Required]
        [MinLength(4)]
        public string FullName { get; set; }


        [Required]
        public DateTime DateOfBirth { get; set; }

        public CreateCustomerDto ToCreateCustomerDto()
        {
            return new CreateCustomerDto
            {
                FullName = FullName,
                DateOfBirth = DateOnly.FromDateTime(DateOfBirth)
            };
        }
    }
}
