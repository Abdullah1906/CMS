﻿namespace courierMs.ViewModel
{
    public class PercelinfoVM
    {
        public int Id { get; set; }
        public Guid ParcelId { get; set; }
        public string? ParcelType { get; set; }
        public double Weight { get; set; }
        public double Price { get; set; }
        public string? Note { get; set; }
        public Guid SenderId { get; set; }
        public Guid ReceiverId { get; set; }

        public string? InvoiceId { get; set; }


        public DateTime CreatedAt { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTime UpdatedAt { get; set; }
        public Guid UpdatedBy { get; set; }
    }
}
