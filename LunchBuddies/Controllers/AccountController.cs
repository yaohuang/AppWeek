using System;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using DirectoryReader.ServiceReference;
using LunchBuddies.Models;
using LunchBuddies.Results;
using Microsoft.Owin.Security.OAuth;
using Microsoft.Owin.Security;
using System.Security.Claims;
using System.Collections.Generic;
using Microsoft.AspNet.Identity.EntityFramework;
using AppUser=LunchBuddies.Models.User;
using System.Globalization;

namespace LunchBuddies.Controllers
{
    public enum CheckEmailResult
    {
        EmailAlreadyExists = 1,
        InvalidEmailAddress = 2,
        ConfirmationTokenSent = 3,
        FailedToSendEmail = 4
    }

    public class EmailValidationResult
    {
        public CheckEmailResult Result { get; set; }
        public string Error { get; set; }
    }

    public class AccountController : ApiController
    {
        private ModelsDbContext context = new ModelsDbContext();

        public AccountController()
        {
            IdentityStore = new IdentityStoreManager(new IdentityStoreContext());
            AuthenticationManager = new IdentityAuthenticationManager(IdentityStore);
            Bearer = IdentityConfig.Bearer;
            ExternalTokenHandler = new ExternalAccessTokenHandler(IdentityConfig.Bearer.AccessTokenHandler);
        }

        public IdentityStoreManager IdentityStore { get; private set; }
        public IdentityAuthenticationManager AuthenticationManager { get; private set; }
        public OAuthBearerAuthenticationOptions Bearer { get; private set; }
        public ISecureDataHandler<ExternalAccessToken> ExternalTokenHandler { get; private set; }

        [HttpGet]
        public async Task<IHttpActionResult> Register(string email)
        {
            if(context.PendingRegistrations.Any(reg => String.Equals(reg.User.Email, email)))
            {
                return Content<EmailValidationResult>(HttpStatusCode.OK, new EmailValidationResult
                {
                    Result = CheckEmailResult.EmailAlreadyExists,
                    Error = "Already sent the email. Pending confirmation."
                });
            }

            if (context.Users.Any(u=>String.Equals(u.Email, email)))
            {
                return Content<EmailValidationResult>(HttpStatusCode.OK, new EmailValidationResult
                {
                    Result = CheckEmailResult.EmailAlreadyExists,
                    Error = "Already registered. Login with your account."
                });
            }

            if (!email.EndsWith("@microsoft.com"))
            {
                return Content<EmailValidationResult>(HttpStatusCode.OK, new EmailValidationResult
                {
                    Result = CheckEmailResult.InvalidEmailAddress,
                    Error = "Sorry, we only allow microsoft domain at this time."
                });
            }

            Person user = RelayService.FindUser(email);

            if (user == null)
            {
                return Content<EmailValidationResult>(HttpStatusCode.OK, new EmailValidationResult
                {
                    Result = CheckEmailResult.InvalidEmailAddress,
                    Error = String.Format("Could not find the user with email: {0}.", email)
                });
            }

            if (!user.Office.StartsWith("3"))
            {
                return Content<EmailValidationResult>(HttpStatusCode.OK, new EmailValidationResult
                {
                    Result = CheckEmailResult.InvalidEmailAddress,
                    Error = "Sorry, you're not qualified for a new account at this moment because you don't work in building 3."
                });
            }

            Guid guid = Guid.NewGuid();
            string pictureUrl = Url.Link("DefaultApi", new { controller = "ProfilePicture", action = "GetPicture", alias = user.Alias });
            var newUser = new AppUser
            {
                UserName = user.Name,
                Email = email,
                Office = user.Office,
                Alias = user.Alias,
                Department = user.Department,
                Telephone = user.Telephone,
                Title = user.Title,
                PictureUrl = pictureUrl,
                Password = "temp"
            };
            context.PendingRegistrations.Add(new PendingRegistration
            {
                User = newUser,
                Id = guid
            });

            // Send email
            string link = Url.Link("DefaultApi", new { controller = "Account", action = "RegistrationConfirmation", token = guid });
            HttpResponseMessage response = await EmailClient.SendEmailAsync(email, 
                String.Format(
                    "Please complete your account registration by clicking the link below:<br/><a href='{0}'>Complete Registration</a>", 
                    link));

            if(!response.IsSuccessStatusCode)
            {
                return Content<EmailValidationResult>(HttpStatusCode.OK, new EmailValidationResult
                {
                    Result = CheckEmailResult.FailedToSendEmail,
                    Error = "Oops, we cannot send the confirmation email at the moment. Please try again later."
                });
            }

            context.SaveChanges();

            return Content<EmailValidationResult>(HttpStatusCode.OK, new EmailValidationResult
            {
                Result = CheckEmailResult.ConfirmationTokenSent
            });
        }
        
