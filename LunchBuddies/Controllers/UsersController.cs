using System;
using System.Linq;
using System.Net;
using System.Web.Http;
using LunchBuddies.Models;

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

        public IHttpActionResult CreateUser(CreateUserViewModel model)
        {
            Guid tokenId = Guid.Parse(model.Token);
            using (ModelsDbContext context = new ModelsDbContext())
            {
                PendingRegistration pendingRegistration = context.PendingRegistrations.FirstOrDefault(reg => reg.Id == tokenId);
                if (pendingRegistration == null)
                {
                    return StatusCode(HttpStatusCode.BadRequest);
                }
                pendingRegistration.User.Password = model.Password;
                context.Users.Add(pendingRegistration.User);
                context.PendingRegistrations.Remove(pendingRegistration);
                context.SaveChanges();
                return StatusCode(HttpStatusCode.OK);
            }
        }
    }
}
