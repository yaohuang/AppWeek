using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Web;
using System.Web.Helpers;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Forms;
using Microsoft.Owin.Security.OAuth;
using LunchBuddies.Models;

namespace WebApplication2
{
    // For more information on ASP.NET Identity, visit http://go.microsoft.com/fwlink/?LinkId=301863
    public static class IdentityConfig
    {
        public const string CookieAuthenticationType = "Cookies";
        public const string LocalLoginProvider = "Local";

        public static IUserSecretStore Secrets { get; set; }
        public static IUserLoginStore Logins { get; set; }
        public static IUserStore Users { get; set; }
        public static IRoleStore Roles { get; set; }
        public static OAuthBearerAuthenticationOptions Bearer { get; set; }
        public static ISecureDataHandler<ClaimsIdentity> ExternalIdentityHandler { get; set; }
        public static string RoleClaimType { get; set; }
        public static string UserNameClaimType { get; set; }
        public static string UserIdClaimType { get; set; }
        public static string ClaimsIssuer { get; set; }

        public static void ConfigureIdentity()
        {
            var dbContextFactory = new DbContextFactory<IdentityDbContext>();
            Secrets = new EFUserSecretStore<UserSecret>(dbContextFactory);
            Logins = new EFUserLoginStore<UserLogin>(dbContextFactory);
            Users = new EFUserStore<User>(dbContextFactory);
            Roles = new EFRoleStore<Role, UserRole>(dbContextFactory);
            Bearer = new OAuthBearerAuthenticationOptions();
            ExternalIdentityHandler = new ClaimsIdentityHandler(Bearer);
            RoleClaimType = ClaimsIdentity.DefaultRoleClaimType;
            UserIdClaimType = "http://schemas.microsoft.com/aspnet/userid";
            UserNameClaimType = "http://schemas.microsoft.com/aspnet/username";
            ClaimsIssuer = ClaimsIdentity.DefaultIssuer;
            AntiForgeryConfig.UniqueClaimTypeIdentifier = IdentityConfig.UserIdClaimType;
        }

        public static async Task<IEnumerable<Claim>> FindClaims(IUserStore userStore, IRoleStore roleStore, string userId, IEnumerable<Claim> providerClaims)
        {
            User user = await userStore.Find(userId) as User;
            if (user != null)
            {
                IList<Claim> userClaims = IdentityConfig.RemoveNameClaims(providerClaims);
                IdentityConfig.AddUserIdentityClaims(userId, user.UserName, userClaims);
                IdentityConfig.AddRoleClaims(await Roles.GetRolesForUser(userId), userClaims);
                return userClaims;
            }

            return null;
        }

        public static ClaimsIdentity CreateBearerIdentity(IEnumerable<Claim> claims)
        {
            return CreateIdentity(claims, Bearer.AuthenticationType);
        }

        public static ClaimsIdentity CreateCookieIdentity(IEnumerable<Claim> claims)
        {
            return CreateIdentity(claims, CookieAuthenticationType);
        }

        private static ClaimsIdentity CreateIdentity(IEnumerable<Claim> claims, string authenticationType)
        {
            return new ClaimsIdentity(claims, authenticationType, UserNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
        }

        private static void DeleteExternalCookie(HttpResponseBase response)
        {
            HttpCookie externalCookie = response.Cookies[FormsAuthenticationDefaults.CookiePrefix + FormsAuthenticationDefaults.ExternalAuthenticationType];
            externalCookie.Expires = DateTime.Now.AddDays(-1);
            response.SetCookie(externalCookie);
        }

        public static async Task<ClaimsIdentity> GetExternalIdentity(HttpContextBase context)
        {
            ClaimsIdentity identity = await context.GetExternalIdentity();

            if (identity == null)
            {
                return null;
            }

            // Change authentication type back to issuer
            Claim providerKeyClaim = identity.FindFirst(ClaimTypes.NameIdentifier);

            if (providerKeyClaim != null && !String.IsNullOrEmpty(providerKeyClaim.Issuer))
            {
                identity = new ClaimsIdentity(identity.Claims, providerKeyClaim.Issuer, identity.NameClaimType, identity.RoleClaimType);
            }

            DeleteExternalCookie(context.Response);

            return identity;
        }

        public static IList<Claim> RemoveNameClaims(IEnumerable<Claim> claims)
        {
            List<Claim> filteredClaims = new List<Claim>();
            foreach (var c in claims)
            {
                // Strip out any existing name/nameid claims
                if (c.Type != ClaimTypes.Name &&
                    c.Type != ClaimTypes.NameIdentifier)
                {
                    filteredClaims.Add(c);
                }
            }
            return filteredClaims;
        }

        public static void AddRoleClaims(IEnumerable<string> roles, IList<Claim> claims)
        {
            foreach (string role in roles)
            {
                claims.Add(new Claim(RoleClaimType, role, ClaimsIssuer));
            }
        }

        public static void AddUserIdentityClaims(string userId, string displayName, IList<Claim> claims)
        {
            claims.Add(new Claim(ClaimTypes.Name, userId, ClaimsIssuer));
            claims.Add(new Claim(UserIdClaimType, userId, ClaimsIssuer));
            claims.Add(new Claim(UserNameClaimType, displayName, ClaimsIssuer));
        }
    }

