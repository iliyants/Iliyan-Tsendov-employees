using System.Collections.Generic;

namespace Employees.Models
{
    public class EmployeePairOutput
    {
        public int FirstEmployeeId { get; set; }
        public int SecondEmployeeId { get; set; }
        public int TotalDaysWorkedTogether { get; set; }

        public List<int> ProjectIdList { get; set; } = new List<int>();

        public override string ToString()
        {
            return $"{string.Join(", ", ProjectIdList)}";
        }
    }
}
