using System.ComponentModel.DataAnnotations;

namespace courierMs.ViewModel
{
    public class CustomerinfoVM
    {

        public int Id { get; set; }
        public Guid SenderId { get; set; }
        public string S_Name { get; set; }
        public string S_PhoneNumber { get; set; }
        public string S_Email { get; set; }
        public string S_Address { get; set; }
        public string S_Note { get; set; }
        public string S_city { get; set; }



        public Guid RecieverId { get; set; }
        public string R_Name { get; set; }
        public string R_PhoneNumber { get; set; }
        public string R_Email { get; set; }
        public string R_Address { get; set; }
        public string R_Note { get; set; }
        public string R_city { get; set; }

        //for admin
        public DateTime CreatedAt { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTime UpdatedAt { get; set; }
        public Guid UpdatedBy { get; set; }
    }

    public class MultiModelVM
    {
        public CustomerinfoVM Customerinfo { get; set; }
        public PercelinfoVM Percelinfo { get; set; }
    }
}
