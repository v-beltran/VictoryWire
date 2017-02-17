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
    public class EmployeeController : Controller
    {
        public ActionResult Index()
        {
            List<EmployeeListViewModel> lEmployees = new List<EmployeeListViewModel>();

            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                List<Employee> lAllEmployees = db.Employee.Where(x => x.CompanyId == Runtime.Account.Id).ToList();
                lAllEmployees.ForEach(x =>
                {
                    EmployeeListViewModel lEmployee = new EmployeeListViewModel();
                    lEmployee.EmployeeId = x.Id;
                    lEmployee.Hired = x.Hired;
                    lEmployee.Name = $"{x.LastName}, {x.FirstName}";
                    lEmployee.Title = x.Title;
                    lEmployee.RateType = x.RateType;
                    lEmployee.Rate = x.Rate;
                    lEmployees.Add(lEmployee);
                });
            }

            return View(lEmployees);
        }

        public ActionResult Create()
        {
            return View("EditEmployee");
        }

        [HttpPost]
        public ActionResult Create(EmployeeFormViewModel model)
        {
            return RedirectToAction("Edit", new { id = 0, model = model });
        }

        public ActionResult Edit(Int32 id)
        {
            EmployeeFormViewModel lEmployee = new EmployeeFormViewModel();

            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                Employee lFindEmployee = db.Employee.Find(id);
                if (lFindEmployee != null)
                {
                    lEmployee.EmployeeId = lFindEmployee.Id;
                    lEmployee.Hired = lFindEmployee.Hired.Date;
                    lEmployee.Terminated = lFindEmployee.Terminated != null ? lFindEmployee.Terminated : DateTime.MinValue;
                    lEmployee.Title = lFindEmployee.Title;
                    lEmployee.Department = lFindEmployee.Department;
                    lEmployee.FirstName = lFindEmployee.FirstName;
                    lEmployee.LastName = lFindEmployee.LastName;
                    lEmployee.RateType = lFindEmployee.RateType.ToString();
                    lEmployee.Rate = lFindEmployee.Rate;

                    lEmployee.Address = lFindEmployee.Contact.Address;
                    lEmployee.City = lFindEmployee.Contact.City;
                    lEmployee.State = lFindEmployee.Contact.State;
                    lEmployee.Zip = lFindEmployee.Contact.Zip;
                    lEmployee.Email = lFindEmployee.Contact.Email;
                    lEmployee.Phone = lFindEmployee.Contact.Phone;
                }
            }

            return View("EditEmployee", lEmployee);
        }

        [HttpPost]
        public ActionResult Edit(Int32? id, EmployeeFormViewModel model)
        {
            if (this.ModelState.IsValid)
            {
                using (ApplicationDbContext db = new ApplicationDbContext())
                {
                    Contact lContact = new Contact();
                    Employee lEmployee = db.Employee.Find(id);

                    if (lEmployee == null)
                    {
                        lEmployee = new Employee() { CompanyId = Runtime.Account.Id };
                    }
                    else
                    {
                        lContact = lEmployee.Contact;
                    }

                    lEmployee.Hired = model.Hired;
                    lEmployee.Terminated = model.Terminated != null ? model.Terminated.Value : DateTime.MinValue;
                    lEmployee.Title = model.Title;
                    lEmployee.Department = model.Department;
                    lEmployee.FirstName = model.FirstName;
                    lEmployee.LastName = model.LastName;
                    lEmployee.RateType = (PayRateType)Enum.Parse(typeof(PayRateType), model.RateType); ;
                    lEmployee.Rate = model.Rate;


                    lContact.Address = model.Address;
                    lContact.City = model.City;
                    lContact.State = model.State;
                    lContact.Zip = model.Zip;
                    lContact.Email = model.Email;
                    lContact.Phone = model.Phone;
                    lEmployee.Contact = lContact;

                    if (id == null | id == 0) { db.Employee.Add(lEmployee); }
                    db.SaveChanges();
                }

                return RedirectToAction("Index");
            }

            return View("EditEmployee", model);
        }

        public ActionResult Delete(Int32 id)
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                Employee lEmployee = db.Employee.Find(id);
                db.Employee.Remove(lEmployee);
                db.SaveChanges();
            }

            return RedirectToAction("Index");
        }
    }
}
