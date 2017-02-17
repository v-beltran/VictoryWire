using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VictoryWire.Shared
{
    [Serializable]
    [Table("Payroll")]
    public class Payroll
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity), Column("payroll_id")]
        public Int32 Id { get; set; }

        [Column("account_id")]
        public Int32 CompanyId { get; set; }

        [Column("last_modified")]
        public DateTime LastModified { get; set; }

        [Column("week_ending")]
        public DateTime WeekEnding { get; set; }

        public virtual ICollection<PayrollDetails> Details { get; set; }

        public virtual Company Company { get; set; }

        public Payroll()
        {
            this.Id = 0;
            this.CompanyId = 0;
            this.LastModified = DateTime.UtcNow;
            this.WeekEnding = DateTime.UtcNow;
            this.Details = new HashSet<PayrollDetails>();
        }
    }
}
