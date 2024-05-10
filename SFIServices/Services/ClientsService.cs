
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

        public Client RecoverClient(string clientRfc)
        {
            return ClientsDAO.RecoverClient(clientRfc);
        }

        public Client GetClientPersonalInformation(string clientRFC)
        {
            return ClientsDAO.GetClientPersonalInformation(clientRFC);
        }

        public bool UpdateClientPersonalInformation(Client client)
        {
            return ClientsDAO.UpdateClientPersonalInformation(client);
        }
        public Client GetWorkCenterInformation(string clientRFC)
        {
            return ClientsDAO.GetWorkCenterInformation(clientRFC);
        }
        public bool UpdateClientWorkCenterlInformation(Client client)
        {
            return ClientsDAO.UpdateClientWorkCenterlInformation(client);
        }
    }
}
