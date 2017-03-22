using AInBox.Astove.Core.Data;
using AInBox.Astove.Core.Extensions;
using AInBox.Astove.Core.Logging;
using AInBox.Messaging.Core;
using AInBox.Messaging.Data;
using AInBox.Messaging.Model;
using AInBox.Messaging.Model.Enums;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Messaging;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;

namespace AInBox.Messaging.Manager
{
    public class EmailManager
    {
        public SendEmailResult SendEmail(SendEmailBinding model)
        {
            try
            {
                if (string.IsNullOrEmpty(model.ReferenceName))
                    throw new ArgumentNullException("model.ReferenceName");

                if (string.IsNullOrEmpty(model.Subject))
                    throw new ArgumentNullException("model.Subject");

                if (string.IsNullOrEmpty(model.To))
                    throw new ArgumentNullException("model.To");

                if (string.IsNullOrEmpty(model.Body))
                    throw new ArgumentNullException("model.Body");

                if (string.IsNullOrEmpty(model.SubjectEncoding))
                    model.SubjectEncoding = Encoding.Default.WebName;

                var emailId = AddToDatabase(model);

                if (emailId > 0)
                {
                    var emailMessage = CreateEmailMessage(emailId, model);
                    QueueMessage(emailMessage);

                    return new SendEmailResult { IsValid = true };
                }

                return new SendEmailResult { Message = "This e-mail can not be added, because it is already in the queue for send or has already been sent.", IsValid = false };
            }
            catch (Exception ex)
            {
                return new SendEmailResult { Message = ex.GetExceptionMessage(), IsValid = false };
            }
        }

        public void SendEmail(EmailMessage emailMessage, Library logger)
        {
            //Getting email record
            var service = Factory.CreateOrGetServiceOf<Email>();
            var email = service.Where(o => o.Id == emailMessage.EmailId, false, o => o.Module, o => o.Module.Enterprise).FirstOrDefault();

            if (email.NotificationDate == null)
            {
                //Create mail message instance 
                var mailMessage = new MailMessage();
                var from = (string.IsNullOrEmpty(email.Module.FromEmail)) ? email.Module.Enterprise.FromEmail : email.Module.FromEmail;
                var messageId = @"<" + Guid.NewGuid().ToString() + from.Substring(from.IndexOf("@")) + ">";
                mailMessage.Headers.Add("Message-ID", messageId);
                mailMessage.Subject = emailMessage.Subject;
                if (!string.IsNullOrEmpty(email.SubjectEncoding))
                    mailMessage.SubjectEncoding = Encoding.GetEncoding(email.SubjectEncoding);

                if (!string.IsNullOrEmpty(email.BodyEncoding))
                    mailMessage.BodyEncoding = Encoding.GetEncoding(email.BodyEncoding);

                var bodyHtml = GetEmailBody(email, EmailContentType.Html, logger);
                var bodyText = GetEmailBody(email, EmailContentType.Text, logger);
                logger.WriteErrorLog(string.Format("Message Received BodyHtml: {0} BodyText: {1}", bodyHtml, bodyText));
                if (!string.IsNullOrEmpty(bodyHtml) && !string.IsNullOrEmpty(bodyText))
                {
                    AlternateView plainTextContent =
                    AlternateView.CreateAlternateViewFromString(bodyText,
                                                null,
                                                MediaTypeNames.Text.Plain);

                    AlternateView htmlTextContent =
                    AlternateView.CreateAlternateViewFromString(bodyHtml,
                                                                null,
                                                                MediaTypeNames.Text.Html);

                    mailMessage.AlternateViews.Add(plainTextContent);
                    mailMessage.AlternateViews.Add(htmlTextContent);
                }
                else
                {
                    mailMessage.Body = (string.IsNullOrEmpty(bodyText)) ? bodyText : bodyHtml;
                    mailMessage.IsBodyHtml = emailMessage.IsBodyHtml;
                }

                emailMessage.To.ForEach(m => mailMessage.To.Add(m));
                emailMessage.CC.ForEach(m => mailMessage.CC.Add(m));
                emailMessage.Bcc.ForEach(m => mailMessage.Bcc.Add(m));

                if (emailMessage.SubjectEncoding != null)
                    mailMessage.SubjectEncoding = emailMessage.SubjectEncoding;

                if (emailMessage.BodyEncoding != null)
                    mailMessage.BodyEncoding = emailMessage.BodyEncoding;

                //Create Smtp client instance
                var s = new SmtpClient();
                var cfg = email.Module.Enterprise;
                if (cfg.IsHostParameterized)
                {
                    var name = (string.IsNullOrEmpty(email.Module.FromName)) ? cfg.FromName : email.Module.FromName;
                    if (string.IsNullOrEmpty(name))
                        mailMessage.From = new MailAddress(from);
                    else
                        mailMessage.From = new MailAddress(from, name);

                    s.EnableSsl = cfg.SSL;
                    s.Port = cfg.Port.GetValueOrDefault(25);
                    s.Host = cfg.Host;

                    if (cfg.IsUsernameParameterized)
                        s.Credentials = new System.Net.NetworkCredential(cfg.ServerUsername, cfg.ServerPassword.Decrypt());
                }

                s.Send(mailMessage);

                s.Dispose();
                mailMessage.Dispose();

                // Upadate notification date
                email.NotificationDate = DateTime.Now;
                service.Edit(email.Id, email);
            }
        }

