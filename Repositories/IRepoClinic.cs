using physioCard.Domain;

namespace physioCard.Repositories
{
    public interface IRepoClinic<T> where T : BaseEntity
    {
        Task<List<T>> GetAllAsync(int docID);
        Task<List<T>> GetByDoctorIDAsync(int id);
        Task<T> GetByClinicID(int id);
        Task<int> CreateClinicAsync(T entity);
    }
}
