using physioCard.Domain;
using System.Collections;
using System.Net;
using System.Net.Mail;

namespace physioCard.wwwroot.lib.Mail
{
    public class sendmail
    {
        public static bool sendingEmail(string to, string subject, string msgbody)
        {
            MailMessage message = new MailMessage();

            //message.IsBodyHtml = true;

            //to = "patelpriyankaa9@gmail.com";
            string from = "info.physiocard@gmail.com";
            string pass = "osqtzwwhfnsunumj";
            //msgbody = "Your OTP is " + randomCode;
            message.To.Add(to);
            message.From = new MailAddress(from);
            message.Subject = subject;
            message.Body = msgbody;
            SmtpClient smtpClient = new SmtpClient("smtp.gmail.com");
            smtpClient.EnableSsl = true;
            smtpClient.Port = 587;
            smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtpClient.Credentials = new NetworkCredential(from, pass);

            try
            {
                smtpClient.Send(message);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public static bool sendingEmailWithImage(string to, string subject, string msgbody, AlternateView alternateViewLogo, AlternateView alternateViewSign)
        {
            MailMessage message = new MailMessage();

            message.IsBodyHtml = false;

            //to = "patelpriyankaa9@gmail.com";
            string from = "info.physiocard@gmail.com";
            string pass = "osqtzwwhfnsunumj";
            //msgbody = "Your OTP is " + randomCode;
            message.To.Add(to);
            message.From = new MailAddress(from);
            message.Subject = subject;
            message.Body = msgbody;
            message.AlternateViews.Add(alternateViewLogo);
            message.AlternateViews.Add(alternateViewSign);

            SmtpClient smtpClient = new SmtpClient("smtp.gmail.com");
            smtpClient.EnableSsl = true;
            smtpClient.Port = 587;
            smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtpClient.Credentials = new NetworkCredential(from, pass);

            try
            {
                smtpClient.Send(message);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public static bool sendingEmailWithHtml(string to, string subject, string msgbody)
        {
            MailMessage message = new MailMessage();

            message.IsBodyHtml = true;

            //to = "patelpriyankaa9@gmail.com";
            string from = "info.physiocard@gmail.com";
            string pass = "osqtzwwhfnsunumj";
            //msgbody = "Your OTP is " + randomCode;
            message.To.Add(to);
            message.From = new MailAddress(from);
            message.Subject = subject;
            message.Body = msgbody;
            SmtpClient smtpClient = new SmtpClient("smtp.gmail.com");
            smtpClient.EnableSsl = true;
            smtpClient.Port = 587;
            smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtpClient.Credentials = new NetworkCredential(from, pass);

            try
            {
                smtpClient.Send(message);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public static string CreateBodyForAppointmentSchedule(Patient p, Doctor d, ArrayList emaildate)
        {
            string body = string.Empty;
            using (StreamReader reader = new StreamReader("Views\\Appointment\\scheduleAppointmentEmailTemplate.cshtml"))
            {
                body = reader.ReadToEnd();
            }
            body = body.Replace("{patientname}", p.fname + " " + p.lname);
            body = body.Replace("{doctorname}", d.fname + " " + d.lname);

            string temp = null;
            foreach (var item in emaildate)
            {
                temp = temp + item + "<br/>";
            }

            body = body.Replace("{appointmentdetails}", temp);

            return body;
        }
        public static string CreateBody(Invoice invoice, Patient patient, Clinic clinic, Doctor doctor)
        {
            string body = string.Empty;
            using (StreamReader reader = new StreamReader("Views\\Invoice\\InvoiceTemplate.cshtml"))
            {
                body = reader.ReadToEnd();
            }

            //body = body.Replace("{id}", employee.Id.ToString());
            //body = body.Replace("{name}", employee.Name);
            //body = body.Replace("{dob}", employee.DOB.ToString("dd-MM-yyyy"));
            //body = body.Replace("{address}", employee.Address);
            //body = body.Replace("{photo}", employee.Photo);

            body = body.Replace("{clinicname}", clinic.name);
            body = body.Replace("{clinicaddress}", clinic.address);
            body = body.Replace("{cliniccontactno}", clinic.contactno.ToString());
            body = body.Replace("{clinicgstno}", clinic.GSTno);

            body = body.Replace("{doctorname}", doctor.fname + " " + doctor.lname);

            body = body.Replace("{patientname}", patient.fname + " " + patient.lname);
            body = body.Replace("{patientcontactno}", patient.contactno.ToString());

            body = body.Replace("{invoiceno}", invoice.invoiceNo);
            body = body.Replace("{invoicedate}", invoice.invoice_date.ToString("dd-MM-yyyy"));

            body = body.Replace("{total_appointment}", invoice.total_appointment.ToString());
            body = body.Replace("{total_amount}", invoice.total_amount.ToString());
            body = body.Replace("{discount}", invoice.discount.ToString());
            body = body.Replace("{gross_amount}", invoice.gross_amount.ToString());

            return body;
        }
    }
}