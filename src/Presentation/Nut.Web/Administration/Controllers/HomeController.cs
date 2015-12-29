using System;
using System.Net;
using System.ServiceModel.Syndication;
using System.Web.Mvc;
using System.Xml;

namespace Nut.Admin.Controllers
{
    public partial class HomeController : BaseAdminController
    {
       

        #region Methods

        public ActionResult Index()
        {
            return View();   
        }

       

        #endregion
    }
}
