using physioCard.Domain;
using physioCard.Repositories;

namespace physioCard.Services
{
    public class LoginService : ILoginService
    {
        private readonly ILoginRepository _loginRepository;

        public LoginService(ILoginRepository loginRepository)
        {
            _loginRepository = loginRepository;
        }

        public async Task<bool> checkemail(String email)
        {
            return await _loginRepository.checkemail(email);
        }

        public async Task<int> LoginAsync(Login l)
        {
            return await _loginRepository.LoginAsync(l);
        }

        public async Task<bool> resetpassword(OTPModel f)
        {
            return await _loginRepository.resetpasswordAsync(f);
        }
    }
}
