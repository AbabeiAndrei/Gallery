using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Mvc;

namespace Gallery.Controllers
{
    public class HomeController : ApiController
    {
        public ActionResult Index(string page)
        {
            return new FilePathResult($"app/partials/{page}.html", "text/html");
        }
    }
}