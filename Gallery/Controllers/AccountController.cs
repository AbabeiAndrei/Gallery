using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Web.Http;
using System.Web.Http.Results;
using Gallery.DataLayer.Base;
using Gallery.DataLayer.Entities;
using Gallery.DataLayer.Entities.Base;
using Gallery.DataLayer.Repositories;
using Gallery.Models;
using Microsoft.Owin.Security;

namespace Gallery.Controllers
{
    [Route("account")]
    public class AccountController : ApiController
    {

        private readonly UserManager _userManager;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly Managers.AuthenticationManager _authManager;

        public AccountController(UserManager userManager, IPasswordHasher<User> passwordHasher, Managers.AuthenticationManager authManager)
        {
            _userManager = userManager;
            _passwordHasher = passwordHasher;
            _authManager = authManager;
        }

        [HttpPost]
        [Route("login")]
        public IHttpActionResult Login([FromBody] LoginModel model)
        {
            if (model == null)
                return BadRequest("Model cannot be null");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = _userManager.Login(model.Email, model.Password);

            if (user == null)
                return NotFound();

            var context = Request.GetOwinContext();
            var authManager = context.Authentication;

            if(_authManager.Authenticate(user, authManager, model.RemeberMe))
                return Redirect("/");

            return InternalServerError();
        }

        [HttpPost]
        [Route("logout")]
        public IHttpActionResult Logout()
        {
            var ctx = Request.GetOwinContext();

            if(_authManager.SingOut(ctx.Authentication))
                return Redirect("/login");

            return InternalServerError();
        }

        [HttpPost]
        [Route("register")]
        public IHttpActionResult Register([FromBody]RegisterModel model)
        {
            if (model == null)
                return BadRequest("Model cannot be null");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (_userManager.Exist(model.Email))
                return Conflict();

            var user = new User
                       {
                            CreatedAt = DateTime.Now,
                            Email = model.Email,
                            FullName = model.FullName,
                            Role = UserRole.Regular,
                            RowState = RowState.Created
                       };

            user.Password = _passwordHasher.HashPassword(model.Password, user);
            
            _userManager.Add(user);

            return Redirect("/login");
        }
    }
}