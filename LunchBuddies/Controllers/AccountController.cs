using System;
using System.Data.Entity.Infrastructure;
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
            var newUser = new User
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

                return Content<User>(HttpStatusCode.OK, pendingRegistration.User);
            }
        }
    }
}
