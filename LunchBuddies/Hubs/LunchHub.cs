using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using LunchBuddies.Models;
using Microsoft.AspNet.SignalR;

namespace LunchBuddies.Hubs
{
    public class LunchHub : Hub
    {
        private static ConcurrentDictionary<string, string> _user = new ConcurrentDictionary<string, string>();
        private static ConcurrentDictionary<string, string> _connectionId = new ConcurrentDictionary<string, string>();
        private static ModelsDbContext _db = new ModelsDbContext();
        private const string RequestGroup = "Lunch_"; 

        public override Task OnConnected()
        {
            _user.TryAdd("UserEmail", Context.ConnectionId);
            _connectionId.TryAdd(Context.ConnectionId, "UserEmail");
            return base.OnConnected();
        }

        public void Echo(string value)
        {
            Clients.Caller.Echo(value);
        }

        public void ToNewsFeed(string message)
        {
            Clients.All.newsFeed(message);
        }

        public void AddGroup(LunchRequest request)
        {
            var connectionId = request.Creator.UserName;
            string groupId = RequestGroup + request.Id;
            Groups.Add(connectionId, groupId);

            foreach(var userLunchRequest in request.Users)
            {
                connectionId = userLunchRequest.User.UserName;
                Groups.Add(connectionId, groupId);
            }
        }

        public void Invite(LunchRequest request)
        {
            var connectionId = request.Creator.UserName;
            string groupId = RequestGroup + request.Id;

            var payload = new
                {
                    Id = request.Id,
                    From = request.Creator.Email,
                    Subject = request.Subject,
                    MeetingPlace = request.MeetingPlace,
                    Invitees = new List<string>(),
                };
            foreach(var user in request.Users)
            {
                payload.Invitees.Add(user.User.Email);
            }

            Clients.Group(groupId, connectionId).Invite(request);
            Clients.Caller.Invite(payload);
        }

        public void FakeInvite(int id)
        {
            LunchRequest request = new LunchRequest();
            request.Creator = new User();
            request.Creator.Email = "gustavoa@microsoft.com";
            request.DateTimeCreated = DateTime.Now;
            request.DateTimeRequest = DateTime.Now;
            request.Id = id;
            request.MeetingPlace = "Alderaan";
            request.Subject = "AppWeek Lunch";
            request.Users = new List<UserLunchRequest>();
            request.Users.Add(new UserLunchRequest { User = new User() { Email = "a@microsoft.com" } });
            request.Users.Add(new UserLunchRequest { User = new User() { Email = "b@microsoft.com" } });
            
            Invite(request);
        }

        public void InviteResponse(UserLunchRequest userResponse)
        {
            string groupId = RequestGroup + userResponse.LunchRequest.Id;

            Clients.Group(groupId).InviteResponse(userResponse.User.UserName, userResponse.LunchStatus);
        }
    }
}