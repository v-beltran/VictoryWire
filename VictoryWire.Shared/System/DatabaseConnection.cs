using System;
using System.Data.Entity;
using System.Web.Configuration;

namespace VictoryWire.Shared
{
    public class DatabaseConnection
    {
        /// <summary>
        /// Database connection to payment information.
        /// </summary>
        public String MainDatabase { get; set; }

        /// <summary>
        /// Create a new connection object.
        /// </summary>
        public DatabaseConnection()
        {
            this.MainDatabase = WebConfigurationManager.ConnectionStrings["MainDatabase"].ConnectionString;
        }
    }

    public class ApplicationDbContext : DbContext
    {
        public DbSet<Account> Account { get; set; }
        public DbSet<Company> Company { get; set; }
        public DbSet<Contact> Contact { get; set; }
        public DbSet<Employee> Employee { get; set; }
        public DbSet<Payroll> Payroll { get; set; }
        public DbSet<PayrollDetails> PayrollDetails { get; set; }


        public ApplicationDbContext() : base(Runtime.Connections.MainDatabase) { }

        public ApplicationDbContext(String connectionString)
        {
            this.Database.Connection.ConnectionString = connectionString;
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}