using physioCard.Domain;

namespace physioCard.Repositories
{
    public interface IRepoDoctorClinic<T> where T : BaseEntity
    {
        Task<bool> IsClinicAsync(int id);
        Task<int> CreateDoctorClinicAsync(T entity);
        Task<int> RemoveAsync(int id);
    }
}
