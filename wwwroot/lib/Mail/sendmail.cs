using System.Net;
using System.Net.Mail;

namespace physioCard.wwwroot.lib.Mail
{
    public class sendmail
    {
        public static bool sendingEmail(string to, string subject, string msgbody)
        {
            MailMessage message = new MailMessage();
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
    }
}
