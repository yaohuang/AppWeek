using System;
using System.Linq;
using System.Net;
using System.Web.Http;
using LunchBuddies.Models;

namespace LunchBuddies.Controllers
{
    public class UsersController : ApiController
    {
        private ModelsDbContext context = new ModelsDbContext();

        [HttpGet]
        public IHttpActionResult CreateUser(string token, string password)
        {
            Guid tokenId = Guid.Parse(token);
            using (ModelsDbContext context = new ModelsDbContext())
            {
                PendingRegistration pendingRegistration = context.PendingRegistrations.FirstOrDefault(reg => reg.Id == tokenId);
                if (pendingRegistration == null)
                {
                    return StatusCode(HttpStatusCode.BadRequest);
                }
                context.Users.Add(pendingRegistration.User);
                context.PendingRegistrations.Remove(pendingRegistration);
                context.SaveChanges();
                return StatusCode(HttpStatusCode.OK);
            }
        }
    }
}
