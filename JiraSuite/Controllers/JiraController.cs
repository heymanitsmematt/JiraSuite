using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using JiraSuite.Managers;
using TechTalk.JiraRestClient;

namespace JiraSuite.Controllers
{
    public class JiraController : AsyncController
    {
        private JiraManager _jiraManager = new JiraManager();
        private NetsuiteManager _netsuiteManager = new NetsuiteManager();
        private UpdateManager _syncManager = new UpdateManager();
        // GET: Jira
        public ActionResult Index()
        {
            return View();
        }

        public async Task<ActionResult> JiraSuiteSync()
        {
            List<DbEntityValidationException> saveErrors = new List<DbEntityValidationException>();
            _netsuiteManager.UpdateDb(saveErrors);
            _jiraManager.UpdateDb(saveErrors);
            _jiraManager.GetJiraTicketsWithMissingInfoFromNetsuite();
            _syncManager.PostNetsuiteUpdates();
            if (saveErrors.Any())
                return Json(new {success = true}, JsonRequestBehavior.AllowGet);
            return Json(new {success = false, saveErrors = saveErrors.Select(x => x.Message).ToString()},
                JsonRequestBehavior.AllowGet);
        }
    }
}
