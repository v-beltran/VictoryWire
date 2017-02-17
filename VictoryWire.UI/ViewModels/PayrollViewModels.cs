using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using VictoryWire.Shared;

namespace VictoryWire.UI.ViewModels
{
    public class PayrollListViewModel
    {
        [Display(Name = "Id")]
        public Int32 PayrollId { get; set; }

        [Display(Name = "Week Ending Date")]
        public DateTime WeekEnding { get; set; }
    }

    public class EmployeePayrollViewModel
    {
        [Display(Name = "Id")]
        public Int32 EmployeeId { get; set; }

        public String Name { get; set; }

        [Display(Name = "Pay Rate Type")]
        public String PayRateType { get; set; }

        [Display(Name = "Pay Rate")]
        public Decimal PayRate { get; set; }

        [Display(Name = "Std. Hours")]
        public Decimal StandardHours { get; set; }

        [Display(Name = "OT Hours")]
        public Decimal OvertimeHours { get; set; }

        public Decimal Deductions { get; set; }

        [Display(Name = "Gross Pay")]
        public Decimal GrossPay { get; set; }

        [Display(Name = "Net Pay")]
        public Decimal NetPay { get; set; }
    }

    public class PayrollDetailsViewModel
    {       
        public List<EmployeePayrollViewModel> EmployeePayrolls { get; set; }
    }
}