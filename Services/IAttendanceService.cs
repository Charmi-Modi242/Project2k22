using physioCard.Domain;

namespace physioCard.Services
{
    public interface IAttendanceService
    {
        public Task<List<Appointment>> getTodaysAppointment(int did);
        public Task<bool> markAttendance(bool attendance, int appointmentID);
    }
}
