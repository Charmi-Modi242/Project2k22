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
        public Task<bool> FixAppointment(Appointment appointment)
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
    }
}
