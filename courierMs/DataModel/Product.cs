using System.ComponentModel.DataAnnotations;

namespace courierMs.DataModel
{
    public class Product
    {
        [Key]
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Image { get; set; }
        public int price { get; set; }

        //for admin
        public DateTime CreatedAt { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTime UpdatedAt { get; set; }
        public Guid UpdatedBy { get; set; }

    }
}
