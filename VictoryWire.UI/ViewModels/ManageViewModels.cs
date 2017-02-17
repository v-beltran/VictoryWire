using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace VictoryWire.UI.ViewModels
{
    public class ProfileAccountDetailsViewModel
    {
        [Required]
        [Display(Name = "First name")]
        public String FirstName { get; set; }

        [Required]
        [Display(Name = "Last name")]
        public String LastName { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public String Email { get; set; }

        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Change Password")]
        public String ChangePassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("ChangePassword", ErrorMessage = "The password and confirmation password do not match.")]
        public String ConfirmPassword { get; set; }
    }

    public class ProfileCompanyDetailsViewModel
    {
        [Required]
        public String Name { get; set; }

        public String Summary { get; set; }

        [Url]
        public String Website { get; set; }
        
        public String Industry { get; set; }

        public String Logo { get; set; }
    }

    public class ProfileCompanyContactViewModel
    {
        [Required]        
        public String Address { get; set; }

        [Required]
        public String City { get; set; }

        [Required]
        public String State { get; set; }

        [Required]        
        public String Zip { get; set; }

        [Required]
        [EmailAddress]
        public String Email { get; set; }

        [Required]
        [Phone]
        public String Phone { get; set; }
    }
}