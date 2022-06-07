using Dapper;
using physioCard.Domain;
using System.Data;

namespace physioCard.Repositories
{
    public class AttendanceRepository : BaseRepository, IAttendanceRepository
    {
        public AttendanceRepository(IConfiguration configuration) : base(configuration)
        {
        }

        public async Task<List<Appointment>> getTodaysAppointmentAsync(int did)
        {
            try
            {
                DateTime date = DateTime.Now;
                string datestr = date.ToString("dd-MM-yyyy");
                string sql = "select a.appointmentID, a.patientID, a.doctorID, a.date_start, a.starttime, a.sessiontime, a.attendance_status, p.fname, p.lname, c.name from appointmentTB a, patientTB p, clinicTB c where a.patientID=p.patientID and p.clinicID=c.clinicID and a.doctorID=@did and date_start=@today_date order by starttime";
                var param = new DynamicParameters();
                param.Add("did", did, DbType.Int32);
                param.Add("today_date", datestr, DbType.Date);
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

        public async Task<bool> markAttendanceAsync(bool attendance, int appointmentID)
        {
            try
            {
                int i;
                DateTime date = DateTime.Now;
                string datestr;
                if (attendance)
                {
                    datestr = date.ToString();
                }
                else
                {
                    datestr = null;
                }
                string sql = "update appointmentTB set attendance_status=@attendance, attendance_mark_time=@date where appointmentID=@appointmentID";
                var param = new DynamicParameters();
                param.Add("attendance", attendance, DbType.Boolean);
                param.Add("date", datestr, DbType.DateTime);
                param.Add("appointmentID", appointmentID, DbType.Int32);
                using (var connection = CreateConnection())
                {
                    i = await connection.ExecuteAsync(sql, param);
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
                throw new Exception(ex.Message);
            }
        }
    }
}
