using physioCard.Domain;
using physioCard.Repositories;

namespace physioCard.Services
{
    public class DoctorService : IDoctorService
    {
        private readonly IDoctorRepository _doctorRepository;

        public DoctorService(IDoctorRepository doctorRepository)
        {
            _doctorRepository = doctorRepository;
        }

        public async Task<Doctor> GetByIDAsync(int dID)
        {
            return (await _doctorRepository.GetByIDAsync(dID));
        }

        public async Task<bool> changepassword(int did, string newpassword)
        {
            return await _doctorRepository.changepasswordAsync(did, newpassword);
        }

        public async Task<bool> checkemail(string email)
        {
            return await _doctorRepository.checkemailAsync(email);
        }

        public async Task<int> SignUpAsync(Doctor doctor)
        {
            return (await _doctorRepository.SignUpAsync(doctor));
        }

        public async Task<int> UpdateDoctorAsync(Doctor doctor)
        {
            return (await _doctorRepository.UpdateDoctorAsync(doctor));
        }
    }
}
