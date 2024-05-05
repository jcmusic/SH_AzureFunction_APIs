namespace SH.Models.Customer
{
    public class CustomerResponse
    {
        public CustomerResponse(Guid customerId, string fullName, DateOnly dateOfBirth)
        {
            CustomerId = customerId;
            FullName = fullName;
            DateOfBirth = dateOfBirth;
        }

        public Guid CustomerId { get; set; }
        public string FullName { get; set; }
        public DateOnly DateOfBirth { get; set; }
    }
}
