using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LunchBuddies.Models;
using Microsoft.AspNet.SignalR;

namespace LunchBuddies.Hubs
{
    public class LunchHub : Hub
    {
        private static ConcurrentDictionary<string, string> _userName = new ConcurrentDictionary<string, string>();
        private static ConcurrentDictionary<string, string> _connectionId = new ConcurrentDictionary<string, string>();
        private static ModelsDbContext _db = new ModelsDbContext();
        private const string RequestGroup = "Lunch_"; 

        public void SetIdentity(string userName)
        {
            _userName.TryAdd(userName, Context.ConnectionId);
            _connectionId.TryAdd(Context.ConnectionId, userName);

            ToNewsFeed(userName + " connected");

            string connectionId = Context.ConnectionId;
            var lunchRequests = _db.LunchRequests.Where(i => i.Creator.Email == userName);
            foreach(var lunchRequest in lunchRequests)
            {
                string groupId = RequestGroup + lunchRequest.Id;
                Groups.Add(connectionId, groupId);
            }

            var userLunchRequests = _db.UserLunchRequests.Where(i => i.User.Email == userName);
            foreach (var userLunchRequest in userLunchRequests)
            {
                string groupId = RequestGroup + userLunchRequest.LunchRequest.Id;
                Groups.Add(connectionId, groupId);
            }
        }

        public override Task OnDisconnected()
        {
            string userName;
            _connectionId.TryGetValue(Context.ConnectionId, out userName);
            ToNewsFeed(userName + " disconnected");
            return base.OnDisconnected();
        }
        
        public void ToNewsFeed(string message)
        {
            Clients.All.newsFeed(message);
        }

        public void AddGroup(LunchRequest lunchRequest)
        {
            string groupId = RequestGroup + lunchRequest.Id;
            string connectionId;                        
            if(_userName.TryGetValue(lunchRequest.Creator.Email, out connectionId))
            {
                Groups.Add(connectionId, groupId);
            }

            foreach(var userLunchRequest in lunchRequest.Users)
            {
                string user = userLunchRequest.User.Email;
                if (_userName.TryGetValue(user, out connectionId))
                {
                    Groups.Add(connectionId, groupId);
                }
            }
        }

        public void Invite(LunchRequest request, bool onlyCaller)
        {
            string groupId = RequestGroup + request.Id;

            var payload = new
                {
                    Id = request.Id,
                    From = request.Creator.Email,
                    Subject = request.Subject,
                    MeetingPlace = request.MeetingPlace,
                    DateTime = request.DateTimeRequest,
                    Invitees = new List<object>(),
                };
            foreach(var userLunchRequest in request.Users)
            {
                payload.Invitees.Add(new 
                { 
                    Id = userLunchRequest.Id, 
                    Email = userLunchRequest.User.Email, 
                    Status = Enum.GetName(typeof(LunchStatus), userLunchRequest.LunchStatus), 
                });
            }

            if (!onlyCaller)
            {
                Clients.Group(groupId).Invite(payload);
            }
            else
            {
                Clients.Caller.Invite(payload);
            }
        }

        public async Task Invitations()
        {
            await Task.Delay(0);

            string userName = Clients.Caller.UserName;

            var lunchRequests = _db.LunchRequests.Where(i => i.Creator.Email == userName);
            foreach (var lunchRequest in lunchRequests)
            {
                Invite(lunchRequest, onlyCaller: true);
            }

            var userLunchRequests = _db.UserLunchRequests.Where(i => i.User.Email == userName);
            foreach(var userLunchRequest in userLunchRequests)
            {
                var lunchRequest = _db.LunchRequests.Find(userLunchRequest.LunchRequest.Id);
                Invite(lunchRequest, onlyCaller: true);
            }
        }

        public void InviteResponse(UserLunchRequest userLunchRequest)
        {
            string groupId = RequestGroup + userLunchRequest.LunchRequest.Id;
            string message = string.Format("{0} responded {1} to invitation {2}",
                userLunchRequest.User.Email, userLunchRequest.LunchStatus, userLunchRequest.LunchRequest.Id);

            Clients.Group(groupId).InviteResponse(new 
            {
                Id = userLunchRequest.Id, 
                Status = Enum.GetName(typeof(LunchStatus), userLunchRequest.LunchStatus),
            });
            Clients.Group(groupId).newsFeed(message);
        }

        public void FakeInvite()
        {
            string creatorName = "gustavoa@microsoft.com";
            string userName = Clients.Caller.UserName;
            if (Clients.Caller.UserName == creatorName)
            {
                creatorName = "yaohuang@microsoft.com";
            }

            LunchRequest request = new LunchRequest();
            request.Creator = _db.Users.Find(creatorName);
            request.DateTimeCreated = DateTime.Now;
            request.DateTimeRequest = DateTime.Now;
            request.MeetingPlace = "Here";
            request.Subject = "AppWeek Lunch";
            request.Users = new List<UserLunchRequest>();
            var users = _db.Users.Where(i => i.Email != request.Creator.Email);
            foreach(var user in users)
            { 
                request.Users.Add(new UserLunchRequest { User = user, LunchStatus = LunchStatus.Unanswered });
            }

            _db.LunchRequests.Add(request);
            try
            {
                _db.SaveChanges();
            }
            catch(Exception e)
            {
                throw e;
            }           

            AddGroup(request);
            Invite(request, onlyCaller: false);
            ToNewsFeed(userName + " created an invitation");
        }

        public void FakeInviteResponse(int id, int response)
        {
            string userName = Clients.Caller.UserName;
            LunchStatus lunchStatus = (LunchStatus)response;

            UserLunchRequest userLunchRequest = _db.UserLunchRequests.First(i => i.LunchRequest.Id == id && i.User.Email == userName);
            userLunchRequest.LunchStatus = lunchStatus;
            _db.SaveChanges();

            InviteResponse(userLunchRequest);
            ToNewsFeed(userName + " responded an invitation");
        }
    }
}