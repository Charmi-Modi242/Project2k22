using Dapper;
using physioCard.Domain;
using System.Data;

namespace physioCard.Repositories
{
    public class AppointmentRepository : BaseRepository, IAppointmentRepository
    {
        public AppointmentRepository(IConfiguration configuration) : base(configuration)
        {
        }

        public async Task<List<Patient>> getPatientNameAsync(int did)
        {
            try
            {
                List<Patient> p;
                string sql = "select * from patientTB where docID=@id order by registerdate desc";
                var param = new DynamicParameters();
                param.Add("id", did, DbType.Int32);
                using (var connection = CreateConnection())
                {
                    p = (await connection.QueryAsync<Patient>(sql, param)).ToList();
                }
                foreach (var item in p)
                {
                    item.fname = item.fname + " " + item.lname;
                }
                return p;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task<bool> FixAppointmentAsync(Appointment appointment)
        {
            try
            {
                int c;
                string sql = "insert into appointmentTB(patientID, doctorID, date_start, starttime, sessiontime, cost) values(@patientID, @doctorID, @date, @time, @session_time, @cost)";
                var param = new DynamicParameters();
                param.Add("patientID", appointment.patientID, DbType.Int32);
                param.Add("doctorID", appointment.doctorID, DbType.Int32);
                param.Add("date", appointment.date_start, DbType.Date);
                param.Add("time", appointment.starttime, DbType.Time);
                param.Add("session_time", appointment.sessiontime, DbType.Time);
                param.Add("cost", appointment.cost, DbType.Currency);

                using (var connection = CreateConnection())
                {
                    c = await connection.ExecuteAsync(sql, param);
                }

                if (c > 0)
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
                throw new Exception(ex.Message);
            }
        }
        public async Task<List<Appointment>> getAllAppointmentsAsync(int did)
        {
            try
            {
                string sql = "select a.appointmentID, a.patientID, a.doctorID, a.date_start, a.starttime, a.sessiontime, p.fname, p.lname, c.name from appointmentTB a, patientTB p, clinicTB c where a.patientID=p.patientID and p.clinicID=c.clinicID and a.doctorID=@did order by a.date_start";
                var param = new DynamicParameters();
                param.Add("did", did, DbType.Int32);
                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<Appointment>(sql, param)).ToList();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task<Appointment> getAppointmentByIDAsync(int appointmentID)
        {
            try
            {
                string sql = "select appointmentID, patientID, doctorID, date_start, starttime, sessiontime, cost from appointmentTB where appointmentID = @aid;";
                var param = new DynamicParameters();
                param.Add("aid", appointmentID, DbType.Int32);
                using (var connection = CreateConnection())
                {
                    return connection.QueryFirstOrDefault<Appointment>(sql, param);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task<bool> rescheduleAppointmentAsync(Appointment appointment)
        {
            try
            {
                int i;
                string sql = "update appointmentTB set date_start=@date_start, starttime=@starttime, sessiontime=@sessiontime, cost=@cost where appointmentID=@appointmentID";
                var param = new DynamicParameters();
                param.Add("appointmentID", appointment.appointmentID, DbType.Int32);
                param.Add("date_start", appointment.date_start, DbType.Date);
                param.Add("starttime", appointment.starttime, DbType.Time);
                param.Add("sessiontime", appointment.sessiontime, DbType.Time);
                param.Add("cost", appointment.cost, DbType.Currency);
                using (var connnection = CreateConnection())
                {
                    i = await connnection.ExecuteAsync(sql, param);
                }
                if (i > 0)
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
                //throw new Exception(ex.Message);
            }
        }
        public async Task<bool> deleteAppointmentAsync(int appointmentID)
        {
            try
            {
                int i;
                string sql = "delete from appointmentTB where appointmentID=@appointmentID";
                var param = new DynamicParameters();
                param.Add("appointmentID", appointmentID, DbType.Int32);
                using (var connnection = CreateConnection())
                {
                    i = await connnection.ExecuteAsync(sql, param);
                }
                if (i > 0)
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
                //throw new Exception(ex.Message);
            }
        }
    }
}