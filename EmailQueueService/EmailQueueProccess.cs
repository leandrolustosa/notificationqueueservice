using System;
using System.Messaging;
using System.ServiceProcess;
using AInBox.Messaging.Model;
using AInBox.Messaging.Manager;
using AInBox.Astove.Core.Logging;

namespace EmailQueueService
{
    partial class EmailQueueProccess : ServiceBase
    {
        private readonly Library logger;
        public EmailQueueProccess()
        {
            InitializeComponent();
            logger = new Library();
        }

        protected override void OnStart(string[] args)
        {
            try
            {
                logger.WriteErrorLog("E-mail Queue Service Started");
                StartupQueue();
            }
            catch (Exception ex)
            {
                logger.WriteErrorLog(ex);
            }
        }

        protected override void OnStop()
        {
            logger.WriteErrorLog("E-mail Queue Service Stopped");
        }

        private void StartupQueue()
        {
            string messageQueuePath = @".\private$\emailqueue";
            
            var queue = new MessageQueue(messageQueuePath);
            queue.Formatter = new BinaryMessageFormatter();
            queue.MessageReadPropertyFilter.SetDefaults();
            queue.MessageReadPropertyFilter.DefaultBodySize = 1024 * 8;
            queue.ReceiveCompleted += MessageQueue_ReceiveCompleted;
            queue.BeginReceive();

            logger.WriteErrorLog("Startup Queue");
        }

        protected void MessageQueue_ReceiveCompleted(object sender, ReceiveCompletedEventArgs e)
        {
            logger.WriteErrorLog("Start MessageQueue_ReceiveCompleted");
            var queue = (MessageQueue)sender;
            try
            {
                logger.WriteErrorLog("Start EndReceive");
                var msg = queue.EndReceive(e.AsyncResult);
                logger.WriteErrorLog("End EndReceive");
                var emailMessage = (EmailMessage)msg.Body;
                logger.WriteErrorLog(string.Format("Message Received EmailId: {0} To: {1} Subject: {2}", emailMessage.EmailId, string.Join(",", emailMessage.To.ToArray()), emailMessage.Subject));
                
                var manager = new EmailManager();
                manager.SendEmail(emailMessage, logger);

                logger.WriteErrorLog("E-Mail Sent with Success");
            }
            catch (Exception ex)
            {
                logger.WriteErrorLog(ex);
            }
            finally
            {
                queue.Refresh();
                queue.Close();

                // Restart the queue
                StartupQueue();
            }
        }
    }
}
