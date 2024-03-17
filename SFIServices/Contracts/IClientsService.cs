﻿using SFIDataAccess.CustomExceptions;
using SFIDataAccess.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace SFIServices.Contracts
{
    [ServiceContract]
    public interface IClientsService
    {
        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        List<Client> RecoverClients();

        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        BankAccount RecoverBankDetails(string cardNumber);
    }
}
