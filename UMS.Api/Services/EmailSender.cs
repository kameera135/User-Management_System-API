using EmailLibrary.Interfaces;
using EmailLibrary.Models.EmailModels;
using MailKit.Net.Smtp;
using MimeKit;
using UMS.Api.Domain.DTO;
using UMS.Api.Interfaces;

namespace UMS.Api.Services
{
    public class EmailSender : IEmailSender
    {
        private readonly IServiceProvider m_serviceProvider;

        public EmailSender(IServiceProvider serviceProvider)
        {
            m_serviceProvider = serviceProvider;
        }

        public void SendEmail(Message message)
        {
            var emailMessage = CreateEmailMessage(message);
        }

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