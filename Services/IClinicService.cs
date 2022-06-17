using physioCard.Domain;

namespace physioCard.Services
{
    public interface IClinicService
    {
        public Task<List<Clinic>> GetAllAsync(int docID);
        public Task<List<Clinic>> GetByDoctorIDAsync(int did);
        public Task<Clinic> GetByClinicID(int cID);
        public Task<int> CreateClinicAsync(Clinic clinic);
    }
}
