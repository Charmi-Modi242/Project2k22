using Dapper;
using physioCard.Domain;
using System.Data;

namespace physioCard.Repositories
{
    public class PatientRepository : BaseRepository, IPatientRepository
    {
        public PatientRepository(IConfiguration configuration) : base(configuration)
        {
        }
        public async Task<string> getdocnameAsync(int did)
        {
            try
            {
                Doctor doctor;
                string name;
                string sql = "select fname, lname from doctorTB where docID=@id";
                var param = new DynamicParameters();
                param.Add("id", did, DbType.Int32);
                using (var connection = CreateConnection())
                {
                    doctor = await connection.QueryFirstOrDefaultAsync<Doctor>(sql, param);
                }
                name = doctor.fname + " " + doctor.lname;
                return name;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }

        public async Task<int> getPatientCount(int did)
        {
            try
            {
                string sql = "select count(*) from patientTB where datepart(MM, registerdate) = datepart(MM, getDate()) AND docID = @id;";
                var param = new DynamicParameters();
                param.Add("id", did, DbType.Int32);
                using (var connection = CreateConnection())
                {
                    return (await connection.QueryFirstOrDefaultAsync<int>(sql, param));
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<int> addpatientAsync(Patient patient)
        {
            try
            {
                string sql = "insert into patientTB(fname, lname, gender, dob, address, email, contactno, chief_complains, medical_history, registerdate, photo, docID, clinicID) values(@fname, @lname, @gender, @dob, @address, @email, @contactno, @chief_complains, @medical_history, @registerdate, @photo, @docID, @clinicID);";
                var param = new DynamicParameters();
                param.Add("fname", patient.fname, DbType.String);
                param.Add("lname", patient.lname, DbType.String);
                param.Add("gender", patient.gender, DbType.String);
                param.Add("dob", patient.dob, DbType.Date);
                param.Add("address", patient.address, DbType.String);
                param.Add("email", patient.email, DbType.String);
                param.Add("contactno", patient.contactno, DbType.Int64);
                param.Add("chief_complains", patient.chief_complains, DbType.String);
                param.Add("medical_history", patient.medical_History, DbType.String);
                param.Add("registerdate", patient.registerdate, DbType.Date);
                param.Add("photo", patient.photo, DbType.String);
                param.Add("docID", patient.docid, DbType.Int32);
                param.Add("clinicID", patient.clinicid, DbType.Int32);

                using (var connection = CreateConnection())
                {
                    return (await connection.ExecuteAsync(sql, param));
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<Patient>> getallpatientAsync(int id)
        {
            try
            {
                string sql = "select * from patientTB where docID=@id order by registerdate desc";
                var param = new DynamicParameters();
                param.Add("id", id, DbType.Int32);
                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<Patient>(sql, param)).ToList();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<Patient> getpatientAsync(int id)
        {
            try
            {
                Patient patient;
                string sql = "select * from patientTB where patientID=@id";
                var param = new DynamicParameters();
                param.Add("id", id, DbType.Int32);
                using (var connection = CreateConnection())
                {
                    patient = await connection.QueryFirstOrDefaultAsync<Patient>(sql, param);
                    return patient;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<int> editpatientAsync(Patient patient)
        {
            try
            {
                string sql = "update patientTB set fname=@fname, lname=@lname, gender=@gender, dob=@dob, address=@address, email=@email, contactno=@contactno, chief_complains=@chief_complains, medical_history=@medical_history, registerdate= @registerdate, photo=@photo, clinicID=@clinicID where patientID=@patientID;";
                var param = new DynamicParameters();
                param.Add("patientID", patient.patientID, DbType.Int32);
                param.Add("fname", patient.fname, DbType.String);
                param.Add("lname", patient.lname, DbType.String);
                param.Add("gender", patient.gender, DbType.String);
                param.Add("dob", patient.dob, DbType.Date);
                param.Add("address", patient.address, DbType.String);
                param.Add("email", patient.email, DbType.String);
                param.Add("contactno", patient.contactno, DbType.Int64);
                param.Add("chief_complains", patient.chief_complains, DbType.String);
                param.Add("medical_history", patient.medical_History, DbType.String);
                param.Add("registerdate", patient.registerdate, DbType.Date);
                param.Add("photo", patient.photo, DbType.String);
                param.Add("clinicID", patient.clinicid, DbType.Int32);

                using (var connection = CreateConnection())
                {
                    return (await connection.ExecuteAsync(sql, param));
                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<Patient>> getpatientbynameAsync(int did, string fname, string lname)
        {
            try
            {
                string sql;
                if (lname == "None")
                {
                    fname = fname + "%";
                    sql = "select * from patientTB where (UPPER(fname) LIKE UPPER(@fname) or UPPER(lname) LIKE UPPER(@fname)) and docID=@did";
                }
                else
                {
                    sql = "select * from patientTB where (UPPER(fname)=UPPER(@fname) and UPPER(lname)=UPPER(@lname)) or (UPPER(lname)=UPPER(@fname) and UPPER(fname)=UPPER(@lname)) and docID=@did";
                }

                var param = new DynamicParameters();
                param.Add("fname", fname, DbType.String);
                param.Add("lname", lname, DbType.String);
                param.Add("did", did, DbType.Int32);
                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<Patient>(sql, param)).ToList();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
