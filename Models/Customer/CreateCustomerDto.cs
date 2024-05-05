namespace SH.Models.Customer
{
    public class CreateCustomerDto
    {
        public string FullName { get; set; } = string.Empty;
        public DateOnly DateOfBirth { get; set; }

        public static CreateCustomerDto New(CreateCustomerRequest request)
        {
            return new CreateCustomerDto
            {

                FullName = request.FullName,
                DateOfBirth = DateOnly.FromDateTime(request.DateOfBirth)
            };
        }
    }
}