        public bool SalvarArquivosEmail(Email email, Module module, string corpoHtml, string corpoTexto)
        {
            try
            {
                var path = "/files/emails";
                var folder = string.Concat("/", email.CreationDate.Year.ToString(), "/", email.CreationDate.Month.ToString(), "/", module.Name);
                var serverPath = System.Web.HttpContext.Current.Server.MapPath(string.Concat(path, folder));
                
                if (!Directory.Exists(serverPath))
                    Directory.CreateDirectory(serverPath);

                var fileNameFormat = "email-{0}-{1}-{2}-{3}.{4}";

                if (!string.IsNullOrEmpty(corpoHtml))
                {
                    var fileNameHtml = string.Format(fileNameFormat, email.Id, email.ModuleId, email.ReferenceName, email.ReferenceId, Astove.Core.Enums.Utility.GetEnumText(EmailContentType.Html));
                    var emailHtmlPath = string.Concat(serverPath, "/", fileNameHtml);
                    var fileHtml = File.CreateText(emailHtmlPath);
                    fileHtml.WriteLine(corpoHtml);
                    fileHtml.Flush();
                    fileHtml.Close();
                }

                if (!string.IsNullOrEmpty(corpoTexto))
                {
                    var fileNameTxt = string.Format(fileNameFormat, email.Id, email.ModuleId, email.ReferenceName, email.ReferenceId, Astove.Core.Enums.Utility.GetEnumText(EmailContentType.Text));
                    var emailTxtPath = string.Concat(serverPath, "/", fileNameTxt);
                    var fileTxt = File.CreateText(emailTxtPath);
                    fileTxt.WriteLine(corpoTexto);
                    fileTxt.Flush();
                    fileTxt.Close();
                }

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string GetEmailBody(Email email, EmailContentType contentType, Library logger)
        {
            try
            {
                var extension = Astove.Core.Enums.Utility.GetEnumText(contentType).ToLower();
                
                var webservicePath = System.Configuration.ConfigurationManager.AppSettings["WebServicePath"];
                var path = "\\files\\emails";
                var folder = string.Concat("\\", email.CreationDate.Year.ToString(), "\\", email.CreationDate.Month.ToString(), "\\", email.Module.Name);
                var serverPath = string.Concat(webservicePath, path, folder);
                
                var fileNameFormat = "email-{0}-{1}-{2}-{3}.{4}";

                var fileName = string.Format(fileNameFormat, email.Id, email.ModuleId, email.ReferenceName, email.ReferenceId, extension);
                var emailPath = string.Concat(serverPath, "\\", fileName);
                logger.WriteErrorLog(emailPath);

                using (StreamReader sr = File.OpenText(emailPath))
                {
                    string body = sr.ReadToEnd();
                    return body;
                }
            }
            catch (Exception ex)
            {
                logger.WriteErrorLog(ex);
                return string.Empty;
            }
        }

        private int AddToDatabase(SendEmailBinding model)
        {
            var priority = 0;
            if (model.Priority > 5)
                priority = 5;
            else if (model.Priority < 0)
                priority = 0;

            var service = Factory.CreateOrGetServiceOf<Email>();

            var count = service.Where(o => o.To.Equals(model.To, StringComparison.CurrentCultureIgnoreCase) && o.ModuleId == model.ModuleId && o.ReferenceId == model.ReferenceId && o.ReferenceName.Equals(model.ReferenceName, StringComparison.CurrentCultureIgnoreCase) && o.ReferenceParameters.Equals(model.ReferenceParameters, StringComparison.CurrentCultureIgnoreCase), true).Count();

            if (count == 0)
            {
                var email = new Email
                {
                    Bco = model.Bcc,
                    Cc = model.CC,
                    CreationDate = DateTime.Now,
                    Priority = priority,
                    ReferenceId = model.ReferenceId,
                    ReferenceName = model.ReferenceName,
                    ReferenceParameters = model.ReferenceParameters,
                    ScheduleDate = model.ScheduleDate,
                    Subject = model.Subject,
                    ModuleId = model.ModuleId,
                    To = model.To
                };

                var result = service.Add(email);

                var moduleService = Factory.CreateOrGetServiceOf<Module>();
                var module = moduleService.GetSingle(email.ModuleId);
                SalvarArquivosEmail(email, module, model.Body, model.BodyText);

                return result;
            }

            return 0;
        }

        private void QueueMessage(EmailMessage emailMessage)
        {
            // Creating message to send to queue
            var message = new Message();
            message.Body = emailMessage;
            message.Recoverable = true;
            message.Formatter = new BinaryMessageFormatter();

            // Instatiating the queue
            var msmqQueuePath = @".\Private$\EmailQueue";
            MessageQueue msmqQueue =
                (!MessageQueue.Exists(msmqQueuePath))
                    ? MessageQueue.Create(msmqQueuePath)
                    : new MessageQueue(msmqQueuePath);

            // Send a message to queue
            msmqQueue.Formatter = new BinaryMessageFormatter();
            msmqQueue.Send(message);
            msmqQueue.Refresh();
            msmqQueue.Close();
        }

        private EmailMessage CreateEmailMessage(int emailId, SendEmailBinding model)
        {
            var emailMessage = new EmailMessage
            {
                EmailId = emailId,
                Bcc = GenerateListEmails(model.Bcc),
                BodyEncoding = Encoding.GetEncoding(model.BodyEncoding),
                CC = GenerateListEmails(model.CC),
                IsBodyHtml = true,
                Subject = model.Subject,
                SubjectEncoding = Encoding.GetEncoding(model.SubjectEncoding),
                To = GenerateListEmails(model.To)
            };

            return emailMessage;
        }

        private List<string> GenerateListEmails(string args)
        {
            if (string.IsNullOrEmpty(args))
                return new List<string>();

            var arr = args.Split(new[] { ",", ";", "|" }, StringSplitOptions.RemoveEmptyEntries);
            var emails = new List<string>(arr);
            emails.ForEach(email => email = email.Trim());

            return emails;
        }
    }
}
