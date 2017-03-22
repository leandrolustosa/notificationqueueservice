using AInBox.Queue.WebService.Console.NotificationService;
using System.Net;

namespace AInBox.Queue.WebService.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            var service = new NotificationService.Service
            {
                Credentials = new NetworkCredential("ainbox", "r4NHzPQ@te4x4*rpHBML"),
                PreAuthenticate = true
            };

            var model = new SendEmailBinding
            {
                Body = "<b>Testing the e-mail with Html</b></br></br>HTML is like this",
                BodyEncoding = "utf-8",
                BodyText = "Testing the e-mail with plain text",
                ModuleId = 1,
                Priority = 0,
                ReferenceId = 5,
                ReferenceName = "contato",
                Subject = "Testing the e-mail with Html or Plain Text",
                SubjectEncoding = "utf-8",
                To = "teste@email.com",
                ScheduleDate = System.DateTime.Now
            };
            var result = service.SendEmail(model);
            System.Console.ReadKey();
        }
    }
}
