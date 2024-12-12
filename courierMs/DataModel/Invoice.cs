using System.ComponentModel.DataAnnotations;

namespace courierMs.DataModel
{
    public class Invoice
    {
        [Key]
        public string? InvoiceId { get; set; }

        public string? Email { get; set; }

        public string? Date { get; set; }

        public Guid ParcelId { get; set; }

        // for admin 
        public DateTime CreatedAt { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTime UpdatedAt { get; set; }
        public Guid UpdatedBy { get; set; }
    }
}
