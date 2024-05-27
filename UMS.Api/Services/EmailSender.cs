using CommonInsfrastructure.Interfaces;
using CommonInsfrastructure.Models.EmailModels;
using MailKit.Net.Smtp;
using MimeKit;
using UMS.Api.Domain.DTO;
using UMS.Api.Interfaces;

namespace UMS.Api.Services
{
    public class EmailSender : IEmailSender
    {
        private readonly ILogger<EmailSender> _logger;
        private readonly EmailConfigurationDTO _configuration;
        private readonly IServiceProvider m_serviceProvider;

        public EmailSender(ILogger<EmailSender> logger, EmailConfigurationDTO configuration, IServiceProvider serviceProvider)
        {

            _logger = logger;
            _configuration = configuration;
            m_serviceProvider = serviceProvider;
        }

        public void SendEmail(Message message)
        {
            var emailMessage = CreateEmailMessage(message);

            Send(emailMessage);
        }

        /*public async Task SendEmailAsync(Message message)
        {
            var mailMessage = CreateEmailMessage(message);

            await SendAsync(mailMessage);   
        }*/

        public async Task SendEmailAsync(Message message, string email, string callback)
        {
            using var scope = m_serviceProvider.CreateScope();
            var m_emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();

            TblEmailList emailList = new TblEmailList();

            emailList.ToList = email;
            emailList.Subject = "Password Reset Token";
            emailList.Status = "N";
            emailList.GeneratedTime = DateTime.Now;

            // Create the email message
            var mimeMessage = CreateEmailMessage(message);

            // Set the email body
            emailList.Body = mimeMessage.HtmlBody;

            m_emailService.SendEmail(emailList);

        }

        private MimeMessage CreateEmailMessage(Message message)
        {
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress("StarHub_Message_Service", _configuration.From));
            emailMessage.To.AddRange(message.To);
            emailMessage.Subject = message.Subject;

            // Read the HTML content from an external file
            var htmlContent = ReadHtmlContentFromFile("email_template.html");

            // Replace placeholders in the HTML content with actual values
            htmlContent = htmlContent.Replace("{Content}", message.Content);


            var bodyBuilder = new BodyBuilder { HtmlBody = htmlContent };

            if (message.Attachments != null && message.Attachments.Any())
            {
                byte[] fileBytes;
                foreach (var attachment in message.Attachments)
                {
                    using (var ms = new MemoryStream())
                    {
                        attachment.CopyTo(ms);
                        fileBytes = ms.ToArray();
                    }

                    bodyBuilder.Attachments.Add(attachment.FileName, fileBytes, ContentType.Parse(attachment.ContentType));
                }
            }

            emailMessage.Body = bodyBuilder.ToMessageBody();
            return emailMessage;
        }

        private void Send(MimeMessage mailMessage)
        {
            using (var client = new SmtpClient())
            {
                try
                {
                    client.Connect(_configuration.SmtpServer, _configuration.Port, true);
                    client.AuthenticationMechanisms.Remove("XOAUTH2");
                    client.Authenticate(_configuration.UserName, _configuration.Password);

                    client.Send(mailMessage);
                }
                catch
                {
                    //log an error message or throw an exception, or both.
                    throw;
                }
                finally
                {
                    client.Disconnect(true);
                    client.Dispose();
                }
            }
        }

        private async Task SendAsync(MimeMessage mailMessage)
        {
            using (var client = new SmtpClient())
            {
                try
                {
                    await client.ConnectAsync(_configuration.SmtpServer, _configuration.Port, true);
                    client.AuthenticationMechanisms.Remove("XOAUTH2");
                    await client.AuthenticateAsync(_configuration.UserName, _configuration.Password);

                    await client.SendAsync(mailMessage);
                }
                catch
                {
                    //log an error message or throw an exception, or both.
                    throw;
                }
                finally
                {
                    await client.DisconnectAsync(true);
                    client.Dispose();
                }
            }
        }

        private string ReadHtmlContentFromFile(string fileName)
        {
            // Read the content of the HTML file
            try
            {
                // Get the current directory
                string currentDirectory = Directory.GetCurrentDirectory();

                // Combine the current directory with the file name
                string filePath = Path.Combine(currentDirectory, fileName);

                return File.ReadAllText(filePath); 
            }
            catch (Exception ex)
            {
                // Handle file read errors
                throw new Exception("Error reading HTML content from file: " + ex.Message);
            }
        }
    }
}
