using physioCard.Domain;
using physioCard.Repositories;

namespace physioCard.Services
{
    public class ClinicService : IClinicService
    {
        private readonly IClinicRepository _clinicRepository;
        public ClinicService(IClinicRepository clinicRepository)
        {
            _clinicRepository = clinicRepository;
        }

        public async Task<int> CreateClinicAsync(Clinic clinic)
        {
            return (await _clinicRepository.CreateClinicAsync(clinic));
        }

        public async Task<List<Clinic>> GetAllAsync(int docID)
        {
            return (await _clinicRepository.GetAllAsync(docID));
        }

        public async Task<Clinic> GetByClinicID(int cID)
        {
            return (await _clinicRepository.GetByClinicID(cID));
        }

        public async Task<List<Clinic>> GetByDoctorIDAsync(int did)
        {
            return (await _clinicRepository.GetByDoctorIDAsync(did));
        }
    }
}
