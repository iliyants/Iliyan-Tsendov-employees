using Employees.Models;
using System.Collections.Generic;

namespace Employees.Services
{
    public interface IEmployeeService
    {
        public EmployeePairOutput GetLongestWorkingPair(List<Employee> employeeList);
    }
}
