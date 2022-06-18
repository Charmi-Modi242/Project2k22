using Dapper;
using physioCard.Domain;
using System.Data;

namespace physioCard.Repositories
{
    public class InvoiceRepository : BaseRepository, IInvoiceRepository
    {
        public InvoiceRepository(IConfiguration configuration) : base(configuration)
        {
        }

        public async Task<List<revenueModel>> getYearlyRevenue(int did)
        {
            try
            {
                string sql = "select DATEPART(MM, invoice_date) as monthID, sum(gross_amount) as revenue FROM invoiceTB where doctorID=@did AND invoice_date > DATEADD(m, -11, (DATEADD(MM, DATEDIFF(mm, 0, GETDATE()), 0))) AND pay_status = 'True' group by DATEPART(MM, invoice_date);";
                var param = new DynamicParameters();
                param.Add("did", did, DbType.Int32);
                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<revenueModel>(sql, param)).ToList();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<int> getRevenueCount(int did)
        {
            try
            {
                string sql = "select sum(gross_amount) from invoiceTB where datepart(MM, invoice_date) = datepart(MM, getDate()) AND datepart(YYYY, invoice_date) = datepart(YYYY, getDate()) AND doctorID = @id AND pay_status = 'True';";
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

        public async Task<List<Appointment>> getAppointmentbyPatientIDAsync(int patientID)
        {
            try
            {
                List<Appointment> appointments = new List<Appointment>();
                bool istatus = false;
                bool astatus = true;
                string sql = "select a.appointmentID, a.patientID, a.date_start, a.starttime, a.sessiontime, a.cost, a.attendance_status, a.invoice_status, p.fname, p.lname from appointmentTB a, patientTB p where p.patientID=@id and a.patientID=p.patientID and (a.invoice_status=@istatus or a.invoice_status IS NULL) and a.attendance_status=@astatus order by a.date_start";
                var param = new DynamicParameters();
                param.Add("id", patientID, DbType.Int32);
                param.Add("istatus", istatus, DbType.Boolean);
                param.Add("astatus", astatus, DbType.Boolean);
                using (var connection = CreateConnection())
                {
                    appointments = (await connection.QueryAsync<Appointment>(sql, param)).ToList();
                }
                if(appointments.Count > 0)
                {
                    foreach(var item in appointments)
                    {
                        item.endtime = item.starttime.Add(item.sessiontime.TimeOfDay);
                    }
                }
                return appointments;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<bool> createInvoiceAsync(Invoice invoice)
        {
            try
            {
                int c;
                string sql = "insert into invoiceTB(invoiceNo, patientID, doctorID, invoice_date, istart_date, iend_date, total_appointment, total_amount, discount, gross_amount) values(@invoiceNo, @patientID, @doctorID, @invoice_date, @istart_date, @iend_date, @total_appointment, @total_amount, @discount, @gross_amount)";
                var param = new DynamicParameters();
                param.Add("invoiceNo",invoice.invoiceNo,DbType.String);
                param.Add("patientID", invoice.patientID, DbType.Int32);
                param.Add("doctorID", invoice.doctorID, DbType.Int32);
                param.Add("invoice_date", invoice.invoice_date, DbType.DateTime);
                param.Add("istart_date", invoice.istart_date, DbType.Date);
                param.Add("iend_date", invoice.iend_date, DbType.Date);
                param.Add("total_appointment", invoice.total_appointment, DbType.Int16);
                param.Add("total_amount", invoice.total_amount, DbType.Currency);
                param.Add("discount", invoice.discount, DbType.Double);
                param.Add("gross_amount", invoice.gross_amount, DbType.Currency);

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
            catch(Exception ex)
            {
                return false;
                throw new Exception(ex.Message);
            }
        }

        public async Task<bool> updateAppointmentInvoiceStatusAsync(List<Appointment> appointments)
        {
            try
            {
                int i = 0;
                bool status = true;
                foreach(var item in appointments)
                {
                    int s;
                    string sql = "update appointmentTB set invoice_status=@invoice_status where appointmentID=@appointmentID";
                    var param = new DynamicParameters();
                    param.Add("appointmentID", item.appointmentID, DbType.Int32);
                    param.Add("invoice_status", status, DbType.Boolean);
                    using (var connnection = CreateConnection())
                    {
                        s = await connnection.ExecuteAsync(sql, param);
                    }
                    i = i + s;
                }
                if (i == (appointments.Count - 1))
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

        public async Task<List<Appointment>> getAppointmentsinInvoiceAsync(Invoice invoice)
        {
            try
            {
                List<Appointment> appointments= new List<Appointment>();
                bool status = true; 
                string sql = "select * from appointmentTB where (date_start between @sdate and @edate) and invoice_status=@status and patientID=@ipatientID;";
                var param = new DynamicParameters();
                param.Add("sdate",invoice.istart_date,DbType.Date);
                param.Add("edate",invoice.iend_date,DbType.Date);
                param.Add("status",status,DbType.Boolean);
                param.Add("ipatientID",invoice.patientID,DbType.Int32);

                using (var connection = CreateConnection())
                {
                    appointments = (await connection.QueryAsync<Appointment>(sql, param)).ToList();
                }
                return appointments;
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<Invoice> getInvoiceByIDAsync(String ino)
        {
            try
            {
                Invoice invoice = new Invoice();
                string sql = "select * from invoiceTB where invoiceNo=@ino;";
                var param = new DynamicParameters();
                param.Add("ino",ino,DbType.String);

                using (var connection = CreateConnection())
                {
                    invoice = await connection.QueryFirstOrDefaultAsync<Invoice>(sql, param);
                }
                return invoice;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<Invoice>> getAllInvoiceMonthlyAsync(int did)
        {
            try 
            { 
                List<Invoice> invoice = new List<Invoice>();
                string sql = "select i.invoiceID, i.invoiceNo, i.invoice_date, i.istart_date, i.iend_date, i.gross_amount, i.pay_status, p.fname, p.lname from invoiceTB i, patientTB p where i.patientID = p.patientID and i.doctorID = @did and DatePart(MM, i.invoice_date) = DatePart(MM, @today);";
                var param = new DynamicParameters();
                param.Add("did",did,DbType.Int32);
                param.Add("today",DateTime.Now.ToString("dd-MM-yyyy"),DbType.Date);
                using (var connection = CreateConnection())
                {
                    invoice = (await connection.QueryAsync<Invoice>(sql, param)).ToList();
                }
                return invoice;
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<bool> payStatusEditAsync(bool status, int invoiceID)
        {
            try
            {
                int i;
                string sql = "update invoiceTB set pay_status=@status where invoiceID=@invoiceID";
                var param = new DynamicParameters();
                param.Add("status", status, DbType.Boolean);
                param.Add("invoiceID", invoiceID, DbType.Int32);
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
            catch(Exception ex)
            {
                return false;
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<Invoice>> seeAllPendingInvoiceAsync(int did)
        {
            try
            {
                bool status = false;
                List<Invoice> invoice = new List<Invoice>();
                string sql = "select i.invoiceID, i.invoiceNo, i.invoice_date, i.istart_date, i.iend_date, i.gross_amount, i.pay_status, p.fname, p.lname from invoiceTB i, patientTB p where i.patientID = p.patientID and i.doctorID = @did and (i.pay_status = @status or i.pay_status is null)";
                var param = new DynamicParameters();
                param.Add("did", did, DbType.Int32);
                param.Add("status", status, DbType.Boolean);
                using (var connection = CreateConnection())
                {
                    invoice = (await connection.QueryAsync<Invoice>(sql, param)).ToList();
                }
                return invoice;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
