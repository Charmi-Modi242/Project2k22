using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using physioCard.Domain;
using physioCard.Services;

namespace physioCard.Controllers
{
    [Authorize]
    public class DashBoardController : Controller
    {
        private readonly IDoctorClinicService _doctorClinicService;
        private readonly IDoctorService _doctorService;
        private readonly IAppointmentService _appointmentService;
        private readonly IAttendanceService _attendanceService;
        private readonly IPatientService _patientService;
        private readonly IInvoiceService _invoiceService;

        public DashBoardController(IDoctorClinicService doctorClinicService, IDoctorService doctorService, IAttendanceService attendanceService, IAppointmentService appointmentService, IPatientService patientService, IInvoiceService invoiceService)
        {
            _doctorClinicService = doctorClinicService;
            _doctorService = doctorService;
            _attendanceService = attendanceService;
            _appointmentService = appointmentService;
            _patientService = patientService;
            _invoiceService = invoiceService;
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
                    List<Appointment> appointments = await _attendanceService.getTodaysAppointment(did);
                    foreach (var item in appointments)
                    {
                        item.endtime = item.starttime.Add(item.sessiontime.TimeOfDay);
                    }
                    ViewBag.appointment = appointments;
                    List<Patient> patient = await _patientService.getallpatient(did);
                    foreach (Patient item in patient)
                    {
                        if (item.medical_History == null)
                        {
                            item.medical_History = "None";
                        }
                        TimeSpan ts = DateTime.Now - item.dob;
                        int age = (int)ts.TotalDays / 365;
                        item.age = age;
                    }
                    ViewBag.patients = patient;
                    ViewBag.patientCount = await _patientService.getPatientCount(did);
                    ViewBag.appointmentCount = await _appointmentService.getAppointmentCount(did);
                    ViewBag.revenueCount = await _invoiceService.getRevenueCount(did);
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
        public async Task<IActionResult> getYearlyData()
        {
            int did = (int)HttpContext.Session.GetInt32("docID");
            List<revenueModel> rm = await _invoiceService.getYearlyRevenue(did);
            foreach (var item in rm)
            {
                DateTime d = new DateTime(2022, item.monthID, 1);
                item.mName = d.ToString("MMM");
            }
            return Json(rm);
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

