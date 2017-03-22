using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Mail;
using System.Net.Mime;

namespace AInBox.Astove.Core.Util
{
    public class EmailManager
    {
        public delegate void SendEmail_Delegate(MailMessage mailMessage);

        public static MailMessage CreateEmailMessage(string subject, string message, bool isBodyHtml, string[] to)
        {
            if (string.IsNullOrEmpty(subject))
                throw new ArgumentNullException(nameof(subject));

            if (string.IsNullOrEmpty(message))
                throw new ArgumentNullException(nameof(message));

            if (to == null)
                throw new ArgumentNullException(nameof(to));

            MailMessage mailMessage = new MailMessage();
            mailMessage.Subject = subject;
            mailMessage.Body = message;
            mailMessage.IsBodyHtml = isBodyHtml;
            mailMessage.SubjectEncoding = System.Text.Encoding.Default;
            mailMessage.BodyEncoding = System.Text.Encoding.Default;

            foreach (string email in to)
                mailMessage.To.Add(email);

            return mailMessage;
        }

        public static MailMessage CreateEmailMessage(string subject, string message, bool isBodyHtml, string[] to, string[] cc, string[] bcc)
        {
            if (string.IsNullOrEmpty(subject))
                throw new ArgumentNullException(nameof(subject));

            if (string.IsNullOrEmpty(message))
                throw new ArgumentNullException(nameof(message));

            if (to == null)
                throw new ArgumentNullException(nameof(to));

            MailMessage mailMessage = new MailMessage();
            mailMessage.Subject = subject;
            mailMessage.Body = message;
            mailMessage.IsBodyHtml = isBodyHtml;
            mailMessage.SubjectEncoding = System.Text.Encoding.Default;
            mailMessage.BodyEncoding = System.Text.Encoding.Default;

            foreach (string email in to)
                mailMessage.To.Add(email);

            if (cc != null)
            {
                foreach (string email in cc)
                    mailMessage.CC.Add(email);
            }

            if (bcc != null)
            {
                foreach (string email in bcc)
                    mailMessage.Bcc.Add(email);
            }

            return mailMessage;
        }

        public static MailMessage CreateEmailMessage(string subject, string message, bool isBodyHtml, Encoding subjectEncoding, Encoding bodyEncoding, string[] to, string[] cc, string[] bcc)
        {
            if (string.IsNullOrEmpty(subject))
                throw new ArgumentNullException(nameof(subject));

            if (string.IsNullOrEmpty(message))
                throw new ArgumentNullException(nameof(message));

            if (to == null)
                throw new ArgumentNullException(nameof(to));

            MailMessage mailMessage = new MailMessage();
            mailMessage.Subject = subject;
            mailMessage.Body = message;
            mailMessage.IsBodyHtml = isBodyHtml;
            mailMessage.SubjectEncoding = (subjectEncoding == null) ? System.Text.Encoding.Default : subjectEncoding;
            mailMessage.BodyEncoding = (bodyEncoding == null) ? System.Text.Encoding.Default : bodyEncoding;

            foreach (string email in to)
                mailMessage.To.Add(email);

            if (cc != null)
            {
                foreach (string email in cc)
                    mailMessage.CC.Add(email);
            }

            if (bcc != null)
            {
                foreach (string email in bcc)
                    mailMessage.Bcc.Add(email);
            }

            return mailMessage;
        }

        public static void SendEmail(string subject, string messageHtml, string messageText, bool isBodyHtml, string[] to, Encoding subjectEncoding = null, Encoding bodyEncoding = null, string[] cc = null, string[] bcc = null)
        {
            if (string.IsNullOrEmpty(subject))
                throw new ArgumentNullException(nameof(subject));

            if (string.IsNullOrEmpty(messageHtml))
                throw new ArgumentNullException(nameof(messageHtml));

            if (string.IsNullOrEmpty(messageText))
                throw new ArgumentNullException(nameof(messageText));

            if (to == null)
                throw new ArgumentNullException(nameof(to));

            if (subjectEncoding == null)
                subjectEncoding = Encoding.Default;

            if (bodyEncoding == null)
                bodyEncoding = Encoding.Default;

            string messageId = string.Concat(@"<", Guid.NewGuid().ToString(), "@", System.Configuration.ConfigurationManager.AppSettings["DomainName"] ,">");

            System.Net.Mail.MailMessage mailMessage = new System.Net.Mail.MailMessage();
            mailMessage.Headers.Add("Message-ID", messageId);
            mailMessage.Subject = subject;
            mailMessage.SubjectEncoding = subjectEncoding;
            mailMessage.BodyEncoding = bodyEncoding;

            AlternateView plainTextContent =
            AlternateView.CreateAlternateViewFromString(messageText,
                                            null,
                                            MediaTypeNames.Text.Plain);

            AlternateView htmlTextContent =
            AlternateView.CreateAlternateViewFromString(messageHtml,
                                                        null,
                                                        MediaTypeNames.Text.Html);

            mailMessage.AlternateViews.Add(plainTextContent);
            mailMessage.AlternateViews.Add(htmlTextContent);

            foreach (string email in to)
                mailMessage.To.Add(email);

            if (cc != null)
            {
                foreach (string email in cc)
                    mailMessage.CC.Add(email);
            }

            if (bcc != null)
            {
                foreach (string email in bcc)
                    mailMessage.Bcc.Add(email);
            }

            SendEmail(mailMessage);
        }

        public static void SendEmail(MailMessage mailMessage)
        {
            SmtpClient smtp = new SmtpClient();
            smtp.Send(mailMessage);
        }
    }
}