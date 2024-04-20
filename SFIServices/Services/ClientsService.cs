
using SFIDataAccess.CustomExceptions;
using SFIDataAccess.DataAccessObjects;
using SFIDataAccess.Model;
using SFIServices.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace SFIServices
{
    public partial class SFIService : IClientsService
    {
        public List<Client> RecoverClients()
        {
            return ClientsDAO.RecoverClients();
        }

        public BankAccount RecoverBankDetails(string cardNumber)
        {
            return ClientsDAO.RecoverBankDetails(cardNumber);
        }

        public bool UpdateBankAccount(BankAccount bankAccount, string cardNumber)
        {
            return ClientsDAO.UpdateBankAccount(bankAccount, cardNumber);
        }

        public bool RegisterClient(Client client)
        {
            return ClientsDAO.RegisterClient(client);
        }

        public List<PersonalReference> RecoverPersonalReferences(string rfc)
        {
            return ClientsDAO.RecoverPersonalReferences(rfc);
        }

        public bool UpdatePersonalReference(PersonalReference personalReference, string currenIneKey)
        {
            return ClientsDAO.UpdatePersonalReference(personalReference, currenIneKey);
        }
    }
}
