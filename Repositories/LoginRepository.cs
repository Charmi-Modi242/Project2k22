using Dapper;
using physioCard.Domain;
using System.Data;

namespace physioCard.Repositories
{
    public class LoginRepository : BaseRepository, ILoginRepository
    {
        public LoginRepository(IConfiguration configuration) : base(configuration)
        { }
        public async Task<int> LoginAsync(Login entity)
        {
            try
            {
                int did;
                var sql = "select docID from doctorTB where email='" + entity.email + "' and password='" + entity.password + "'";
                using (var connection = CreateConnection())
                {
                    //did = await connection.QueryFirstOrDefaultAsync<int>(sql);
                    did = await connection.QueryFirstOrDefaultAsync<int>(sql);
                }
                if (did != 0)
                {
                    return did;
                }
                else { return 0; }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task<bool> checkemail(String email)
        {
            try
            {
                int did;
                var sql = "select docID from doctorTB where email=@email";
                var param = new DynamicParameters();
                param.Add("email", email, DbType.String);
                using (var connection = CreateConnection())
                {
                    did = await connection.QueryFirstOrDefaultAsync<int>(sql, param);
                }
                if (did > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
                throw new Exception(ex.Message);
            }
        }
        public async Task<bool> resetpasswordAsync(OTPModel f)
        {
            int a;
            try
            {
                var sql = "update doctorTB set password=@password where email=@email";
                var param = new DynamicParameters();
                param.Add("email", f.email, DbType.String);
                param.Add("password", f.password, DbType.String);
                using (var connection = CreateConnection())
                {
                    a = await connection.ExecuteAsync(sql, param);
                }
                if (a > 0)
                {
                    return true;
                }
                else { return false; }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


    }
}
