using LunchBuddies.Models;
using System.Collections.Generic;

namespace LunchBuddies.Models
{
    public class IndexModel
    {
        public IEnumerable<ExternalLogin> ExternalLoginProviders { get; set; }

        public string ExternalLoginProvider { get; set; }

        public string ExternalLoginState { get; set; }

        public bool LogOff { get; set; }
    }
}