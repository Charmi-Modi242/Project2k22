using physioCard.Domain;

namespace physioCard.Repositories
{
    public interface IRepoInvoice<T> where T : BaseEntity
    {
        Task<List<Appointment>> getAppointmentbyPatientIDAsync(int patientID);

        Task<bool> createInvoiceAsync(T invoice);

        Task<bool> updateAppointmentInvoiceStatusAsync(List<Appointment> appointments);

        Task<List<Appointment>> getAppointmentsinInvoiceAsync(Invoice invoice);

        Task<Invoice> getInvoiceByIDAsync(String ino);

        Task<List<Invoice>> getAllInvoiceMonthlyAsync(int did);

        Task<bool> payStatusEditAsync(bool status, int invoiceID);

        Task<List<Invoice>> seeAllPendingInvoiceAsync(int did);

        Task<List<revenueModel>> getYearlyRevenue(int did);

        Task<int> getRevenueCount(int did);
    }
}
