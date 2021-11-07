using Employees.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Employees.Helpers
{
    public static class FileProcessHelper
    {
        public static List<Employee> GenerateEmployeeList(IFormFile file)
        {

            var listOfEmployees = new List<Employee>();

            var result = new StringBuilder();

            using (var reader = new StreamReader(file.OpenReadStream()))
            {
                while (reader.Peek() >= 0)
                {
                    result.AppendLine(reader.ReadLine());
                }
            }

            var data = result.ToString().Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);

            foreach (var line in data)
            {
                var singleEntry = line.Split(", ");

                try
                {
                    listOfEmployees.Add(new Employee()
                    {
                        Id = int.Parse(singleEntry[0]),
                        ProjectId = int.Parse(singleEntry[1]),
                        DateFrom = DateTime.Parse(singleEntry[2]),
                        DateTo = singleEntry[3] == "NULL" ? DateTime.Now : DateTime.Parse(singleEntry[3])
                    });

                }
                catch
                {
                    return null;
                }

            }

            return listOfEmployees;
        }
    }
}
