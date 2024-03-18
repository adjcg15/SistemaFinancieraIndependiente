using SFIDataAccess.CustomExceptions;
using SFIDataAccess.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Data.SqlClient;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace SFIDataAccess.DataAccessObjects
{
    public static class ClientsDAO
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
                throw new FaultException<ServiceFault>(new ServiceFault("No fue posible recuperar los datos"), new FaultReason("Error"));
            }
            catch (DbUpdateException)
            {
                throw new FaultException<ServiceFault>(new ServiceFault("No fue posible recuperar los datos"), new FaultReason("Error"));
            }
            catch (DbEntityValidationException)
            {
                throw new FaultException<ServiceFault>(new ServiceFault("No fue posible recuperar los datos"), new FaultReason("Error"));
            }

            return clientsList;
        }

        public static BankAccount RecoverBankDetails(string cardNumber)
        {
            BankAccount bankAccount = new BankAccount();

            try
            {
                using (var context = new SFIDatabaseContext())
                {
                    var account = (from bankaccount in context.bank_accounts
                                   where bankaccount.card_number == cardNumber
                                   select bankaccount).FirstOrDefault();
                    if (account != null)
                    {
                        bankAccount.Card_number = account.card_number;
                        bankAccount.Bank = account.bank;
                        bankAccount.Holder = account.holder;
                    }
                }
            }
            catch (System.Data.Entity.Core.EntityException)
            {
                throw new FaultException<ServiceFault>(new ServiceFault("No fue posible recuperar los datos"), new FaultReason("Error"));
            }
            catch (DbUpdateException)
            {
                throw new FaultException<ServiceFault>(new ServiceFault("No fue posible recuperar los datos"), new FaultReason("Error"));
            }
            catch (DbEntityValidationException)
            {
                throw new FaultException<ServiceFault>(new ServiceFault("No fue posible recuperar los datos"), new FaultReason("Error"));
            }

            return bankAccount;
        }

        public static bool UpdateBankAccount(BankAccount bankAccount, string cardNumber)
        {
            bool success;
            try
            {
            using (var dbContext = new SFIDatabaseContext())
            {
                var currentCardNumberParam = new SqlParameter("@CurrentCardNumber", cardNumber);
                var newBankParam = new SqlParameter("@NewBank", bankAccount.Bank);
                var newHolderParam = new SqlParameter("@NewHolder", bankAccount.Holder);
                var newCardNumberParam = new SqlParameter("@NewCardNumber", bankAccount.Card_number);

                var successParam = new SqlParameter("@Success", SqlDbType.Bit);
                successParam.Direction = ParameterDirection.Output;

                dbContext.Database.ExecuteSqlCommand(
                    "EXEC UpdateBankAccount @CurrentCardNumber, @NewBank, @NewHolder, @NewCardNumber, @Success OUTPUT",
                    currentCardNumberParam, newBankParam, newHolderParam, newCardNumberParam, successParam);

                success = (bool)successParam.Value;
            }
            }
            catch (System.Data.Entity.Core.EntityException)
            {
                throw new FaultException<ServiceFault>(new ServiceFault("No fue posible recuperar los datos"), new FaultReason("Error"));
            }
            catch (DbUpdateException)
            {
                throw new FaultException<ServiceFault>(new ServiceFault("No fue posible recuperar los datos"), new FaultReason("Error"));
            }
            catch (DbEntityValidationException)
            {
                throw new FaultException<ServiceFault>(new ServiceFault("No fue posible recuperar los datos"), new FaultReason("Error"));
            }

            return success;
        }
    }
}
