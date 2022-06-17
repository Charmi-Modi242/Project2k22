using physioCard.Domain;
using Dapper;
using System.Data;

namespace physioCard.Repositories
{
    public class ClinicRepository : BaseRepository, IClinicRepository
    {
        public ClinicRepository(IConfiguration configuration) : base(configuration)
        {
        }

        public async Task<int> CreateClinicAsync(Clinic entity)
        {
            try
            {
                var query = "INSERT INTO clinicTB VALUES(@name, @address, @contactno, @GSTno, @headSign, @logo);";
                var param = new DynamicParameters();
                param.Add("name", entity.name, DbType.String);
                param.Add("address", entity.address, DbType.String);
                param.Add("contactno", entity.contactno, DbType.Int64);
                param.Add("GSTno", entity.GSTno, DbType.String);
                param.Add("headSign", entity.head_signature, DbType.String);
                param.Add("logo", entity.logo, DbType.String);
                using (var connection = CreateConnection()) {
                    return (await connection.ExecuteAsync(query, param));
                }
            }
            catch (Exception ex) {
                throw new Exception(ex.Message, ex);
            }
        }

        public async Task<List<Clinic>> GetAllAsync(int docID)
        {
            try
            {
                var query = "select * from clinicTB where clinicID NOT IN(select clinicID from doctor_clinicTB where docID=@did);";
                var param = new DynamicParameters();
                param.Add("did", docID, DbType.Int32);
                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<Clinic>(query, param)).ToList();
                }
            }
            catch (Exception ex) {
                throw new Exception(ex.Message, ex);
            }
        }

        public async Task<Clinic> GetByClinicID(int id)
        {
            try
            {
                var query = "SELECT * FROM clinicTB WHERE clinicID=@cID";
                var param = new DynamicParameters();
                param.Add("cID", id);
                using(var connection = CreateConnection()) 
                {
                    return (await connection.QueryFirstOrDefaultAsync<Clinic>(query,param));
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public async Task<List<Clinic>> GetByDoctorIDAsync(int id)
        {
            try
            {
                var query = "SELECT c.clinicID, name FROM clinicTB c, doctor_clinicTB dc WHERE dc.docID = @did AND c.clinicID = dc.clinicID order by name;";
                var param = new DynamicParameters();
                param.Add("did", id, DbType.Int32);
                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<Clinic>(query, param)).ToList();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
    }
}
