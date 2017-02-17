using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using VictoryWire.Shared;

namespace VictoryWire.UI.ViewModels
{
    public class EmployeeListViewModel
    {
        public Int32 EmployeeId { get; set; }

        public String Name { get; set; }

        public DateTime Hired { get; set; }

        public String Title { get; set; }

        [Display(Name = "Pay Rate Type")]
        public PayRateType RateType { get; set; }

        [Display(Name = "Pay Rate")]
        public Decimal Rate { get; set; }
    }

    public class EmployeeFormViewModel
    {
        public Int32 EmployeeId { get; set; }

        [Required]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Hired Date")]
        public DateTime Hired { get; set; }

        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Terminated Date")]
        public DateTime? Terminated { get; set; }

        [Required]
        [Display(Name = "First Name")]
        public String FirstName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        public String LastName { get; set; }

        [Required]
        [Display(Name = "Title")]
        public String Title { get; set; }

        [Required]
        public String Department { get; set; }

        [Required]
        [Display(Name = "Pay Rate Type")]
        public String RateType { get; set; }

        [Required]
        [Display(Name = "Pay Rate")]
        public Decimal Rate { get; set; }

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