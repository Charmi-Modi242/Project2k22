using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using physioCard.Domain;
using physioCard.Services;
using physioCard.wwwroot.lib.Mail;
using System.Collections;

namespace physioCard.Controllers
{
    [Authorize]
    public class AppointmentController : Controller
    {
        private readonly DashBoardController _dashBoardController;
        private readonly IAppointmentService _appointmentService;
        private readonly IPatientService _patientService;
        private readonly IDoctorService _doctorService;

        public AppointmentController(DashBoardController dashBoardController, IAppointmentService appointmentService, IPatientService patientService, IDoctorService doctorService)
        {
            _dashBoardController = dashBoardController;
            _appointmentService = appointmentService;
            _patientService = patientService;
            _doctorService = doctorService;
        }
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> FixAppointment()
        {
            int did = (int)HttpContext.Session.GetInt32("docID");
            ViewBag.data = await _dashBoardController.getProfileAsync(did);
            ViewBag.PatientName = new SelectList(await _appointmentService.getPatientName(did), "patientID", "fname");
            List<Appointment> appointment = new List<Appointment>();
            appointment.Add(new Appointment { appointmentID = 0, doctorID = 0, patientID = 0, fname = "xyz", lname = "xyz", name = "xyz", date_start = DateTime.Now, date_end = DateTime.Now, starttime = DateTime.Now, endtime = DateTime.Now, sessiontime = DateTime.Now, cost = 0 });
            ViewBag.AppointmentClashStatus = false;
            ViewBag.ClashedAppointments = appointment;
            return View();
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> FixAppointment(Appointment appointment)
        {
            List<Appointment> sappointment = new List<Appointment>();
            try
            {
                bool cstatus;
                //String today = DateTime.Now.ToString("dd-MM-yyyy");
                int did = (int)HttpContext.Session.GetInt32("docID");
                ViewBag.PatientName = new SelectList(await _appointmentService.getPatientName(did), "patientID", "fname");
                ViewBag.data = await _dashBoardController.getProfileAsync(did);
                appointment.doctorID = did;
                appointment.cost = (int)appointment.cost;
                string day = null;
                ArrayList emaildate = new ArrayList();
                if (ModelState.IsValid)
                {
                    DateTime today = DateTime.Now.Date;
                    DateTime date = appointment.date_start.Date;
                    String time = DateTime.Now.ToString("HH-mm");
                    if (DateTime.Compare(date, today) >= 0)
                    {
                        if (DateTime.Equals(appointment.date_start, appointment.date_end))
                        {
                            cstatus = await _appointmentService.checkAppointmentClashes(appointment);
                            if (cstatus)
                            {
                                ViewBag.AppointmentClashStatus = true;
                                List<Appointment> clashed_appointments = await _appointmentService.getClashedAppointments(appointment);
                                ViewBag.ClashedAppointments = clashed_appointments;
                                return View();
                            }
                            else
                            {
                                string tdate = await _appointmentService.FixAppointment(appointment);
                                emaildate.Add(tdate);
                            }
                        }
                        else
                        {

                            List<Appointment> clashed_appointments = new List<Appointment>();
                            cstatus = await _appointmentService.checkAppointmentClashes(appointment);
                            if (cstatus)
                            {
                                ViewBag.AppointmentClashStatus = true;
                                List<Appointment> tempclashed_appointments = await _appointmentService.getClashedAppointments(appointment);
                                foreach (var item in tempclashed_appointments)
                                {
                                    clashed_appointments.Add(item);
                                }
                            }
                            else
                            {
                                string tdate = await _appointmentService.FixAppointment(appointment);
                                emaildate.Add(tdate);
                            }

                            int days = (appointment.date_end - appointment.date_start).Days;
                            for (int i = 0; i < days; i++)
                            {
                                appointment.date_start = appointment.date_start.AddDays(1);
                                for (int d = 0; d <= 6; d++)
                                {
                                    if (appointment.days[d] != false)
                                    {
                                        switch (d)
                                        {
                                            case 0:
                                                day = "Monday";
                                                break;
                                            case 1:
                                                day = "Tuesday";
                                                break;
                                            case 2:
                                                day = "Wednesday";
                                                break;
                                            case 3:
                                                day = "Thursday";
                                                break;
                                            case 4:
                                                day = "Friday";
                                                break;
                                            case 5:
                                                day = "Saturday";
                                                break;
                                            case 6:
                                                day = "Sunday";
                                                break;
                                        }
                                        if (appointment.date_start.DayOfWeek.ToString() == day)
                                        {
                                            cstatus = await _appointmentService.checkAppointmentClashes(appointment);
                                            if (cstatus)
                                            {
                                                ViewBag.AppointmentClashStatus = true;
                                                List<Appointment> tempclashed_appointments = await _appointmentService.getClashedAppointments(appointment);
                                                foreach (var item in tempclashed_appointments)
                                                {
                                                    clashed_appointments.Add(item);
                                                }
                                            }
                                            else
                                            {
                                                string tdate = await _appointmentService.FixAppointment(appointment);
                                                emaildate.Add(tdate);
                                            }
                                        }
                                    }
                                }
                            }

                            if (clashed_appointments.Count > 0)
                            {
                                Patient p1 = await _patientService.getpatient(appointment.patientID);
                                Doctor doc1 = await _doctorService.GetByIDAsync(did);
                                string to1 = p1.email;
                                string subject1 = "Appointment Scheduled Notice";
                                string message1 = sendmail.CreateBodyForAppointmentSchedule(p1, doc1, emaildate);
                                sendmail.sendingEmailWithHtml(to1, subject1, message1);
                                ViewBag.ClashedAppointments = clashed_appointments;
                                return View();
                            }
                        }

                        Patient p = await _patientService.getpatient(appointment.patientID);
                        Doctor doc = await _doctorService.GetByIDAsync(did);
                        string to = p.email;
                        string subject = "Appointment Scheduled Notice";
                        string message = sendmail.CreateBodyForAppointmentSchedule(p, doc, emaildate);
                        sendmail.sendingEmailWithHtml(to, subject, message);
                        return RedirectToAction(nameof(showAppointment));
                    }
                    else
                    {
                        sappointment.Add(new Appointment { appointmentID = 0, doctorID = 0, patientID = 0, fname = "xyz", lname = "xyz", name = "xyz", date_start = DateTime.Now, date_end = DateTime.Now, starttime = DateTime.Now, endtime = DateTime.Now, sessiontime = DateTime.Now, cost = 0 });
                        ViewBag.AppointmentClashStatus = false;
                        ViewBag.ClashedAppointments = sappointment;
                        ViewBag.DateError = true;
                        return View();
                    }
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Unable to fix appointment. " +
                "Try Again, and if the problem persists" +
                "See your system administrator.");
            }
            sappointment.Add(new Appointment { appointmentID = 0, doctorID = 0, patientID = 0, fname = "xyz", lname = "xyz", name = "xyz", date_start = DateTime.Now, date_end = DateTime.Now, starttime = DateTime.Now, endtime = DateTime.Now, sessiontime = DateTime.Now, cost = 0 });
            ViewBag.AppointmentClashStatus = false;
            ViewBag.ClashedAppointments = sappointment;
            return View();
        }

        public async Task<IActionResult> showAppointment()
        {
            int did = (int)HttpContext.Session.GetInt32("docID");
            ViewBag.did = did;
            ViewBag.data = await _dashBoardController.getProfileAsync(did);
            List<Appointment> appointments = await _appointmentService.getAllAppointments(did);
            foreach (var item in appointments)
            {
                item.endtime = item.starttime.Add(item.sessiontime.TimeOfDay);
            }
            ViewBag.appointments = appointments;
            return View();
        }

        public async Task<IActionResult> getCurrMonthAppointment()
        {
            int did = (int)HttpContext.Session.GetInt32("docID");
            List<Appointment> appointments = await _appointmentService.getAllAppointments(did);
            foreach (var item in appointments)
            {
                item.endtime = item.starttime.Add(item.sessiontime.TimeOfDay);
            }
            return Json(appointments);
            //Appointment appointment = null;
            //var tupleTwoModels = new Tuple<List<Appointment>, Appointment>(appointments, appointment);
            //ViewBag.appointments = appointments;
            //return View();
        }

        public async Task<IActionResult> getMonthlyAppointments(DateTime mDate)
        {
            int did = (int)HttpContext.Session.GetInt32("docID");
            List<Appointment> appointments = await _appointmentService.getMonthlyAppointment(did, mDate);
            foreach (var item in appointments)
            {
                item.endtime = item.starttime.Add(item.sessiontime.TimeOfDay);
            }
            return Json(appointments);
        }

        //public async Task<IActionResult> showAppointment(bool sdelete)
        //{
        // int did = (int)HttpContext.Session.GetInt32("docID");
        // ViewBag.data = await _dashBoardController.getProfileAsync(did);
        // List<Appointment> appointments = await _appointmentService.getAllAppointments(did);
        // foreach (var item in appointments)
        // {
        // item.endtime = item.starttime.Add(item.sessiontime.TimeOfDay);
        // }
        // ViewBag.statusdelete = sdelete;
        // //Appointment appointment = null;
        // //var tupleTwoModels = new Tuple<List<Appointment>, Appointment>(appointments, appointment);
        // return View(appointments);
        //}

        //[AutoValidateAntiforgeryToken]
        [HttpPost]
        public async Task<IActionResult> getAppointmentByID(int appointmentID)
        {
            Appointment appointment = await _appointmentService.getAppointmentByID(appointmentID);
            return Json(appointment);
        }

        [AutoValidateAntiforgeryToken]
        [HttpPost]
        public async Task<IActionResult> rescheduleAppointment(Appointment appointment)
        {
            try
            {
                int did = (int)HttpContext.Session.GetInt32("docID");
                ViewBag.data = await _dashBoardController.getProfileAsync(did);
                List<Appointment> appointments = await _appointmentService.getAllAppointments(did);
                foreach (var item in appointments)
                {
                    item.endtime = item.starttime.Add(item.sessiontime.TimeOfDay);
                }
                ViewBag.appointments = appointments;
                if (ModelState.IsValid)
                {
                    bool cstatus = await _appointmentService.checkAppointmentClashes(appointment);

                    if (cstatus)
                    {
                        ViewBag.clashstatus = true;
                        //ViewBag.statusupdate = false;
                        return View("showAppointment");
                    }
                    else
                    {
                        bool status = await _appointmentService.rescheduleAppointment(appointment);

                        Patient p = await _patientService.getpatient(appointment.patientID);
                        Doctor doc = await _doctorService.GetByIDAsync(did);
                        string to = p.email;
                        string subject = "Appointment Rescheduled Notice";
                        string message = "<h4>Dear " + p.fname + " " + p.lname + "</h4><p>Your appointment has been rescheduled on " + appointment.date_start.ToString("dd-MM-yyyy") + " " + appointment.starttime.ToString("HH:mm") + " for " + appointment.sessiontime.ToString("HH") + " hour and " + appointment.sessiontime.ToString("mm") + " minutes. Kindly contact your physiotherapist if you have any issues regarding it. </p><h5>Thank You</h5>";
                        sendmail.sendingEmailWithHtml(to, subject, message);

                        if (status)
                        {
                            ViewBag.statusupdate = true;
                            return View("showAppointment");
                        }
                        else
                        {
                            ViewBag.statusupdate = false;
                            return View("showAppointment");
                        }
                    }
                }
                else
                {
                    ViewBag.statusupdate = false;
                    return View("showAppointment");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Unable to reschedule appointment. " +
                "Try Again, and if the problem persists" +
                "See your system administrator.");
                ViewBag.statusupdate = false;
                return View("showAppointment");
            }
        }

        [HttpPost]
        public async Task<IActionResult> deleteAppointment(int appointmentID)
        {
            try
            {
                int did = (int)HttpContext.Session.GetInt32("docID");
                Appointment appointment = await _appointmentService.getAppointmentByID(appointmentID);
                bool status = await _appointmentService.deleteAppointment(appointmentID);

                Patient p = await _patientService.getpatient(appointment.patientID);
                Doctor doc = await _doctorService.GetByIDAsync(did);
                string to = p.email;
                string subject = "Appointment Cancellation Notice";
                string message = "<h4>Dear " + p.fname + " " + p.lname + "</h4><p>Your appointment that was on " + appointment.date_start.ToString("dd-MM-yyyy") + " " + appointment.starttime.ToString("HH:mm") + " has been cancelled. Kindly contact your physiotherapist if you have any issues regarding it. </p><h5>Thank You</h5>";
                sendmail.sendingEmailWithHtml(to, subject, message);

                ViewBag.data = await _dashBoardController.getProfileAsync(did);
                List<Appointment> appointments = await _appointmentService.getAllAppointments(did);
                foreach (var item in appointments)
                {
                    item.endtime = item.starttime.Add(item.sessiontime.TimeOfDay);
                }
                ViewBag.appointments = appointments;
                if (status)
                {
                    ViewBag.statusdelete = true;
                }
                else
                {
                    ViewBag.statusdelete = false;
                }
                return View("showAppointment");
            }
            catch (Exception ex)
            {
                ViewBag.statusdelete = false;

                ModelState.AddModelError("", "Unable to reschedule appointment. " +
                "Try Again, and if the problem persists" +
                "See your system administrator.");
                return View("showAppointment");
            }
        }

        public async Task<ActionResult> AppointmentCalendar()
        {
            int did = (int)HttpContext.Session.GetInt32("docID");
            ViewBag.data = await _dashBoardController.getProfileAsync(did);
            return View();
        }

        public async Task<IActionResult> GetAppointments()
        {
            int did = (int)HttpContext.Session.GetInt32("docID");
            List<Appointment> appList = await _appointmentService.getAppointments(did);
            foreach (var item in appList)
            {
                item.endtime = item.starttime.Add(item.sessiontime.TimeOfDay);
            }
            return Json(appList);
        }
    }
}
