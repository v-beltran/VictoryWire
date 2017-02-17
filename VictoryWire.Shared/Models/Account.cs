using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace VictoryWire.Shared
{
    [Serializable]
    [Table("Account")]
    public class Account
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity), Column("account_id")]
        public Int32 Id { get; set; }

        [Column("created")]
        public DateTime Created { get; set; }

        [Column("last_login")]
        public DateTime LastLogin { get; set; }

        [Column("last_modified")]
        public DateTime LastModified { get; set; }

        [Column("first_name")]
        public String FirstName { get; set; }

        [Column("last_name")]
        public String LastName { get; set; }

        [Column("email")]
        public String Email { get; set; }

        [JsonIgnore]
        [Column("password")]
        public String Password { get; set; }

        [Column("type")]
        public AccountType Type { get; set; }

        [JsonIgnore]
        public virtual Company Company { get; set; }

        public Account()
        {
            this.Id = 0;
            this.Created = DateTime.MinValue;
            this.LastLogin = DateTime.MinValue;
            this.LastModified = DateTime.MinValue;
            this.FirstName = String.Empty;
            this.LastName = String.Empty;
            this.Email = String.Empty;
            this.Type = AccountType.Free;
        }
    }

    public enum AccountType
    {
        /// <summary>
        /// Free
        /// </summary>
        Free = 0,
        /// <summary>
        /// Standard
        /// </summary>
        Standard = 1,
        /// <summary>
        /// Premium
        /// </summary>
        Premium = 2
    }
}
