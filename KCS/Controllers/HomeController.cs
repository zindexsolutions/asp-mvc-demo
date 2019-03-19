using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KCS.Controllers
{
    public class HomeController : BaseController
    {
        // GET: Home
        [HttpGet]
        [AllowAnonymous]
        public ActionResult Index()
        {
            return View();
        }

        #region When user haven't rights of accesssing requested page than we redirect them to our custom error page.

        [AllowAnonymous]
        public ActionResult ErrorPage()
        {
            return View();
        }

        #endregion When user haven't rights of accesssing requested page than we redirect them to our custom error page.
    }
}