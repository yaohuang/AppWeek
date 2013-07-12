using LunchBuddies.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.OData;
using Microsoft.AspNet.Identity;

namespace LunchBuddies.Controllers
{
    public class UserLunchRequestsController : ODataController
    {
        private ModelsDbContext context = new ModelsDbContext();

    }
}