using physioCard.Domain;

namespace physioCard.Repositories
{
    public interface IRepoDoctor<T> where T : BaseEntity
    {
        Task<int> SignUpAsync(T entity);
        Task<T> GetByIDAsync(int id);
        Task<int> UpdateDoctorAsync(T entity);
        Task<bool> changepasswordAsync(int did, string newpassword);
        Task<bool> checkemailAsync(string email);
    }
}
