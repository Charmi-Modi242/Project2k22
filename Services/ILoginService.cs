using physioCard.Domain;

namespace physioCard.Services
{
    public interface ILoginService
    {
        public Task<int> LoginAsync(Login l);
        public Task<bool> checkemail(string email);
        public Task<bool> resetpassword(OTPModel f);
    }
}
