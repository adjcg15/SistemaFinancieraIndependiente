using SFIDataAccess.DataAccessObjects;
using SFIDataAccess.Model;
using SFIServices.Contracts;
using System.Collections.Generic;

namespace SFIServices
{
    public partial class SFIService : ICreditGrantingPolicies
    {
        public bool RegisterCreditGrantingPolicy(CreditGrantingPolicy NewPolicy)
        {
            return CreditGrantingPoliciesDAO.RegisterCreditGrantingPolicy(NewPolicy);
        }

        public List<CreditGrantingPolicy> GetAllCreditGrantingPolicies()
        {
            return CreditGrantingPoliciesDAO.GetAllCreditGrantingPolicies();
        }

        public bool UpdateCreditGrantingPolicy(CreditGrantingPolicy policy)
        {
            return CreditGrantingPoliciesDAO.UpdateCreditGrantingPolicy(policy);
        }
    }
}
