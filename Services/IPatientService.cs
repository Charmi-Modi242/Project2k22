using physioCard.Domain;

namespace physioCard.Services
{
    public interface IPatientService
    {
        public Task<string> getdocname(int did);
        public Task<int> getPatientCount(int did);
        public Task<int> addpatient(Patient patient);
        public Task<List<Patient>> getallpatient(int id);
        public Task<Patient> getpatient(int id);
        public Task<int> editpatient(Patient patient);
        public Task<List<Patient>> getpatientbyname(int did, String fname, String lname);
    }
}