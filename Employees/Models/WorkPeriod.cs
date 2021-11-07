using System;

namespace Employees.Models
{
    public class WorkPeriod
    {
        public int ProjectId { get; set; }
        public DateTime DateStart { get; set; }
        public DateTime DateEnd { get; set; }
    }
}
