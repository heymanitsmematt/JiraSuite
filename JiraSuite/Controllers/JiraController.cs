using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
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
        private NetsuiteManager _netsuiteManager = new NetsuiteManager();
        // GET: Jira
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult JiraSuiteSync()
        {
            List<DbEntityValidationException> saveErrors = new List<DbEntityValidationException>();
            //_netsuiteManager.UpdateDb();
            _jiraManager.UpdateDb(saveErrors);
            if (saveErrors.Any())
                return Json(new {success = true}, JsonRequestBehavior.AllowGet);
            else
                return Json(new {success = false, saveErrors = saveErrors.Select(x => x.Message).ToString()},
                    JsonRequestBehavior.AllowGet);
        }
    }
}
