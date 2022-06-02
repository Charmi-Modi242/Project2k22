using physioCard.Domain;
using physioCard.Repositories;

namespace physioCard.Services
{
    public class DoctorClinicService : IDoctorClinicService
    {
        private readonly IDoctorClinicRepository _doctorClinicRepository;
        public DoctorClinicService(IDoctorClinicRepository doctorClinicRepository)
        {
            _doctorClinicRepository = doctorClinicRepository;
        }

        public async Task<int> CreateDoctorClinicAsync(Doctor_Clinic doctor_clinic)
        {
            return (await _doctorClinicRepository.CreateDoctorClinicAsync(doctor_clinic));
        }

        public async Task<bool> IsClinicAsync(int dID)
        {
            return (await _doctorClinicRepository.IsClinicAsync(dID));
        }

        public async Task<int> RemoveAsync(int cID)
        {
            return (await _doctorClinicRepository.RemoveAsync(cID));
        }
    }
}
