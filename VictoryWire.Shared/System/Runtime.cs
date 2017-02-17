using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Security;

namespace VictoryWire.Shared
{
    public class Runtime
    {
        /// <summary>
        /// Returns true if compiled in debug mode.
        /// </summary>
        public static Boolean IsDebug
        {
            get
            {
#if DEBUG
                return true;
#else
                return false;
#endif
            }
        }

        /// <summary>
        /// Does not end with slash ('/').
        /// </summary>
        public static String BaseUrl
        {
            get
            {
                String lBase = HttpContext.Current.Request.Url.Scheme + "://" + HttpContext.Current.Request.Url.Authority;
                if (lBase.EndsWith("/")) { lBase.TrimEnd('/'); }
                return lBase;
            }
        }

        /// <summary>
        /// The current context of authenticated account.
        /// </summary>
        public static Account Account
        {
            get
            {
                //Get a reference to the current HTTP context for ease
                HttpContext lCurrent = HttpContext.Current;

                if (lCurrent.Items["Runtime.Account"] == null)
                {
                    Account lAccount = null;

                    if (lCurrent.User != null && lCurrent.User.Identity.IsAuthenticated)
                    {
                        //Don't keep decrypting the cookie cause that is slow
                        FormsIdentity lIdentity = (FormsIdentity)HttpContext.Current.User.Identity;
                        FormsAuthenticationTicket lTicket = lIdentity.Ticket;
                        lAccount = JsonConvert.DeserializeObject<Account>(lTicket.UserData);
                    }


                    //Save the result of all dat work into the current HTTP context for faster future re-use
                    lCurrent.Items["Runtime.Account"] = lAccount;
                }

                return (Account)lCurrent.Items["Runtime.Account"];
            }
            set
            {
                FormsAuthenticationTicket lFormsTicket = new FormsAuthenticationTicket(1, value.Email, DateTime.UtcNow, DateTime.UtcNow.Add(FormsAuthentication.Timeout), false, JsonConvert.SerializeObject(value), FormsAuthentication.FormsCookiePath);
                HttpContext.Current.Response.Cookies.Add(new HttpCookie(FormsAuthentication.FormsCookieName, FormsAuthentication.Encrypt(lFormsTicket)));
                HttpContext.Current.Items["Runtime.Account"] = value;
            }
        }

        /// <summary>
        /// The connections to databases for this application.
        /// </summary>
        public static DatabaseConnection Connections
        {
            get
            {
                //Get a reference to the current HTTP context for ease
                HttpContext lCurrent = HttpContext.Current;

                if (lCurrent.Cache["Runtime.Connections"] == null)
                {
                    lCurrent.Cache["Runtime.Connections"] = new DatabaseConnection();
                }

                return (DatabaseConnection)lCurrent.Cache["Runtime.Connections"];
            }
        }

        /// <summary>
        /// The storage for this application.
        /// </summary>
        public static Storage Storage
        {
            get
            {
                //Get a reference to the current HTTP context for ease
                HttpContext lCurrent = HttpContext.Current;

                if (lCurrent.Cache["Runtime.Storage"] == null)
                {
                    lCurrent.Cache["Runtime.Storage"] = new Storage();
                }

                return (Storage)lCurrent.Cache["Runtime.Storage"];
            }
        }

    }
}
