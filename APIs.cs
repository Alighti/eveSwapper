using System;
using System.Collections.Generic;
using System.Net;
using RestSharp;
using System.Text.RegularExpressions;
using System.Linq;

namespace eveSwapper
{

    class APIs
    {
        public RestClient client = new RestClient();
        public RestRequest request = new RestRequest();
        public CookieContainer cookies = new CookieContainer();
        static private Random random = new Random();
        public string API_BASE = "https://i.instagram.com/api/v1";
        string CONSENT_PATH = "/consent/update_dob/";
        string CURRENT_PATH = "/accounts/current_user/?edit=true";
        string SET_PATH = "/accounts/set_username/";
        string BIO_PATH = "/accounts/set_biography/";
        string WEB_BASE = "https://www.instagram.com";
        string CHECK14_PATH = "/accounts/web_create_ajax/attempt/";
        public string USER_AGENT = $"Instagram {random.Next(5, 100)}.{random.Next(6, 10)}.{random.Next(0, 10)} Android (18/2.1; 160dpi; 720x900; ZTE; LAVA-9L7EZ; pdfz; hq3143; en_US)";
        public string username;
        public string email;
        public string phone_number;
        public string biography;

        public HttpStatusCode Consent(string Cookies)
        {
            client = new RestClient(API_BASE);
            client.Proxy = null;
            request = new RestRequest(CONSENT_PATH, Method.POST);
            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("X-CSRFToken", "missing");
            headers.Add("Content-Type", "application/x-www-form-urlencoded; charset=UTF-8");
            headers.Add("Connection", "keep-alive");
            request.AddCookie("sessionid",Cookies);
            client.UserAgent = USER_AGENT;
            client.AddDefaultHeaders(headers);
            request.AddParameter("signed_body", "SIGNATURE.{\"current_screen_key\":\"dob\",\"day\":\"1\",\"year\":\"1998\",\"month\":\"1\"}", ParameterType.RequestBody);
            var execute = client.Execute(request);
            return execute.StatusCode;
        }
        public HttpStatusCode Set_username(string u, string Cookies)
        {
            client = new RestClient(API_BASE);
            client.Proxy = null;
            request = new RestRequest(SET_PATH, Method.POST);
            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("X-CSRFToken", "missing");
            headers.Add("Content-Type", "application/x-www-form-urlencoded; charset=UTF-8");
            headers.Add("Connection", "keep-alive");
            request.AddCookie("sessionid", Cookies);
            client.UserAgent = USER_AGENT;
            client.AddDefaultHeaders(headers);
            request.AddParameter("", $"username={u}",ParameterType.RequestBody);
            var execute = client.Execute(request);
            return execute.StatusCode;
        }
        public HttpStatusCode Set_biography(string b, string Cookies)
        {
            client = new RestClient(API_BASE);
            client.Proxy = null;
            request = new RestRequest(BIO_PATH, Method.POST);
            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("X-CSRFToken", "missing");
            headers.Add("Content-Type", "application/x-www-form-urlencoded; charset=UTF-8");
            headers.Add("Connection", "keep-alive");
            request.AddCookie("sessionid", Cookies);
            client.UserAgent = USER_AGENT;
            client.AddDefaultHeaders(headers);
            request.AddParameter("", $"raw_text={b}", ParameterType.RequestBody);
            var execute = client.Execute(request);
            return execute.StatusCode;
        }
        public HttpStatusCode CHECK_BLOCK(string u, string Cookies)
        {
            client = new RestClient(API_BASE);
            client.Proxy = null;
            request = new RestRequest(SET_PATH, Method.POST);
            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("X-CSRFToken", "missing");
            headers.Add("Content-Type", "application/x-www-form-urlencoded; charset=UTF-8");
            headers.Add("Connection", "close");
            request.AddCookie("sessionid", Cookies);
            client.UserAgent = USER_AGENT;
            client.AddDefaultHeaders(headers);
            request.AddParameter("", $"username={u}", ParameterType.RequestBody);
            var execute = client.Execute(request);
            return execute.StatusCode;
        }
        public string CURRENT_USER(string Cookies)
        {
            client = new RestClient(API_BASE);
            client.Proxy = null;
            request = new RestRequest(CURRENT_PATH, Method.GET);
            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("X-CSRFToken", "missing");
            headers.Add("Content-Type", "application/x-www-form-urlencoded; charset=UTF-8");
            headers.Add("Connection", "close");
            request.AddCookie("sessionid", Cookies);
            client.UserAgent = USER_AGENT;
            client.AddDefaultHeaders(headers);
            var execute = client.Execute(request);
            username = Regex.Match(execute.Content, "\"username\":\"(.*?)\"").Groups[1].Value  ;
            email = Regex.Match(execute.Content, "\"email\":\"(.*?)\"").Groups[1].Value;
            phone_number = Regex.Match(execute.Content, "\"phone_number\":\"(.*?)\"").Groups[1].Value;
            biography = Regex.Match(execute.Content, "\"biography\":\"(.*?)\"").Groups[1].Value;
            return username;
        }
        public bool CHECK_14(string target)
        {
            bool check = false;
            client = new RestClient(WEB_BASE);
            client.Proxy = null;
            request = new RestRequest(CHECK14_PATH, Method.POST);
            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("X-CSRFToken", "missing");
            headers.Add("Content-Type", "application/x-www-form-urlencoded");
            client.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:82.0) Gecko/20100101 Firefox/82.0";
            client.AddDefaultHeaders(headers);
            request.AddParameter("hacker", $"email=&username={target}&first_name=&opt_into_one_tap=false", ParameterType.RequestBody);
            var execute = client.Execute(request);
            if (execute.Content.Contains("username_held_by_others"))
            {
                check = true;
            }
           
            return check;
        }
       
        public string RandomString(int length)
        {
            return new string(Enumerable.Repeat("ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789".ToLower(), length).Select((Func<string, char>)((string s) => s[random.Next(s.Length)])).ToArray());
        }
    }
}
