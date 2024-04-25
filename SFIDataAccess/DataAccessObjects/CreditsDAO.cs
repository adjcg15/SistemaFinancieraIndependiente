using SFIDataAccess.CustomExceptions;
using SFIDataAccess.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Core;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
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
            catch (SqlException)
            {
                throw new FaultException<ServiceFault>(
                    new ServiceFault("No fue posible recuperar los tipos de crédito, intente más tarde"),
                    new FaultReason("Error")
                );
            }

            return allCredits;
        }

        public static List<CreditApplication> GetAllCreditApplications()
        {
            List<CreditApplication> allCreditApplications = new List<CreditApplication>();

            try
            {
                using (var context = new SFIDatabaseContext())
                {
                    context.credit_applications
                        .OrderByDescending(creditApplication => creditApplication.expedition_date)
                        .ToList()
                        .ForEach(storedCreditApplication => {
                            CreditType creditType = new CreditType
                            {
                                Identifier = storedCreditApplication.credit_types.id_credit_type,
                                Name = storedCreditApplication.credit_types.name
                            };

                            dictum storedDictum = context.dictums
                                .Where(dictum => dictum.credit_application_invoice == storedCreditApplication.invoice)
                                .FirstOrDefault();

                            Dictum associatedDictum = null;
                            if (storedDictum != null)
                            {
                                associatedDictum = new Dictum
                                {
                                    GenerationDate = storedDictum.generation_date,
                                    IsApproved = storedDictum.is_approved
                                };
                            }

                            CreditApplication creditApplication = new CreditApplication
                            {
                                Invoice = storedCreditApplication.invoice,
                                ExpeditionDate = storedCreditApplication.expedition_date,
                                RequestedAmount = storedCreditApplication.requested_amount,
                                CreditType = creditType,
                                Dictum = associatedDictum
                            };

                            allCreditApplications.Add(creditApplication);
                        });
                }
            }
            catch (EntityException)
            {
                throw new FaultException<ServiceFault>(
                    new ServiceFault("Servidor no disponible. No fue posible recuperar las solicitudes " +
                        "de crédito de los clientes, intente más tarde"),
                    new FaultReason("Error")
                );
            }
            catch (SqlException)
            {
                throw new FaultException<ServiceFault>(
                    new ServiceFault("Servidor no disponible. No fue posible recuperar las solicitudes " +
                        "de crédito de los clientes, intente más tarde"),
                    new FaultReason("Error")
                );
            }

            return allCreditApplications;
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

        public static CreditApplication RecoverCreditApplication(string invoice)
        {
            CreditApplication fullCreditApplication = null;

            try
            {
                using (var context = new SFIDatabaseContext())
                {
                    var creditApplicationInformation = (from creditApplication in context.credit_applications
                                                        where creditApplication.invoice == invoice
                                                        select creditApplication).FirstOrDefault();
                    var digitalDocumentsList = (from digitalDocument in context.digital_documents
                                               where digitalDocument.credit_application_invoice == invoice
                                               select digitalDocument).ToList();
                    var clientInformation = (from client in context.clients
                                             where client.rfc == creditApplicationInformation.client_rfc
                                             select client).FirstOrDefault();
                    var clientAddress = (from address in context.addresses
                                         where address.id_address == clientInformation.id_address
                                         select address).FirstOrDefault();
                    var phoneNumberType = (from contactMethodType in context.contact_method_types
                                           where contactMethodType.name == "PhoneNumber"
                                           select contactMethodType.id_contact_method_type).FirstOrDefault();
                    var clientPhoneNumbers = (from contactMethod in context.contact_methods
                                              where contactMethod.client_rfc == creditApplicationInformation.client_rfc &&
                                              contactMethod.id_contact_method_type == phoneNumberType
                                              select contactMethod).ToList();
                    var emailType = (from contactMethodType in context.contact_method_types
                                     where contactMethodType.name == "Email"
                                     select contactMethodType.id_contact_method_type).FirstOrDefault();
                    var clientEmails = (from contactMethod in context.contact_methods
                                        where contactMethod.client_rfc == creditApplicationInformation.client_rfc &&
                                        contactMethod.id_contact_method_type == emailType
                                        select contactMethod).ToList();
                    var workCenterInformation = (from workCenter in context.work_centers
                                                 where workCenter.id_work_center == clientInformation.id_work_center
                                                 select workCenter).FirstOrDefault();
                    var creditConditionInformation = (from creditCondition in context.credit_conditions
                                                      where creditCondition.identifier == creditApplicationInformation.credit_condition_identifier
                                                      select creditCondition).FirstOrDefault();
                    Address addressClient = new Address
                    {
                        Street = clientAddress.street,
                        City = clientAddress.city,
                        Neighborhod = clientAddress.neighborhod,
                        Municipality = clientAddress.municipality,
                        InteriorNumber = clientAddress.inteior_number,
                        OutdoorNumber = clientAddress.outdoor_number,
                        PostCode = clientAddress.post_code,
                        State = clientAddress.state
                    };

                    WorkCenter clientWorkCenter = new WorkCenter
                    {
                        CompanyName = workCenterInformation.company_name,
                        PhoneNumber = workCenterInformation.phone_number,
                        EmployeePosition = workCenterInformation.employee_position,
                        Salary = workCenterInformation.salary,
                        EmployeeSeniority = workCenterInformation.employee_seniority,
                        HumanResourcesPhone = workCenterInformation.human_resources_phone
                    };

                    List<ContacMethod> contactMethods = new List<ContacMethod>();
                    foreach (var phoneNumber in clientPhoneNumbers)
                    {
                        ContacMethod contactMethod = new ContacMethod
                        {
                            Value = phoneNumber.value,
                            MethodType = "PhoneNumber"
                        };
                        contactMethods.Add(contactMethod);
                    }
                    foreach (var email in clientEmails)
                    {
                        ContacMethod contactMethod = new ContacMethod
                        {
                            Value = email.value,
                            MethodType = "Email"
                        };
                        contactMethods.Add(contactMethod);
                    }
                    Client clientInfo = new Client
                    {
                        Rfc = clientInformation.rfc,
                        Curp = clientInformation.curp,
                        Birthdate = clientInformation.birthdate,
                        Name = clientInformation.name,
                        LastName = clientInformation.last_name,
                        Surname = clientInformation.surname,
                        Address = addressClient,
                        WorkCenter = clientWorkCenter,
                        ContacMethods = contactMethods
                    };
                    List<DigitalDocument> digitalDocuments = new List<DigitalDocument> ();
                    foreach (var document in digitalDocumentsList)
                    {
                        DigitalDocument digitalDocument = new DigitalDocument
                        {
                            Name = document.name,
                            Format = document.format,
                            Content = document.content
                        };
                        digitalDocuments.Add(digitalDocument);
                    }
                    CreditCondition creditConditionInfo = new CreditCondition
                    {
                        AdvancePaymentReduction = creditConditionInformation.advance_payment_reduction,
                        InterestOnArrears = creditConditionInformation.interest_on_arrears,
                        IsActive = creditConditionInformation.is_active,
                        IsIvaApplied = creditConditionInformation.is_iva_applied,
                        Identifier = creditConditionInformation.identifier,
                        InterestRate = creditConditionInformation.interest_rate,
                        PaymentMonths = creditConditionInformation.payment_months
                    };
                    fullCreditApplication = new CreditApplication
                    {
                        Invoice = creditApplicationInformation.invoice,
                        MinimumAmountAccepted = creditApplicationInformation.minimun_amount_accepted,
                        RequestedAmount = creditApplicationInformation.requested_amount,
                        Client = clientInfo,
                        CreditCondition = creditConditionInfo
                    };
                }
            }
            catch (EntityException)
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

            return fullCreditApplication;
        }
    }
}