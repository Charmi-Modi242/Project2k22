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

        //public async Task<List<revenueModel>> getYearlyRevenue(int did)
        //{
        //    try
        //    {
        //        string sql = "select DATEPART(MM, date_start) as monthID, sum(cost) as revenue FROM appointmentTB where doctorID=@did AND date_start > DATEADD(m, -11, (DATEADD(MM, DATEDIFF(mm, 0, GETDATE()), 0))) AND  group by DATEPART(MM, date_start);";
        //        var param = new DynamicParameters();
        //        param.Add("did", did, DbType.Int32);
        //        using (var connection = CreateConnection())
        //        {
        //            return (await connection.QueryAsync<revenueModel>(sql, param)).ToList();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception(ex.Message);
        //    }
        //}

        public async Task<List<Appointment>> getMonthlyAppointment(int did, DateTime mdate)
        {
            try
            {
                string sql = "select a.appointmentID, a.patientID, a.doctorID, a.date_start, a.starttime, a.sessiontime, a.attendance_status, p.fname, p.lname, c.name from appointmentTB a, patientTB p, clinicTB c where doctorID=@did AND DATEPART(MM, date_start) = DATEPART(MM, @tdate) AND DATEPART(YYYY, date_start) = DATEPART(YYYY, @tdate) AND (a.attendance_status = @status OR a.attendance_status IS NULL) AND a.patientID=p.patientID AND p.clinicID=c.clinicID order by starttime;";
                var param = new DynamicParameters();
                param.Add("did", did, DbType.Int32);
                param.Add("tdate", mdate, DbType.Date);
                param.Add("status", false, DbType.Boolean);
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

        public async Task<int> getAppointmentCount(int did)
        {
            try
            {
                string sql = "select count(*) from appointmentTB where datepart(MM, date_start) = datepart(MM, getDate()) AND doctorID = @id;";
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

        //public async Task<int> getRevenueCount(int did)
        //{
        //    try
        //    {
        //        string sql = "select sum(cost) from appointmentTB where datepart(MM, date_start) = datepart(MM, getDate()) AND doctorID = @id;";
        //        var param = new DynamicParameters();
        //        param.Add("id", did, DbType.Int32);
        //        using (var connection = CreateConnection())
        //        {
        //            return (await connection.QueryFirstOrDefaultAsync<int>(sql, param));
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception(ex.Message);
        //    }
        //}

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
        public async Task<string> FixAppointmentAsync(Appointment appointment)
        {
            try
            {
                int c;
                string date = appointment.date_start.ToString("dd-MM-yyyy") + " at " + appointment.starttime.ToString("HH:mm") + " for " + appointment.sessiontime.ToString("HH") + " hour and " + appointment.sessiontime.ToString("mm") + " minutes ";
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
                    return date;
                }
                else
                {
                    return null;
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
                DateTime date = DateTime.Now;
                string datestr = date.ToString("dd-MM-yyyy");
                string sql = "select a.appointmentID, a.patientID, a.doctorID, a.date_start, a.starttime, a.sessiontime, a.attendance_status, p.fname, p.lname, c.name from appointmentTB a, patientTB p, clinicTB c where a.patientID=p.patientID and p.clinicID=c.clinicID and a.doctorID=@did and a.date_start>=@date and DATEPART(MM, date_start) = DATEPART(MM, GETDATE()) AND (a.attendance_status = @status OR a.attendance_status IS NULL) order by a.date_start, a.starttime";
                var param = new DynamicParameters();
                param.Add("did", did, DbType.Int32);
                param.Add("date", datestr, DbType.Date);
                param.Add("status", false, DbType.Boolean);
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

        public async Task<List<Appointment>> getAppointmentsAsync(int did)
        {
            try
            {
                DateTime date = DateTime.Now;
                string datestr = date.ToString("dd-MM-yyyy");
                string sql = "select a.appointmentID, a.patientID, a.doctorID, a.date_start, a.starttime, a.sessiontime, a.attendance_status, p.fname, p.lname, c.name from appointmentTB a, patientTB p, clinicTB c where a.patientID=p.patientID and p.clinicID=c.clinicID and a.doctorID=@did and a.date_start>=@date order by a.date_start, a.starttime";
                var param = new DynamicParameters();
                param.Add("did", did, DbType.Int32);
                param.Add("date", datestr, DbType.Date);
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

        public async Task<bool> checkAppointmentClashesAsync(Appointment appointment)
        {
            try
            {
                List<Appointment> appointments = new List<Appointment>();
                string sql = "select * from appointmentTB where date_start=@date and doctorID=@did;";
                var param = new DynamicParameters();
                param.Add("date", appointment.date_start, DbType.Date);
                param.Add("did", appointment.doctorID, DbType.Int32);
                using (var connection = CreateConnection())
                {
                    appointments = (await connection.QueryAsync<Appointment>(sql, param)).ToList();
                }
                if (appointments.Count == 0)
                {
                    return false;
                }
                else
                {
                    int count = 0;
                    appointment.endtime = appointment.starttime.Add(appointment.sessiontime.TimeOfDay);
                    //String astemp = appointment.starttime.ToString("HH-mm");
                    //String aetemp = appointment.endtime.ToString("HH-mm");
                    TimeSpan astime = appointment.starttime.TimeOfDay;
                    TimeSpan aetime = appointment.endtime.TimeOfDay;
                    int asminutes = (int)astime.TotalMinutes;
                    int aeminutes = (int)aetime.TotalMinutes;
                    foreach (var item in appointments)
                    {
                        item.endtime = item.starttime.Add(item.sessiontime.TimeOfDay);
                        //String istemp = item.starttime.ToString("HH-mm");
                        //String ietemp = item.endtime.ToString("HH-mm");
                        TimeSpan istime = item.starttime.TimeOfDay;
                        TimeSpan ietime = item.endtime.TimeOfDay;
                        int isminutes = (int)istime.TotalMinutes;
                        int ieminutes = (int)ietime.TotalMinutes;

                        ////if (astemp.CompareTo(istemp) == 0 || (astemp.CompareTo(istemp) > 0 && aetemp.CompareTo(ietemp) < 0) || (aetemp.CompareTo(istemp) > 0 && aetemp.CompareTo(ietemp) < 0) || (astemp.CompareTo(istemp) > 0 && astemp.CompareTo(ietemp) < 0))
                        ////{
                        //// count++;
                        ////}
                        //if((istemp.CompareTo(astemp) == 0) || (ietemp.CompareTo(aetemp) == 0) || ((istemp.CompareTo(astemp) > 0 && istemp.CompareTo(aetemp) < 0) && (ietemp.CompareTo(astemp) > 0 && ietemp.CompareTo(aetemp) < 0)) || ((istemp.CompareTo(astemp) < 0 && istemp.CompareTo(aetemp) < 0) && (ietemp.CompareTo(astemp) > 0 && ietemp.CompareTo(aetemp) > 0)) || ((istemp.CompareTo(astemp) < 0 && istemp.CompareTo(aetemp) < 0) && (ietemp.CompareTo(astemp) < 0 && ietemp.CompareTo(aetemp) < 0)) || ((istemp.CompareTo(astemp)>0 && istemp.CompareTo(aetemp) < 0) && (ietemp.CompareTo(astemp) > 0 && ietemp.CompareTo(aetemp) > 0)))
                        //{
                        // count++;
                        //}

                        for (int i = asminutes; i < aeminutes; i++)
                        {
                            for (int j = isminutes; j <= ieminutes; j++)
                            {
                                if (i == j)
                                {
                                    count++;
                                    break;
                                    ;
                                }
                            }
                            if (count > 0)
                            {
                                break;
                            }
                        }
                    }
                    if (count > 0)
                    {
                        return true;
                    }
                    return false;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<Appointment>> getClashedAppointmentsAsync(Appointment appointment)
        {
            try
            {
                List<Appointment> appointments = new List<Appointment>();
                List<Appointment> clashappointments = new List<Appointment>();
                string sql = "select a.appointmentID, a.patientID, a.doctorID, a.date_start, a.starttime, a.sessiontime, p.fname, p.lname, c.name from appointmentTB a, patientTB p, clinicTB c where a.patientID=p.patientID and p.clinicID=c.clinicID and a.doctorID=@did and a.date_start=@date";
                var param = new DynamicParameters();
                param.Add("date", appointment.date_start, DbType.Date);
                param.Add("did", appointment.doctorID, DbType.Int32);
                using (var connection = CreateConnection())
                {
                    appointments = (await connection.QueryAsync<Appointment>(sql, param)).ToList();
                }
                appointment.endtime = appointment.starttime.Add(appointment.sessiontime.TimeOfDay);
                //String astemp = appointment.starttime.ToString("HH-mm");
                //String aetemp = appointment.endtime.ToString("HH-mm");
                TimeSpan astime = appointment.starttime.TimeOfDay;
                TimeSpan aetime = appointment.endtime.TimeOfDay;
                int asminutes = (int)astime.TotalMinutes;
                int aeminutes = (int)aetime.TotalMinutes;
                foreach (var item in appointments)
                {
                    item.endtime = item.starttime.Add(item.sessiontime.TimeOfDay);
                    //String istemp = item.starttime.ToString("HH-mm");
                    //String ietemp = item.endtime.ToString("HH-mm");
                    TimeSpan istime = item.starttime.TimeOfDay;
                    TimeSpan ietime = item.endtime.TimeOfDay;
                    int isminutes = (int)istime.TotalMinutes;
                    int ieminutes = (int)ietime.TotalMinutes;
                    ////if (astemp.CompareTo(istemp) == 0 || (astemp.CompareTo(istemp) > 0 && aetemp.CompareTo(ietemp) < 0) || (aetemp.CompareTo(istemp) > 0 && aetemp.CompareTo(ietemp) < 0) || (astemp.CompareTo(istemp) > 0 && astemp.CompareTo(ietemp) < 0))
                    ////{
                    //// count++;
                    ////}
                    //if((istemp.CompareTo(astemp) == 0) || (ietemp.CompareTo(aetemp) == 0) || ((istemp.CompareTo(astemp) > 0 && istemp.CompareTo(aetemp) < 0) && (ietemp.CompareTo(astemp) > 0 && ietemp.CompareTo(aetemp) < 0)) || ((istemp.CompareTo(astemp) < 0 && istemp.CompareTo(aetemp) < 0) && (ietemp.CompareTo(astemp) > 0 && ietemp.CompareTo(aetemp) > 0)) || ((istemp.CompareTo(astemp) < 0 && istemp.CompareTo(aetemp) < 0) && (ietemp.CompareTo(astemp) < 0 && ietemp.CompareTo(aetemp) < 0)) || ((istemp.CompareTo(astemp)>0 && istemp.CompareTo(aetemp) < 0) && (ietemp.CompareTo(astemp) > 0 && ietemp.CompareTo(aetemp) > 0)))
                    //{
                    // count++;
                    //}
                    int count = 0;
                    for (int i = asminutes; i < aeminutes; i++)
                    {
                        for (int j = isminutes; j <= ieminutes; j++)
                        {
                            if (i == j)
                            {
                                count++;
                                break;
                            }
                        }
                        if (count > 0)
                        {
                            break;
                        }
                    }
                    if (count > 0)
                    {
                        clashappointments.Add(item);
                    }
                }
                return clashappointments;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}