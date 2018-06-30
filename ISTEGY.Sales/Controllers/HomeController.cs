using ISTEGY.Sales.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ISTEGY.Sales.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        DatabaseContext db = new DatabaseContext();
        public ActionResult Index()
        {
            return View();
        }
    }
}