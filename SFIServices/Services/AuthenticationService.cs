using SFIDataAccess.DataAccessObjects;
using SFIDataAccess.Model;
using SFIServices.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFIServices
{
    public partial class SFIService : IAuthenticationService
    {
        public Employee Login(string employeeNumber, string passwordHash)
        {
            return EmployeesDAO.Login(employeeNumber, passwordHash);
        }
    }
}
