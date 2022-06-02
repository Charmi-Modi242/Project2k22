using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using physioCard.Domain;
using physioCard.Services;

namespace physioCard.Controllers
{
    [Authorize]
    public class DashBoardController : Controller
    {
        private readonly IDoctorClinicService _doctorClinicService;
        private readonly IDoctorService _doctorService;

        public DashBoardController(IDoctorClinicService doctorClinicService, IDoctorService doctorService)
        {
            _doctorClinicService = doctorClinicService;
            _doctorService = doctorService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<ActionResult> DashBoard()
        {
            try
            {
                int did;
                int uid = Int32.Parse(Request.Cookies["uID"]);
                if (uid == 0)
                {
                    did = (int)HttpContext.Session.GetInt32("docID");
                }
                else
                {
                    did = uid;
                    HttpContext.Session.SetInt32("docID", did);
                }
                bool IsClinic = await _doctorClinicService.IsClinicAsync(did);
                if (IsClinic == false)
                    return RedirectToAction("addDoctorClinic", "Clinic");
                else
                {
                    ViewBag.data = await getProfileAsync(did);
                    return View();
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Unable to show dashboard. " +
                "Try Again, and if the problem persists" +
                "See your system administrator.");
            }
            return View();
        }

        public async Task<string[]> getProfileAsync(int did)
        {
            Doctor doctor = await _doctorService.GetByIDAsync(did);
            string name = doctor.fname + " " + doctor.lname;
            string[] str = { doctor.docID.ToString(), doctor.photo, name };
            return str;
        }
    }
}

