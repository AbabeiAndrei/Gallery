using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Web;
using Gallery.DataLayer.Base;
using Gallery.DataLayer.Entities;
using Gallery.DataLayer.Entities.Base;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;

namespace Gallery.Managers
{
    public class AuthenticationManager
    {
        private readonly IContext _context;

        public AuthenticationManager(IContext context)
        {
            _context = context;
        }

        public const string APPLICATION_COKIE_AUTH = DefaultAuthenticationTypes.ApplicationCookie;

        public const string APPLICATION_COKIE_USER = "userToken";

        public bool Authenticate(User user, HttpRequestMessage request, bool isPersistent)
        {
            var context = request.GetOwinContext();
            var authManager = context.Authentication;

            var result = Authenticate(user, authManager, isPersistent);

            return result;
        }

        public bool Authenticate(User user, IAuthenticationManager manager, bool isPersistent)
        {
            var claims = new[]
                         {
                             new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                             new Claim(ClaimTypes.Email, user.Email),
                             new Claim(ClaimTypes.Name, user.FullName),
                             new Claim(ClaimTypes.Role, user.Role.ToString())
                         };

            var identity = new ClaimsIdentity(claims, APPLICATION_COKIE_AUTH);

            manager.SignIn(new AuthenticationProperties
                           {
                               AllowRefresh = true,
                               IsPersistent = isPersistent,
                               ExpiresUtc = DateTimeOffset.Now.AddDays(7)
                           }, identity);



            return true;
        }

        public bool SingOut(IAuthenticationManager manager)
        {
            manager.SignOut(APPLICATION_COKIE_AUTH);
            return true;
        }

        public bool HasAccess<T>(int userId, T resource, Operation operation, object data = null) where T : IEntity
        {
            var user = _context.GetById<User>(userId);

            return resource.HasAccess(user, operation, data);
        }
    }

    public static class AuthenitcationManagerEx
    {
        public static int GetUserId(this HttpRequestMessage request)
        {
            try
            {
                //var context = request.GetOwinContext();

                //var user = context?.Authentication?.User;

                //if (user == null)
                //    return -1;

                //var claimIdentity = user.FindFirst(ClaimTypes.NameIdentifier);

                var cookie = request.Headers.GetCookies(AuthenticationManager.APPLICATION_COKIE_USER).FirstOrDefault();
                int userId;

                if (cookie == null || !int.TryParse(cookie[AuthenticationManager.APPLICATION_COKIE_USER].Value, out userId))
                    return -1;

                return userId;
            }
            catch (Exception)
            {
                return -1;
            }
        }

        public static bool IsLoggedIn(this HttpRequestMessage request)
        {
            return GetUserId(request) > 0;
        }

        public static int GetUserId(this HttpRequestBase request)
        {
            try
            {
                //var context = request.GetOwinContext();

                //if (context.Authentication == null)
                //    return -1;

                //var user = context.Authentication.User;
                //var claimIdentity = user.FindFirst(ClaimTypes.NameIdentifier);
                //int userId;

                //if (claimIdentity == null || !int.TryParse(claimIdentity.Value, out userId))
                //    return -1;


                var cookie = request.Cookies.Get(AuthenticationManager.APPLICATION_COKIE_USER);
                int userId;

                if (cookie == null || !int.TryParse(cookie.Value, out userId))
                    return -1;

                return userId;
            }
            catch (Exception ex)
            {
                return -1;
            }
        }

        public static bool IsLoggedIn(this HttpRequestBase request)
        {
            return GetUserId(request) > 0;
        }
    }
}