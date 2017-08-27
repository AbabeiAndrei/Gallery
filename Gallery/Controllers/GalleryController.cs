using System.Web.Mvc;
using Autofac;
using Gallery.DataLayer.Managers;
using Gallery.Managers;

namespace Gallery.Controllers
{
    public class GalleryController : Controller
    {
        private readonly UserManager _userManager;

        public GalleryController()
        {
            _userManager = Startup.Resolver.Resolve<UserManager>();
        }

        [HttpGet]
        public ActionResult Discover()
        {
            if (!Request.IsLoggedIn())
                return RedirectToAction("Login");

            ViewBag.UserName = _userManager.GetById(Request.GetUserId())?.FullName;
            return View();
        }

        [HttpGet]
        public ActionResult Photos()
        {
            if (!Request.IsLoggedIn())
                return RedirectToAction("Login");

            ViewBag.UserName = _userManager.GetById(Request.GetUserId())?.FullName;
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult Login()
        {
            if (Request.IsLoggedIn())
                return RedirectToAction("Discover");
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