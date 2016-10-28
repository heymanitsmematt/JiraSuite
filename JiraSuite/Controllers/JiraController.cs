using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using JiraSuite.Managers;
using TechTalk.JiraRestClient;

namespace JiraSuite.Controllers
{
    public class JiraController : Controller
    {
        private JiraManager _jiraManager = new JiraManager();
        // GET: Jira
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult JiraSuiteSync()
        {
            _jiraManager.UpdateDb();
            return Json(new {success = true});
        }
    }
}