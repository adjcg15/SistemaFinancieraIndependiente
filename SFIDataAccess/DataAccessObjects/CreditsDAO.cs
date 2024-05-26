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
using System.Diagnostics;
using System.Linq;
using System.Runtime.Remoting.Contexts;
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

                            regime applicableRegime =
                                storedCredit.regimes
                                .Where(regime => regime.application_end_date == null)
                                .FirstOrDefault() ??
                                storedCredit.regimes
                                .Where(regime => regime.application_end_date != null)
                                .OrderByDescending(regime => regime.application_end_date)
                                .FirstOrDefault();

                            credit_conditions storedApplicableCondition = applicableRegime.credit_conditions;

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
                    var creditTypeInformation = (from creditType in context.credit_types
                                          where creditType.id_credit_type == creditConditionInformation.id_credit_type
                                          select creditType).FirstOrDefault();
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
                    CreditType creditTypeInfo = new CreditType
                    {
                        Identifier = creditTypeInformation.id_credit_type,
                        Name = creditTypeInformation.name
                    };
                    fullCreditApplication = new CreditApplication
                    {
                        Invoice = creditApplicationInformation.invoice,
                        MinimumAmountAccepted = creditApplicationInformation.minimun_amount_accepted,
                        RequestedAmount = creditApplicationInformation.requested_amount,
                        Purpose = creditApplicationInformation.purpose,
                        Client = clientInfo,
                        CreditCondition = creditConditionInfo,
                        CreditType = creditTypeInfo,
                        DigitalDocuments = digitalDocuments
                    };
                }
            }
            catch (EntityException)
            {
                throw new FaultException<ServiceFault>(new ServiceFault("No fue posble recuperar la solicitud de crédito del cliente, inténtelo de nuevo más tarde"));
            }
            catch (DbUpdateException)
            {
                throw new FaultException<ServiceFault>(new ServiceFault("No fue posble recuperar la solicitud de crédito del cliente, inténtelo de nuevo más tarde"));
            }
            catch (DbEntityValidationException)
            {
                throw new FaultException<ServiceFault>(new ServiceFault("No fue posble recuperar la solicitud de crédito del cliente, inténtelo de nuevo más tarde"));
            }
            catch (SqlException)
            {
                throw new FaultException<ServiceFault>(new ServiceFault("No fue posble recuperar la solicitud de crédito del cliente, inténtelo de nuevo más tarde"));
            }

            return fullCreditApplication;
        }

        public static bool GenerateApprovedDictum(List<CreditGrantingPolicy> allPolicies, List<CreditGrantingPolicy> policesThatApply, Dictum dictum, CreditApplication creditApplication, float amountApproved)
        {
            bool success = false;

            DataTable Dictum = new DataTable();
            Dictum.Columns.Add("credit_application_invoice", typeof(string));
            Dictum.Columns.Add("is_approved", typeof(bool));
            Dictum.Columns.Add("justification", typeof(string));
            Dictum.Columns.Add("generation_date", typeof(DateTime));
            Dictum.Columns.Add("employee_number", typeof(string));

            Dictum.Rows.Add(creditApplication.Invoice, dictum.IsApproved, dictum.Justification, dictum.GenerationDate, dictum.EmployeeNumber);

            DataTable CreditGrantingPolices = new DataTable();
            CreditGrantingPolices.Columns.Add("id_credit_granting_policy", typeof(int));
            CreditGrantingPolices.Columns.Add("is_applied", typeof(bool));

            foreach (var policy in allPolicies)
            {
                if (policesThatApply.Find(appliedPolicy => appliedPolicy.Title == policy.Title) != null)
                {
                    CreditGrantingPolices.Rows.Add(policy.Identifier, true);
                }
                else
                {
                    CreditGrantingPolices.Rows.Add(policy.Identifier, false);
                }
            }

            DataTable CreditCondition = new DataTable();
            CreditCondition.Columns.Add("interest_rate", typeof(float));
            CreditCondition.Columns.Add("is_iva_applied", typeof(bool));
            CreditCondition.Columns.Add("interest_on_arrears", typeof(float));
            CreditCondition.Columns.Add("advance_payment_reduction", typeof(float));
            CreditCondition.Columns.Add("payment_months", typeof(int));
            CreditCondition.Columns.Add("identifier", typeof(string));

            CreditCondition.Rows.Add(
                creditApplication.CreditCondition.InterestRate,
                creditApplication.CreditCondition.IsIvaApplied,
                creditApplication.CreditCondition.InterestOnArrears,
                creditApplication.CreditCondition.AdvancePaymentReduction,
                creditApplication.CreditCondition.PaymentMonths,
                creditApplication.CreditCondition.Identifier);

            try
            {
                using (var dbContext = new SFIDatabaseContext())
                {
                    var dictumTable = new SqlParameter("@Dictum", SqlDbType.Structured)
                    {
                        Value = Dictum,
                        TypeName = "Dictum"
                    };
                    var policesTable = new SqlParameter("@CreditGrantingPolices", SqlDbType.Structured)
                    {
                        Value = CreditGrantingPolices,
                        TypeName = "CreditGrantingPolices"
                    };
                    var creditConditionTable = new SqlParameter("@CreditCondition", SqlDbType.Structured)
                    {
                        Value = CreditCondition,
                        TypeName = "CreditCondition"
                    };
                    var amountApprovedValue = new SqlParameter("@AmountApproved", amountApproved);

                    var successParam = new SqlParameter("@Success", SqlDbType.Bit)
                    {
                        Direction = ParameterDirection.Output
                    };

                    dbContext.Database.ExecuteSqlCommand("EXEC GenerateApprovedDictum @Dictum, @CreditGrantingPolices, " +
                        "@CreditCondition, @AmountApproved, @Success OUTPUT", dictumTable, policesTable, creditConditionTable, amountApprovedValue, successParam);

                    success = (bool)successParam.Value;
                }
            }
            catch (EntityException)
            {
                throw new FaultException<ServiceFault>(new ServiceFault("No fue posible generar el dictamen de autorización de crédito para el cliente, inténtelo de nuevo más tarde"));
            }
            catch (DbUpdateException)
            {
                throw new FaultException<ServiceFault>(new ServiceFault("No fue posible generar el dictamen de autorización de crédito para el cliente, inténtelo de nuevo más tarde"));
            }
            catch (DbEntityValidationException)
            {
                throw new FaultException<ServiceFault>(new ServiceFault("No fue posible generar el dictamen de autorización de crédito para el cliente, inténtelo de nuevo más tarde"));
            }
            catch (SqlException)
            {
                throw new FaultException<ServiceFault>(new ServiceFault("No fue posible generar el dictamen de autorización de crédito para el cliente, inténtelo de nuevo más tarde"));
            }

            return success;
        }

        public static bool GenerateRejectedDictum(Dictum dictum, CreditApplication creditApplication)
        {
            bool success = false;

            try
            {
                DataTable Dictum = new DataTable();
                Dictum.Columns.Add("credit_application_invoice", typeof(string));
                Dictum.Columns.Add("is_approved", typeof(bool));
                Dictum.Columns.Add("justification", typeof(string));
                Dictum.Columns.Add("generation_date", typeof(DateTime));
                Dictum.Columns.Add("employee_number", typeof(string));

                Dictum.Rows.Add(creditApplication.Invoice, dictum.IsApproved, dictum.Justification, dictum.GenerationDate, dictum.EmployeeNumber);

                using (var dbContext = new SFIDatabaseContext())
                {
                    var dictumTable = new SqlParameter("@Dictum", SqlDbType.Structured)
                    {
                        Value = Dictum,
                        TypeName = "Dictum"
                    };

                    var successParam = new SqlParameter("@Success", SqlDbType.Bit)
                    {
                        Direction = ParameterDirection.Output
                    };

                    dbContext.Database.ExecuteSqlCommand("EXEC GenerateRejectedDictum @Dictum, @Success OUTPUT", dictumTable, successParam);

                    success = (bool)successParam.Value;
                }
            }
            catch (EntityException)
            {
                throw new FaultException<ServiceFault>(new ServiceFault("No fue posible generar el dictamen de autorización de crédito para el cliente, inténtelo de nuevo más tarde"));
            }
            catch (DbUpdateException)
            {
                throw new FaultException<ServiceFault>(new ServiceFault("No fue posible generar el dictamen de autorización de crédito para el cliente, inténtelo de nuevo más tarde"));
            }
            catch (DbEntityValidationException)
            {
                throw new FaultException<ServiceFault>(new ServiceFault("No fue posible generar el dictamen de autorización de crédito para el cliente, inténtelo de nuevo más tarde"));
            }
            catch (SqlException)
            {
                throw new FaultException<ServiceFault>(new ServiceFault("No fue posible generar el dictamen de autorización de crédito para el cliente, inténtelo de nuevo más tarde"));
            }

            return success;
        }
        public static int GetCreditTypeIdByCreditInvoice(string creditInvoice)
        {
            int creditTypeId = 0;

            try
            {
                using (var context = new SFIDatabaseContext())
                {
                    var result = context.credits
                        .Where(credit => credit.invoice == creditInvoice)
                        .Select(credit => credit.id_credit_type)
                        .FirstOrDefault();

                    if (result != 0)
                    {
                        creditTypeId = result;
                    }
                }
            }
            catch (EntityException)
            {
                throw new FaultException<ServiceFault>(new ServiceFault("No fue posible generar el dictamen de autorización de crédito para el cliente, inténtelo de nuevo más tarde"));
            }
            catch (DbUpdateException)
            {
                throw new FaultException<ServiceFault>(new ServiceFault("No fue posible generar el dictamen de autorización de crédito para el cliente, inténtelo de nuevo más tarde"));
            }
            catch (DbEntityValidationException)
            {
                throw new FaultException<ServiceFault>(new ServiceFault("No fue posible generar el dictamen de autorización de crédito para el cliente, inténtelo de nuevo más tarde"));
            }
            catch (SqlException)
            {
                throw new FaultException<ServiceFault>(new ServiceFault("No fue posible generar el dictamen de autorización de crédito para el cliente, inténtelo de nuevo más tarde"));
            }

            return creditTypeId;
        }
        public static bool AssociateNewCreditCondition(string creditInvoice, string newCreditConditionIdentifier)
        {
            bool success = false;

            try
            {
                using (var dbContext = new SFIDatabaseContext())
                {
                    var creditInvoiceParam = new SqlParameter("@CreditInvoice", SqlDbType.VarChar, 18)
                    {
                        Value = creditInvoice
                    };

                    var newCreditConditionIdentifierParam = new SqlParameter("@NewCreditConditionIdentifier", SqlDbType.VarChar, 6)
                    {
                        Value = newCreditConditionIdentifier
                    };

                    var successParam = new SqlParameter("@Success", SqlDbType.Bit)
                    {
                        Direction = ParameterDirection.Output
                    };

                    dbContext.Database.ExecuteSqlCommand("EXEC AssociateNewCreditCondition @CreditInvoice, @NewCreditConditionIdentifier, @Success OUTPUT",
                                                         creditInvoiceParam, newCreditConditionIdentifierParam, successParam);

                    success = (bool)successParam.Value;
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

            return success;
        }
        public static bool VerifyFirstPaymentReconciled(string creditInvoice)
        {
            try
            {
                using (var context = new SFIDatabaseContext())
                {
                    var firstPayment = context.payments
                        .Where(p => p.credit_invoice == creditInvoice)
                        .OrderBy(p => p.planned_date)
                        .FirstOrDefault();
                    return firstPayment != null && firstPayment.reconciliation_date != null;
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
        }

        public static List<Payment> GetPaymentsByCreditInvoice(string creditInvoice)
        {
            List<Payment> payments = new List<Payment>();

            try
            {
                using (var context = new SFIDatabaseContext())
                {
                    var dbPayments = context.payments
                        .Where(payment => payment.credit_invoice == creditInvoice)
                        .OrderBy(payment => payment.planned_date)
                        .ToList();

                    var dbAppliedCreditCondition = context.regimes
                        .Where(regime => regime.credit_invoice == creditInvoice && regime.application_end_date == null)
                        .FirstOrDefault()?
                        .credit_conditions;

                    foreach (var payment in dbPayments)
                    {
                        decimal paymentInterest = 0;
                        if(payment.reconciliation_date.HasValue)
                        {
                            TimeSpan paymentPeriod = payment.reconciliation_date.Value - payment.planned_date;
                            int days = paymentPeriod.Days;

                            if (days < 0)
                            {
                                paymentInterest = (decimal)(dbAppliedCreditCondition?.advance_payment_reduction * days);
                            }
                            else if (days > 0)
                            {
                                paymentInterest = (decimal)(dbAppliedCreditCondition?.interest_on_arrears * days);
                            }
                        }

                        payments.Add(new Payment
                        {
                            amount = (double)payment.amount,
                            invoice = payment.invoice,
                            planned_date = payment.planned_date,
                            credit_invoice = payment.credit_invoice,
                            reconciliation_date = payment.reconciliation_date,
                            Interest = paymentInterest * 100
                        });
                    }
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
            return payments;
        }

        public static Payment GetPaymentByInvoice(string invoice)
        {
            try
            {
                using (var context = new SFIDatabaseContext())
                {
                    var paymentEntity = context.payments.FirstOrDefault(p => p.invoice == invoice);

                    if (paymentEntity != null)
                    {
                        var payment = new Payment
                        {
                            amount = (double)paymentEntity.amount,
                            invoice = paymentEntity.invoice,
                            planned_date = paymentEntity.planned_date,
                            credit_invoice = paymentEntity.credit_invoice,
                            reconciliation_date = paymentEntity.reconciliation_date
                        };

                        return payment;
                    }
                    else
                    {
                        return null;
                    }
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
        }

        public static decimal ClosePayment(string invoice)
        {
            decimal interest;
            try
            {
                using (var dbContext = new SFIDatabaseContext())
                {
                    var interestPercentage = new SqlParameter("@interest_percentage", SqlDbType.Decimal);
                    interestPercentage.Direction = ParameterDirection.Output;

                    dbContext.Database.ExecuteSqlCommand("EXEC ClosePayment @payment_invoice, @interest_percentage OUTPUT",
                        new SqlParameter("@payment_invoice", invoice),
                        interestPercentage)
                    ;
                    interest = (decimal)interestPercentage.Value;
                }
            }
            catch (EntityException)
            {
                throw new FaultException<ServiceFault>(new ServiceFault("No fue posible ejecutar el procedimiento"), new FaultReason("Error de entidad"));
            }
            catch (DbUpdateException)
            {
                throw new FaultException<ServiceFault>(new ServiceFault("No fue posible ejecutar el procedimiento"), new FaultReason("Error de actualización de base de datos"));
            }
            catch (DbEntityValidationException)
            {
                throw new FaultException<ServiceFault>(new ServiceFault("No fue posible ejecutar el procedimiento"), new FaultReason("Error de validación de entidad"));
            }
            return interest;
        }

        public static void InsertIntoPaymentLayouts(string captureLine, Payment payment)
        {
            try
            {
                using (var context = new SFIDatabaseContext())
                {
                    var paymentRecord = context.payments.FirstOrDefault(p => p.invoice == payment.invoice);

                    if (paymentRecord != null)
                    {
                        var existingLayout = context.payment_layouts.FirstOrDefault(l => l.id_payment == paymentRecord.id_payment);
                        if (existingLayout != null)
                        {
                            return;
                        }
                        var newLayout = new payment_layouts
                        {
                            capture_line = captureLine,
                            generation_date = DateTime.Now,
                            id_payment = paymentRecord.id_payment
                        };
                        context.payment_layouts.Add(newLayout);
                        context.SaveChanges();
                    }
                    else
                    {
                        throw new InvalidOperationException("No se encontró el pago en la base de datos.");
                    }
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
        }

        public static PaymentLayout GetPaymentLayoutByPaymentId(int paymentId)
        {
            SFIDatabaseContext context = new SFIDatabaseContext();
            try
            {
                var paymentLayout = context.payment_layouts
                    .Where(p => p.id_payment == paymentId)
                    .FirstOrDefault();

                if (paymentLayout != null)
                {
                    return new PaymentLayout
                    {
                        capture_line = paymentLayout.capture_line,
                        generation_date = paymentLayout.generation_date,
                        id_payment = paymentLayout.id_payment
                    };
                }
                else
                {
                    return null;
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
        }

        public static List<Payment> GetAllPaymentsSortedByPlannedDate()
        {
            try
            {
                using (var context = new SFIDatabaseContext())
                {
                    var payments = context.payments
                        .OrderBy(p => p.planned_date)
                        .ToList();
                    return payments.Select(p => new Payment
                    {
                        id = p.id_payment,
                        amount = (double)p.amount,
                        invoice = p.invoice,
                        planned_date = p.planned_date,
                        credit_invoice = p.credit_invoice,
                        reconciliation_date = p.reconciliation_date
                    }).ToList();
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
        }

        public static List<Credit> RecoverCreditsWithPaymentsInTheMonthAndYear(int month, int year)
        {
            List<Credit> creditsWithPaymentsList = new List<Credit>();
            try
            {
                using (var context = new SFIDatabaseContext())
                {
                    var credits = context.credits
                        .Join(context.payments,
                            credit => credit.invoice,
                            payment => payment.credit_invoice,
                            (credit, payment) => new { Credit = credit, Payment = payment })
                        .Where(cp => cp.Payment.planned_date.Year == year && cp.Payment.planned_date.Month == month)
                        .ToList();

                    if (credits != null)
                    {
                        foreach (var credit in credits)
                        {
                            Payment[] payment = new Payment[1];
                            payment[0] = new Payment
                            {
                                planned_date = credit.Payment.planned_date,
                                reconciliation_date = credit.Payment.reconciliation_date,
                                amount = (double)credit.Payment.amount
                            };

                            Credit creditWithPayment = new Credit
                            {
                                Invoice = credit.Credit.invoice,
                                AmountApproved = credit.Credit.ammount_approved,
                                Payments = payment
                            };
                            creditsWithPaymentsList.Add(creditWithPayment);
                        }
                    }
                }
            }
            catch (EntityException)
            {
                throw new FaultException<ServiceFault>(new ServiceFault("No fue posible recuperar la información para la eficiencia de corbo, inténtelo de nuevo más tarde"));
            }
            catch (DbUpdateException)
            {
                throw new FaultException<ServiceFault>(new ServiceFault("No fue posible recuperar la información para la eficiencia de corbo, inténtelo de nuevo más tarde"));
            }
            catch (DbEntityValidationException)
            {
                throw new FaultException<ServiceFault>(new ServiceFault("No fue posible recuperar la información para la eficiencia de corbo, inténtelo de nuevo más tarde"));
            }
            catch (SqlException)
            {
                throw new FaultException<ServiceFault>(new ServiceFault("No fue posible recuperar la información para la eficiencia de corbo, inténtelo de nuevo más tarde"));
            }

            return creditsWithPaymentsList;
        }

        public static Credit GetCreditForCollectionEfficiency(string invoice)
        {
            Credit credit = null;

            try
            {
                using (var context = new SFIDatabaseContext())
                {
                    var storedCredit = context.credits
                        .FirstOrDefault(dbCredit => dbCredit.invoice == invoice);

                    if(storedCredit != null)
                    {
                        var storedCreditClient = storedCredit.client;
                        var storedCreditPayments = storedCredit.payments;

                        credit = new Credit() { 
                            Invoice = storedCredit.invoice,
                            AmountApproved = storedCredit.ammount_approved
                        };

                        if(storedCreditClient != null)
                        {
                            credit.Client = new Client()
                            {
                                Rfc = storedCreditClient.rfc,
                                Curp = storedCreditClient.curp,
                                Name = storedCreditClient.name,
                                LastName = storedCreditClient.last_name,
                                Surname = storedCreditClient.surname
                            };
                        }

                        if(storedCreditPayments != null)
                        {
                            credit.Payments = storedCreditPayments
                                .Select(p => new Payment()
                                {
                                    id = p.id_payment,
                                    amount = (double)p.amount,
                                    planned_date = p.planned_date,
                                    reconciliation_date = p.reconciliation_date
                                })
                                .ToArray();
                        }
                    }
                }
            }
            catch (EntityException)
            {
                throw new FaultException<ServiceFault>(
                    new ServiceFault("No fue posible recuperar la información para " +
                        "generar la eficiencia mensual, intente más tarde"),
                    new FaultReason("Error")
                );
            }
            catch (SqlException)
            {
                throw new FaultException<ServiceFault>(
                    new ServiceFault("No fue posible recuperar la información para " +
                        "generar la eficiencia mensual, intente más tarde"),
                    new FaultReason("Error")
                );
            }

            return credit;
        }
    }
}
