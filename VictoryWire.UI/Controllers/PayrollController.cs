using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VictoryWire.Shared;
using VictoryWire.UI.ViewModels;

namespace VictoryWire.UI.Controllers
{
    [Authorize]
    public class PayrollController : Controller
    {
        public ActionResult Index()
        {
            List<PayrollListViewModel> lPayrolls = new List<PayrollListViewModel>();

            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                List<Payroll> lAllPayrolls = db.Payroll.Where(x => x.CompanyId == Runtime.Account.Id).OrderByDescending(x => x.WeekEnding).ToList();
                lAllPayrolls.ForEach(x =>
                {
                    PayrollListViewModel lPayroll = new PayrollListViewModel();
                    lPayroll.PayrollId = x.Id;
                    lPayroll.WeekEnding = x.WeekEnding;
                    lPayrolls.Add(lPayroll);
                });
            }

            return View(lPayrolls);
        }

        [HttpPost]
        public ActionResult Create(String weekending)
        {
            DateTime lWeekEnding = DateTime.MinValue;
            if(DateTime.TryParse(weekending, out lWeekEnding))
            {
                using(ApplicationDbContext db = new ApplicationDbContext())
                {
                    Payroll lNewPayroll = new Payroll();
                    lNewPayroll.CompanyId = Runtime.Account.Id;
                    lNewPayroll.WeekEnding = lWeekEnding;
                    db.Payroll.Add(lNewPayroll);
                    db.SaveChanges();
                }
            }
            else
            {
                this.ModelState.AddModelError("week", "Please enter a valid date.");
            }

            return RedirectToAction("Index");
        }

        public ActionResult Edit(Int32 id)
        {
            PayrollDetailsViewModel lPayroll = new PayrollDetailsViewModel();
            lPayroll.EmployeePayrolls = new List<EmployeePayrollViewModel>();

            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                Payroll lFindPayroll = db.Payroll.Find(id);
                if (lFindPayroll != null)
                {
                    List<PayrollDetails> lFindDetails = lFindPayroll.Details.ToList();
                    lFindDetails.ForEach(x =>
                    {
                        EmployeePayrollViewModel lEmployeeDetails = new EmployeePayrollViewModel();
                        lEmployeeDetails.EmployeeId = x.EmployeeId;
                        lEmployeeDetails.Name = $"{x.Employee.LastName}, {x.Employee.FirstName}";
                        lEmployeeDetails.PayRateType = x.Employee.RateType.ToString();
                        lEmployeeDetails.PayRate = x.Employee.Rate;
                        lEmployeeDetails.StandardHours = x.HoursStandardWorked;
                        lEmployeeDetails.OvertimeHours = x.HoursOvertimeWorked;
                        lEmployeeDetails.Deductions = x.Deductions;
                        
                        if(x.Employee.RateType == PayRateType.Hourly)
                        {
                            lEmployeeDetails.GrossPay = ((x.HoursStandardWorked * x.Employee.Rate) + (1.5M * x.Employee.Rate * x.HoursOvertimeWorked)) - x.Deductions;
                        }
                        else
                        {
                            lEmployeeDetails.GrossPay = ((x.Employee.Rate / 2080) * 40) - x.Deductions;
                        }

                        lEmployeeDetails.NetPay = lEmployeeDetails.GrossPay - lEmployeeDetails.Deductions;
                        lPayroll.EmployeePayrolls.Add(lEmployeeDetails);
                    });
                }
            }

            return View("EditPayroll", lPayroll);
        }

        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        public ActionResult Delete(int id)
        {
            return View();
        }
    }
}
