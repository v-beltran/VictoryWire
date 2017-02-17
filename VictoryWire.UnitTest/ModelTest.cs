using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VictoryWire.Shared;
using System.Web.Configuration;

namespace VictoryWire.UnitTest
{
    [TestClass]
    public class ModelTest
    {

        #region " Test Properties "

        public String TestDatabase
        {
            get
            {
                return WebConfigurationManager.ConnectionStrings["MainDatabase"].ConnectionString;
            }
        }

        public Account TestAccount
        {
            get
            {
                Account lAccount = new Account();
                lAccount.Created = DateTime.UtcNow;
                lAccount.LastModified = DateTime.UtcNow;
                lAccount.LastLogin = DateTime.UtcNow;
                lAccount.FirstName = "CreateAccount";
                lAccount.LastName = "Test";
                lAccount.Email = "createaccount@test.com";
                lAccount.Password = Security.ByteArrayToHex(Security.SHACngHash("createaccount", 256), true);
                lAccount.Type = AccountType.Premium;
                return lAccount;
            }
        }

        public Contact TestContact
        {
            get
            {
                Contact lContact = new Contact();
                lContact.LastModified = DateTime.UtcNow;
                lContact.Address = "123 Create Contact Test St.";
                lContact.City = "Create Contact";
                lContact.Email = "createcontact@test.com";
                lContact.Phone = "123-456-7890";
                lContact.State = "Contact";
                lContact.Zip = "12345";
                return lContact;
            }
        }

        public Company TestCompany
        {
            get
            {
                Company lCompany = new Company();
                lCompany.LastModified = DateTime.UtcNow;
                lCompany.Industry = "Create Company";
                lCompany.Name = "Create Company Test";
                lCompany.Summary = "Create Company";
                lCompany.Website = "http://www.createcompany.com";
                lCompany.Logo = "http://www.createcompany.com/logo.png";
                return lCompany;
            }
        }

        public Employee TestEmployee
        {
            get
            {
                Employee lEmployee = new Employee();
                lEmployee.LastModified = DateTime.UtcNow;
                lEmployee.Hired = DateTime.UtcNow;
                lEmployee.Terminated = DateTime.MinValue;
                lEmployee.Title = "Create Employee";
                lEmployee.Department = "Create Employee";
                lEmployee.FirstName = "Create Employee";
                lEmployee.LastName = "Test";
                lEmployee.RateType = PayRateType.Hourly;
                lEmployee.Rate = 25.75M;
                return lEmployee;
            }
        }

        public Payroll TestPayroll
        {
            get
            {
                Payroll lPayroll = new Payroll();
                lPayroll.LastModified = DateTime.UtcNow;
                lPayroll.WeekEnding = DateTime.UtcNow;
                return lPayroll;
            }            
        }

        public PayrollDetails TestPayrollDetails
        {
            get
            {
                PayrollDetails lPayrollDetails = new PayrollDetails();
                lPayrollDetails.LastModified = DateTime.UtcNow;
                lPayrollDetails.HoursStandardWorked = 40;
                lPayrollDetails.HoursOvertimeWorked = 10;
                lPayrollDetails.Deductions = 475.50M;
                return lPayrollDetails;
            }
        }

        #endregion

        #region " Test Methods "

        [TestMethod]
        public void CreateAccount()
        {
            using (ApplicationDbContext db = new ApplicationDbContext(this.TestDatabase))
            {
                Account lAccount = this.TestAccount;

                db.Account.Add(lAccount);
                db.SaveChanges();
            }
        }

        [TestMethod]
        public void CreateContact()
        {
            using (ApplicationDbContext db = new ApplicationDbContext(this.TestDatabase))
            {
                Contact lContact = this.TestContact;  
                       
                db.Contact.Add(lContact);
                db.SaveChanges();
            }
        }

        [TestMethod]
        public void CreateAccountWithCompany()
        {
            using (ApplicationDbContext db = new ApplicationDbContext(this.TestDatabase))
            {
                Account lAccount = this.TestAccount;
                Company lCompany = this.TestCompany;
                Contact lContact = this.TestContact;

                lCompany.Contact = lContact;
                lAccount.Company = lCompany;

                db.Account.Add(lAccount);
                db.SaveChanges();
            }
        }

        [TestMethod]
        public void CreateAccountWithCompanyAndEmployees()
        {
            using (ApplicationDbContext db = new ApplicationDbContext(this.TestDatabase))
            {
                Account lAccount = this.TestAccount;
                Company lCompany = this.TestCompany;
                Contact lCompanyContact = this.TestContact;
                Contact lEmployeeContact = this.TestContact;
                Employee lCompanyEmployee = this.TestEmployee;

                lCompanyEmployee.Contact = lEmployeeContact;
                lCompany.Contact = lCompanyContact;
                lCompany.Employees.Add(lCompanyEmployee);
                lAccount.Company = lCompany;

                db.Account.Add(lAccount);
                db.SaveChanges();
            }
        }

