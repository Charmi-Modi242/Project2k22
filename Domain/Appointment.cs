using System.ComponentModel.DataAnnotations;

namespace physioCard.Domain
{
    public class Appointment : BaseEntity
    { 
        public int appointmentID { get; set; }
        [Required(ErrorMessage = "SELECT PATIENT")]
        public int patientID { get; set; }
        [Required]
        public int doctorID { get; set; }
        [Required(ErrorMessage = "SELECT DATE")]
        public DateTime date_start { get; set; }
        public bool[]? days { get; set; }
        public DateTime date_end { get; set; }
        [Required(ErrorMessage = "SELECT SESSION START TIME")]
        public DateTime starttime { get; set; }
        public DateTime endtime { get; set; }
        [Required(ErrorMessage = "SELECT SESSION TIME")]
        public DateTime sessiontime { get; set; }
        [Required(ErrorMessage = "ENTER COST PER SESSION")]
        [RegularExpression("\\d+", ErrorMessage = "ADD VALID COST AS IN INTEGER")]
        public int cost { get; set; }
        public String? fname { get; set; }
        public String? lname { get; set; }
        public String? name { get; set; }
        public bool? attendance_status { get; set; }
    }
}
