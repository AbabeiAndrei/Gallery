using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Owin.Host.SystemWeb;
using System.Web.Mvc;
using Gallery.Managers;

namespace Gallery.Controllers
{
    public class GalleryController : Controller
    {
        [HttpGet]
        public ActionResult Discover()
        {
            if (!Request.IsLoggedIn())
                return RedirectToAction("Login");
            return View();
        }

        [HttpGet]
        public ActionResult Photos()
        {
            if (!Request.IsLoggedIn())
                return RedirectToAction("Login");
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult Login()
        {
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }
    }
}