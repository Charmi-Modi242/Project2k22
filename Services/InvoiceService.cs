using physioCard.Domain;
using physioCard.Repositories;
using System.Net.Mail;

namespace physioCard.Services
{
    public class InvoiceService : IInvoiceService
    {
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public InvoiceService(IInvoiceRepository invoiceRepository, IWebHostEnvironment webHostEnvironment)
        {
            _invoiceRepository = invoiceRepository;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<List<Appointment>> getAppointmentbyPatientID(int patientID)
        {
            return await _invoiceRepository.getAppointmentbyPatientIDAsync(patientID);
        }

        public async Task<bool> createInvoice(Invoice invoice)
        {
            return await _invoiceRepository.createInvoiceAsync(invoice);
        }

        public async Task<bool> updateAppointmentInvoiceStatus(List<Appointment> appointments)
        {
            return await _invoiceRepository.updateAppointmentInvoiceStatusAsync(appointments);
        }

        public async Task<List<Appointment>> getAppointmentsinInvoice(Invoice invoice)
        {
            return await _invoiceRepository.getAppointmentsinInvoiceAsync(invoice);
        }

        public async Task<Invoice> getInvoiceByID(String ino)
        {
            return await _invoiceRepository.getInvoiceByIDAsync(ino);
        }

        public AlternateView CreateAlternateViewofLogo(string body, Clinic clinic)
        {
            string path = _webHostEnvironment.WebRootPath;
            AlternateView alternateView = AlternateView.CreateAlternateViewFromString(body, null, "text/html");
            LinkedResource linkedResource = new LinkedResource(Path.Combine(path , clinic.logo), "image/jpg");
            linkedResource.ContentId = "sample";
            alternateView.LinkedResources.Add(linkedResource);
            return alternateView;

        }

        public AlternateView CreateAlternateViewofSign(string body, Clinic clinic)
        {
            string path = _webHostEnvironment.WebRootPath;
            AlternateView alternateView = AlternateView.CreateAlternateViewFromString(body, null, "text/html");
            LinkedResource linkedResource = new LinkedResource(Path.Combine(path, clinic.head_signature), "image/jpg");
            linkedResource.ContentId = "sample1";
            alternateView.LinkedResources.Add(linkedResource);
            return alternateView;

        }

        public async Task<List<Invoice>> getAllInvoiceMonthly(int did)
        {
            return await _invoiceRepository.getAllInvoiceMonthlyAsync(did);
        }

        public async Task<bool> payStatusEdit(bool status, int invoiceID)
        {
            return await _invoiceRepository.payStatusEditAsync(status, invoiceID);
        }

        public async Task<List<Invoice>> seeAllPendingInvoice(int did)
        {
            return await _invoiceRepository.seeAllPendingInvoiceAsync(did);
        }

        public async Task<List<revenueModel>> getYearlyRevenue(int did)
        {
            return await _invoiceRepository.getYearlyRevenue(did);
        }

        public async Task<int> getRevenueCount(int did)
        {
            return await _invoiceRepository.getRevenueCount(did);
        }
    }
}
