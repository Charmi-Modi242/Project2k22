using physioCard.Domain;

namespace physioCard.Services
{
    public interface IDoctorClinicService
    {
        public Task<bool> IsClinicAsync(int dID);
        public Task<int> CreateDoctorClinicAsync(Doctor_Clinic doctor_clinic);
        public Task<int> RemoveAsync(int cID);
    }
}
