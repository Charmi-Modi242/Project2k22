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
            return View();
        }



        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> FixAppointment(Appointment appointment)
        {
            try
            {
                int did = (int)HttpContext.Session.GetInt32("docID");
                ViewBag.PatientName = new SelectList(await _appointmentService.getPatientName(did), "patientID", "fname");
                ViewBag.data = await _dashBoardController.getProfileAsync(did);
                appointment.doctorID = did;
                appointment.cost = (int)appointment.cost;
                if (ModelState.IsValid)
                {
                    if (DateTime.Equals(appointment.date_start, appointment.date_end))
                    {
                        await _appointmentService.FixAppointment(appointment);
                    }
                    else
                    {
                        await _appointmentService.FixAppointment(appointment);
                        int days = (appointment.date_end - appointment.date_start).Days;
                        //String[] tempdays = null;
                        //int i=0;
                        //foreach(var day in appointment.days)
                        //{
                        // if(day!="false")
                        // {
                        // tempdays[i] = day;
                        // i++;
                        // }
                        //}
                        for (int i = 0; i < days; i++)
                        {
                            appointment.date_start = appointment.date_start.AddDays(1);
                            foreach (var day in appointment.days)
                            {
                                if (day != "false")
                                {
                                    if (appointment.date_start.DayOfWeek.ToString() == day)
                                    {
                                        await _appointmentService.FixAppointment(appointment);
                                    }
                                }

                            }
                        }
                    }
                    return RedirectToAction(nameof(showAppointment));
                }




            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Unable to fix appointment. " +
                "Try Again, and if the problem persists" +
                "See your system administrator.");
            }
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
