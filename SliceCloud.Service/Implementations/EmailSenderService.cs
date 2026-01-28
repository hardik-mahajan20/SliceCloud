using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using SliceCloud.Service.Interfaces;

namespace SliceCloud.Service.Implementations;

public class EmailSenderService(IConfiguration configuration, IWebHostEnvironment webHostEnvironment) : IEmailSenderService
{
    private readonly IConfiguration _configuration = configuration;
    private readonly IWebHostEnvironment _webHostEnvironment = webHostEnvironment;

    #region SendEmail

    public async Task SendEmailAsync(string toEmail, string subject, string htmlBody, string imagePath)
    {
        string? smtpHost = _configuration["EmailSettings:SmtpHost"];
        int smtpPort = int.Parse(_configuration["EmailSettings:SmtpPort"]!);
        string? smtpUser = _configuration["EmailSettings:SmtpUser"];
        string? smtpPass = _configuration["EmailSettings:SmtpPass"];
        string? fromEmail = _configuration["EmailSettings:FromEmail"];

        using (SmtpClient? client = new SmtpClient(smtpHost, smtpPort))
        {
            client.Credentials = new NetworkCredential(smtpUser, smtpPass);
            client.EnableSsl = true;

            MailMessage mailMessage = new()
            {
                From = new MailAddress(fromEmail ?? string.Empty, "SliceCloud Support"),
                Subject = subject,
                IsBodyHtml = true
            }
            ;

            mailMessage.To.Add(toEmail);

            AlternateView? alternateView = AlternateView.CreateAlternateViewFromString(htmlBody, null, MediaTypeNames.Text.Html);

            if (File.Exists(imagePath))
            {
                LinkedResource logo = new LinkedResource(imagePath, MediaTypeNames.Image.Jpeg)
                {
                    ContentId = "SliceCloudLogo",
                    TransferEncoding = TransferEncoding.Base64,
                    ContentType = new ContentType("image/png")
                };

                alternateView.LinkedResources.Add(logo);
            }

            mailMessage.AlternateViews.Add(alternateView);

            await client.SendMailAsync(mailMessage);
        }
    }

    #endregion

    #region SendResetPasswordEmail

    public async Task SendResetPasswordEmailAsync(string toEmail, string? resetLink)
    {
        string? imagePath = Path.Combine(_webHostEnvironment.WebRootPath, "images", "logo.png");

        string subject = "Reset Your Password - SliceCloud";

        string body = $@"
<html>
<body style='font-family: Arial, sans-serif; margin: 0; padding: 0;'>
    <table role='presentation' width='100%' cellspacing='0' cellpadding='0' border='0' style='background-color: #f5f5f5; padding: 20px 0; text-align: center;'>
        <tr>
            <td align='center'>
                <table role='presentation' width='600px' cellspacing='0' cellpadding='0' border='0' style='background: white; padding: 20px; text-align: center;'>
                    <tr>
                        <td style='background-color: #0066A7; padding: 20px; text-align: center;'>
                            <table role='presentation' cellspacing='0' cellpadding='0' border='0' align='center'>
                                <tr>
                                    <td valign='middle'>
                                        <!-- Ensure image size is 20px by 20px using inline CSS -->
                                        <div style='display: flex; align-items: center; justify-content: center;'>
                                            <!-- Image -->
                                            <img src='cid:SliceCloudLogo' alt='SliceCloud Logo' style='width: 40px; height: 40px; border-radius: 5px; margin-right: 10px;' />
                                            <!-- Text (h2 with inline-block) -->
                                            <h2 style='color: white; margin: 0; font-size: 22px; display: inline-block;'>SliceCloud</h2>
                                        </div>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td style='text-align: left; padding: 20px; font-size: 16px; color: #333;'>
                            <p>Dear SliceCloud User,</p>
                            <p>Please <a href='{resetLink}' style='color: blue;'>click here</a> to reset your account password.</p>
                            <p>If you encounter any issues or have any questions, please do not hesitate to contact our support team.</p>
                            <p style='color: red; font-weight: bold;'>Important Note: For security reasons, the link will expire in 24 hours.</p>
                            <p>If you did not request a password reset, please ignore this email or contact our support team immediately.</p>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
</body>
</html>";

        await SendEmailAsync(toEmail, subject, body, imagePath);
    }

    #endregion

}


