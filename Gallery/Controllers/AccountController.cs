using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Results;
using Autofac;
using Gallery.DataLayer;
using Gallery.DataLayer.Base;
using Gallery.DataLayer.Entities;
using Gallery.DataLayer.Entities.Base;
using Gallery.DataLayer.Repositories;
using Gallery.Managers;
using Gallery.Models;
using Microsoft.Owin.Security;
using AuthenticationManager = Gallery.Managers.AuthenticationManager;

namespace Gallery.Controllers
{
    [Route("gallery/account")]
    public class AccountController : ApiController
    {
        private readonly UserManager _userManager;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly Managers.AuthenticationManager _authManager;

        public AccountController()
            : this(Startup.Resolver.Resolve<UserManager>(),
                   Startup.Resolver.Resolve<IPasswordHasher<User>>(),
                   Startup.Resolver.Resolve<Managers.AuthenticationManager>())
        {
        }

        public AccountController(UserManager userManager, IPasswordHasher<User> passwordHasher, Managers.AuthenticationManager authManager)
        {
            _userManager = userManager;
            _passwordHasher = passwordHasher;
            _authManager = authManager;
        }

        [HttpPost]
        [Route("gallery/account/login")]
        public async Task<HttpResponseMessage> Login([FromBody] LoginModel model)
        {
            if (model == null)
            {
                return await BadRequest("Model cannot be null").ExecuteAsync(CancellationToken.None);
            }


            if (!ModelState.IsValid)
                return await BadRequest(ModelState).ExecuteAsync(CancellationToken.None);

            var user = _userManager.Login(model.Email, model.Password);

            if (user == null)
                return await NotFound().ExecuteAsync(CancellationToken.None);
            
            if (_authManager.Authenticate(user, Request, model.RememberMe))
            {
                var resp = new HttpResponseMessage(HttpStatusCode.OK);

                var cookie = new CookieHeaderValue(AuthenticationManager.APPLICATION_COKIE_USER, user.Id.ToString())
                {
                    Expires = DateTimeOffset.Now.AddDays(1),
                    Domain = Request.RequestUri.Host,
                    Path = "/"
                };


                resp.Headers.AddCookies(new[] { cookie });
                return resp;
            }

            return await InternalServerError().ExecuteAsync(CancellationToken.None);
        }

        [HttpPost]
        [Route("gallery/account/logout")]
        public async Task<HttpResponseMessage> Logout()
        {
            var ctx = Request.GetOwinContext();

            if (_authManager.SingOut(ctx.Authentication))
            {

                var resp = new HttpResponseMessage(HttpStatusCode.OK);

                var cookie = new CookieHeaderValue(AuthenticationManager.APPLICATION_COKIE_USER, "")
                {
                    Expires = DateTimeOffset.Now.AddDays(-1),
                    Domain = Request.RequestUri.Host,
                    Path = "/"
                };


                resp.Headers.AddCookies(new[] { cookie });
                return resp;
            }

            return await InternalServerError().ExecuteAsync(CancellationToken.None);
        }

        [HttpPost]
        [Route("gallery/account/register")]
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

            return Ok();
        }
    }
}