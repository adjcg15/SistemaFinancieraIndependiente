using SFIDataAccess.CustomExceptions;
using SFIDataAccess.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SFIDataAccess.DataAccessObjects
{
    public class PolicyGrantingDAO
    {
        public static bool RegisterPolicyGranting(PolicyGranting newPolicy)
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
            catch (Exception ex)
            {
                Debug.WriteLine($"Error registering the credit granting policy: {ex.Message}");
                return false;
            }
        }
    }
}
