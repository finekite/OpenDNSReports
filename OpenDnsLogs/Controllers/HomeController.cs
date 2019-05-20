using OpenDns.Contracts;
using OpenDnsLogs.Models;
using OpenDnsLogs.Orchestrators;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace OpenDnsLogs.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHomeOrchestrator homeOrchestrator;

        public HomeController(IHomeOrchestrator homeOrchestrator)
        {
            this.homeOrchestrator = homeOrchestrator;
        }

        public ActionResult Index(LoginModel model)
        {
            return View(model);
        }

        public async Task<ActionResult> Login(LoginModel model)
        {
            try
            {
                if (await homeOrchestrator.VerifyOpenDNSLogin(model.Login))
                {
                    return View("MyDashboard");
                }

                model.ErrorMessage = "Login unsuccessful. Please ensure you have an OpenDns account";

                return View("Index", model);
            }
            catch(Exception ex)
            {
                model.ErrorMessage = "Something went wrong while processing your request. Please try again.";

                return View("Index", model);
            }
        }

        [HttpPost]
        public async Task<ActionResult> GenerateReportNow(ReportRequestDTO reportRequest)
        {
            if (!ModelHasCustomErrors<ModelError>((x) => !x.ErrorMessage.Contains("Email") && !x.ErrorMessage.Contains("From")))
            {
                try
                {
                    var model = await homeOrchestrator.GenerateReport(reportRequest);
                    return PartialView("_GenerateReportNow", model);
                }
                catch (IndexOutOfRangeException ex)
                {
                    Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    return Json(new { Message = "Your session has expired please generate report again. Thanks!", Success = false }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    return Json(new { Message = "Your account is not authrorized to retrive data from these dates. Please shorten date range.", Success = false }, JsonRequestBehavior.AllowGet);
                }
            }

            Response.StatusCode = (int)HttpStatusCode.BadRequest;
            return View("ReportModal", reportRequest);
        }

        [HttpPost]
        public ActionResult GetNextSet(DomainListModel model)
        {
            try
            {
                model = homeOrchestrator.GetNextSet(model);
                return PartialView("_GenerateReportNow", model);
            }
            catch
            {
                return Json("Your session has expired please generate report again. Thanks!");
            }
        }

        [HttpPost]
        public ActionResult GetPreviousSet(DomainListModel model)
        {
            try
            {
                model = homeOrchestrator.GetPreviousSet(model);
                return PartialView("_GenerateReportNow", model);
            }
            catch
            {
                return Json("Your session has expired please generate report again. Thanks!");
            }
        }

        [HttpPost]
        public async Task<ActionResult> SignUpForEmail(ReportRequestDTO reportRequest)
        {
            var model = new ReportResponseModel();

            if (ModelHasCustomErrors<ModelError>((x) => !x.ErrorMessage.Contains("Start") && !x.ErrorMessage.Contains("End")))
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return PartialView("ReportModal", reportRequest);
            }

            try
            {
                model.ReportResponseDTO = await homeOrchestrator.SetUpAccount(reportRequest);
                return PartialView("_SignUpForEmail", model);
            }
            catch (Exception ex)
            {
                return Json("Something went wrong while processing your request. Please try again later" + Environment.NewLine + ex.Message);
            }
        }

        private bool ModelHasCustomErrors<T>(Func<T, bool> errorCondition) where T : class
        {
            foreach (ModelState state in ViewData.ModelState.Values.Where(x => x.Errors.Count > 0))
            {
                foreach (var error in state.Errors)
                {
                    if (errorCondition(error as T))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}