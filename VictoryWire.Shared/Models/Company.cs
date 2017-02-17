using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VictoryWire.Shared
{
    [Serializable]
    [Table("Company")]
    public class Company
    {
        [Key, ForeignKey("AccountOwner"), DatabaseGenerated(DatabaseGeneratedOption.None), Column("account_id")]
        public Int32 Id { get; set; }

        [Column("contact_id")]
        public Int32 ContactId { get; set; }

        [Column("last_modified")]
        public DateTime LastModified { get; set; }

        [Column("name")]
        public String Name { get; set; }

        [Column("summary")]
        public String Summary { get; set; }

        [Column("website")]
        public String Website { get; set; }

        [Column("industry")]
        public String Industry { get; set; }

        [Column("logo")]
        public String Logo { get; set; }

        [ForeignKey("ContactId")]
        public virtual Contact Contact { get; set; }

        public virtual Account AccountOwner { get; set; }

        public virtual ICollection<Employee> Employees { get; set; }

        public virtual ICollection<Payroll> Payrolls { get; set; }

        public Company()
        {
            this.Id = 0;
            this.LastModified = DateTime.MinValue;
            this.Name = String.Empty;
            this.Summary = String.Empty;
            this.Website = String.Empty;
            this.Industry = String.Empty;
            this.Logo = String.Empty;
            this.Employees = new HashSet<Employee>();
            this.Payrolls = new HashSet<Payroll>();
        }
    }
}