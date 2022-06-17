using physioCard.Domain;
using physioCard.Repositories;

namespace physioCard.Services
{
    public class AppointmentService : IAppointmentService
    {
        private readonly IAppointmentRepository _appointmentRepository;
        public AppointmentService(IAppointmentRepository appointmentRepository)
        {
            _appointmentRepository = appointmentRepository;
        }
        public Task<List<Patient>> getPatientName(int did)
        {
            return _appointmentRepository.getPatientNameAsync(did);
        }
        public Task<string> FixAppointment(Appointment appointment)
        {
            return _appointmentRepository.FixAppointmentAsync(appointment);
        }
        public Task<List<Appointment>> getAllAppointments(int did)
        {
            return _appointmentRepository.getAllAppointmentsAsync(did);
        }
        public Task<Appointment> getAppointmentByID(int appointmentID)
        {
            return _appointmentRepository.getAppointmentByIDAsync(appointmentID);
        }
        public Task<bool> rescheduleAppointment(Appointment appointment)
        {
            return _appointmentRepository.rescheduleAppointmentAsync(appointment);
        }
        public Task<bool> deleteAppointment(int appointmentID)
        {
            return _appointmentRepository.deleteAppointmentAsync(appointmentID);
        }
        public Task<bool> checkAppointmentClashes(Appointment appointment)
        {
            return _appointmentRepository.checkAppointmentClashesAsync(appointment);
        }
        public Task<List<Appointment>> getClashedAppointments(Appointment appointment)
        {
            return _appointmentRepository.getClashedAppointmentsAsync(appointment);
        }
        public async Task<List<Appointment>> getMonthlyAppointment(int did, DateTime mdate)
        {
            return await _appointmentRepository.getMonthlyAppointment(did, mdate);
        }
        public Task<List<Appointment>> getAppointments(int did)
        {
            return _appointmentRepository.getAppointmentsAsync(did);
        }
        //public async Task<List<revenueModel>> getYearlyRevenue(int did)
        //{
        //    return await _appointmentRepository.getYearlyRevenue(did);
        //}
        public async Task<int> getAppointmentCount(int did)
        {
            return await _appointmentRepository.getAppointmentCount(did);
        }
        //public async Task<int> getRevenueCount(int did)
        //{
        //    return await _appointmentRepository.getRevenueCount(did);
        //}
    }
}
