using AInBox.Messaging.Model;
using AInBox.Messaging.Manager;
using System.Web.Services;

namespace AInBox.Queue.WebService
{
    /// <summary>
    /// Serviço de notificação através de e-mail AI'n'Box
    /// </summary>
    [WebService(Namespace = "http://notifications.ainbox.com.br/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    [System.Web.Script.Services.ScriptService]
    public class Service : System.Web.Services.WebService
    {
        [WebMethod]
        public SendEmailResult SendEmail(SendEmailBinding model)
        {
            var manager = new EmailManager();
            var result = manager.SendEmail(model);

            return result;
        }

        [WebMethod]
        public AddEnterpriseToDatabaseResult AddEnterpriseToDatabase(AddEnterpriseToDatabaseBinding model)
        {
            var manager = new EnterpriseManager();
            var result = manager.AddToDatabase(model);

            return result;
        }

        [WebMethod]
        public AddModuleToDatabaseResult AddModuleToDatabase(AddModuleToDatabaseBinding model)
        {
            var manager = new ModuleManager();
            var result = manager.AddToDatabase(model);

            return result;
        }
    }
}
