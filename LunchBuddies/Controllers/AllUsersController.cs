using LunchBuddies.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LunchBuddies.Controllers
{
    public class AllUsersController : Controller
    {
        //
        // GET: /AllUsers/
        public ActionResult Index()
        {
            using (var db = new ModelsDbContext())
            {
                var userList = db.Users;

                User[] users = userList.ToArray();
                return View(users);
            }
        }

        //
        // GET: /AllUsers/5
        public ActionResult Floor(int floor)
        {
            using (var db = new ModelsDbContext())
            {
                var userList = filterByFloor(db.Users, floor);

                User[] users = userList.ToArray();
                ViewBag.floor = floor;
                return View(users);
            }
        }

        private IEnumerable<User> filterByFloor(IEnumerable<User> users, int floor)
        {
            return users.Where(u => u.Office.StartsWith("3/" + floor.ToString()));
        }
    }
}
