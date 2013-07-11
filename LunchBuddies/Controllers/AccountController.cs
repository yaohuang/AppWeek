using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using DirectoryReader.ServiceReference;
using LunchBuddies.Models;

namespace LunchBuddies.Controllers
{
    public enum CheckEmailResult
    {
        EmailAlreadyExists = 1,
        InvalidEmailAddress = 2,
        ConfirmationTokenSent = 3
    }

    public class EmailValidationResult
    {
        public CheckEmailResult Result { get; set; }
        public string Message { get; set; }
    }

    public class AccountController : ApiController
    {
        private ModelsDbContext context = new ModelsDbContext();

        [HttpGet]
        public async Task<IHttpActionResult> Register(string email)
        {
            if(context.PendingRegistrations.Any(reg => String.Equals(reg.User.Email, email)))
            {
                return Content<EmailValidationResult>(HttpStatusCode.OK, new EmailValidationResult
                {
                    Result = CheckEmailResult.EmailAlreadyExists,
                    Message = "Already sent the email. Pending confirmation."
                });
            }

            if (context.Users.Any(u=>String.Equals(u.Email, email)))
            {
                return Content<EmailValidationResult>(HttpStatusCode.OK, new EmailValidationResult
                {
                    Result = CheckEmailResult.EmailAlreadyExists,
                    Message = "Already registered. Login with your account."
                });
            }

            if (!email.EndsWith("@microsoft.com"))
            {
                return Content<EmailValidationResult>(HttpStatusCode.OK, new EmailValidationResult
                {
                    Result = CheckEmailResult.InvalidEmailAddress,
                    Message = "Sorry, we only allow microsoft domain at this time."
                });
            }

            Person user = RelayService.FindUser(email);

            if (user == null)
            {
                return Content<string>(HttpStatusCode.NotFound, String.Format("Could not find the user with email: {0}.", email));
            }

            if (!user.Office.StartsWith("3"))
            {
                return Content<EmailValidationResult>(HttpStatusCode.OK, new EmailValidationResult
                {
                    Result = CheckEmailResult.InvalidEmailAddress,
                    Message = "Sorry, we only allow users in building 3 at this time."
                });
            }

            Guid guid = Guid.NewGuid();
            var newUser = new User
            {
                UserName = user.Name,
                Email = email,
                Office = user.Office,
                Alias = user.Alias,
                Department = user.Department,
                Telephone = user.Telephone,
                Title = user.Title,
                Password = "temp"
            };
            context.PendingRegistrations.Add(new PendingRegistration
            {
                User = newUser,
                Id = guid
            });
            context.SaveChanges();

            // Send email
            string link = Url.Link("DefaultApi", new { controller = "Users", action = "RegistrationConfirmation", token = guid });
            HttpResponseMessage response = await EmailClient.SendEmailAsync(email, "Please confirm your account registration by clicking on the link below: \r\n" + link);

            return Content<EmailValidationResult>(HttpStatusCode.OK, new EmailValidationResult
            {
                Result = CheckEmailResult.ConfirmationTokenSent,
                Message = "Request submitted. Please check your email for confirmation."
            });
        }
        
        public Task<IHttpActionResult> ResendConfirmation(string email)
        {
            var pendingRegistration = context.PendingRegistrations.FirstOrDefault(reg => String.Equals(reg.User.Email, email));
            if (pendingRegistration != null)
            {
                context.PendingRegistrations.Remove(pendingRegistration);
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
                PendingRegistration pendingRegistration = context.PendingRegistrations.FirstOrDefault(reg => reg.Id == tokenId);
                if (pendingRegistration == null)
                {
                    return StatusCode(HttpStatusCode.BadRequest);
                }

                return Content<User>(HttpStatusCode.OK, pendingRegistration.User);
            }
        }
    }
}
