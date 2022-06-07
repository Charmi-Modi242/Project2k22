using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using physioCard.Domain;
using physioCard.Services;

namespace physioCard.Controllers
{
    [Authorize]
    public class AppointmentController : Controller
    {
        private readonly DashBoardController _dashBoardController;
        private readonly IAppointmentService _appointmentService;
        private readonly IPatientService _patientService;

        public AppointmentController(DashBoardController dashBoardController, IAppointmentService appointmentService, IPatientService patientService)
        {
            _dashBoardController = dashBoardController;
            _appointmentService = appointmentService;
            _patientService = patientService;
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
                                await _appointmentService.FixAppointment(appointment);
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
                                await _appointmentService.FixAppointment(appointment);
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
                                                await _appointmentService.FixAppointment(appointment);
                                            }
                                        }
                                    }
                                }
                            }
                            if (clashed_appointments.Count > 0)
                            {
                                ViewBag.ClashedAppointments = clashed_appointments;
                                return View();
                            }
                        }
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
            ViewBag.data = await _dashBoardController.getProfileAsync(did);
            List<Appointment> appointments = await _appointmentService.getAllAppointments(did);
            foreach (var item in appointments)
            {
                item.endtime = item.starttime.Add(item.sessiontime.TimeOfDay);
            }
            //Appointment appointment = null;
            //var tupleTwoModels = new Tuple<List<Appointment>, Appointment>(appointments, appointment);
            ViewBag.appointments = appointments;
            return View();
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
                if (ModelState.IsValid)
                {
                    bool status = await _appointmentService.rescheduleAppointment(appointment);
                    ViewBag.data = await _dashBoardController.getProfileAsync(did);
                    List<Appointment> appointments = await _appointmentService.getAllAppointments(did);
                    foreach (var item in appointments)
                    {
                        item.endtime = item.starttime.Add(item.sessiontime.TimeOfDay);
                    }
                    ViewBag.appointments = appointments;
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
                bool status = await _appointmentService.deleteAppointment(appointmentID);
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
    }
}
