using physioCard.Domain;

namespace physioCard.Services
{
    public interface IAppointmentService
    {
        public Task<List<Patient>> getPatientName(int did);
        public Task<bool> FixAppointment(Appointment appointment);
        public Task<List<Appointment>> getAllAppointments(int did);
        public Task<Appointment> getAppointmentByID(int appointmentID);
        public Task<bool> rescheduleAppointment(Appointment appointment);
        public Task<bool> deleteAppointment(int appointmentID);
        public Task<bool> checkAppointmentClashes(Appointment appointment);
        public Task<List<Appointment>> getClashedAppointments(Appointment appointment);
        //public Task<List<Appointment>> getAppointmentsAsync();
        public Task<List<revenueModel>> getYearlyRevenue(int did);
        public Task<List<Appointment>> getMonthlyAppointment(int did, DateTime mdate);
        public Task<int> getAppointmentCount(int did);
        public Task<int> getRevenueCount(int did);
    }
}
