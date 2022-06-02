namespace physioCard.Domain
{
    public class Doctor_Clinic : BaseEntity
    {
        public int docID { get; set; }
        public int clinicID { get; set; }
        public int[] clinics { get; set; }
    }
}
