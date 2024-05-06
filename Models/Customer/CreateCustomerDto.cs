namespace SH.Models.Customer
{
    public class CreateCustomerDto
    {
        public string FullName { get; set; } = string.Empty;
        public DateOnly DateOfBirth { get; set; }
    }
}
