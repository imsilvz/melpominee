using System.Net;
using System.Net.Mail;
namespace Melpominee.app.Utilities;

public class MailManager
{
    private static Lazy<MailManager> _instance = new Lazy<MailManager>(() => new MailManager());
    public static MailManager Instance => _instance.Value;

    private SmtpClient _client;
    public MailManager() {
        _client = new SmtpClient(SecretManager.Instance.GetSecret("mail_host"))
        {
            Port = 587,
            Credentials = new NetworkCredential(SecretManager.Instance.GetSecret("mail_address"), SecretManager.Instance.GetSecret("mail_password")),
            EnableSsl = true
        };
    }

    public bool SendMail(string recipient, string subject, string body)
    {
        try 
        {
            string? mailAddress = SecretManager.Instance.GetSecret("mail_address");
            if (mailAddress is not null)
            {
                MailMessage mail = new MailMessage(mailAddress, recipient, subject, body);
                mail.IsBodyHtml = true;
                _client.Send(mail);
            }
        }
        catch (Exception)
        {
            return false;
        }
        return true;
    }
}