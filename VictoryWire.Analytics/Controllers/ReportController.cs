using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Script.Serialization;
using VictoryWire.Shared;

using System.Data.Entity;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace VictoryWire.Analytics.Controllers
{
    public class ReportController : ApiController
    {
        #region " Download Endpoint "

        /// <summary>
        /// Endpoint for reporting data for the dashboard UI.
        /// </summary>
        /// <param name="type">Key that is either events, revenue, or misc.</param>
        /// <param name="name">Key that represents a specific report.</param>
        /// <param name="filter">Key that represents a specific filter.</param>
        /// <returns></returns>
        [Route("api/report/download")]
        [HttpGet]
        public HttpResponseMessage GetDownload([FromUri] String type, [FromUri] String name, [FromUri] String filter)
        {
            String lData = String.Empty;
            String lFileName = String.Empty;

            switch (type)
            {
                case "payroll":
                    switch (name)
                    {

                        case "newaccounts":
                            lFileName = $"{DateTime.Now.Date.ToString("MMddyyyy")} Newest Accounts.csv";
                            lData = this.GetNewestAccountsAttachments();
                            break;
                        case "newhires":
                            lFileName = $"{DateTime.Now.Date.ToString("MMddyyyy")} Newest Employee Hires.csv";
                            lData = this.GetNewestHiresAttachment();
                            break;
                    }
                    break;
            }

            HttpResponseMessage lResponse = Request.CreateResponse(HttpStatusCode.OK);
            lResponse.Content = new StringContent(lData);
            lResponse.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");
            lResponse.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment");
            lResponse.Content.Headers.ContentDisposition.FileName = lFileName;

            return lResponse;
        }

        private String GetNewestHiresAttachment()
        {
            StringBuilder lData = new StringBuilder();
            lData.AppendLine("Id,Name,Hire Date,Company");

            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                List<Employee> lEmployees = db.Employee.OrderByDescending(x => x.Hired).ToList();

                foreach (Employee lEmployee in lEmployees)
                {
                    lData.AppendLine($"{lEmployee.Id},{this.CsvSafeTrim($"{lEmployee.LastName},{lEmployee.FirstName}")},{lEmployee.Hired.ToString()},{this.CsvSafeTrim(lEmployee.Company.Name)}");
                }
            }

            return lData.ToString();
        }

        private String GetNewestAccountsAttachments()
        {
            StringBuilder lData = new StringBuilder();
            lData.AppendLine("Id,Name,Join Date,Company");

            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                List<Account> lAccounts = db.Account.OrderByDescending(x => x.Created).ToList();

                foreach (Account lAccount in lAccounts)
                {
                    lData.AppendLine($"{lAccount.Id},{this.CsvSafeTrim($"{lAccount.LastName},{lAccount.FirstName}")},{lAccount.Created.ToString()},{this.CsvSafeTrim(lAccount.Company.Name)}");
                }
            }

            return lData.ToString();
        }

        #endregion

        #region " UI Data Endpoint "

        /// <summary>
        /// Endpoint for reporting data for the dashboard UI.
        /// </summary>
        /// <param name="callback">Callback for JSONP.</param>
        /// <param name="type">Key that is either events, revenue, or misc.</param>
        /// <param name="name">Key that represents a specific report.</param>
        /// <param name="filter">Key that represents a specific filter.</param>
        /// <returns></returns>
        [Route("api/report/data")]
        [HttpGet]
        public HttpResponseMessage GetData([FromUri] String callback, [FromUri] String type, [FromUri] String name, [FromUri] String filter)
        {

            /*
             * JSON to be returned. Expected results based on output type:
             * 
             * Text  - { "text": "abc" }
             * List  - { "list": ["a", "b", "c", "d", "e"] }
             * Chart - { "chart": { [ "data1": value1, "data2": value2 ] } }
             * 
             */
            String lData = String.Empty;
            String lCallback = HttpUtility.HtmlEncode(callback);

            switch (type)
            {
                case "payroll":
                    switch (name)
                    {
                        case "totalaccounts":
                            lData = this.GetTotalAccounts();
                            break;
                        case "newaccounts":
                            lData = this.GetNewestAccounts();
                            break;
                        case "largestindustries":
                            lData = this.GetLargestIndustries();
                            break;
                        case "totalmoney":
                            lData = this.GetTotalMoney();
                            break;
                        case "mostpopulous":
                            lData = this.GetMostPopulousState();
                            break;
                        case "newhires":
                            lData = this.GetNewHires();
                            break;
                        case "totalterminations":
                            lData = this.GetTotalTerminations();
                            break;
                        case "accountscreated":
                            lData = this.GetAccountsCreatedOverPeriod(filter);
                            break;
                        case "highestpaid":
                            lData = this.GetHighestPaidEmployees();
                            break;
                    }
                    break;
            }

            HttpResponseMessage lResponse = Request.CreateResponse(HttpStatusCode.OK);
            lResponse.Content = new StringContent($"{callback}({lData})", System.Text.Encoding.UTF8, "application/javascript");
            return lResponse;
        }

        private String GetTotalAccounts()
        {
            Dictionary<String, Object> lData = new Dictionary<String, Object>();

            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                lData["text"] = db.Account.ToList().Count().ToString();
            }

            return new JavaScriptSerializer().Serialize(lData);
        }

        private String GetAccountsCreatedOverPeriod(String filter)
        {
            Dictionary<String, Object> lData = new Dictionary<String, Object>();

            switch (filter)
            {
                case "daily":
                    List<Object> lDailyEvents = new List<Object>();

                    using (ApplicationDbContext db = new ApplicationDbContext())
                    {
                        DateTime lTarget = DateTime.UtcNow.AddDays(-6);

                        while (lTarget <= DateTime.UtcNow)
                        {
                            // Get number of events for this day
                            Int32 lCount = db.Account.Where(x => x.Created.Day == lTarget.Day && x.Created.Month == lTarget.Month && x.Created.Year == lTarget.Year).Count();

                            // Add to data list
                            Object lDay = new { date = lTarget.ToString("MM/dd"), count = lCount.ToString() };
                            lDailyEvents.Add(lDay);

                            // Go to next day
                            lTarget = lTarget.AddDays(1);
                        }
                    }

                    lData["chart"] = lDailyEvents;

                    break;
                case "monthly":
                    List<Object> lMonthlyEvents = new List<Object>();

                    using (ApplicationDbContext db = new ApplicationDbContext())
                    {
                        DateTime lTarget = DateTime.UtcNow.AddMonths(-9);

                        while (lTarget <= DateTime.UtcNow)
                        {
                            // Get number of events for this month
                            Int32 lCount = db.Account.Where(x => x.Created.Month == lTarget.Month && x.Created.Year == lTarget.Year).Count();

                            // Add to data list
                            Object lMonth = new { date = lTarget.ToString("MMM"), count = lCount.ToString() };
                            lMonthlyEvents.Add(lMonth);

                            // Go to next month
                            lTarget = lTarget.AddMonths(1);
                        }
                    }

                    lData["chart"] = lMonthlyEvents;

                    break;
                case "yearly":
                    List<Object> lYearlyEvents = new List<Object>();

                    using (ApplicationDbContext db = new ApplicationDbContext())
                    {
                        DateTime lTarget = DateTime.UtcNow.AddYears(-4);

                        while (lTarget <= DateTime.UtcNow)
                        {
                            // Get number of events for this year
                            Int32 lCount = db.Account.Where(x => x.Created.Year == lTarget.Year).Count();

                            // Add to data list
                            Object lYear = new { date = lTarget.ToString("yyyy"), count = lCount.ToString() };
                            lYearlyEvents.Add(lYear);

                            // Go to next year
                            lTarget = lTarget.AddYears(1);
                        }
                    }

                    lData["chart"] = lYearlyEvents;

                    break;
            }

            return new JavaScriptSerializer().Serialize(lData);
        }

        private String GetNewestAccounts()
        {
            Dictionary<String, Object> lData = new Dictionary<String, Object>();
            List<Object> lNewAccounts = new List<Object>();

            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                List<Account> lAccounts = db.Account.OrderByDescending(x => x.Created).Take(5).ToList();

                foreach (Account lAccount in lAccounts)
                {
                    Object lNewAccount = new { name = $"{lAccount.LastName}, {lAccount.FirstName}", company = lAccount.Company.Name, joindate = lAccount.Created };
                    lNewAccounts.Add(lNewAccount);
                }

                lData["list"] = lNewAccounts;
            }

            return new JavaScriptSerializer().Serialize(lData);
        }

        private String GetLargestIndustries()
        {
            Dictionary<String, Object> lData = new Dictionary<String, Object>();
            List<Object> lLargestIndustries = new List<Object>();

            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                var lIndustries = db.Company.GroupBy(x => x.Industry, (industry, y) => new { Industry = industry, Count = y.Count() }).OrderByDescending(x => x.Count);

                foreach (var lIndustry in lIndustries)
                {
                    if (!String.IsNullOrEmpty(lIndustry.Industry))
                    {
                        Object lLargeIndustry = new { industry = lIndustry.Industry, count = lIndustry.Count.ToString() };
                        lLargestIndustries.Add(lLargeIndustry);
                    }
                }

                lData["list"] = lLargestIndustries;
            }

            return new JavaScriptSerializer().Serialize(lData);
        }

        private String GetTotalMoney()
        {
            Dictionary<String, Object> lData = new Dictionary<String, Object>();

            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                Decimal lTotal = 0;

                List<PayrollDetails> lHourlyWorkers = db.PayrollDetails.Where(x => x.Employee.RateType == PayRateType.Hourly).Include(x => x.Employee).ToList();
                lHourlyWorkers.ForEach(x => lTotal += (((x.HoursStandardWorked * x.Employee.Rate) + (1.5M * x.Employee.Rate * x.HoursOvertimeWorked)) - x.Deductions));

                List<PayrollDetails> lSalaryWorkers = db.PayrollDetails.Where(x => x.Employee.RateType == PayRateType.Salary).Include(x => x.Employee).ToList();
                lSalaryWorkers.ForEach(x => lTotal += (((x.Employee.Rate / 2080) * 40) - x.Deductions));

                lData["text"] = lTotal.ToString("C2");
            }

            return new JavaScriptSerializer().Serialize(lData);
        }

        private String GetMostPopulousState()
        {
            Dictionary<String, Object> lData = new Dictionary<String, Object>();
            List<Object> lMostPopularStates = new List<Object>();

            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                var lPopularStates = db.Contact.GroupBy(x => x.State, (state, y) => new { State = state, Count = y.Count() }).OrderByDescending(x => x.Count);

                foreach (var lPopularState in lPopularStates)
                {
                    if (!String.IsNullOrEmpty(lPopularState.State))
                    {
                        Object lState = new { state = lPopularState.State, count = lPopularState.Count.ToString() };
                        lMostPopularStates.Add(lState);
                    }
                }

                lData["list"] = lMostPopularStates;
            }

            return new JavaScriptSerializer().Serialize(lData);
        }

        private String GetNewHires()
        {
            Dictionary<String, Object> lData = new Dictionary<String, Object>();
            List<Object> lNewEmployees = new List<Object>();

            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                List<Employee> lEmployees = db.Employee.OrderByDescending(x => x.Hired).Take(5).ToList();

                foreach (Employee lEmployee in lEmployees)
                {
                    Object lNewEmployee = new { name = $"{lEmployee.LastName}, {lEmployee.FirstName}", company = lEmployee.Company.Name, hiredate = lEmployee.Hired };
                    lNewEmployees.Add(lNewEmployee);
                }

                lData["list"] = lNewEmployees;
            }

            return new JavaScriptSerializer().Serialize(lData);
        }

        private String GetHighestPaidEmployees()
        {
            Dictionary<String, Object> lData = new Dictionary<String, Object>();
            List<Object> lHighestPaidEmployees = new List<Object>();

            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                List<Employee> lHourlyEmployees = db.Employee.Where(x => x.RateType == PayRateType.Hourly).OrderByDescending(x => x.Rate).Take(5).ToList();
                List<Employee> lSalaryEmployees = db.Employee.Where(x => x.RateType == PayRateType.Salary).OrderByDescending(x => x.Rate).Take(5).ToList();

                lSalaryEmployees.ForEach(x =>
                {
                    x.Rate = (x.Rate / 2080);
                    lHourlyEmployees.Add(x);
                });
                lHourlyEmployees = lHourlyEmployees.OrderByDescending(x => x.Rate).Take(5).ToList();

                foreach (Employee lEmployee in lHourlyEmployees)
                {
                    Object lHighPaidEmployee = new { name = $"{lEmployee.LastName}, {lEmployee.FirstName}", count = lEmployee.Rate };
                    lHighestPaidEmployees.Add(lHighPaidEmployee);
                }
            }

            lData["chart"] = lHighestPaidEmployees;

            return new JavaScriptSerializer().Serialize(lData);
        }

        private String GetTotalTerminations()
        {
            Dictionary<String, Object> lData = new Dictionary<String, Object>();

            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                lData["text"] = db.Employee.Where(x => x.Terminated > DateTime.MinValue).Count().ToString();
            }

            return new JavaScriptSerializer().Serialize(lData);
        }

        #endregion

        #region " Helper Methods "

        private String CsvSafeTrim(String value)
        {
            String newValue = value;

            if (newValue.IndexOfAny(new char[] { '"', ',' }) != -1)
            {
                newValue = String.Format("\"{0}\"", newValue.Replace("\"", "\"\""));
            }

            return newValue;
        }

        #endregion
    }
}
