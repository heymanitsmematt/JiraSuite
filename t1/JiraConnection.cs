using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TechTalk.JiraRestClient;
using static System.Configuration.ConfigurationManager;

namespace JiraSuite.Jira
{
    public class JiraConnection
    {
        private readonly string _baseUrl;
        private readonly string _username;
        private readonly string _password;
        private readonly JiraClient _client;

        public JiraConnection()
        {
            _baseUrl = AppSettings["JiraUrl"];
            _username = AppSettings["JiraUsername"];
            _password = AppSettings["JiraPassword"];
            _client = new JiraClient(_baseUrl, _username, _password);
        }


        public JiraClient Client
        {
            get { return _client; }
        }

    }
}
