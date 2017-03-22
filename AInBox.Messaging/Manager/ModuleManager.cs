using AInBox.Astove.Core.Extensions;
using AInBox.Messaging.Core;
using AInBox.Messaging.Data;
using AInBox.Messaging.Model;
using System;
using System.Linq;

namespace AInBox.Messaging.Manager
{
    public class ModuleManager
    {
        public AddModuleToDatabaseResult AddToDatabase(AddModuleToDatabaseBinding model)
        {
            try
            {
                var service = Factory.CreateOrGetServiceOf<Module>();
                var count = service.Where(o => o.Name.Equals(model.Name, System.StringComparison.CurrentCultureIgnoreCase) && o.EnterpriseId == model.EnterpriseId).Count();
                if (count == 0)
                {
                    var obj = new Module
                    {
                        Active = true,
                        Name = model.Name,
                        EnterpriseId = model.EnterpriseId,
                        FromEmail = model.FromEmail,
                        FromName = model.FromName,
                        RestrictIps = model.RestrictIps
                    };

                    var id = service.Add(obj);
                    if (id == 0)
                        throw new Exception("It was not possible to insert the module/project, try again, if persist the problem contact the administrator.");

                    return new AddModuleToDatabaseResult { IsValid = true };
                }

                return new AddModuleToDatabaseResult { IsValid = false, Message = "The module/project has already been added." };
            }
            catch (Exception ex)
            {
                return new AddModuleToDatabaseResult { IsValid = false, Message = ex.GetExceptionMessageWithStackTrace() };
            }
        }
    }
}
