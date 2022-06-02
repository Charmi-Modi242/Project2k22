using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using physioCard.Domain;
using physioCard.Services;

namespace physioCard.Controllers
{
    [Authorize]
    public class ClinicController : Controller
    {
        private readonly IClinicService _clinicService;
        private readonly IDoctorClinicService _doctorClinicService;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly DashBoardController _dashBoardController;
        public ClinicController(IClinicService clinicService, IDoctorClinicService doctorClinicService, IWebHostEnvironment webHostEnvironment, DashBoardController dashBoardController)
        {
            _clinicService = clinicService;
            _doctorClinicService = doctorClinicService;
            _webHostEnvironment = webHostEnvironment;
            _dashBoardController = dashBoardController;
        }
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> AddDoctorClinic()
        {
            ViewBag.did = HttpContext.Session.GetInt32("docID");
            ViewBag.data = await _dashBoardController.getProfileAsync(ViewBag.did);
            ViewBag.clinics = new MultiSelectList(await _clinicService.GetAllAsync(), "clinicID", "name");
            bool IsClinic = await _doctorClinicService.IsClinicAsync(ViewBag.did);
            ViewData["IsClinic"] = IsClinic;
            return View();
        }

        public async Task<IActionResult> getClinicByID(int id)
        {
            Clinic clinic = await _clinicService.GetByClinicID(id);
            //var s = Json(clinic);
            return Json(clinic);
        }

        public async Task<IActionResult> GetByClinicID(int id)
        {
            try
            {
                Clinic clinic = await _clinicService.GetByClinicID(id);
                int did = (int)HttpContext.Session.GetInt32("docID");
                ViewBag.data = await _dashBoardController.getProfileAsync(did);
                return View(clinic);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Unable to show Clinic. " +
                                             "Try Again, and if the problem persists" +
                                             "See your system administrator.");
            }
            return View();
        }

        public async Task<IActionResult> RemoveClinic(int id)
        {
            try
            {
                await _doctorClinicService.RemoveAsync(id);
                int did = (int)HttpContext.Session.GetInt32("docID");
                bool IsClinic = await _doctorClinicService.IsClinicAsync(did);
                if (IsClinic)
                {
                    return RedirectToAction("ViewProfile", "Doctor");
                }
                else
                {
                    return RedirectToAction(nameof(AddDoctorClinic));
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Unable to Delete Clinic. " +
                                             "Try Again, and if the problem persists" +
                                             "See your system administrator.");
            }
            return View();
        }

        [AutoValidateAntiforgeryToken]
        [HttpPost]
        public async Task<IActionResult> AddDoctorClinic(Doctor_Clinic doctor_clinic)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    await _doctorClinicService.CreateDoctorClinicAsync(doctor_clinic);
                    return RedirectToAction("DashBoard", "DashBoard");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Unable to add clinic/clinics in doctor. " +
                                             "Try Again, and if the problem persists" +
                                             "See you system administrator.");
            }
            ViewBag.clinics = new MultiSelectList(await _clinicService.GetAllAsync(), "clinicID", "name");
            return View();
        }
        public async Task<IActionResult> CreateClinic()
        {
            int did = (int)HttpContext.Session.GetInt32("docID");
            ViewBag.data = await _dashBoardController.getProfileAsync(did);
            bool IsClinic = await _doctorClinicService.IsClinicAsync(did);
            ViewData["IsClinic"] = IsClinic;
            return View();
        }

        [AutoValidateAntiforgeryToken]
        [HttpPost]
        public async Task<IActionResult> CreateClinic(Clinic clinic)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var headSign = HttpContext.Request.Form.Files.GetFile("head_signature_img");
                    var headSignFolder = "Clinic/head_signature/";
                    headSignFolder += Guid.NewGuid().ToString().Substring(0, 8) + "_" + headSign.FileName;
                    string headSignServer = Path.Combine(_webHostEnvironment.WebRootPath, headSignFolder);
                    await headSign.CopyToAsync(new FileStream(headSignServer, FileMode.Create));
                    clinic.head_signature = headSignFolder;

                    var logoImg = HttpContext.Request.Form.Files.GetFile("logo_img");
                    var logoFolder = "Clinic/logo/";
                    logoFolder += Guid.NewGuid().ToString().Substring(0, 8) + "_" + logoImg.FileName;
                    string logoServer = Path.Combine(_webHostEnvironment.WebRootPath, logoFolder);
                    await logoImg.CopyToAsync(new FileStream(logoServer, FileMode.Create));
                    clinic.logo = logoFolder;

                    await _clinicService.CreateClinicAsync(clinic);
                    return RedirectToAction(nameof(AddDoctorClinic));
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Unable to Add Clinic. " +
                                             "Try Again, and if the problem persists" +
                                             "See you system administrator.");
            }
            return View(clinic);
        }
    }
}
