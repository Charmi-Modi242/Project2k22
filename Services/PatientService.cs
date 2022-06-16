using physioCard.Domain;
using physioCard.Repositories;

namespace physioCard.Services
{
    public class PatientService : IPatientService
    {
        private readonly IPatientRepository _patientRepository;
        public PatientService(IPatientRepository patientRepository)
        {
            _patientRepository = patientRepository;
        }
        public async Task<string> getdocname(int did)
        {
            return await (_patientRepository.getdocnameAsync(did));
        }
        public async Task<int> getPatientCount(int did)
        {
            return await _patientRepository.getPatientCount(did);
        }
        public async Task<int> addpatient(Patient patient)
        {
            return await (_patientRepository.addpatientAsync(patient));
        }
        public async Task<List<Patient>> getallpatient(int id)
        {
            return await _patientRepository.getallpatientAsync(id);
        }
        public async Task<Patient> getpatient(int id)
        {
            return await _patientRepository.getpatientAsync(id);
        }
        public async Task<int> editpatient(Patient patient)
        {
            return await _patientRepository.editpatientAsync(patient);
        }
        public async Task<List<Patient>> getpatientbyname(int did, string fname, string lname)
        {
            return await _patientRepository.getpatientbynameAsync(did, fname, lname);
        }
    }
}
