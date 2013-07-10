using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.ServiceModel;
using System.Threading.Tasks;
using System.Web.Http;
using DirectoryReader.ServiceReference;
using LunchBuddies.Models;
using Microsoft.ServiceBus;

namespace LunchBuddies.Controllers
{
    [ServiceContract]
    public interface IUserService
    {
        [OperationContract]
        Person FindUser(string name);
    }

    public class UsersController : ApiController
    {
        [HttpGet]
        public async Task<IHttpActionResult> Register(string email)
        {
            // TODO: check if email already registered or pending confirmation

            if (!email.EndsWith("@microsoft.com"))
            {
                return Content<string>(HttpStatusCode.BadRequest, "Sorry, we only allow microsoft domain at this time.");
            }

            Person user = RelayService.FindUser(email);

            if (user == null)
            {
                return Content<string>(HttpStatusCode.NotFound, String.Format("Could not find the user with email: {0}.", email));
            }

            if (!user.Office.StartsWith("3"))
            {
                return Content<string>(HttpStatusCode.BadRequest, "Sorry, we only allow users in building 3 at this time.");
            }

            Guid guid = Guid.NewGuid();
            using (ModelsDbContext context = new ModelsDbContext())
            {
                context.PendingRegistrations.Add(new PendingRegistration
                    {
                        Email = email,
                        Id = guid
                    });
                context.SaveChanges();
            }

            // Send email
            string link = Url.Link("DefaultApi", new { controller = "Users", action = "RegistrationConfirmation", token = guid });
            HttpResponseMessage response = await EmailClient.SendEmailAsync(email, "Please confirm your account registration by clicking on the link below: \r\n"+link);

            return Content<string>(HttpStatusCode.OK, "Request submitted. Please check your email for confirmation.");
        }
        
        [HttpGet]
        public IHttpActionResult RegistrationConfirmation(string token)
        {
            Guid tokenId = Guid.Parse(token);
            using (ModelsDbContext context = new ModelsDbContext())
            {
                PendingRegistration pendingRegistration = context.PendingRegistrations.FirstOrDefault(reg => reg.Id == tokenId);
                if (pendingRegistration == null)
                {
                    return StatusCode(HttpStatusCode.BadRequest);
                }
                Person user = RelayService.FindUser(pendingRegistration.Email);
                return Content<Person>(HttpStatusCode.OK, user);
            }
        }

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
                Person user = RelayService.FindUser(pendingRegistration.Email);

                context.Users.Add(new User
                    {
                        Password = password,
                        Name = user.Name
                    });
                context.SaveChanges();
                return StatusCode(HttpStatusCode.OK);
            }
        }
    }
}
