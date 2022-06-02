using System.Text;

namespace physioCard.wwwroot.lib.Securities
{
    public class CustomSecurity
    {
        public static string Key = "afed@@$$pqerds@";

        public static string ConvertToEncrypt(string password) {
            if (string.IsNullOrEmpty(password))
            {
                return "";
            }
            else {
                password += Key;
                var passwordBytes = Encoding.UTF8.GetBytes(password);
                return Convert.ToBase64String(passwordBytes);
            }
        }

        public static string ConvertToDecrypt(string password)
        {
            if (string.IsNullOrEmpty(password))
            {
                return "";
            }
            else
            {
                var base64Password = Convert.FromBase64String(password);
                var origin = Encoding.UTF8.GetString(base64Password);
                origin = origin.Substring(0, origin.Length - Key.Length);
                return origin;
            }
        }
    }
}
