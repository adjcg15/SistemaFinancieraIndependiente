﻿using SFIDataAccess.CustomExceptions;
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
    public partial class SFIService : ICreditsService
    {
        public List<CreditType> GetAllCreditTypes()
        {
            return CreditsDAO.GetAllCreditTypes();
        }

        public List<Credit> GetAllCredits()
        {
            return CreditsDAO.GetAllCredits();
        }

        public void RegisterCreditApplication(CreditApplication newApplication)
        {
            if(newApplication.CreditCondition == null)
            {
                throw new FaultException<ServiceFault>(
                    new ServiceFault("La condición de crédito que aplica a la solicitud es obligatoria")
                );
            }

            if(newApplication.Client == null)
            {
                throw new FaultException<ServiceFault>(
                    new ServiceFault("El cliente de la solicitud de crédito es obligatorio")
                );
            }

            if(newApplication.CreditType == null)
            {
                throw new FaultException<ServiceFault>(
                    new ServiceFault("El tipo de crédito de la solicitud de crédito es obligatorio")
                );
            } 

            if(newApplication.DigitalDocuments.Count != 3)
            {
                throw new FaultException<ServiceFault>(
                    new ServiceFault("Los tres documentos digitales de la solicitud son obligatorios: " +
                    "INE, comprobante de domicilio y comprobante de ingresos")
                );
            }

            CreditsDAO.RegisterCreditApplication(newApplication);
        }
    }
}