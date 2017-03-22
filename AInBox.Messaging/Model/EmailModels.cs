using AInBox.Astove.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AInBox.Messaging.Model
{
    [Serializable]
    public class EmailMessage
    {
        public EmailMessage()
        {
            this.To = new List<string>();
            this.CC = new List<string>();
            this.Bcc = new List<string>();
        }

        public int EmailId { get; set; }
        public string Subject { get; set; }
        public List<string> To { get; set; }
        public List<string> CC { get; set; }
        public List<string> Bcc { get; set; }
        public Encoding SubjectEncoding { get; set; }
        public Encoding BodyEncoding { get; set; }
        public bool IsBodyHtml { get; set; }
    }

    [Serializable]
    public class SendEmailBinding
    {
        public int ModuleId { get; set; }
        public string ReferenceName { get; set; }
        public int ReferenceId { get; set; }
        public string ReferenceParameters { get; set; }
        public DateTime? ScheduleDate { get; set; }
        public int Priority { get; set; }
        public string To { get; set; }
        public string CC { get; set; }
        public string Bcc { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public string BodyText { get; set; }
        public string SubjectEncoding { get; set; }
        public string BodyEncoding { get; set; }
        public bool IsBodyHtml { get; set; }
    }

    [Serializable]
    public class SendEmailResult : BaseResultModel
    {
    }
}
