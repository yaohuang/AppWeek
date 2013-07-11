using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.ServiceModel;
using System.Web;
using DirectoryReader.ServiceReference;
using LunchBuddies.Controllers;
using Microsoft.ServiceBus;

namespace LunchBuddies
{
    public static class RelayService
    {
        private static string _issuerSecret = ConfigurationManager.AppSettings["ServiceBusKey"];
        public static Person FindUser(string email)
        {
            var channelFactory = new ChannelFactory<IUserService>(
                new NetTcpRelayBinding(),
                new EndpointAddress(ServiceBusEnvironment.CreateServiceUri("sb", "servicerelay", "userservice")));
                
            channelFactory.Endpoint.Behaviors.Add(new TransportClientEndpointBehavior
            {
                TokenProvider = TokenProvider.CreateSharedSecretTokenProvider("owner", _issuerSecret)
            });

            var proxy = channelFactory.CreateChannel();
            Person user = proxy.FindUser(email.Substring(0, email.IndexOf("@microsoft.com")).Replace(".", " "));
            return user;
        }
    }

    [ServiceContract]
    public interface IUserService
    {
        [OperationContract]
        Person FindUser(string name);
    }
}