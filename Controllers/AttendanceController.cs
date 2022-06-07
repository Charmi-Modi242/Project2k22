using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using physioCard.Domain;
using physioCard.Services;

namespace physioCard.Controllers
{
    [Authorize]
    public class AttendanceController : Controller
    {
        private readonly IAttendanceService _attendanceService;
        private readonly DashBoardController _dashBoardController;

        public AttendanceController(IAttendanceService attendanceService, DashBoardController dashBoardController)
        {
            _attendanceService = attendanceService;
            _dashBoardController = dashBoardController;
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
            return status;
        }
    }
}
