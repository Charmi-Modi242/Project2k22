using physioCard.Domain;
using System.Data.SqlClient;
using System.Data;
using Dapper;

namespace physioCard.Repositories
{
    public class DoctorRepository : BaseRepository, IDoctorRepository
    {
        public DoctorRepository(IConfiguration configuration) : base(configuration)
        {
        }

        public async Task<Doctor> GetByIDAsync(int id)
        {
            try
            {
                var query = "SELECT * FROM doctorTB WHERE docID=@id";
                var param = new DynamicParameters();
                param.Add("id", id, DbType.Int32);
                using (var connection = CreateConnection())
                {
                    return (await connection.QueryFirstOrDefaultAsync<Doctor>(query, param));
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public async Task<bool> changepasswordAsync(int did, string newpassword)
        {
            try
            {
                string sql = "update doctorTB set password=@newpassword where docID=@did";
                var param = new DynamicParameters();
                param.Add("did", did, DbType.Int32);
                param.Add("newpassword", newpassword, DbType.String);
                using (var connection = CreateConnection())
                {
                    await connection.ExecuteAsync(sql, param);
                }
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<bool> checkemailAsync(string email)
        {
            try
            {
                string temail;
                string sql = "select email from doctorTB where email=@email";
                var param = new DynamicParameters();
                param.Add("email", email, DbType.String);
                using (var connection = CreateConnection())
                {
                    temail = await connection.QueryFirstOrDefaultAsync<string>(sql, param);
                }
                if (temail == email)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<int> SignUpAsync(Doctor entity)
        {
            try
            {
                var query = "INSERT INTO doctorTB(fname, lname, email, password, contactno, photo) VALUES(@fname, @lname, @email, @password, @contactno, @photo);";
                var param = new DynamicParameters();
                param.Add("fname", entity.fname, DbType.String);
                param.Add("lname", entity.lname, DbType.String);
                param.Add("email", entity.email, DbType.String);
                param.Add("password", entity.password, DbType.String);
                param.Add("contactno", entity.contactno, DbType.Int64);
                param.Add("photo", entity.photo, DbType.String);
                using (var connection = CreateConnection())
                {
                    return (await connection.ExecuteAsync(query, param));
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public async Task<int> UpdateDoctorAsync(Doctor entity)
        {
            try
            {
                var query = "UPDATE doctorTB SET fname=@fname, lname=@lname, email=@email, contactno=@contactno, photo=@photo WHERE docID=@did";
                var param = new DynamicParameters();
                param.Add("fname", entity.fname, DbType.String);
                param.Add("lname", entity.lname, DbType.String);
                param.Add("email", entity.email, DbType.String);
                param.Add("contactno", entity.contactno, DbType.Int64);
                param.Add("photo", entity.photo, DbType.String);
                param.Add("did", entity.docID, DbType.Int32);
                using (var connection = CreateConnection())
                {
                    return await connection.ExecuteAsync(query, param);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
    }
}
