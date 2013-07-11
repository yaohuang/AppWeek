using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.Owin.Security;
using LunchBuddies.Models;
using WebApplication2;
using LunchBuddies.Models;

namespace LunchBuddies.Controllers
{
    public class HomeController : Controller
    {
        public HomeController() : this(IdentityConfig.ExternalIdentityHandler) { }

        public HomeController(ISecureDataHandler<ClaimsIdentity> externalIdentityHandler)
        {
            ExternalIdentityHandler = externalIdentityHandler;
        }

        public ISecureDataHandler<ClaimsIdentity> ExternalIdentityHandler { get; set; }

        public async Task<ActionResult> Index(bool? logOff)
        {
            IndexModel model = new IndexModel
            {
                ExternalLoginProviders = GetExternalLoginProviders(),
                LogOff = logOff.HasValue && logOff.Value
            };

            ClaimsIdentity externalIdentity = await GetExternalIdentity();

            string state;

            if (externalIdentity != null)
            {
                state = ExternalIdentityHandler.Protect(externalIdentity);
            }
            else
            {
                state = null;
            }

            model.ExternalLoginProvider = GetLoginProvider(externalIdentity);
            model.ExternalLoginState = state;

            return View(model);
        }

        private Task<ClaimsIdentity> GetExternalIdentity()
        {
            return IdentityConfig.GetExternalIdentity(HttpContext);
        }

        private IEnumerable<ExternalLogin> GetExternalLoginProviders()
        {
            IEnumerable<AuthenticationDescription> descriptions = HttpContext.GetExternalAuthenticationTypes();
            List<ExternalLogin> logins = new List<ExternalLogin>();

            foreach (AuthenticationDescription description in descriptions)
            {
                logins.Add(new ExternalLogin
                {
                    Name = description.Caption,
                    Url = Url.HttpRouteUrl("ExternalLogin", new
                    {
                        provider = description.AuthenticationType,
                        returnUrl = Url.Action("Index")
                    })
                });
            }

            return logins;
        }

        private string GetLoginProvider(ClaimsIdentity identity)
        {
            if (identity == null)
            {
                return null;
            }

            Claim providerKeyClaim = identity.FindFirst(ClaimTypes.NameIdentifier);

            if (providerKeyClaim == null)
            {
                return null;
            }

            return providerKeyClaim.Issuer;
        }
    }
}