using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using VictoryWire.Shared;
using VictoryWire.UI.ViewModels;

namespace VictoryWire.UI.Controllers
{
    [Authorize]
    public class ManageController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult MyProfile()
        {
            return View();
        }

        public PartialViewResult EditAccountDetails()
        {
            ProfileAccountDetailsViewModel lModel = new ProfileAccountDetailsViewModel();
            lModel.FirstName = Runtime.Account.FirstName;
            lModel.LastName = Runtime.Account.LastName;
            lModel.Email = Runtime.Account.Email;
            return PartialView(lModel);
        }

        [HttpPost]
        public ActionResult EditAccountDetails(ProfileAccountDetailsViewModel model)
        {
            if (this.ModelState.IsValid)
            {
                using (ApplicationDbContext db = new ApplicationDbContext())
                {
                    Account lCurrentAccount = db.Account.Find(Runtime.Account.Id);
                    lCurrentAccount.FirstName = model.FirstName;
                    lCurrentAccount.LastName = model.LastName;

                    if (model.ChangePassword != null)
                    {
                        String lNewPassword = Security.ByteArrayToHex(Security.SHACngHash(model.ChangePassword, 256), true);
                        if (lNewPassword != model.ChangePassword)
                        {
                            lCurrentAccount.Password = lNewPassword;
                        }
                    }

                    if (model.Email != Runtime.Account.Email)
                    {
                        Account lExistingAccount = db.Account.Where(x => x.Email == model.Email).FirstOrDefault();
                        if (lExistingAccount == null)
                        {
                            lCurrentAccount.Email = model.Email;
                        }
                        else
                        {
                            this.ModelState.AddModelError("Email", "Email already exists. Please use a different email.");
                        }
                    }

                    if (this.ModelState.IsValid)
                    {
                        lCurrentAccount.LastModified = DateTime.UtcNow;
                        db.SaveChanges();
                    }
                }
            }

            return PartialView(model);
        }

        public PartialViewResult EditCompanyDetails()
        {
            ProfileCompanyDetailsViewModel lModel = new ProfileCompanyDetailsViewModel();

            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                Company lCompany = db.Company.Find(Runtime.Account.Id);
                lModel.Name = lCompany.Name;
                lModel.Summary = lCompany.Summary;
                lModel.Industry = lCompany.Industry;
                lModel.Website = lCompany.Website;
                lModel.Logo = lCompany.Logo;
            }

            return PartialView(lModel);
        }

        [HttpPost]
        public ActionResult EditCompanyDetails(ProfileCompanyDetailsViewModel model)
        {
            if (this.ModelState.IsValid)
            {
                using (ApplicationDbContext db = new ApplicationDbContext())
                {
                    Company lCompany = db.Company.Find(Runtime.Account.Id);
                    lCompany.LastModified = DateTime.UtcNow;
                    lCompany.Name = model.Name;
                    lCompany.Summary = !String.IsNullOrEmpty(model.Summary) ? model.Summary : String.Empty;
                    lCompany.Industry = !String.IsNullOrEmpty(model.Industry) ? model.Industry : String.Empty;
                    lCompany.Website = !String.IsNullOrEmpty(model.Website) ? model.Website : String.Empty;

                    if(lCompany.Logo.Length > 0) { model.Logo = lCompany.Logo + "?t=" + DateTime.UtcNow.Ticks; }

                    db.SaveChanges();
                }
            }

            return PartialView(model);
        }

        [HttpPost]
        public ActionResult UploadCompanyLogo(HttpPostedFileBase logo)
        {
            if (this.ModelState.IsValid)
            {
                using (ApplicationDbContext db = new ApplicationDbContext())
                {
                    try
                    {
                        if (Regex.IsMatch(logo.FileName, "\\.(gif|jpg|jpeg|png)$", RegexOptions.IgnoreCase))
                        {
                            Company lCompany = db.Company.Find(Runtime.Account.Id);

                            String lFileName = Path.GetFileName(logo.FileName);
                            String lAzureFileName = Runtime.Account.Id.ToString() + "/company-logo";
                            String lMimeType = MimeMapping.GetMimeMapping(lFileName);
                            Uri lAzureUri = null;

                            using (MemoryStream ms = new MemoryStream())
                            {
                                logo.InputStream.CopyTo(ms);
                                Byte[] lImageData = ms.ToArray();
                                lAzureUri = Runtime.Storage.WriteAllBytes(lAzureFileName, lImageData, lMimeType);
                            }
                            
                            if(lAzureUri != null)
                            {
                                lCompany.Logo = lAzureUri.AbsoluteUri;
                            }                            

                            db.SaveChanges();

                            // Success
                            return Json(new { image = lCompany.Logo, success = true, message = String.Empty });
                        }
                    }
                    catch(Exception ex)
                    {
                        // Failed uploading image
                        Response.StatusCode = (Int32)System.Net.HttpStatusCode.BadRequest;
                        return Json(new { image = String.Empty, success = false, message = "Image failed to upload. Please try again." });
                    }
                }
            }

            // Not an image
            Response.StatusCode = (Int32)System.Net.HttpStatusCode.BadRequest;
            return Json(new { image = String.Empty, success = false, message = "Image type must be a gif, jpg, jpeg or png." });
        }

        public PartialViewResult EditCompanyContact()
        {
            ProfileCompanyContactViewModel lModel = new ProfileCompanyContactViewModel();

            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                Company lCompany = db.Company.Find(Runtime.Account.Id);
                lModel.Address = lCompany.Contact.Address;
                lModel.City = lCompany.Contact.City;
                lModel.State = lCompany.Contact.State;
                lModel.Zip = lCompany.Contact.Zip;
                lModel.Email = lCompany.Contact.Email;
                lModel.Phone = lCompany.Contact.Phone;
            }

            return PartialView(lModel);
        }

        [HttpPost]
        public ActionResult EditCompanyContact(ProfileCompanyContactViewModel model)
        {
            if (this.ModelState.IsValid)
            {
                using (ApplicationDbContext db = new ApplicationDbContext())
                {
                    Company lCompany = db.Company.Find(Runtime.Account.Id);
                    lCompany.Contact.LastModified = DateTime.UtcNow;
                    lCompany.Contact.Address = model.Address;
                    lCompany.Contact.City = model.City;
                    lCompany.Contact.State = model.State;
                    lCompany.Contact.Zip = model.Zip;
                    lCompany.Contact.Email = model.Email;
                    lCompany.Contact.Phone = model.Phone;
                    db.SaveChanges();
                }
            }

            return PartialView(model);
        }
    }
}