        [HttpGet]
        public Task<IHttpActionResult> ResendConfirmation(string email)
        {
            var pendingRegistration = context.PendingRegistrations.FirstOrDefault(reg => String.Equals(reg.User.Email, email));
            if (pendingRegistration != null)
            {
                context.PendingRegistrations.Remove(pendingRegistration);
                context.SaveChanges();
                return Register(email);
            }

            return Task.FromResult<IHttpActionResult>(StatusCode(HttpStatusCode.NotFound));
        }

        [HttpGet]
        public IHttpActionResult RegistrationConfirmation(string token)
        {
            Guid tokenId = Guid.Parse(token);
            using (ModelsDbContext context = new ModelsDbContext())
            {
                ((IObjectContextAdapter)context).ObjectContext.ContextOptions.ProxyCreationEnabled = false;
                PendingRegistration pendingRegistration = context.PendingRegistrations.Include("User").FirstOrDefault(reg => reg.Id == tokenId);
                if (pendingRegistration == null)
                {
                    return StatusCode(HttpStatusCode.BadRequest);
                }

                return Content<AppUser>(HttpStatusCode.OK, pendingRegistration.User);
            }
        }

        [HttpPost]
        public async Task<IHttpActionResult> Login(OAuthPasswordCredentialsBindingModel model)
        {
            if (model == null)
            {
                return OAuthBadRequest(OAuthAccessTokenError.InvalidRequest);
            }

            if (model.grant_type != "password")
            {
                return OAuthBadRequest(OAuthAccessTokenError.UnsupportedGrantType);
            }

            ClaimsIdentity identity = await GetIdentityAsync(model.username);
            string token = Bearer.AccessTokenHandler.Protect(new AuthenticationTicket(identity, new AuthenticationExtra()));

            return OAuthAccessToken(token, "bearer", model.username);
        }

        private async Task<ClaimsIdentity> GetIdentityAsync(string userId)
        {
            IList<Claim> claims = null;

            return new ClaimsIdentity(claims, Bearer.AuthenticationType, AuthenticationManager.UserNameClaimType,
                AuthenticationManager.RoleClaimType);
        }

        private IHttpActionResult OAuthBadRequest(OAuthAccessTokenError error, string errorDescription = null)
        {
            return new OAuthAccessTokenBadRequestResult(error, errorDescription, this);
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

        public class ExternalAccessToken
        {
            public string LoginProvider { get; set; }

            public string ProviderKey { get; set; }

            public string DisplayName { get; set; }

            public DateTime Expires { get; set; }

            public bool IsValid
            {
                get
                {
                    return DateTime.UtcNow < Expires;
                }
            }
        }

        private class ExternalAccessTokenHandler : ISecureDataHandler<ExternalAccessToken>
        {
            private const string LoginProvider = "p";
            private const string ProviderKey = "k";
            private const string DisplayName = "d";
            private const string Expires = "e";

            private static readonly ClaimsIdentity emptyIdentity = new ClaimsIdentity(claims: null,
                authenticationType: String.Empty);

            public ExternalAccessTokenHandler(ISecureDataHandler<AuthenticationTicket> innerHandler)
            {
                InnerHandler = innerHandler;
            }

            public ISecureDataHandler<AuthenticationTicket> InnerHandler { get; set; }

            public string Protect(ExternalAccessToken data)
            {
                return InnerHandler.Protect(new AuthenticationTicket(emptyIdentity, Serialize(data)));
            }

            public ExternalAccessToken Unprotect(string protectedText)
            {
                AuthenticationTicket ticket = InnerHandler.Unprotect(protectedText);

                if (ticket == null)
                {
                    return null;
                }

                AuthenticationExtra extra = ticket.Extra;

                if (extra == null)
                {
                    return null;
                }

                return Deserialize(extra);
            }

            private static AuthenticationExtra Serialize(ExternalAccessToken token)
            {
                AuthenticationExtra extra = new AuthenticationExtra();

                if (token == null)
                {
                    return extra;
                }

                extra.Properties[LoginProvider] = token.LoginProvider ?? String.Empty;
                extra.Properties[ProviderKey] = token.ProviderKey ?? String.Empty;
                extra.Properties[DisplayName] = token.DisplayName ?? String.Empty;
                extra.Properties[Expires] = token.Expires.ToString("u", CultureInfo.InvariantCulture);
                return extra;
            }

            private static ExternalAccessToken Deserialize(AuthenticationExtra extra)
            {
                return new ExternalAccessToken
                {
                    LoginProvider = extra.Properties[LoginProvider],
                    ProviderKey = extra.Properties[ProviderKey],
                    DisplayName = extra.Properties[DisplayName],
                    Expires = DateTime.ParseExact(extra.Properties[Expires], "u", CultureInfo.InvariantCulture)
                };
            }
        }
    }
}
