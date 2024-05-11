using SFIDataAccess;
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

        public List<CreditApplication> GetAllCreditApplications()
        {
            return CreditsDAO.GetAllCreditApplications();
        }

        public CreditApplication RecoverCreditApplication(string invoice)
        {
            return CreditsDAO.RecoverCreditApplication(invoice);
        }

        public bool GenerateApprovedDictum(List<CreditGrantingPolicy> allPolicies, List<CreditGrantingPolicy> policesThatApply, Dictum dictum, CreditApplication creditApplication, float amountApproved)
        {
            return CreditsDAO.GenerateApprovedDictum(allPolicies, policesThatApply, dictum, creditApplication, amountApproved);
        }

        public bool GenerateRejectedDictum(Dictum dictum, CreditApplication creditApplication)
        {
            return CreditsDAO.GenerateRejectedDictum(dictum, creditApplication);
        }
        public int GetCreditTypeIdByCreditInvoice(string creditInvoice)
        {
            return CreditsDAO.GetCreditTypeIdByCreditInvoice(creditInvoice);
        }
        public bool AssociateNewCreditCondition(string creditInvoice, string newCreditConditionIdentifier)
        {
            return CreditsDAO.AssociateNewCreditCondition(creditInvoice, newCreditConditionIdentifier);
        }

        public bool VerifyFirstPaymentReconciled(string creditInvoice)
        {
            return CreditsDAO.VerifyFirstPaymentReconciled(creditInvoice);
        }

        public  List<Payment> GetPaymentsByCreditInvoice(string creditInvoice)
        {
            return CreditsDAO.GetPaymentsByCreditInvoice(creditInvoice);
        }

        public Payment GetPaymentByInvoice(string invoice)
        {
            return CreditsDAO.GetPaymentByInvoice(invoice);
        }

        public decimal ClosePayment(string invoice)
        {
            return CreditsDAO.ClosePayment(invoice);
        }

        public void InsertIntoPaymentLayouts(string captureLine, Payment payment)
        {
            CreditsDAO.InsertIntoPaymentLayouts(captureLine, payment);
        }

        public PaymentLayout GetPaymentLayoutByPaymentId(int paymentId)
        {
            return CreditsDAO.GetPaymentLayoutByPaymentId(paymentId);
        }

        public List<Payment> GetAllPaymentsSortedByPlannedDate()
        {
            return CreditsDAO.GetAllPaymentsSortedByPlannedDate();
        }

        public List<Credit> RecoverCreditsWithPaymentsInTheMonthAndYear(int month, int year)
        {
            return CreditsDAO.RecoverCreditsWithPaymentsInTheMonthAndYear(month, year);
        }
    }
}
