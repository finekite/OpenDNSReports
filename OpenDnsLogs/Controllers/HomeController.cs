using OpenDns.Contracts;
using OpenDnsLogs.Models;
using OpenDnsLogs.Orchestrators;
using Serilog;
using System;
using System.Configuration;
using System.IO;
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
            if (!ModelHasCustomErrors<ModelError>((x) => !x.ErrorMessage.ToLower().Contains("Email")))
            {

                try
                {
                    if (await homeOrchestrator.VerifyOpenDNSLoginNewHttpClient(model.Login))
                    {
                        return View("MyDashboard", new ReportRequestDTO { EmailAddress = model.Login.UserName, Password = model.Login.Password });
                    }

                    model.ErrorMessage = "Login unsuccessful. Please ensure you have an OpenDns account";

                    return View("Index", model);
                }
                catch (Exception ex)
                {
                    model.ErrorMessage = "Something went wrong while processing your request. Please try again.";

                    return View("Index", model);
                }
            }

            return View("Index", model);
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
                catch (Exception ex)
                {
                    Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    return Json(new { Message = "Something went wrong while processing you request. Please try reloading the page and/or shortening the date range.", Success = false }, JsonRequestBehavior.AllowGet);
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

            if (ModelHasCustomErrors<ModelError>(x => !x.ErrorMessage.Contains("Start") && !x.ErrorMessage.Contains("End")))
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return PartialView("ReportModal", reportRequest);
            }

            try
            {
                if (await homeOrchestrator.VerifyOpenDNSLogin(new LoginDto { Password = reportRequest.Password, UserName = reportRequest.EmailAddress }))
                {
                    model.ReportResponseDTO = await homeOrchestrator.SetUpAccount(reportRequest);
                    return PartialView("_SignUpForEmail", model);
                }
                return Json("We were unable to verify your OpenDNS login. Please check your credentials and try again.");
            }
            catch (Exception ex)
            {
                return Json("Something went wrong while processing your request. Please try again later" + Environment.NewLine + ex.Message);
            }
        }

        public async Task<ActionResult> ManageEmailReports(string email)
        {
            try
            {
                var userId = await homeOrchestrator.GetUserId(email);

                var model = new ManageEmailReportsModel
                {
                    EmailReportSettingsList = await homeOrchestrator.GetEmailReportSettings(email),
                    Email = email,
                    UserId = await homeOrchestrator.GetUserId(email)
                };

                if (model.EmailReportSettingsList == null)
                {
                    return Json("We could not find any reports for your account. Please go to Set Up Email Reports to set up your email settings.", JsonRequestBehavior.AllowGet);
                }

                return PartialView("_ManageReportSettings", model);
            }
            catch (Exception ex)
            {
                return Json("Something went wrong while processing your request. Please try again later" + Environment.NewLine + ex.Message, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public async Task<ActionResult> AddEmailReportSettings(ManageEmailReportsModel model)
        {
            homeOrchestrator.AddEmailReportSetting(model);
            return await ManageEmailReports(model.Email);
        }

        [HttpPost]
        public async Task<ActionResult> EditEmailReportSettings(ManageEmailReportsModel model)
        {
            homeOrchestrator.EditEmailReportSetting(model);
            return await ManageEmailReports(model.Email);
        }

        [HttpPost]
        public async Task<ActionResult> DeleteEmailReportSettings(ManageEmailReportsModel model)
        {
            homeOrchestrator.DeleteEmailReportSetting(model);
            return await ManageEmailReports(model.Email);
        }

        public ActionResult SeeLogs()
        {
            var path = Path.Combine(Server.MapPath("~"), ConfigurationManager.AppSettings["LogFile"]);
            return View(model: System.IO.File.ReadAllText(path));
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

        protected override void OnException(ExceptionContext filterContext)
        {
            filterContext.ExceptionHandled = true;

            var exception = filterContext.Exception;

            Log.Logger = new LoggerConfiguration().WriteTo.File(AppDomain.CurrentDomain.BaseDirectory + ConfigurationManager.AppSettings["LogFile"]).CreateLogger();
            Log.Information("{@Exception}", exception);
            Log.CloseAndFlush();

            base.OnException(filterContext);
        }
    }
}