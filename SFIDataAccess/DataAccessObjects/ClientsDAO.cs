
using SFIDataAccess.CustomExceptions;
using SFIDataAccess.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Core;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Security.Cryptography;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SFIDataAccess.DataAccessObjects
{
    public static class ClientsDAO
    {
        public static List<Client> RecoverClients()
        {
            List<Client> clientsList = new List<Client>();
            WorkCenter workCenter = new WorkCenter();

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
                                   from credit_application in creditApplicationsGroup.DefaultIfEmpty()
                                   join work_center in context.work_centers
                                   on client.id_work_center equals work_center.id_work_center
                                   join credit_application_state in context.credit_applications_state
                                   on credit_application.id_credit_application_state equals credit_application_state.id_credit_application_state into creditApplicationStateGroup
                                   from credit_application_state in creditApplicationStateGroup.DefaultIfEmpty()
                                   select new
                                   {
                                       client,
                                       credit,
                                       work_center,
                                       has_credit_application = creditApplicationStateGroup.Any(ca => ca.name == "CREADA"),
                                       has_active_credit = credit != null && credit.settlement_date == null
                                   })
                                   .GroupBy(result => result.client.rfc)
                                   .Select(group => new
                                   {
                                       group.FirstOrDefault().client,
                                       has_credit_application = group.Any(g => g.has_credit_application),
                                       has_active_credit = group.Any(g => g.has_active_credit),
                                       WorkCenter = group.FirstOrDefault().work_center
                                   })
                                   .ToList();



                    foreach (var item in clients)
                    {
                        workCenter.CompanyName = item.client.work_centers.company_name;
                        workCenter.Salary = item.client.work_centers.salary;
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
                            Has_credit_application = item.has_credit_application,
                            WorkCenter = workCenter
                        };
                        clientsList.Add(clientItem);
                    }
                }
            }
            catch (EntityException)
            {
                throw new FaultException<ServiceFault>(new ServiceFault("No fue posible recuperar los clientes, inténtelo de nuevo más tarde"));
            }
            catch (DbUpdateException)
            {
                throw new FaultException<ServiceFault>(new ServiceFault("No fue posible recuperar los clientes, inténtelo de nuevo más tarde"));
            }
            catch (DbEntityValidationException)
            {
                throw new FaultException<ServiceFault>(new ServiceFault("No fue posible recuperar los clientes, inténtelo de nuevo más tarde"));
            }
            catch (SqlException)
            {
                throw new FaultException<ServiceFault>(new ServiceFault("No fue posible recuperar los clientes, inténtelo de nuevo más tarde"));
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
                        bankAccount.CardNumber = account.card_number;
                        bankAccount.Bank = account.bank;
                        bankAccount.Holder = account.holder;
                    }
                }
            }
            catch (EntityException)
            {
                throw new FaultException<ServiceFault>(new ServiceFault("No fue posible recuperar la cuenta bancaria del cliente, inténtelo de nuevo más tarde"));
            }
            catch (DbUpdateException)
            {
                throw new FaultException<ServiceFault>(new ServiceFault("No fue posible recuperar la cuenta bancaria del cliente, inténtelo de nuevo más tarde"));
            }
            catch (DbEntityValidationException)
            {
                throw new FaultException<ServiceFault>(new ServiceFault("No fue posible recuperar la cuenta bancaria del cliente, inténtelo de nuevo más tarde"));
            }
            catch (SqlException)
            {
                throw new FaultException<ServiceFault>(new ServiceFault("No fue posible recuperar la cuenta bancaria del cliente, inténtelo de nuevo más tarde"));
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
                    var newCardNumberParam = new SqlParameter("@NewCardNumber", bankAccount.CardNumber);

                    var successParam = new SqlParameter("@Success", SqlDbType.Bit);
                    successParam.Direction = ParameterDirection.Output;

                    dbContext.Database.ExecuteSqlCommand(
                        "EXEC UpdateBankAccount @CurrentCardNumber, @NewBank, @NewHolder, @NewCardNumber, @Success OUTPUT",
                        currentCardNumberParam, newBankParam, newHolderParam, newCardNumberParam, successParam);

                    success = (bool)successParam.Value;
                }
            }
            catch (EntityException)
            {
                throw new FaultException<ServiceFault>(new ServiceFault("No fue posible actualizar la cuenta bancaria del cliente, inténtelo de nuevo más tarde"));
            }
            catch (DbUpdateException)
            {
                throw new FaultException<ServiceFault>(new ServiceFault("No fue posible actualizar la cuenta bancaria del cliente, inténtelo de nuevo más tarde"));
            }
            catch (DbEntityValidationException)
            {
                throw new FaultException<ServiceFault>(new ServiceFault("No fue posible actualizar la cuenta bancaria del cliente, inténtelo de nuevo más tarde"));
            }
            catch (SqlException)
            {
                throw new FaultException<ServiceFault>(new ServiceFault("No fue posible actualizar la cuenta bancaria del cliente, inténtelo de nuevo más tarde"));
            }

            return success;
        }

        public static bool RegisterClient(Client client)
        {
            bool success;
            try
            {
                using (var dbContext = new SFIDatabaseContext())
                {
                    var clientName = new SqlParameter("@ClientName", client.Name);
                    var clientSurname = new SqlParameter("@ClientSurname", client.Surname);
                    var clientLastName = new SqlParameter("@ClientLastName", client.LastName);
                    var clientBirthdate = new SqlParameter("@ClientBirthdate", client.Birthdate);
                    var clientCurp = new SqlParameter("@ClientCurp", client.Curp);
                    var clientRfc = new SqlParameter("@ClientRfc", client.Rfc);
                    var clientStreet = new SqlParameter("@ClientStreet", client.Address.Street);
                    var clientNeighborhood = new SqlParameter("@ClientNeighborhood", client.Address.Neighborhod);
                    var clientInteriorNumber = new SqlParameter("@ClientInteriorNumber", client.Address.InteriorNumber);
                    var clientOutdoorNumber = new SqlParameter("@ClientOutdoorNumber", client.Address.OutdoorNumber);
                    var clientPostCode = new SqlParameter("@ClientPostCode", client.Address.PostCode);
                    var clientCity = new SqlParameter("@ClientCity", client.Address.City);
                    var clientMunicipality = new SqlParameter("@ClientMunicipality", client.Address.Municipality);
                    var clientState = new SqlParameter("@ClientState", client.Address.State);
                    var cardNumber = new SqlParameter("@CardNumber", client.BankAccount.CardNumber);
                    var holder = new SqlParameter("@Holder", client.BankAccount.Holder);
                    var bank = new SqlParameter("@Bank", client.BankAccount.Bank);
                    var companyName = new SqlParameter("@CompanyName", client.WorkCenter.CompanyName);
                    var workCenterPhoneNumber = new SqlParameter("@WorkCenterPhoneNumber", client.WorkCenter.PhoneNumber);
                    var employeePosition = new SqlParameter("@EmployeePosition", client.WorkCenter.EmployeePosition);
                    var salary = new SqlParameter("@Salary", client.WorkCenter.Salary);
                    var employeeSeniority = new SqlParameter("@EmployeeSeniority", client.WorkCenter.EmployeeSeniority);
                    var humanResourcesPhone = new SqlParameter("@HumanResourcesPhone", client.WorkCenter.HumanResourcesPhone);
                    var workCenterStreet = new SqlParameter("@WorkCenterStreet", client.WorkCenter.Address.Street);
                    var workCenterNeighborhood = new SqlParameter("@WorkCenterNeighborhood", client.WorkCenter.Address.Neighborhod);
                    var workCenterInteriorNumber = new SqlParameter("@WorkCenterInteriorNumber", client.WorkCenter.Address.InteriorNumber);
                    var workCenterOutdoorNumber = new SqlParameter("@WorkCenterOutdoorNumber", client.WorkCenter.Address.OutdoorNumber);
                    var workCenterPostCode = new SqlParameter("@WorkCenterPostCode", client.WorkCenter.Address.PostCode);
                    var workCenterCity = new SqlParameter("@WorkCenterCity", client.WorkCenter.Address.City);
                    var workCenterMunicipality = new SqlParameter("@WorkCenterMunicipality", client.WorkCenter.Address.Municipality);
                    var workCenterState = new SqlParameter("@WorkCenterState", client.WorkCenter.Address.State);
                    var clientPhoneNumberFirst = new SqlParameter("@ClientPhoneNumberFirst", client.ContacMethods[0].Value);
                    var clientPhoneNumberFirstType = new SqlParameter("@ClientPhoneNumberFirstType", client.ContacMethods[0].MethodType);
                    var clientPhoneNumberSecond = new SqlParameter("@ClientPhoneNumberSecond", client.ContacMethods[1].Value);
                    var clientPhoneNumberSecondType = new SqlParameter("@ClientPhoneNumberSecondType", client.ContacMethods[1].MethodType);
                    var clientPhoneNumberThird = new SqlParameter("@ClientPhoneNumberThird", client.ContacMethods[2].Value);
                    var clientPhoneNumberThirdType = new SqlParameter("@ClientPhoneNumberThirdType", client.ContacMethods[2].MethodType);
                    var clientPhoneNumberFourth = new SqlParameter("@ClientPhoneNumberFourth", client.ContacMethods[3].Value);
                    var clientPhoneNumberFourthType = new SqlParameter("@ClientPhoneNumberFourthType", client.ContacMethods[3].MethodType);
                    var clientEmailFirst = new SqlParameter("@ClientEmailFirst", client.ContacMethods[4].Value);
                    var clientEmailFirstType = new SqlParameter("@ClientEmailFirstType", client.ContacMethods[4].MethodType);
                    var clientEmailSecond = new SqlParameter("@ClientEmailSecond", client.ContacMethods[5].Value);
                    var clientEmailSecondType = new SqlParameter("@ClientEmailSecondType", client.ContacMethods[5].MethodType);
                    var clientEmailThird = new SqlParameter("@ClientEmailThird", client.ContacMethods[6].Value);
                    var clientEmailThirdType = new SqlParameter("@ClientEmailThirdType", client.ContacMethods[6].MethodType);
                    var referenceNameFirst = new SqlParameter("@ReferenceNameFirst", client.PersonalReferences[0].Name);
                    var referenceSurnameFirst = new SqlParameter("@ReferenceSurnameFirst", client.PersonalReferences[0].Surname);
                    var referenceLastNameFirst = new SqlParameter("@ReferenceLastNameFirst", client.PersonalReferences[0].LastName);
                    var referencePhoneNumberFirst = new SqlParameter("@ReferencePhoneNumberFirst", client.PersonalReferences[0].PhoneNumber);
                    var referenceKinshipFirst = new SqlParameter("@ReferenceKinshipFirst", client.PersonalReferences[0].Kinship);
                    var referenceRelationshipFirst = new SqlParameter("@ReferenceRelationshipFirst", client.PersonalReferences[0].RelationshipYears);
                    var referenceIneKeyFirst = new SqlParameter("@ReferenceIneKeyFirst", client.PersonalReferences[0].IneKey);
                    var referenceNameSecond = new SqlParameter("@ReferenceNameSecond", client.PersonalReferences[1].Name);
                    var referenceSurnameSecond = new SqlParameter("@ReferenceSurnameSecond", client.PersonalReferences[1].Surname);
                    var referenceLastNameSecond = new SqlParameter("@ReferenceLastNameSecond", client.PersonalReferences[1].LastName);
                    var referencePhoneNumberSecond = new SqlParameter("@ReferencePhoneNumberSecond", client.PersonalReferences[1].PhoneNumber);
                    var referenceKinshipSecond = new SqlParameter("@ReferenceKinshipSecond", client.PersonalReferences[1].Kinship);
                    var referenceRelationshipSecond = new SqlParameter("@ReferenceRelationshipSecond", client.PersonalReferences[1].RelationshipYears);
                    var referenceIneKeySecond = new SqlParameter("@ReferenceIneKeySecond", client.PersonalReferences[1].IneKey);
                    var referenceStreetFirst = new SqlParameter("@ReferenceStreetFirst", client.PersonalReferences[0].Address.Street);
                    var referenceNeighborhoodFirst = new SqlParameter("@ReferenceNeighborhoodFirst", client.PersonalReferences[0].Address.Neighborhod);
                    var referenceInteriorNumberFirst = new SqlParameter("@ReferenceInteriorNumberFirst", client.PersonalReferences[0].Address.InteriorNumber);
                    var referenceOutdoorNumberFirst = new SqlParameter("@ReferenceOutdoorNumberFirst", client.PersonalReferences[0].Address.OutdoorNumber);
                    var referencePostCodeFirst = new SqlParameter("@ReferencePostCodeFirst", client.PersonalReferences[0].Address.PostCode);
                    var referenceCityFirst = new SqlParameter("@ReferenceCityFirst", client.PersonalReferences[0].Address.City);
                    var referenceMunicipalityFirst = new SqlParameter("@ReferenceMunicipalityFirst", client.PersonalReferences[0].Address.Municipality);
                    var referenceStateFirst = new SqlParameter("@ReferenceStateFirst", client.PersonalReferences[0].Address.State);
                    var referenceStreetSecond = new SqlParameter("@ReferenceStreetSecond", client.PersonalReferences[1].Address.Street);
                    var referenceNeighborhoodSecond = new SqlParameter("@ReferenceNeighborhoodSecond", client.PersonalReferences[1].Address.Neighborhod);
                    var referenceInteriorNumberSecond = new SqlParameter("@ReferenceInteriorNumberSecond", client.PersonalReferences[1].Address.InteriorNumber);
                    var referenceOutdoorNumberSecond = new SqlParameter("@ReferenceOutdoorNumberSecond", client.PersonalReferences[1].Address.OutdoorNumber);
                    var referencePostCodeSecond = new SqlParameter("@ReferencePostCodeSecond", client.PersonalReferences[1].Address.PostCode);
                    var referenceCitySecond = new SqlParameter("@ReferenceCitySecond", client.PersonalReferences[1].Address.City);
                    var referenceMunicipalitySecond = new SqlParameter("@ReferenceMunicipalitySecond", client.PersonalReferences[1].Address.Municipality);
                    var referenceStateSecond = new SqlParameter("@ReferenceStateSecond", client.PersonalReferences[1].Address.State);

                    var successParam = new SqlParameter("@Success", SqlDbType.Bit);
                    successParam.Direction = ParameterDirection.Output;

                    dbContext.Database.ExecuteSqlCommand(
                        "EXEC RegisterClient @ClientName, @ClientSurname, @ClientLastName, @ClientBirthdate, @ClientCurp, @ClientRfc, " +
                        "@ClientStreet, @ClientNeighborhood, @ClientInteriorNumber, @ClientOutdoorNumber, @ClientPostCode, @ClientCity, " +
                        "@ClientMunicipality, @ClientState, @CardNumber, @Bank, @Holder, @CompanyName, @WorkCenterPhoneNumber, " +
                        "@EmployeePosition, @Salary, @EmployeeSeniority, @HumanResourcesPhone, @WorkCenterStreet, @WorkCenterNeighborhood, " +
                        "@WorkCenterInteriorNumber, @WorkCenterOutdoorNumber, @WorkCenterPostCode, @WorkCenterCity, @WorkCenterMunicipality, " +
                        "@WorkCenterState, @ClientPhoneNumberFirst, @ClientPhoneNumberFirstType, @ClientPhoneNumberSecond, " +
                        "@ClientPhoneNumberSecondType, @ClientPhoneNumberThird, @ClientPhoneNumberThirdType, @ClientPhoneNumberFourth, " +
                        "@ClientPhoneNumberFourthType, @ClientEmailFirst, @ClientEmailFirstType, @ClientEmailSecond, @ClientEmailSecondType, " +
                        "@ClientEmailThird, @ClientEmailThirdType, @ReferenceNameFirst, @ReferenceSurnameFirst, @ReferenceLastNameFirst, " +
                        "@ReferencePhoneNumberFirst, @ReferenceKinshipFirst, @ReferenceRelationshipFirst, @ReferenceIneKeyFirst, " +
                        "@ReferenceStreetFirst, @ReferenceNeighborhoodFirst, @ReferenceInteriorNumberFirst, @ReferenceOutdoorNumberFirst, " +
                        "@ReferencePostCodeFirst, @ReferenceCityFirst, @ReferenceMunicipalityFirst, @ReferenceStateFirst, @ReferenceNameSecond, " +
                        "@ReferenceSurnameSecond, @ReferenceLastNameSecond, @ReferencePhoneNumberSecond, @ReferenceKinshipSecond, " +
                        "@ReferenceRelationshipSecond, @ReferenceIneKeySecond, @ReferenceStreetSecond, @ReferenceNeighborhoodSecond, " +
                        "@ReferenceInteriorNumberSecond, @ReferenceOutdoorNumberSecond, @ReferencePostCodeSecond, @ReferenceCitySecond, " +
                        "@ReferenceMunicipalitySecond, @ReferenceStateSecond, @Success OUTPUT",
                        clientName, clientSurname, clientLastName, clientBirthdate, clientCurp, clientRfc, clientStreet, clientNeighborhood,
                        clientInteriorNumber, clientOutdoorNumber, clientPostCode, clientCity, clientMunicipality, clientState, cardNumber,
                        bank, holder, companyName, workCenterPhoneNumber, employeePosition, salary, employeeSeniority, humanResourcesPhone,
                        workCenterStreet, workCenterNeighborhood, workCenterInteriorNumber, workCenterOutdoorNumber, workCenterPostCode,
                        workCenterCity, workCenterMunicipality, workCenterState, clientPhoneNumberFirst, clientPhoneNumberFirstType,
                        clientPhoneNumberSecond, clientPhoneNumberSecondType, clientPhoneNumberThird, clientPhoneNumberThirdType,
                        clientPhoneNumberFourth, clientPhoneNumberFourthType, clientEmailFirst, clientEmailFirstType, clientEmailSecond,
                        clientEmailSecondType, clientEmailThird, clientEmailThirdType, referenceNameFirst, referenceSurnameFirst,
                        referenceLastNameFirst, referencePhoneNumberFirst, referenceKinshipFirst, referenceRelationshipFirst, referenceIneKeyFirst,
                        referenceStreetFirst, referenceNeighborhoodFirst, referenceInteriorNumberFirst, referenceOutdoorNumberFirst,
                        referencePostCodeFirst, referenceCityFirst, referenceMunicipalityFirst, referenceStateFirst, referenceNameSecond,
                        referenceSurnameSecond, referenceLastNameSecond, referencePhoneNumberSecond, referenceKinshipSecond,
                        referenceRelationshipSecond, referenceIneKeySecond, referenceStreetSecond, referenceNeighborhoodSecond,
                        referenceInteriorNumberSecond, referenceOutdoorNumberSecond, referencePostCodeSecond, referenceCitySecond,
                        referenceMunicipalitySecond, referenceStateSecond, successParam
                        );

                    success = (bool)successParam.Value;
                }
            }
            catch (EntityException)
            {
                throw new FaultException<ServiceFault>(new ServiceFault("No fue posible registrar al cliente, inténtelo de nuevo más tarde"));
            }
            catch (DbUpdateException)
            {
                throw new FaultException<ServiceFault>(new ServiceFault("No fue posible registrar al cliente, inténtelo de nuevo más tarde"));
            }
            catch (DbEntityValidationException)
            {
                throw new FaultException<ServiceFault>(new ServiceFault("No fue posible registrar al cliente, inténtelo de nuevo más tarde"));
            }
            catch (SqlException)
            {
                throw new FaultException<ServiceFault>(new ServiceFault("No fue posible registrar al cliente, inténtelo de nuevo más tarde"));
            }

            return success;
        }

        public static List<PersonalReference> RecoverPersonalReferences(string rfc)
        {
            List<PersonalReference> personalReferencesList = new List<PersonalReference>();

            try
            {
                using (var context = new SFIDatabaseContext())
                {
                    var personalReferences = (from personalReference in context.personal_references
                                              join address in context.addresses
                                              on personalReference.id_address equals address.id_address
                                              where personalReference.client_rfc == rfc
                                              select new
                                              {
                                                  personalReference,
                                                  address
                                              }).ToList();
                    if (personalReferences != null)
                    {
                        foreach (var item in personalReferences)
                        {
                            Address personalReferenceAddress = new Address
                            {
                                IdAddress = item.address.id_address,
                                Street = item.address.street,
                                Neighborhod = item.address.neighborhod,
                                InteriorNumber = item.address.inteior_number,
                                OutdoorNumber = item.address.outdoor_number,
                                PostCode = item.address.post_code,
                                Municipality = item.address.municipality,
                                City = item.address.city,
                                State = item.address.state,
                            };
                            PersonalReference personalReference = new PersonalReference
                            {
                                IdPersonalReference = item.personalReference.id_personal_reference,
                                Name = item.personalReference.name,
                                Surname = item.personalReference.surname,
                                LastName = item.personalReference.last_name,
                                PhoneNumber = item.personalReference.phone_number,
                                Kinship = item.personalReference.kinship,
                                RelationshipYears = item.personalReference.relationship_years,
                                IneKey = item.personalReference.ine_key,
                                ClientRfc = item.personalReference.client_rfc,
                                Address = personalReferenceAddress
                            };
                            personalReferencesList.Add(personalReference);
                        }
                    }
                }
            }
            catch (EntityException)
            {
                throw new FaultException<ServiceFault>(new ServiceFault("No fue posible recuperar las referencias personales del cliente, inténtelo de nuevo más tarde"));
            }
            catch (DbUpdateException)
            {
                throw new FaultException<ServiceFault>(new ServiceFault("No fue posible recuperar las referencias personales del cliente, inténtelo de nuevo más tarde"));
            }
            catch (DbEntityValidationException)
            {
                throw new FaultException<ServiceFault>(new ServiceFault("No fue posible recuperar las referencias personales del cliente, inténtelo de nuevo más tarde"));
            }
            catch (SqlException)
            {
                throw new FaultException<ServiceFault>(new ServiceFault("No fue posible recuperar las referencias personales del cliente, inténtelo de nuevo más tarde"));
            }

            return personalReferencesList;
        }

        public static bool UpdatePersonalReference(PersonalReference personalReference, string currentIneKey)
        {
            bool success = false;

            try
            {
                using (var dbContext = new SFIDatabaseContext())
                {
                    var idPersonalReference = new SqlParameter("@IdPersonalReference", personalReference.IdPersonalReference);
                    var currentInekey = new SqlParameter("@CurrentIneKey", currentIneKey);
                    var ineKey = new SqlParameter("@IneKey", personalReference.IneKey);
                    var name = new SqlParameter("@Name", personalReference.Name);
                    var surname = new SqlParameter("@Surname", personalReference.Surname);
                    var lastName = new SqlParameter("@LastName", personalReference.LastName);
                    var kinship = new SqlParameter("@Kinship", personalReference.Kinship);
                    var relationshipYears = new SqlParameter("@RelationshipYears", personalReference.RelationshipYears);
                    var idAddress = new SqlParameter("@IdAddress", personalReference.Address.IdAddress);
                    var clientRfc = new SqlParameter("@ClientRfc", personalReference.ClientRfc);
                    var phoneNumber = new SqlParameter("@PhoneNumber", personalReference.PhoneNumber);
                    var street = new SqlParameter("@Street", personalReference.Address.Street);
                    var neighborhood = new SqlParameter("@Neighborhood", personalReference.Address.Neighborhod);
                    var interiorNumber = new SqlParameter("@InteriorNumber", personalReference.Address.InteriorNumber);
                    var outdoorNumber = new SqlParameter("@OutdoorNumber", personalReference.Address.OutdoorNumber);
                    var postCode = new SqlParameter("@PostCode", personalReference.Address.PostCode);
                    var city = new SqlParameter("@City", personalReference.Address.City);
                    var municipality = new SqlParameter("@Municipality", personalReference.Address.Municipality);
                    var state = new SqlParameter("@State", personalReference.Address.State);

                    var successParam = new SqlParameter("@Success", SqlDbType.Bit);
                    successParam.Direction = ParameterDirection.Output;

                    dbContext.Database.ExecuteSqlCommand(
                        "EXEC UpdatePersonalReference @IdPersonalReference, @CurrentIneKey, @IneKey, @Name, @Surname, @LastName, " +
                        "@Kinship, @RelationshipYears, @IdAddress, @ClientRfc, @PhoneNumber, @Street, @Neighborhood, @InteriorNumber, " +
                        "@OutdoorNumber, @PostCode, @City, @Municipality, @State, @Success OUTPUT",
                        idPersonalReference, currentInekey, ineKey, name, surname, lastName, kinship, relationshipYears, idAddress, clientRfc, 
                        phoneNumber, street, neighborhood, interiorNumber, outdoorNumber, postCode, city, municipality, state, successParam);

                    success = (bool)successParam.Value;
                }
            }
            catch (EntityException)
            {
                throw new FaultException<ServiceFault>(new ServiceFault("No fue posible actualizar la referencia personal del cliente, inténtelo de nuevo más tarde"));
            }
            catch (DbUpdateException)
            {
                throw new FaultException<ServiceFault>(new ServiceFault("No fue posible actualizar la referencia personal del cliente, inténtelo de nuevo más tarde"));
            }
            catch (DbEntityValidationException)
            {
                throw new FaultException<ServiceFault>(new ServiceFault("No fue posible actualizar la referencia personal del cliente, inténtelo de nuevo más tarde"));
            }
            catch (SqlException)
            {
                throw new FaultException<ServiceFault>(new ServiceFault("No fue posible actualizar la referencia personal del cliente, inténtelo de nuevo más tarde"));
            }

            return success;
        }

        public static Client RecoverClient(string clientRfc)
        {
            Client fullClient = null;

            try
            {
                using (var context = new SFIDatabaseContext())
                {
                    var clientInformation = (from client in context.clients
                                             where client.rfc == clientRfc
                                             select client).FirstOrDefault();
                    var clientAddress = (from address in context.addresses
                                         where address.id_address == clientInformation.id_address
                                         select address).FirstOrDefault();
                    var phoneNumberType = (from contactMethodType in  context.contact_method_types
                                           where contactMethodType.name == "PhoneNumber"
                                           select contactMethodType.id_contact_method_type).FirstOrDefault();
                    var clientPhoneNumbers = (from contactMethod in context.contact_methods
                                              where contactMethod.client_rfc == clientRfc && 
                                              contactMethod.id_contact_method_type == phoneNumberType
                                              select contactMethod).ToList();
                    var emailType = (from contactMethodType in context.contact_method_types
                                     where contactMethodType.name == "Email"
                                     select contactMethodType.id_contact_method_type).FirstOrDefault();
                    var clientEmails = (from contactMethod in context.contact_methods
                                       where contactMethod.client_rfc == clientRfc &&
                                       contactMethod.id_contact_method_type == emailType
                                       select contactMethod).ToList();
                    var bankAccountInformation = (from bankAccount in context.bank_accounts 
                                                  where bankAccount.card_number == clientInformation.card_number
                                                  select bankAccount).FirstOrDefault();
                    var workCenterInformation = (from workCenter in context.work_centers
                                                 where workCenter.id_work_center == clientInformation.id_work_center
                                                 select workCenter).FirstOrDefault();
                    var workCenterAddress = (from address in context.addresses
                                             where address.id_address == workCenterInformation.id_address
                                             select address).FirstOrDefault();
                    var personalReferencesInformation = (from personalReference in context.personal_references
                                                         where personalReference.client_rfc == clientRfc
                                                         select personalReference).ToList();
                    var personalReferenceFirst = personalReferencesInformation[0];
                    var personalReferenceSecond = personalReferencesInformation[1];
                    var personalReferenceAddresFirst = (from referenceAddressFirst in context.addresses
                                                        where referenceAddressFirst.id_address == personalReferenceFirst.id_address
                                                        select referenceAddressFirst).FirstOrDefault();
                    var personalReferenceAddresSecond = (from referenceAddressSecond in context.addresses
                                                         where referenceAddressSecond.id_address == personalReferenceSecond.id_address
                                                        select referenceAddressSecond).FirstOrDefault();
                    Address addressWorkCenter = new Address
                    {
                        Street = workCenterAddress.street,
                        City = workCenterAddress.city,
                        Neighborhod = workCenterAddress.neighborhod,
                        Municipality = workCenterAddress.municipality,
                        InteriorNumber = workCenterAddress.inteior_number,
                        OutdoorNumber = workCenterAddress.outdoor_number,
                        PostCode = workCenterAddress.post_code,
                        State = workCenterAddress.state
                    };
                    Address addresspersonalReferenceFirst = new Address
                    {
                        Street = personalReferenceAddresFirst.street,
                        City = personalReferenceAddresFirst.city,
                        Neighborhod = personalReferenceAddresFirst.neighborhod,
                        Municipality = personalReferenceAddresFirst.municipality,
                        InteriorNumber = personalReferenceAddresFirst.inteior_number,
                        OutdoorNumber = personalReferenceAddresFirst.outdoor_number,
                        PostCode = personalReferenceAddresFirst.post_code,
                        State = personalReferenceAddresFirst.state
                    };
                    PersonalReference personalReferenceClientFirst = new PersonalReference
                    {
                        Name = personalReferencesInformation[0].name,
                        Surname = personalReferencesInformation[0].surname,
                        LastName = personalReferencesInformation[0].last_name,
                        PhoneNumber = personalReferencesInformation[0].phone_number,
                        Kinship = personalReferencesInformation[0].kinship,
                        RelationshipYears = personalReferencesInformation[0].relationship_years,
                        IneKey = personalReferencesInformation[0].ine_key,
                        Address = addresspersonalReferenceFirst
                    };
                    Address addresspersonalReferenceSecond = new Address
                    {
                        Street = personalReferenceAddresSecond.street,
                        City = personalReferenceAddresSecond.city,
                        Neighborhod = personalReferenceAddresSecond.neighborhod,
                        Municipality = personalReferenceAddresSecond.municipality,
                        InteriorNumber = personalReferenceAddresSecond.inteior_number,
                        OutdoorNumber = personalReferenceAddresSecond.outdoor_number,
                        PostCode = personalReferenceAddresSecond.post_code,
                        State = personalReferenceAddresSecond.state
                    };
                    PersonalReference personalReferenceClientSecond = new PersonalReference
                    {
                        Name = personalReferencesInformation[1].name,
                        Surname = personalReferencesInformation[1].surname,
                        LastName = personalReferencesInformation[1].last_name,
                        PhoneNumber = personalReferencesInformation[1].phone_number,
                        Kinship = personalReferencesInformation[1].kinship,
                        RelationshipYears = personalReferencesInformation[1].relationship_years,
                        IneKey = personalReferencesInformation[1].ine_key,
                        Address = addresspersonalReferenceSecond
                    };
                    BankAccount clientBankAccount = new BankAccount
                    {
                        CardNumber = bankAccountInformation.card_number,
                        Bank = bankAccountInformation.bank,
                        Holder = bankAccountInformation.holder
                    };

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
                        HumanResourcesPhone = workCenterInformation.human_resources_phone,
                        Address = addressWorkCenter
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

                    List<PersonalReference> personalReferences = new List<PersonalReference>
                    {
                        personalReferenceClientFirst,
                        personalReferenceClientSecond
                    };

                    fullClient = new Client
                    {
                        Rfc = clientInformation.rfc,
                        Curp = clientInformation.curp,
                        Birthdate = clientInformation.birthdate,
                        Name = clientInformation.name,
                        LastName = clientInformation.last_name,
                        Surname = clientInformation.surname,
                        BankAccount = clientBankAccount,
                        Address = addressClient,
                        WorkCenter = clientWorkCenter,
                        ContacMethods = contactMethods,
                        PersonalReferences = personalReferences
                    };
                }
            }
            catch (EntityException)
            {
                throw new FaultException<ServiceFault>(new ServiceFault("No fue posible recuperar la información del cliente, inténtelo de nuevo más tarde"));
            }
            catch (DbUpdateException)
            {
                throw new FaultException<ServiceFault>(new ServiceFault("No fue posible recuperar la información del cliente, inténtelo de nuevo más tarde"));
            }
            catch (DbEntityValidationException)
            {
                throw new FaultException<ServiceFault>(new ServiceFault("No fue posible recuperar la información del cliente, inténtelo de nuevo más tarde"));
            }
            catch (SqlException)
            {
                throw new FaultException<ServiceFault>(new ServiceFault("No fue posible recuperar la información del cliente, inténtelo de nuevo más tarde"));
            }

            return fullClient;
        }

        public static Client GetClientPersonalInformation(string clientRFC)
        {
            Client client = null;

            try
            {
                using (var context = new SFIDatabaseContext())
                {
                    client storedClient = context.clients
                        .Where(dbClient => dbClient.rfc == clientRFC)
                        .FirstOrDefault();

                    if(storedClient != null)
                    {
                        client = new Client
                        {
                            Rfc = storedClient.rfc,
                            Curp = storedClient.curp,
                            Name = storedClient.name,
                            LastName = storedClient.last_name,
                            Surname = storedClient.surname,
                            Birthdate = storedClient.birthdate
                        };

                        if(storedClient.address != null)
                        {
                            client.Address = new Address
                            {
                                Street = storedClient.address.street,
                                Neighborhod = storedClient.address.neighborhod,
                                InteriorNumber = storedClient.address.inteior_number,
                                OutdoorNumber = storedClient.address.outdoor_number,
                                PostCode = storedClient.address.post_code,
                                City = storedClient.address.city,
                                Municipality = storedClient.address.municipality,
                                State = storedClient.address.state
                            };
                        }
                    }
                }
            }
            catch (EntityException)
            {
                throw new FaultException<ServiceFault>(
                    new ServiceFault("Servidor no disponible. No fue posible recuperar la información del cliente, " +
                    "por favor inténtelo más tarde"),
                    new FaultReason("Error")
                );
            }
            catch(SqlException)
            {
                throw new FaultException<ServiceFault>(
                    new ServiceFault("Servidor no disponible. No fue posible recuperar la información del cliente, " +
                    "por favor inténtelo más tarde"),
                    new FaultReason("Error")
                );
            }

            return client;
        }

        public static bool UpdateClientPersonalInformation(Client client)
        {
            bool updated = false;

            try
            {
                using (var context = new SFIDatabaseContext())
                {
                    context.Database.ExecuteSqlCommand(
                        "EXEC UpdateClientPersonalInformation @client_rfc, @client_name, " +
                        "@client_last_name, @client_surname, @client_birthdate, " +
                        "@address_street, @address_neighborhood, @address_interior_number, " +
                        "@address_outdoor_number, @address_post_code, @address_city, " +
                        "@address_municipality, @address_state",
                        new SqlParameter("@client_rfc", client.Rfc),
                        new SqlParameter("@client_name", client.Name),
                        new SqlParameter("@client_last_name", client.LastName),
                        new SqlParameter("@client_surname", client.Surname),
                        new SqlParameter("@client_birthdate", client.Birthdate),
                        new SqlParameter("@address_street", client.Address.Street),
                        new SqlParameter("@address_neighborhood", client.Address.Neighborhod),
                        new SqlParameter("@address_interior_number", client.Address.InteriorNumber),
                        new SqlParameter("@address_outdoor_number", client.Address.OutdoorNumber),
                        new SqlParameter("@address_post_code", client.Address.PostCode),
                        new SqlParameter("@address_city", client.Address.City),
                        new SqlParameter("@address_municipality", client.Address.Municipality),
                        new SqlParameter("@address_state", client.Address.State)
                    );

                    updated = true;
                }
            }
            catch (EntityException)
            {
                throw new FaultException<ServiceFault>(
                    new ServiceFault("Servidor no disponible. No fue posible actualizar la información del cliente, " +
                    "por favor inténtelo más tarde")
                );
            }
            catch (SqlException)
            {
                throw new FaultException<ServiceFault>(
                    new ServiceFault("Servidor no disponible. No fue posible actualizar la información del cliente, " +
                    "por favor inténtelo más tarde")
                );
            }

            return updated;
        }
        public static Client GetWorkCenterInformation(string clientRFC)
        {
            Client clientWorkCenterInformation = null;
            try
            {
                using (var context = new SFIDatabaseContext())
                {
                    var clientInformation = (from client in context.clients
                                             where client.rfc == clientRFC
                                             select client).FirstOrDefault();
                    var workCenterInformation = (from workCenter in context.work_centers
                                                 where workCenter.id_work_center == clientInformation.id_work_center
                                                 select workCenter).FirstOrDefault();
                    var workCenterAddress = (from address in context.addresses
                                             where address.id_address == workCenterInformation.id_address
                                             select address).FirstOrDefault();
                    Address addressWorkCenter = new Address
                    {
                        Street = workCenterAddress.street,
                        City = workCenterAddress.city,
                        Neighborhod = workCenterAddress.neighborhod,
                        Municipality = workCenterAddress.municipality,
                        InteriorNumber = workCenterAddress.inteior_number,
                        OutdoorNumber = workCenterAddress.outdoor_number,
                        PostCode = workCenterAddress.post_code,
                        State = workCenterAddress.state
                    };
                    WorkCenter clientWorkCenter = new WorkCenter
                    {
                        CompanyName = workCenterInformation.company_name,
                        PhoneNumber = workCenterInformation.phone_number,
                        EmployeePosition = workCenterInformation.employee_position,
                        Salary = workCenterInformation.salary,
                        EmployeeSeniority = workCenterInformation.employee_seniority,
                        HumanResourcesPhone = workCenterInformation.human_resources_phone,
                        Address = addressWorkCenter
                    };
                    clientWorkCenterInformation = new Client
                    {
                        Rfc = clientInformation.rfc,
                        Curp = clientInformation.curp,
                        Birthdate = clientInformation.birthdate,
                        Name = clientInformation.name,
                        LastName = clientInformation.last_name,
                        Surname = clientInformation.surname,
                        WorkCenter = clientWorkCenter,
                    };
                }
            }
            catch (EntityException)
            {
                throw new FaultException<ServiceFault>(
                    new ServiceFault("Servidor no disponible. No fue posible recuperar la información del centro de trabajo del cliente, " +
                    "por favor inténtelo más tarde"),
                    new FaultReason("Error")
                );
            }
            catch (SqlException)
            {
                throw new FaultException<ServiceFault>(
                    new ServiceFault("Servidor no disponible. No fue posible recuperar la nformación del centro de trabajo del cliente, " +
                    "por favor inténtelo más tarde"),
                    new FaultReason("Error")
                );
            }

            return clientWorkCenterInformation;
        }
        public static bool UpdateClientWorkCenterlInformation(Client client)
        {
            bool updated = false;

            try
            {
                using (var context = new SFIDatabaseContext())
                {
                    context.Database.ExecuteSqlCommand(
                        "EXEC UpdateWorkCenterAndAddress @ClientRfc, @CompanyName, @PhoneNumber, " +
                        "@EmployeePosition, @Salary, @EmployeeSeniority, @HumanResourcesPhone, " +
                        "@Street, @Neighborhod, @InteriorNumber, @OutdoorNumber, @PostCode, " +
                        "@City, @Municipality, @State",
                        new SqlParameter("@ClientRfc", client.Rfc),
                        new SqlParameter("@CompanyName", client.WorkCenter.CompanyName),
                        new SqlParameter("@PhoneNumber", client.WorkCenter.PhoneNumber),
                        new SqlParameter("@EmployeePosition", client.WorkCenter.EmployeePosition),
                        new SqlParameter("@Salary", client.WorkCenter.Salary),
                        new SqlParameter("@EmployeeSeniority", client.WorkCenter.EmployeeSeniority),
                        new SqlParameter("@HumanResourcesPhone", client.WorkCenter.HumanResourcesPhone),
                        new SqlParameter("@Street", client.WorkCenter.Address.Street),
                        new SqlParameter("@Neighborhod", client.WorkCenter.Address.Neighborhod),
                        new SqlParameter("@InteriorNumber", client.WorkCenter.Address.InteriorNumber),
                        new SqlParameter("@OutdoorNumber", client.WorkCenter.Address.OutdoorNumber),
                        new SqlParameter("@PostCode", client.WorkCenter.Address.PostCode),
                        new SqlParameter("@City", client.WorkCenter.Address.City),
                        new SqlParameter("@Municipality", client.WorkCenter.Address.Municipality),
                        new SqlParameter("@State", client.WorkCenter.Address.State)
                    );

                    updated = true;
                }
            }
            catch (EntityException)
            {
                throw new FaultException<ServiceFault>(
                    new ServiceFault("Servidor no disponible. No fue posible actualizar la información del centro de trabajo del cliente " +
                    "por favor inténtelo más tarde")
                );
            }
            catch (SqlException)
            {
                throw new FaultException<ServiceFault>(
                    new ServiceFault("Servidor no disponible. No fue posible actualizar la información del centro de trabajo del cliente, " +
                    "por favor inténtelo más tarde")
                );
            }

            return updated;
        }

    }
}