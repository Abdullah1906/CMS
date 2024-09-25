namespace courierMs.ViewModel
{
    public class LookupVM
    {
        public int Id { get; set; }

        public string Type { get; set; }

        public string Value { get; set; }

        public string Name { get; set; }

        public bool IsActive { get; set; }

        public int Serial { get; set; }

        // admin use
        public DateTime CreatedAt { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTime UpdatedAt { get; set; }
        public Guid UpdatedBy { get; set; }
    }
}
