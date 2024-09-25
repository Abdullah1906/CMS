using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace courierMs.Areas.Identity.Model
{
    public class ApplicationUser: IdentityUser
    {
        [StringLength(255)]
        [Display(Name = "firstname")]
        public string Firstname { get; set; }

        [StringLength(255)]
        [Display(Name = "lastname")]
        public string Lastname { get; set; }
    }
}
