using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;

namespace Demo_RealTime
{
    public class MvcApplication : System.Web.HttpApplication
    {
        public MvcApplication()
        {
            AuthenticateRequest += new EventHandler(MvcApplication_AuthenticateRequest);
        }

        void MvcApplication_AuthenticateRequest(object sender, EventArgs e)
        { 
            var request = Context.Request;
            var _type = request.CurrentExecutionFilePathExtension;//如：.css、.js、.jpg  mvc页面由于不加.html或.cshtml 所以扩展名返回"";
            if (_type == "")
            {
                string fromUser = request["fromUser"];
                if (string.IsNullOrEmpty(fromUser) == false)
                {
                    //HttpContext.Current.User = new GenericPrincipal(
                    //     new GenericIdentity(fromUser, ""),
                    //     null
                    //     );
                    HttpContext.Current.User = new Principal(
                                   new GenericIdentity(fromUser, "1"),
                                   new string[] { fromUser }
                               ); 

                }
            }
        }


        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
         
       
    }

    /// <summary>
    /// 存放登录用户ID
    /// </summary>
    public class Principal : GenericPrincipal
    {
        public string uName { get; set; }

        public Principal(IIdentity identity, string[] roles)
            : base(identity, roles)
        {
            this.uName = roles[0];
        }
    }
}
