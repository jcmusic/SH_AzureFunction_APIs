namespace SH.DAL.Entities
{
    public class Customer
    {
        public Guid CustomerId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }
    }
}
