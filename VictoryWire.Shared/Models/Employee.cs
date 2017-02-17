using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VictoryWire.Shared
{
    public enum PayRateType
    {
        /// <summary>
        /// Hourly rate
        /// </summary>
        Hourly = 0,
        /// <summary>
        /// Salary rate
        /// </summary>
        Salary = 1
    }

    [Serializable]
    [Table("Employee")]
    public class Employee
    {        
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity), Column("employee_id")]
        public Int32 Id { get; set; }

        [Column("account_id")]
        public Int32 CompanyId { get; set; }

        [Column("contact_id")]
        public Int32 ContactId { get; set; }

        [Column("last_modified")]
        public DateTime LastModified { get; set; }

        [Column("hired")]
        public DateTime Hired { get; set; }

        [Column("terminated")]
        public DateTime Terminated { get; set; }

        [Column("first_name")]
        public String FirstName { get; set; }

        [Column("last_name")]
        public String LastName { get; set; }

        [Column("title")]
        public String Title { get; set; }

        [Column("department")]
        public String Department { get; set; }

        [Column("pay_rate_type")]
        public PayRateType RateType { get; set; }

        [Column("pay_rate")]
        public Decimal Rate { get; set; }

        [ForeignKey("CompanyId")]
        public virtual Company Company { get; set; }

        [ForeignKey("ContactId")]
        public virtual Contact Contact { get; set; }

        public virtual ICollection<PayrollDetails> Payrolls { get; set; }

        public Employee()
        {
            this.Id = 0;
            this.CompanyId = 0;
            this.LastModified = DateTime.MinValue;
            this.Hired = DateTime.MinValue;
            this.Terminated = DateTime.MinValue;
            this.FirstName = String.Empty;
            this.LastName = String.Empty;
            this.Title = String.Empty;
            this.Department = String.Empty;
            this.RateType = PayRateType.Hourly;
            this.Rate = 0;
            this.Payrolls = new HashSet<PayrollDetails>();
        }

    }
}