    public class ClaimsIdentityHandler : ISecureDataHandler<ClaimsIdentity>
    {
        private const string AuthenticationTypeClaim = "_AuthenticationType";
        private const string NameClaimTypeClaim = "_NameClaimType";
        private const string RoleClaimTypeClaim = "_RoleClaimType";

        public ClaimsIdentityHandler()
            : this(IdentityConfig.Bearer)
        {
        }

        public ClaimsIdentityHandler(OAuthBearerAuthenticationOptions bearer)
        {
            Bearer = bearer;
        }

        public OAuthBearerAuthenticationOptions Bearer { get; set; }

        public string Protect(ClaimsIdentity data)
        {
            return Bearer.AccessTokenHandler.Protect(new AuthenticationTicket(CreateUnauthenticated(data),
                (IDictionary<string, string>)null));
        }

        public ClaimsIdentity Unprotect(string protectedText)
        {
            AuthenticationTicket ticket = Bearer.AccessTokenHandler.Unprotect(protectedText);

            if (ticket == null)
            {
                return null;
            }

            ClaimsIdentity unauthenticated = ticket.Identity;

            if (unauthenticated == null)
            {
                return null;
            }

            return CreateAuthenticated(unauthenticated);
        }

        private static ClaimsIdentity CreateUnauthenticated(ClaimsIdentity identity)
        {
            const string Suppress = "_";
            ClaimsIdentity unauthenticated = new ClaimsIdentity(identity.Claims, String.Empty, Suppress, Suppress);
            unauthenticated.AddClaim(new Claim(AuthenticationTypeClaim, identity.AuthenticationType));
            unauthenticated.AddClaim(new Claim(NameClaimTypeClaim, identity.NameClaimType));
            unauthenticated.AddClaim(new Claim(RoleClaimTypeClaim, identity.RoleClaimType));
            return unauthenticated;
        }

        private static ClaimsIdentity CreateAuthenticated(ClaimsIdentity identity)
        {
            Claim authenticationType = identity.FindFirst(AuthenticationTypeClaim);
            identity.RemoveClaim(authenticationType);
            Claim roleClaimType = identity.FindFirst(RoleClaimTypeClaim);
            identity.RemoveClaim(roleClaimType);
            Claim nameClaimType = identity.FindFirst(NameClaimTypeClaim);
            identity.RemoveClaim(nameClaimType);
            return new ClaimsIdentity(identity.Claims, authenticationType.Value, nameClaimType.Value, roleClaimType.Value);
        }
    }
}

namespace Microsoft.AspNet.Identity
{
    public static class IdentityExtensions
    {
        public static string GetUserName(this IIdentity identity)
        {
            return identity.Name;
        }

        public static string GetUserId(this IIdentity identity)
        {
            ClaimsIdentity ci = identity as ClaimsIdentity;
            if (ci != null)
            {
                return ci.FindFirstValue(WebApplication2.IdentityConfig.UserIdClaimType);
            }
            return String.Empty;
        }

        public static string FindFirstValue(this ClaimsIdentity identity, string claimType)
        {
            Claim claim = identity.FindFirst(claimType);
            if (claim != null)
            {
                return claim.Value;
            }
            return null;
        }
    }
}