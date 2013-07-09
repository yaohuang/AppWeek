using System;
using System.Collections.Generic;
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

        public static Task<HttpResponseMessage> SendEmailAsync()
        {
            UriBuilder uri = new UriBuilder("https://sendgrid.com/api/mail.send.json");
            StringBuilder query = new StringBuilder();

            query.AppendFormat("api_user={0}", HttpUtility.UrlEncode("azure_8461cc7e7ed7d06de774dc84ca84bfe8@azure.com"));
            query.AppendFormat("&api_key={0}", HttpUtility.UrlEncode("di3nxta3"));
            query.AppendFormat("&from={0}", HttpUtility.UrlEncode("confirmation@lunchbuddies.com"));
            query.AppendFormat("&to={0}", HttpUtility.UrlEncode("robert.huang.007@gmail.com"));
            query.AppendFormat("&subject={0}", HttpUtility.UrlEncode("registration confirmation"));
            query.AppendFormat("&text={0}", HttpUtility.UrlEncode("Please confirm your account registration by clicking on the link below: "));
            uri.Query = query.ToString();
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, uri.Uri);
            return _client.SendAsync(request);
        }
    }
}