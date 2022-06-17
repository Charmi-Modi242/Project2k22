using physioCard.Domain;
using System.Net.Mail;

namespace physioCard.Services
{
    public interface IInvoiceService
    {
        public Task<List<Appointment>> getAppointmentbyPatientID(int patientID);

        public Task<bool> createInvoice(Invoice invoice);

        public Task<bool> updateAppointmentInvoiceStatus(List<Appointment> appointments);

        public Task<List<Appointment>> getAppointmentsinInvoice(Invoice invoice);

        public Task<Invoice> getInvoiceByID(String ino);

        public AlternateView CreateAlternateViewofLogo(string body, Clinic clinic);

        public AlternateView CreateAlternateViewofSign(string body, Clinic clinic);

        public Task<List<Invoice>> getAllInvoiceMonthly(int did);

        public Task<bool> payStatusEdit(bool status, int invoiceID);

        public Task<List<Invoice>> seeAllPendingInvoice(int did);

        public Task<List<revenueModel>> getYearlyRevenue(int did);

        public Task<int> getRevenueCount(int did);
    }
}
