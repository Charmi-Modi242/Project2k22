using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using physioCard.Domain;
using physioCard.Services;
using physioCard.wwwroot.lib.Mail;

namespace physioCard.Controllers
{
    [Authorize]
    public class AttendanceController : Controller
    {
        private readonly IAttendanceService _attendanceService;
        private readonly DashBoardController _dashBoardController;
        private readonly IAppointmentService _appointmentService;
        private readonly IPatientService _patientService;

        public AttendanceController(IAttendanceService attendanceService, DashBoardController dashBoardController, IAppointmentService appointmentService, IPatientService patientService) 
        {
            _attendanceService = attendanceService;
            _dashBoardController = dashBoardController;
            _appointmentService = appointmentService;
            _patientService = patientService;
        }
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> showTodaysAppointment()
        {
            int did = (int)HttpContext.Session.GetInt32("docID");
            ViewBag.data = await _dashBoardController.getProfileAsync(did);
            List<Appointment> appointments = await _attendanceService.getTodaysAppointment(did);
            foreach (var item in appointments)
            {
                item.endtime = item.starttime.Add(item.sessiontime.TimeOfDay);
            }
            ViewBag.appointments = appointments;
            return View();
        }

        [HttpPost]
        public async Task<bool> makeAttendance(bool attendance, int appointmentID)
        {
            bool status = await _attendanceService.markAttendance(attendance, appointmentID);
            Appointment appointment = await _appointmentService.getAppointmentByID(appointmentID);
            Patient patient = await _patientService.getpatient(appointment.patientID);
            string to = patient.email;
            string subject = "Attendance Marking Update";
            string message = string.Empty;
            if (attendance)
            {
                message = "<h4>Dear " + patient.fname + " " + patient.lname + "</h4><p>As you have got your today's physiotherapy session(<b>Appointment scheduled on " + appointment.date_start.ToString("dd-MM-yyyy") + "</b>) your attendance has been marked present. If you have not attended and still your attendance has been marked present kindly contact your physiotherapist as appointments that are marked as attended will be included in invoice.<br/><h5>Thank You</h5>";
            }
            else
            {
                message = "<h4>Dear " + patient.fname + " " + patient.lname + "</h4><p>Your attendance for appointment scheduled on " + appointment.date_start.ToString("dd-MM-yyyy") + " has been marked absent. Kindly contact your physiotherapist if you have any issues regarding it. </p><h5>Thank You</h5>";
            }
            sendmail.sendingEmailWithHtml(to, subject, message);
            return status;
        }
    }
}
