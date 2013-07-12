using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Http;
using LunchBuddies.Models;
using LunchBuddies.Results;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OAuth;
using Microsoft.AspNet.Identity.EntityFramework;

namespace LunchBuddies.Controllers
{
    public class CreateUserViewModel
    {
        public string Token { get; set; }
        public string Password { get; set; }
    }
    public class UsersController : ApiController
    {
        private ModelsDbContext context = new ModelsDbContext();

        public IdentityStoreManager IdentityStore { get; private set; }
        public OAuthBearerAuthenticationOptions Bearer { get; private set; }

        public IdentityAuthenticationManager AuthenticationManager { get; private set; }

        public UsersController()
        {
            IdentityStore = new IdentityStoreManager(new IdentityStoreContext());
            Bearer = IdentityConfig.Bearer;
            AuthenticationManager = new IdentityAuthenticationManager(IdentityStore);
        }
        public async Task<IHttpActionResult> CreateUser(CreateUserViewModel model)
        {
            if (String.IsNullOrEmpty(model.Password))
            {
                return StatusCode(HttpStatusCode.BadRequest);
            }
            Guid tokenId = Guid.Parse(model.Token);
            using (ModelsDbContext context = new ModelsDbContext())
            {
                PendingRegistration pendingRegistration = context.PendingRegistrations.FirstOrDefault(reg => reg.Id == tokenId);
                if (pendingRegistration == null)
                {
                    return StatusCode(HttpStatusCode.BadRequest);
                }
                var user = pendingRegistration.UserView;                
                context.Users.Add(new LunchBuddies.Models.User
                    {
                        UserName = user.UserName,
                        Alias = user.Alias,
                        Department = user.Department,
                        Email = user.Email,
                        Office = user.Office,
                        Password = model.Password,
                        PictureUrl = user.PictureUrl,
                        Telephone = user.Telephone,
                        Title = user.Title
                    });

                context.PendingRegistrations.Remove(pendingRegistration);
                context.SaveChanges();

                ClaimsIdentity identity = await GetIdentityAsync(user.UserName);
                string token = Bearer.AccessTokenHandler.Protect(new AuthenticationTicket(identity, new AuthenticationExtra()));

                return OAuthAccessToken(token, "bearer", user.UserName);


                //return StatusCode(HttpStatusCode.OK);
            }
        }

        private IHttpActionResult OAuthAccessToken(string accessToken, string tokenType, string userName)
        {
            return new OAuthAccessTokenResult(new OAuthTokenViewModel
            {
                AccessToken = accessToken,
                TokenType = tokenType,
                UserName = userName
            }, this);
        }

        private async Task<ClaimsIdentity> GetIdentityAsync(string userId)
        {
            IList<Claim> claims = await AuthenticationManager.GetUserIdentityClaims(userId, new Claim[0]);
            claims.Add(new Claim(AuthenticationManager.UserNameClaimType, userId));

            return new ClaimsIdentity(claims, Bearer.AuthenticationType, AuthenticationManager.UserNameClaimType,
                AuthenticationManager.RoleClaimType);
        }
    }
}
