namespace physioCard.Domain
{
    public class DoctorNotes : BaseEntity
    {
        public int patientID { get; set; }
        public string? notes { get; set; }
    }
}
