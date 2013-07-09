using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace LunchBuddies
{
    public static class EmailClient
    {
        private static HttpClient _client = new HttpClient();
        private static string _emailClient = ConfigurationManager.AppSettings["EmailClientUser"];
        private static string _emailClientKey = ConfigurationManager.AppSettings["EmailClientKey"];

        public static Task<HttpResponseMessage> SendEmailAsync(string destination, string content)
        {
            UriBuilder uri = new UriBuilder("https://sendgrid.com/api/mail.send.json");
            StringBuilder query = new StringBuilder();

            if (String.IsNullOrEmpty(_emailClient) || String.IsNullOrEmpty(_emailClientKey))
            {
                throw new InvalidOperationException("EmailClientUser/EmailClientKey is missing from the configuration.");
            }

            query.AppendFormat("api_user={0}", HttpUtility.UrlEncode(_emailClient));
            query.AppendFormat("&api_key={0}", HttpUtility.UrlEncode(_emailClientKey));
            query.AppendFormat("&from={0}", HttpUtility.UrlEncode("confirmation@lunchbuddies.com"));
            query.AppendFormat("&to={0}", HttpUtility.UrlEncode(destination));
            query.AppendFormat("&subject={0}", HttpUtility.UrlEncode("registration confirmation"));
            query.AppendFormat("&text={0}", HttpUtility.UrlEncode(content));
            uri.Query = query.ToString();
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, uri.Uri);
            return _client.SendAsync(request);
        }
    }
}