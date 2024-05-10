namespace SH.Models.Customer
{
    public class CustomerDto
    {
        public CustomerDto()
        { }
        public CustomerDto(Guid customerId, string fullName, DateOnly dateOfBirth, byte[] avatar, byte[] profileImage)
        {
            CustomerId = customerId;
            FullName = fullName;
            DateOfBirth = dateOfBirth;
            Avatar = avatar;
            ProfileImage = profileImage;
        }

        public Guid CustomerId { get; set; }
        public string? FullName { get; set; } = String.Empty;
        public DateOnly DateOfBirth { get; set; }
        public byte[]? Avatar { get; set; }
        public byte[]? ProfileImage { get; set; }
    }
}
