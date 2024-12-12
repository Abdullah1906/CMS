namespace courierMs.ViewModel
{
    public class ReportVM
    {
        public CustomerInfo? Senderinfo { get; set; }
        public CustomerInfo? Receiverinfo { get; set; }
        public HolderInfo? Holderinfo { get; set; }
        public PercelInfo? Percelinfo { get; set; }
        public double Discount { get; set; }
        public double Total { get; set; }

    }
    public class CustomerInfo
    {
        public Guid CustomerId { get; set; }
        public string? Name { get; set; }
        public string? Number { get; set; }
        public string? Address { get; set; }
        public string? Email { get; set; }
    }
    public class HolderInfo
    {
        public Guid CustomerId { get; set; }
        public string? InvoiceId { get; set; }
        public string? HolderName { get; set; }
        public DateTime Date { get; set; }

    }
    public class PercelInfo
    {
        public string? PercelType { get; set; }
        public double Weight { get; set; }
        public double Price { get; set; }
        public string? Note { get; set; }

    }
}
