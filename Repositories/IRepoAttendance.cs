using physioCard.Domain;

namespace physioCard.Repositories
{
    public interface IRepoAttendance<T> where T : BaseEntity
    {
        Task<List<Appointment>> getTodaysAppointmentAsync(int did);
        Task<bool> markAttendanceAsync(bool attendance, int appointmentID);
    }
}
