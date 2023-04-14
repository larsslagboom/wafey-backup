using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Wafey
{
    public class User
    {
        DBHelper connection = new DBHelper();

        [JsonProperty("UserID")]
        public int UserID { get; set; }
        [JsonProperty("UserFirstName")]
        public string UserFirstName { get; set; }
        [JsonProperty("UserLastName")]
        public string UserLastName { get; set; }
        [JsonProperty("UserEmail")]
        public string UserEmail { get; set; }
        [JsonProperty("UserSubscription")]
        public int UserSubscription { get; set; }
        [JsonProperty("UserPassword")]
        public string UserPassword { get; set; }
        [JsonProperty("UserSalt")]
        public string UserSalt { get; set; }
        [JsonProperty("Admin")]
        public int Admin { get; set; }

        public User loadCurrentUser(int userID)
        {

            List<User> usersData = connection.getData<User>("http://145.44.233.180/selectuser.php?id=" + userID);

            if(usersData.Count > 0)
            {
                return usersData.First();
            }

            return null;
        }

        public bool SubscribeUser(int userID)
        {

            string result = new WebClient().DownloadString("http://145.44.233.180/updateuser.php?subscribe&id=" + userID);

            if (result.Equals("Success"))
            {
                return true;
            }
            return false;
        }
        
    }

}
