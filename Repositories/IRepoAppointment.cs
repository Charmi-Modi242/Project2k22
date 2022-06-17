using physioCard.Domain;

namespace physioCard.Repositories
{
    public interface IRepoAppointment<T> where T : BaseEntity
    {
        Task<List<Patient>> getPatientNameAsync(int did);
        //Task<List<revenueModel>> getYearlyRevenue(int did);
        Task<int> getAppointmentCount(int did);
        //Task<int> getRevenueCount(int did);
        Task<string> FixAppointmentAsync(Appointment appointment);
        Task<List<Appointment>> getAllAppointmentsAsync(int did);
        Task<Appointment> getAppointmentByIDAsync(int appointmentID);
        Task<bool> rescheduleAppointmentAsync(Appointment appointment);
        Task<bool> deleteAppointmentAsync(int appointmentID);
        Task<bool> checkAppointmentClashesAsync(Appointment appointment);
        Task<List<Appointment>> getClashedAppointmentsAsync(Appointment appointment);
        Task<List<T>> getMonthlyAppointment(int did, DateTime mdate);
        Task<List<Appointment>> getAppointmentsAsync(int did);
    }
}
