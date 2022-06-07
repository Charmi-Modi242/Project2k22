using physioCard.Domain;
using physioCard.Repositories;

namespace physioCard.Services
{
    public class AttendanceService : IAttendanceService
    {
        private readonly IAttendanceRepository _attendanceRepository;
        public AttendanceService(IAttendanceRepository attendanceRepository)
        {
            _attendanceRepository = attendanceRepository;
        }

        public Task<List<Appointment>> getTodaysAppointment(int did)
        {
            return _attendanceRepository.getTodaysAppointmentAsync(did);
        }

        public Task<bool> markAttendance(bool attendance, int appointmentID)
        {
            return _attendanceRepository.markAttendanceAsync(attendance, appointmentID);
        }
    }
}