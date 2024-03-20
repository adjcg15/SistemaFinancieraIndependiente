using SFIDataAccess.CustomExceptions;
using SFIDataAccess.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace SFIDataAccess.DataAccessObjects
{
    public class EmployeesDAO
    {
        public static Employee Login(string employeeNumber, string passwordHash)
        {
            Employee employee = null;

            try
            {
                using (var context = new SFIDatabaseContext())
                {
                    system_accounts accountStored = context.system_accounts
                        .FirstOrDefault(
                            account => account.employee_number == employeeNumber && account.password == passwordHash
                        );

                    if(accountStored != null)
                    {
                        EmployeeRole role = new EmployeeRole();
                        role.Identifier = accountStored.employee_roles.id_employee_rol;
                        role.Name = accountStored.employee_roles.name;

                        employee = new Employee();
                        employee.EmployeeNumber = accountStored.employee_number;
                        employee.Name = accountStored.name;
                        employee.Surname = accountStored.surname;
                        employee.LastName = accountStored.last_name;
                        employee.Password = accountStored.password;
                        employee.EmployeeRole = role;
                    }
                }
            }
            catch (EntityException)
            {
                throw new FaultException<ServiceFault>(
                    new ServiceFault("El sistema no se encuentra disponible, por favor intente más tarde"),
                    new FaultReason("Error")
                );
            }

            return employee;
        }
    }
}
