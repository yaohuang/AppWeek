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

namespace LunchBuddies.Controllers
{
    public class LunchRequestsController : ODataController
    {
        private ModelsDbContext _db = new ModelsDbContext();

        [Queryable]
        public IQueryable<LunchRequest> GetLunchRequests()
        {
            return _db.LunchRequests;
        }

        [Queryable]
        public SingleResult<LunchRequest> GetLunchRequest([FromODataUri]int id)
        {
            return SingleResult.Create(_db.LunchRequests.Where(l => l.Id == id));
        }

        public async Task<HttpResponseMessage> PostLunchRequest(LunchRequest lunchRequest)
        {
            lunchRequest = _db.LunchRequests.Add(lunchRequest);
            await _db.SaveChangesAsync();

            return Request.CreateResponse(HttpStatusCode.Created, lunchRequest);
        }

        public async Task<HttpResponseMessage> DeleteLunchRequest([FromODataUri]int id)
        {
            LunchRequest lunchRequest = new LunchRequest { Id = id };
            _db.Entry(lunchRequest).State = System.Data.Entity.EntityState.Deleted;
            await _db.SaveChangesAsync();

            return Request.CreateResponse(HttpStatusCode.OK);
        }
    }
}