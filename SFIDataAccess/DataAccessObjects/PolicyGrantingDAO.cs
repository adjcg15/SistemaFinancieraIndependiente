using SFIDataAccess.CustomExceptions;
using SFIDataAccess.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace SFIDataAccess.DataAccessObjects
{
    public class PolicyGrantingDAO
    {
        public static void RegisterPolicyGranting(PolicyGranting newPolicy)
        {
            try
            {
                using (var context = new SFIDatabaseContext())
                {
                    var policy = new PolicyGranting
                    {
                        Title = newPolicy.Title,
                        Description = newPolicy.Description,
                        EffectiveDate = newPolicy.EffectiveDate,
                        IsActive = newPolicy.IsActive
                    };

                    PolicyGrantingDAO.RegisterPolicyGranting(policy);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al registrar la política de concesión de crédito en la base de datos", ex);
                //throw new FaultException<ServiceFault>(new ServiceFault("Error al registrar la política de concesión de crédito"));
            }
        }
    }
}