        [TestMethod]
        public void UpdateAccountWithCompanyAndEmployees()
        {
            Int32 lInsertedAccountId = 0;

            using (ApplicationDbContext db = new ApplicationDbContext(this.TestDatabase))
            {
                Account lAccount = this.TestAccount;
                Company lCompany = this.TestCompany;
                Contact lCompanyContact = this.TestContact;
                Contact lEmployeeContact = this.TestContact;
                Employee lCompanyEmployee = this.TestEmployee;

                lCompanyEmployee.Contact = lEmployeeContact;
                lCompany.Contact = lCompanyContact;
                lCompany.Employees.Add(lCompanyEmployee);
                lAccount.Company = lCompany;

                db.Account.Add(lAccount);
                db.SaveChanges();

                lInsertedAccountId = lAccount.Id;
            }

            using (ApplicationDbContext db = new ApplicationDbContext(this.TestDatabase))
            {
                Account lUpdateAccount = db.Account.Find(lInsertedAccountId);
                lUpdateAccount.FirstName = "Updated Account First Name";
                lUpdateAccount.Company.Name = "Updated Company Name";
                lUpdateAccount.Company.Contact.Address = "Updated Company Address";
                lUpdateAccount.Company.Employees.FirstOrDefault().Title = "Updated First Company Employee's Title";
                lUpdateAccount.Company.Employees.FirstOrDefault().Contact.Address = "Updated First Company Employee's Address";
                db.SaveChanges();
            }
        }

        [TestMethod]
        public void CreatePayroll()
        {           
            using (ApplicationDbContext db = new ApplicationDbContext(this.TestDatabase))
            {
                Account lAccount = this.TestAccount;
                Company lCompany = this.TestCompany;
                Contact lCompanyContact = this.TestContact;
                Contact lEmployeeContact = this.TestContact;
                Employee lCompanyEmployee = this.TestEmployee;
                Payroll lCompanyPayroll = this.TestPayroll;
                PayrollDetails lCompanyPayrollDetails = this.TestPayrollDetails;

                lCompanyEmployee.Contact = lEmployeeContact;
                lCompany.Contact = lCompanyContact;
                lCompany.Employees.Add(lCompanyEmployee);
                lAccount.Company = lCompany;
                lAccount.Company.Payrolls.Add(lCompanyPayroll);
                db.Account.Add(lAccount);
                db.SaveChanges();

                lCompanyPayrollDetails.EmployeeId = lCompanyEmployee.Id;
                lCompanyPayroll.Company.Payrolls.FirstOrDefault().Details.Add(lCompanyPayrollDetails);
                db.SaveChanges();
            }
        }

        [TestMethod]
        public void UpdatePayroll()
        {
            Int32 lUpdatePayrollId = 0;

            using (ApplicationDbContext db = new ApplicationDbContext(this.TestDatabase))
            {
                Account lAccount = this.TestAccount;
                Company lCompany = this.TestCompany;
                Contact lCompanyContact = this.TestContact;
                Contact lEmployeeContact = this.TestContact;
                Employee lCompanyEmployee = this.TestEmployee;
                Payroll lCompanyPayroll = this.TestPayroll;
                PayrollDetails lCompanyPayrollDetails = this.TestPayrollDetails;

                lCompanyEmployee.Contact = lEmployeeContact;
                lCompany.Contact = lCompanyContact;
                lCompany.Employees.Add(lCompanyEmployee);
                lAccount.Company = lCompany;
                lAccount.Company.Payrolls.Add(lCompanyPayroll);
                db.Account.Add(lAccount);
                db.SaveChanges();

                lCompanyPayrollDetails.EmployeeId = lCompanyEmployee.Id;
                lCompanyPayroll.Company.Payrolls.FirstOrDefault().Details.Add(lCompanyPayrollDetails);
                db.SaveChanges();

                lUpdatePayrollId = lCompanyPayroll.Id;
            }

            using (ApplicationDbContext db = new ApplicationDbContext(this.TestDatabase))
            {
                Payroll lUpdatePayroll = db.Payroll.Find(lUpdatePayrollId);
                lUpdatePayroll.WeekEnding = DateTime.UtcNow.AddDays(7);
                lUpdatePayroll.Details.FirstOrDefault().HoursOvertimeWorked = 20;
                db.SaveChanges();
            }
        }

        #endregion
    }
}
