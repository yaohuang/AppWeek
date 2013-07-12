using LunchBuddies.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.OData;
using Microsoft.AspNet.Identity;

namespace LunchBuddies.Controllers
{
    public class LunchRequestsController : ODataController
    {
        private ModelsDbContext context = new ModelsDbContext();

        [Queryable]
        public IEnumerable<LunchRequest> GetLunchRequests()
        {
            User u = GetCurrentUser();
            if (u != null)
            {
                var createdLunches = context.LunchRequests.Where(l => l.Creator.Email == u.Email);
                return u.LunchRequests.Select(l => l.LunchRequest).ToArray().Union(createdLunches.ToArray());    
            }
            return new List<LunchRequest>();
        }

        [Queryable]
        public SingleResult<LunchRequest> GetLunchRequest([FromODataUri]int id)
        {
            return SingleResult.Create(context.LunchRequests.Where(l => l.Id == id));
        }

        public async Task<HttpResponseMessage> PostLunchRequest(LunchRequest lunchRequest)
        {
            if (lunchRequest == null && !ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }

            lunchRequest.Creator = GetCurrentUser();

            lunchRequest = context.LunchRequests.Add(lunchRequest);

            await context.SaveChangesAsync();

            return Request.CreateResponse(HttpStatusCode.Created, lunchRequest);
        }

        public async Task<HttpResponseMessage> PatchLunchRequest([FromODataUri]int key, Delta<LunchRequest> delta)
        {
            context.Configuration.ValidateOnSaveEnabled = false;
            var lr = context.LunchRequests.Single(s => s.Id == key);
            delta.Patch(lr);
            await context.SaveChangesAsync();
            return Request.CreateResponse(HttpStatusCode.OK);
        }

        public async Task<HttpResponseMessage> DeleteLunchRequest([FromODataUri]int key)
        {
            context.LunchRequests.RemoveRange(context.LunchRequests.Where(l => l.Id == key));
            await context.SaveChangesAsync();

            return Request.CreateResponse(HttpStatusCode.OK);
        }

        private User GetCurrentUser()
        {
            return context.Users.Find(User.Identity.GetUserName());
        }
    }
}