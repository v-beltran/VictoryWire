using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VictoryWire.Shared
{
    [Serializable]
    [Table("Payroll_Details")]
    public class PayrollDetails
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity), Column("payroll_details_id")]
        public Int32 Id { get; set; }

        [Column("payroll_id")]
        public Int32 PayrollId { get; set; }

        [Column("employee_id")]
        public Int32 EmployeeId { get; set; }

        [Column("last_modified")]
        public DateTime LastModified { get; set; }

        [Column("hours_std_worked")]
        public Int32 HoursStandardWorked { get; set; }

        [Column("hours_ot_worked")]
        public Int32 HoursOvertimeWorked { get; set; }

        [Column("deductions")]
        public Decimal Deductions { get; set; }

        [ForeignKey("PayrollId")]
        public virtual Payroll Payroll { get; set; }

        [ForeignKey("EmployeeId")]
        public virtual Employee Employee { get; set; }

        public PayrollDetails()
        {
            this.Id = 0;
            this.PayrollId = 0;
            this.EmployeeId = 0;
            this.LastModified = DateTime.UtcNow;
            this.HoursStandardWorked = 0;
            this.HoursOvertimeWorked = 0;
            this.Deductions = 0;
        }
    }
}
