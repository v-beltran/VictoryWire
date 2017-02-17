using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using VictoryWire.Shared;
using VictoryWire.UI.ViewModels;

namespace VictoryWire.UI.Controllers
{
    public class AccountController : Controller
    {

        #region " Sign Up "

        public ActionResult SignUp()
        {
            if (Runtime.Account != null)
            {
                return RedirectToAction("Index", "Manage");
            }

            return View();
        }

        [HttpPost]
        public ActionResult SignUp(SignUpViewModel model)
        {
            if (this.ModelState.IsValid)
            {
                using (ApplicationDbContext db = new ApplicationDbContext())
                {
                    Account lExistingAccount = db.Account.Where(x => x.Email == model.Email).FirstOrDefault();
                    if (lExistingAccount == null)
                    {
                        Account lNewAccount = new Account();
                        lNewAccount.Created = DateTime.UtcNow;
                        lNewAccount.LastLogin = DateTime.UtcNow;
                        lNewAccount.LastModified = DateTime.MinValue;
                        lNewAccount.FirstName = model.FirstName;
                        lNewAccount.LastName = model.LastName;
                        lNewAccount.Email = model.Email;
                        lNewAccount.Password = Security.ByteArrayToHex(Security.SHACngHash(model.Password, 256), true);
                        lNewAccount.Type = AccountType.Free;

                        Company lNewCompany = new Company();
                        lNewCompany.LastModified = DateTime.MinValue;
                        lNewCompany.Name = model.CompanyName;

                        Contact lCompanyContact = new Contact();
                        lCompanyContact.Email = model.Email;

                        lNewCompany.Contact = lCompanyContact;
                        lNewAccount.Company = lNewCompany;

                        db.Account.Add(lNewAccount);
                        db.SaveChanges();

                        Runtime.Account = lNewAccount;
                    }
                    else
                    {
                        this.ModelState.AddModelError("Email", "Email already exists. Please use a different email.");
                    }
                }

                if (Runtime.Account != null)
                {
                    return RedirectToAction("Index", "Manage");
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        #endregion

        #region " Log In "

        public ActionResult LogIn()
        {
            if (Runtime.Account != null)
            {
                return RedirectToAction("Index", "Manage");
            }

            return View();
        }

        [HttpPost]
        public ActionResult LogIn(LogInViewModel model)
        {
            if (this.ModelState.IsValid)
            {
                using (ApplicationDbContext db = new ApplicationDbContext())
                {
                    String lPasswordHash = Security.ByteArrayToHex(Security.SHACngHash(model.Password, 256), true);
                    Account lExistingAccount = db.Account.Where(x => x.Email == model.Email && x.Password == lPasswordHash).FirstOrDefault();
                    if (lExistingAccount != null)
                    {
                        db.Account.Attach(lExistingAccount);
                        lExistingAccount.LastLogin = DateTime.UtcNow;
                        db.SaveChanges();

                        Runtime.Account = lExistingAccount;
                    }
                    else
                    {
                        this.ModelState.AddModelError("", "Email does not exist or password is incorrect.");
                    }
                }

                if (Runtime.Account != null)
                {
                    return RedirectToAction("Index", "Manage");
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        #endregion

        #region " Log Out "

        public ActionResult LogOut()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("LogIn", "Account");
        }

        #endregion

        #region " Other Actions "

        public ActionResult Index()
        {
            return RedirectToAction("LogIn", "Account");
        }

        #endregion

    }
}
