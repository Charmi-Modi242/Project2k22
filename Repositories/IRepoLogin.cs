using physioCard.Domain;

namespace physioCard.Repositories
{
    public interface IRepoLogin<T> where T : BaseEntity
    {
        Task<int> LoginAsync(T l);
        Task<bool> checkemail(String email);
        Task<bool> resetpasswordAsync(OTPModel f);
    }
}
