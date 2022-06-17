namespace physioCard.Domain
{
    public class Invoice : BaseEntity
    {
        public int invoiceID { get; set; }

        public String? invoiceNo { get; set; }

        public int patientID { get; set; }

        public int doctorID { get; set; }

        public DateTime invoice_date { get; set; }

        public DateTime istart_date { get; set; }

        public DateTime iend_date { get; set; }

        public int total_appointment { get; set; }

        public int total_amount { get; set; }

        public double discount { get; set; }

        public double tax { get; set; }

        public double gross_amount { get; set; }

        public bool pay_status { get; set; }

        public string? fname { get; set; }

        public string? lname { get; set; }

        public string? name { get; set; }

    }
}
