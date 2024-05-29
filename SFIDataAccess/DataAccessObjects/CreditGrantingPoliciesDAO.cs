using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using SFIDataAccess.CustomExceptions;
using SFIDataAccess.Model;
using System.Data.Entity.Core;

namespace SFIDataAccess.DataAccessObjects
{
    public class CreditGrantingPoliciesDAO
    {
        public static bool RegisterCreditGrantingPolicy(CreditGrantingPolicy newPolicy)
        {
            try
            {
                using (var context = new SFIDatabaseContext())
                {
                    var titleParam = new SqlParameter("@Title", newPolicy.Title);
                    var descriptionParam = new SqlParameter("@Description", newPolicy.Description);
                    var effectiveDateParam = new SqlParameter("@EffectiveDate", newPolicy.EffectiveDate.Date);
                    var isActiveParam = new SqlParameter("@Active", newPolicy.IsActive);
                    var resultParam = new SqlParameter("@Result", SqlDbType.Int) { Direction = ParameterDirection.Output };

                    context.Database.ExecuteSqlCommand("InsertCreditPolicyProcedure @Title, @Description, @EffectiveDate, @Active, @Result OUTPUT",
                                                       titleParam, descriptionParam, effectiveDateParam, isActiveParam, resultParam);

                    int result = (int)resultParam.Value;
                    return result == 0;
                }
            }
            catch (EntityException)
            {
                throw new FaultException<ServiceFault>(new ServiceFault("No fue posible recuperar los datos"));
            }
            catch (DbUpdateException)
            {
                throw new FaultException<ServiceFault>(new ServiceFault("No fue posible recuperar los datos"));
            }
            catch (DbEntityValidationException)
            {
                throw new FaultException<ServiceFault>(new ServiceFault("No fue posible recuperar los datos"));
            }
            catch (SqlException)
            {
                throw new FaultException<ServiceFault>(new ServiceFault("No fue posible recuperar los datos"));
            }
        }

        public static List<CreditGrantingPolicy> GetAllCreditGrantingPolicies()
        {
            List<CreditGrantingPolicy> policies = new List<CreditGrantingPolicy>();

            try
            {
                using (var context = new SFIDatabaseContext())
                {
                    context.credit_granting_polices
                        .ToList().ForEach(policyStored => {
                            CreditGrantingPolicy policy = new CreditGrantingPolicy
                            {
                                Identifier = policyStored.id_credit_granting_policy,
                                Title = policyStored.title,
                                Description = policyStored.description,
                                EffectiveDate = policyStored.effective_date,
                                IsActive = policyStored.is_active
                            };

                            policies.Add(policy);
                        });
                }
            }
            catch (EntityException)
            {
                throw new FaultException<ServiceFault>(new ServiceFault("No fue posible recuperar los datos"));
            }
            catch (DbUpdateException)
            {
                throw new FaultException<ServiceFault>(new ServiceFault("No fue posible recuperar los datos"));
            }
            catch (DbEntityValidationException)
            {
                throw new FaultException<ServiceFault>(new ServiceFault("No fue posible recuperar los datos"));
            }
            catch (SqlException)
            {
                throw new FaultException<ServiceFault>(new ServiceFault("No fue posible recuperar los datos"));
            }

            return policies;
        }

        public static List<CreditGrantingPolicy> RecoverActivesCreditGrantingPolicies()
        {
            List<CreditGrantingPolicy> policiesList = new List<CreditGrantingPolicy>();

            try
            {
                using (var context = new SFIDatabaseContext())
                {
                    var polices = (from creditGrantingPolices in context.credit_granting_polices
                                   where creditGrantingPolices.is_active == true
                                   select creditGrantingPolices).ToList();
                    if (polices != null)
                    {
                        foreach (var policy in polices)
                        {
                            CreditGrantingPolicy creditGrantingPolicy = new CreditGrantingPolicy
                            {
                                Identifier = policy.id_credit_granting_policy,
                                Title = policy.title,
                                Description = policy.description,
                                EffectiveDate = policy.effective_date,
                                IsActive = policy.is_active
                            };
                            policiesList.Add(creditGrantingPolicy);
                        }
                    }
                }
            }
            catch (EntityException)
            {
                throw new FaultException<ServiceFault>(new ServiceFault("No fue posble recuperar las políticas para generar el dictamen, inténtelo de nuevo más tarde"));
            }
            catch (DbUpdateException)
            {
                throw new FaultException<ServiceFault>(new ServiceFault("No fue posble recuperar las políticas para generar el dictamen, inténtelo de nuevo más tarde"));
            }
            catch (DbEntityValidationException)
            {
                throw new FaultException<ServiceFault>(new ServiceFault("No fue posble recuperar las políticas para generar el dictamen, inténtelo de nuevo más tarde"));
            } 
            catch (SqlException)
            {
                throw new FaultException<ServiceFault>(new ServiceFault("No fue posble recuperar las políticas para generar el dictamen, inténtelo de nuevo más tarde"));
            }

            return policiesList;
        }

        public static bool UpdateCreditGrantingPolicy(CreditGrantingPolicy policy)
        {
            bool updated = false;

            try
            {
                using (var context = new SFIDatabaseContext())
                {
                    var updateParam = new SqlParameter("@updated", SqlDbType.Bit);
                    updateParam.Direction = ParameterDirection.Output;

                    context.Database.ExecuteSqlCommand(
                        "EXEC UpdateCreditGrantingPolicy @id_credit_granting_policy, " +
                        "@title, @is_active, @effective_date, @description, @updated OUTPUT",
                        new SqlParameter("@id_credit_granting_policy", policy.Identifier),
                        new SqlParameter("@title", policy.Title),
                        new SqlParameter("@is_active", policy.IsActive),
                        new SqlParameter("@effective_date", policy.EffectiveDate),
                        new SqlParameter("@description", policy.Description),
                        updateParam
                    );

                    updated = (bool)updateParam.Value;
                }
            }
            catch (EntityException)
            {
                throw new FaultException<ServiceFault>(
                    new ServiceFault("Servidor no disponible. No fue posible actualizar la información de la política, " +
                    "por favor inténtelo más tarde")
                );
            }
            catch (SqlException)
            {
                throw new FaultException<ServiceFault>(
                    new ServiceFault("Servidor no disponible. No fue posible actualizar la información de la política, " +
                    "por favor inténtelo más tarde")
                );
            }

            return updated;
        }
    }
}
