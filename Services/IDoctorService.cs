using physioCard.Domain;

namespace physioCard.Services
{
    public interface IDoctorService
    {
        public Task<Doctor> GetByIDAsync(int dID);
        public Task<int> SignUpAsync(Doctor doctor);
        public Task<int> UpdateDoctorAsync(Doctor doctor);
        public Task<bool> changepassword(int did, string newpassword);
        public Task<bool> checkemail(String email);
    }
}
