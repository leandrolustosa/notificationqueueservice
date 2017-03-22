using AInBox.Astove.Core.Extensions;
using AInBox.Messaging.Core;
using AInBox.Messaging.Data;
using AInBox.Messaging.Model;
using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace AInBox.Messaging.Manager
{
    public class EnterpriseManager
    {
        public AddEnterpriseToDatabaseResult AddToDatabase(AddEnterpriseToDatabaseBinding model)
        {
            try
            {
                var service = Factory.CreateOrGetServiceOf<Enterprise>();
                var count = service.Where(o => o.Username.Equals(model.Username)).Count();
                if (count == 0)
                {
                    var obj = new Enterprise
                    {
                        FromName = model.FromName,
                        FromEmail = model.FromEmail,
                        Host = model.Host,
                        Name = model.Name,
                        Password = model.Password.Encrypt(),
                        Port = model.Port,
                        ServerPassword = model.ServerPassword.Encrypt(),
                        ServerUsername = model.ServerUsername,
                        Username = model.Username
                    };

                    var id = service.Add(obj);
                    if (id == 0)
                        throw new Exception("It was not possible to insert the module/project, try again, if persist the problem contact the administrator.");
                    
                    return new AddEnterpriseToDatabaseResult { IsValid = true };
                }

                return new AddEnterpriseToDatabaseResult { IsValid = false, Message = "The user informed is already been used for another enterprise, or your company is already registered in our database." };
            }
            catch (Exception ex)
            {
                return new AddEnterpriseToDatabaseResult { IsValid = false, Message = ex.GetExceptionMessageWithStackTrace() };
            }
        }
    }
}
