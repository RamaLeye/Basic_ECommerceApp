using Newtonsoft.Json;
using System;

namespace UserSDK
{
    [Serializable]
    public class User
    {
        [JsonProperty("last_name")]
        string last_name { get; set; }
        [JsonProperty("first_name")]
        string first_name { get; set; }
        [JsonProperty("email")]
        string mail { get; set; }
        [JsonProperty("username")]
        string username { get; set; }

        public User(string ln, string fn, string m, string un)
        {
            last_name = ln;
            first_name = fn;
            mail = m;
            username = un;
        }

        public User()
        {
            last_name = "";
            first_name = "";
            mail = "";
            username = "";
        }

        public User(User u)
        {
            last_name = u.getLastName();
            first_name = u.getFirstName();
            mail = u.getMail();
            username = u.getUserName();
        }

        public override string ToString()
        {
            string profil = last_name + " ---- " + first_name;
            return profil;
        }

        public User getUser(string username)
        {
            return this;
        }

        public string getLastName()
        {
            return last_name;
        }

        public string getFirstName()
        {
            return first_name;
        }

        public string getMail()
        {
            return mail;
        }
        public string getUserName()
        {
            return username;
        }
    }
}
