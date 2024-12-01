namespace courierMs.ViewModel
{
    public class ReceiverInfoVM
    {
        public int Id { get; set; }
        public Guid RecieverId { get; set; }
        public string? Name { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }
        public string? Note { get; set; }
        public string? city { get; set; }

        //for admin
        public DateTime CreatedAt { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTime UpdatedAt { get; set; }
        public Guid UpdatedBy { get; set; }
    }
}
