using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VictoryWire.Shared
{
    [Serializable]
    [Table("Contact")]
    public class Contact
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity), Column("contact_id")]
        public Int32 ContactId { get; set; }

        [Column("last_modified")]
        public DateTime LastModified { get; set; }

        [Column("address")]
        public String Address { get; set; }

        [Column("city")]
        public String City { get; set; }

        [Column("state")]
        public String State { get; set; }

        [Column("zip")]
        public String Zip { get; set; }

        [Column("email")]
        public String Email { get; set; }

        [Column("phone")]
        public String Phone { get; set; }

        public Contact()
        {
            this.ContactId = 0;
            this.Address = String.Empty;
            this.City = String.Empty;
            this.State = String.Empty;
            this.Zip = String.Empty;
            this.Email = String.Empty;
            this.Phone = String.Empty;            
        }
    }
}
