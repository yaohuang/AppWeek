using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using LunchBuddies.Models;
using Microsoft.AspNet.SignalR;

namespace LunchBuddies.Hubs
{
    public class LunchHub : Hub
    {
        private static ConcurrentDictionary<string, string> _userMap = new ConcurrentDictionary<string, string>();
        private static ModelsDbContext _db = new ModelsDbContext();

        public override Task OnConnected()
        {
            _userMap.TryAdd("UserEmail", Context.ConnectionId);
            return base.OnConnected();
        }

        public void Echo(string value)
        {
            Clients.Caller.Echo(value);
        }

        public void MyLuchRequests()
        {
            
            Clients.Caller.myLunchRequests();
        }
    }
}