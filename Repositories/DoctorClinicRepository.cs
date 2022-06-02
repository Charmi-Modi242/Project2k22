using Dapper;
using physioCard.Domain;
using System.Data;

namespace physioCard.Repositories
{
    public class DoctorClinicRepository : BaseRepository, IDoctorClinicRepository
    {
        public DoctorClinicRepository(IConfiguration configuration) : base(configuration)
        {
        }

        public async Task<int> CreateDoctorClinicAsync(Doctor_Clinic entity)
        {
            try
            {
                var query = "INSERT INTO doctor_clinicTB VALUES(@docID, @clinicID0)";
                var param = new DynamicParameters();
                param.Add("docID", entity.docID, DbType.Int32);
                param.Add("clinicID0", entity.clinics[0], DbType.Int32);
                if (entity.clinics.Length > 1)
                {
                    for (int i = 1; i < entity.clinics.Length; i++)
                    {
                        query += ", (@docID, @clinicID" + i + ")";
                        param.Add("clinicID" + i + "", entity.clinics[i], DbType.Int32);
                    }
                }
                query += ";";

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

        public async Task<bool> IsClinicAsync(int id)
        {
            try
            {
                var query = "select count(docID) from doctor_clinicTB where docID=@did";
                var param = new DynamicParameters();
                param.Add("did", id, DbType.Int32);
                using (var connection = CreateConnection())
                {
                    int did = (int)(await connection.ExecuteScalarAsync(query, param));
                    if (did == 0)
                        return false;
                    else
                        return true;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public async Task<int> RemoveAsync(int id)
        {
            try
            {
                var query = "DELETE FROM doctor_clinicTB WHERE clinicID=@cID";
                var param = new DynamicParameters();
                param.Add("cID", id, DbType.Int32);
                using (var connection = CreateConnection())
                {
                    return (await connection.ExecuteAsync(query, param));
                }
            } catch(Exception ex) {
                throw new Exception(ex.Message, ex);
            }
        }
    }
}
