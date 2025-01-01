using System.Net;
using System.Net.Mail;
namespace PetStore.Pages.Customer

{
    public class Verify_Profile_Update
    {
        private readonly IConfiguration _configuration;

        public Verify_Profile_Update(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendVerificationEmailAsync(string toEmail)
        {
            var smtpSettings = _configuration.GetSection("SmtpSettings");

            var smtpClient = new SmtpClient(smtpSettings["Server"])
            {
                Port = int.Parse(smtpSettings["Port"]),
                Credentials = new NetworkCredential(smtpSettings["Username"], smtpSettings["Password"]),
                EnableSsl = bool.Parse(smtpSettings["UseSSL"]),
            };

            //var mailMessage = new MailMessage
            //{
            //    From = new MailAddress(smtpSettings["SenderEmail"], smtpSettings["SenderName"]),
            //    Subject = "Verify Profile Update",
            //    Body = $"You have successfully updated your account information on {DateTime.Now:dddd, MMMM dd, yyyy hh:mm tt}.",
            //    IsBodyHtml = true,
            //};

            var mailMessage = new MailMessage
            {
                From = new MailAddress(smtpSettings["SenderEmail"], smtpSettings["SenderName"]),
                Subject = "Verify Profile Update",
                Body = $@"
    <html>
        <body>
            <pre>

          /\_/\                                                  /\_/\
         (^.^ )          You have successfully updated          ( >.< )
         > ^ <             your account information              > ^ <
        /     \      on {DateTime.Now:dddd, MMMM dd, yyyy hh:mm tt}     /     \       
       (   /\  )                                               (  /\   )
                    Thank you for using PetStore service ❤
            </pre>
        </body>
    </html>",
                IsBodyHtml = true // Đảm bảo email được gửi dưới dạng HTML.
            };


            mailMessage.To.Add(toEmail);

            await smtpClient.SendMailAsync(mailMessage);
        }
    }
}
