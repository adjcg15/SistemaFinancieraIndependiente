
using SFIDataAccess.CustomExceptions;
using SFIDataAccess.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.Remoting.Messaging;
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
                                   where credit != null || credit_application != null || credit == null && credit_application == null
                                   select new
                                   {
                                       client,
                                       has_active_credit = credit != null,
                                       has_credit_application = credit_application != null,
                                       work_center.company_name
                                   }).Distinct().ToList();


                    foreach (var item in clients)
                    {
                        workCenter.CompanyName = item.client.work_centers.company_name;
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
            catch (System.Data.Entity.Core.EntityException)
            {
                throw new FaultException<ServiceFault>(
                    new ServiceFault("No fue posible establecer una conexión con la base de datos, " +
                    "por favor inténtelo más tarde"), new FaultReason("Error"));
            }
            catch (DbUpdateException)
            {
                throw new FaultException<ServiceFault>(
                    new ServiceFault("No fue posible establecer una conexión con la base de datos, " +
                    "por favor inténtelo más tarde"), new FaultReason("Error"));
            }
            catch (DbEntityValidationException)
            {
                throw new FaultException<ServiceFault>(
                    new ServiceFault("No fue posible establecer una conexión con la base de datos, " +
                    "por favor inténtelo más tarde"), new FaultReason("Error"));
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
                    var newCardNumberParam = new SqlParameter("@NewCardNumber", bankAccount.CardNumber);

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
                    var referenceStreetSecond = new SqlParameter("@ReferenceStreetSecond", client.PersonalReferences[0].Address.Street);
                    var referenceNeighborhoodSecond = new SqlParameter("@ReferenceNeighborhoodSecond", client.PersonalReferences[0].Address.Neighborhod);
                    var referenceInteriorNumberSecond = new SqlParameter("@ReferenceInteriorNumberSecond", client.PersonalReferences[0].Address.InteriorNumber);
                    var referenceOutdoorNumberSecond = new SqlParameter("@ReferenceOutdoorNumberSecond", client.PersonalReferences[0].Address.OutdoorNumber);
                    var referencePostCodeSecond = new SqlParameter("@ReferencePostCodeSecond", client.PersonalReferences[0].Address.PostCode);
                    var referenceCitySecond = new SqlParameter("@ReferenceCitySecond", client.PersonalReferences[0].Address.City);
                    var referenceMunicipalitySecond = new SqlParameter("@ReferenceMunicipalitySecond", client.PersonalReferences[0].Address.Municipality);
                    var referenceStateSecond = new SqlParameter("@ReferenceStateSecond", client.PersonalReferences[0].Address.State);

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
