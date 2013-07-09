using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LunchBuddies.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            //System.Net.Http.HttpResponseMessage response = EmailClient.SendEmailAsync().Result;
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}