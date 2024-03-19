using SFIDataAccess.CustomExceptions;
using SFIDataAccess.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Core;
using System.Data.SqlClient;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace SFIDataAccess.DataAccessObjects
{
    public class CreditsDAO
    {
        public static List<CreditType> GetAllCreditTypes()
        {
            List<CreditType> creditTypes = new List<CreditType>();

            try
            {
                using (var context = new SFIDatabaseContext())
                {
                    context.credit_types.ToList().ForEach(storedCreditType =>
                    {
                        CreditType creditType = new CreditType();
                        creditType.Identifier = storedCreditType.id_credit_type;
                        creditType.Name = storedCreditType.name;

                        creditTypes.Add(creditType);
                    });
                }
            }
            catch (EntityException)
            {
                throw new FaultException<ServiceFault>(
                    new ServiceFault("No fue posible recuperar los tipos de crédito, intente más tarde"),
                    new FaultReason("Error")
                );
            }

            return creditTypes;
        }

        public static List<Credit> GetAllCredits()
        {
            List<Credit> allCredits = new List<Credit>();

            try
            {
                using (var context = new SFIDatabaseContext())
                {
                    context.credits
                        .OrderByDescending(credit => credit.approval_date)
                        .ToList()
                        .ForEach(storedCredit => {
                            Client creditOwner = new Client
                            {
                                Birthdate = storedCredit.client.birthdate,
                                Curp = storedCredit.client.curp,
                                Name = storedCredit.client.name,
                                LastName = storedCredit.client.last_name,
                                Surname = storedCredit.client.surname,
                                Rfc = storedCredit.client.rfc
                            };

                            credit_conditions storedApplicableCondition = storedCredit.regimes
                                .Where(regime => regime.application_end_date == null)
                                .FirstOrDefault()
                                .credit_conditions;
                            CreditCondition appliedCreditCondition = new CreditCondition
                            {
                                Identifier = storedApplicableCondition.identifier,
                                AdvancePaymentReduction = storedApplicableCondition.advance_payment_reduction,
                                InterestOnArrears = storedApplicableCondition.interest_on_arrears,
                                InterestRate = storedApplicableCondition.interest_rate,
                                IsActive = storedApplicableCondition.is_active,
                                IsIvaApplied = storedApplicableCondition.is_iva_applied,
                                PaymentMonths = storedApplicableCondition.payment_months
                            };

                            Credit credit = new Credit
                            {
                                AmountApproved = storedCredit.ammount_approved,
                                ApprovalDate = storedCredit.approval_date,
                                Invoice = storedCredit.invoice,
                                SettlementDate = storedCredit.settlement_date,
                                WithdrawalDate = storedCredit.withdrawal_date,
                                Client = creditOwner,
                                CreditCondition = appliedCreditCondition
                            };

                            allCredits.Add(credit);
                        });
                }
            }
            catch (EntityException)
            {
                throw new FaultException<ServiceFault>(
                    new ServiceFault("No fue posible recuperar los tipos de crédito, intente más tarde"),
                    new FaultReason("Error")
                );
            }

            return allCredits;
        }

        public static void RegisterCreditApplication(CreditApplication applicationInformation)
        {
            const string INVOICE_CHARACTERS = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

            Random rnd = new Random();
            string newApplicationInvoice = applicationInformation.Client.Rfc 
                + new string(Enumerable.Repeat(INVOICE_CHARACTERS, 5)
                    .Select(s => s[rnd.Next(s.Length)])
                    .ToArray());

            try
            {
                using (var context = new SFIDatabaseContext())
                {
                    var digitalDocumentsTable = new DataTable();
                    digitalDocumentsTable.Columns.Add("name", typeof(string));
                    digitalDocumentsTable.Columns.Add("format", typeof(string));
                    digitalDocumentsTable.Columns.Add("content", typeof(byte[]));

                    foreach (var document in applicationInformation.DigitalDocuments)
                    {
                        digitalDocumentsTable.Rows.Add(document.Name, document.Format, document.Content);
                    }

                    context.Database.ExecuteSqlCommand(
                        "EXEC RegisterCreditApplication @invoice, @expedition_date, " +
                        "@minimun_amount_accepted, @purpose, @requested_amount, @client_rfc, " +
                        "@employee_number, @credit_condition_identifier, " +
                        "@id_credit_type, @digital_documents",
                        new SqlParameter("@invoice", newApplicationInvoice),
                        new SqlParameter("@expedition_date", DateTime.Now),
                        new SqlParameter("@minimun_amount_accepted", applicationInformation.MinimumAmountAccepted),
                        new SqlParameter("@purpose", applicationInformation.Purpose),
                        new SqlParameter("@requested_amount", applicationInformation.RequestedAmount),
                        new SqlParameter("@client_rfc", applicationInformation.Client.Rfc),
                        new SqlParameter("@employee_number", applicationInformation.EmployeeNumber),
                        new SqlParameter("@credit_condition_identifier", applicationInformation.CreditCondition.Identifier),
                        new SqlParameter("@id_credit_type", applicationInformation.CreditType.Identifier),
                        new SqlParameter("@digital_documents", SqlDbType.Structured)
                        {
                            TypeName = "dbo.digital_document_type",
                            Value = digitalDocumentsTable
                        }
                    );
                }
            }
            catch (EntityException ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.ToString());
                throw new FaultException<ServiceFault>(
                    new ServiceFault("No fue guardar la información de la solicitud, intente más tarde"),
                    new FaultReason("Error")
                );
            }
        }
    }
}
