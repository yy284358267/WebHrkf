using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Demo_RealTime.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index(string fromUser)
        {
           
            return View();
        }

        public ActionResult About(string fromUser,string ToUser)
        {
            //查找符合条件的客服
            string username = string.Empty;
            string select = "select Top 1 username from users where state='在线' and card='客服'  order by ID";
            SqlDataReader dr = SqlHelper.GetReader(select, System.Data.CommandType.Text);
            while (dr.Read())
            {
               username = dr["username"].ToString();
            }
            dr.Close();
            ViewBag.User = username;
            if (string.IsNullOrEmpty(username))
            {
                string selectorther = "select Top 1 username from users where state='接客' and card='客服'  order by count";
        
                SqlDataReader dorther = SqlHelper.GetReader(selectorther, System.Data.CommandType.Text);
                while (dorther.Read())
                {
                    username = dorther["username"].ToString();
                }
                dorther.Close();
                ViewBag.User = username;
                return View();
            }
            else
            {
                return View();
            }
           
        }

        public ActionResult Contact(string fromUser)
        {
           
            return View();
        }

        //private void FormsSignIn(string uName)
        //{
        //    FormsAuthentication.SetAuthCookie(uName, true);
        //    DateTime expiration =DateTime.Now.AddMinutes(60); 
        //    var authTicket = new FormsAuthenticationTicket(
        //            1,                             // version
        //            uName,                      // user name
        //            DateTime.Now,                 // created
        //            expiration,   // expires
        //            true,                   // persistent?
        //            "111",                        // can be used to store roles
        //            FormsAuthentication.FormsCookiePath
        //            );
          
        //    string encryptedTicket = FormsAuthentication.Encrypt(authTicket);
        //    var authCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket);
        //    authCookie.Expires = expiration; 
        //    Response.ContentType = "UTF-8";
        //    Response.Cookies.Add(authCookie);
        //}

    }
}