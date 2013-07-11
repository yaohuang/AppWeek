using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;

namespace LunchBuddies.Controllers
{
    public class ProfilePictureController : ApiController
    {
        public HttpResponseMessage GetPicture(string alias)
        {
            HttpResponseMessage response = new HttpResponseMessage();
            Stream stream = RelayService.GetProfileImage(alias);
            var content = new StreamContent(stream);
            content.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");
            response.Content = content;
            return response;
        }
    }
}
