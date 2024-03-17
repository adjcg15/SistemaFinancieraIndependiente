using SFIDataAccess.CustomExceptions;
using SFIDataAccess.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace SFIDataAccess.DataAccessObjects
{
    public class ClientsDAO
    {
        public static List<Client> RecoverClients()
        {
            List<Client> clientsList = new List<Client>();

            try
            {
                using (var context = new SFIDatabaseContext())
                {
                    var clients = (from client in context.clients
                                   join credit in context.credits
                                   on client.rfc equals credit.client_rfc into creditGroup
                                   from credit in creditGroup.DefaultIfEmpty()
                                   join credit_applications in context.credit_applications
                                   on client.rfc equals credit_applications.client_rfc into creditApplicationsGroup
                                   from credit_application in creditGroup.DefaultIfEmpty()
                                   select new
                                   {
                                       client,
                                       has_active_credit = credit != null,
                                       has_credit_application = credit_application != null
                                   }).ToList();

                    foreach (var item in clients)
                    {
                        Client clientItem = new Client
                        {
                            Curp = item.client.curp,
                            Rfc = item.client.rfc,
                            Birthdate = item.client.birthdate,
                            Name = item.client.name,
                            LastName = item.client.last_name,
                            Surname = item.client.surname,
                            Id_work_center = item.client.id_work_center,
                            Card_number = item.client.card_number,
                            Id_address = item.client.id_address,
                            Has_active_credit = item.has_active_credit,
                            Has_credit_application = item.has_credit_application
                        };
                        clientsList.Add(clientItem);
                    }
                }
            }
            catch (System.Data.Entity.Core.EntityException)
            {
                throw new FaultException<ServiceFault>(new ServiceFault("No fue posible recuperar los datos"));
            }
            catch (DbUpdateException)
            {
                throw new FaultException<ServiceFault>(new ServiceFault("No fue posible recuperar los datos"));
            }
            catch (DbEntityValidationException)
            {
                throw new FaultException<ServiceFault>(new ServiceFault("No fue posible recuperar los datos"));
            }

            return clientsList;
        }
    }
}
