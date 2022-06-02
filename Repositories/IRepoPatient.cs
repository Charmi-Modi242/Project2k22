using physioCard.Domain;

namespace physioCard.Repositories
{
    public interface IRepoPatient<T> where T : BaseEntity
    {
        Task<string> getdocnameAsync(int did);
        Task<int> addpatientAsync(Patient patient);
        Task<List<T>> getallpatientAsync(int id);
        Task<T> getpatientAsync(int id);
        Task<int> editpatientAsync(Patient patient);
        Task<List<T>> getpatientbynameAsync(int did, String fname, String lanme);
    }
}
