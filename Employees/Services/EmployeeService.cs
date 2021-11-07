using Employees.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Employees.Services
{
    public class EmployeeService : IEmployeeService
    {
        public EmployeePairOutput GetLongestWorkingPair(List<Employee> employeeList)
        {
            return GetResult(GetPairOfEmployeesWithWorkPeriods(employeeList));
        }


        private Dictionary<KeyValuePair<int, int>, List<WorkPeriod>> GetPairOfEmployeesWithWorkPeriods(List<Employee> listOfEmployes)
        {

            var pairOfEmployeedWithWorkPeriods = new Dictionary<KeyValuePair<int, int>, List<WorkPeriod>>();


            //These loops go through each and every possible combination of employees and assign a key value pair of those employees with a list of their total working periods as a team
            for (int firstEmployee = 0; firstEmployee < listOfEmployes.Count; firstEmployee++)
            {
                for (int secondEmployee = firstEmployee + 1; secondEmployee < listOfEmployes.Count; secondEmployee++)
                {
                    //If the project id`s are not equal, or the first employee id and the second employee id are equal, we skip the iteration
                    if (listOfEmployes[firstEmployee].ProjectId.Equals(listOfEmployes[secondEmployee].ProjectId) && !listOfEmployes[firstEmployee].Id.Equals(listOfEmployes[secondEmployee].Id))

                    {
                        DateTime startOfWorkPeriod = DateTime.Now;
                        DateTime endOfWorkPeriod = DateTime.Now;

                        //If the working periods of the two employees have no overlap, we move on to the next iteration
                        if (listOfEmployes[firstEmployee].DateTo < listOfEmployes[secondEmployee].DateFrom || listOfEmployes[firstEmployee].DateFrom > listOfEmployes[secondEmployee].DateTo)
                        {
                            continue;
                        }
                        else if (listOfEmployes[firstEmployee].DateFrom <= listOfEmployes[secondEmployee].DateFrom && listOfEmployes[firstEmployee].DateTo <= listOfEmployes[secondEmployee].DateTo)
                        {
                            startOfWorkPeriod = listOfEmployes[secondEmployee].DateFrom;
                            endOfWorkPeriod = listOfEmployes[firstEmployee].DateTo;
                        }
                        else if (listOfEmployes[firstEmployee].DateFrom >= listOfEmployes[secondEmployee].DateFrom && listOfEmployes[firstEmployee].DateTo <= listOfEmployes[secondEmployee].DateTo)
                        {
                            startOfWorkPeriod = listOfEmployes[firstEmployee].DateFrom;
                            endOfWorkPeriod = listOfEmployes[firstEmployee].DateTo;
                        }
                        else if (listOfEmployes[firstEmployee].DateFrom >= listOfEmployes[secondEmployee].DateFrom && listOfEmployes[firstEmployee].DateTo >= listOfEmployes[secondEmployee].DateTo)
                        {
                            startOfWorkPeriod = listOfEmployes[firstEmployee].DateFrom;
                            endOfWorkPeriod = listOfEmployes[secondEmployee].DateTo;
                        }
                        else if (listOfEmployes[firstEmployee].DateFrom <= listOfEmployes[secondEmployee].DateFrom && listOfEmployes[firstEmployee].DateTo >= listOfEmployes[secondEmployee].DateTo)
                        {
                            startOfWorkPeriod = listOfEmployes[secondEmployee].DateFrom;
                            endOfWorkPeriod = listOfEmployes[secondEmployee].DateTo;
                        }



                        var employeesKvp = new KeyValuePair<int, int>(listOfEmployes[firstEmployee].Id, listOfEmployes[secondEmployee].Id);

                        //This keeps employee kvp unique, f.e. - pair of employees with id`s 1 and 2, should be the same as 2 and 1
                        var employeesKvpInReverse = new KeyValuePair<int, int>(listOfEmployes[secondEmployee].Id, listOfEmployes[firstEmployee].Id);

                        var newWorkPeriod = new WorkPeriod() { DateStart = startOfWorkPeriod, DateEnd = endOfWorkPeriod, ProjectId = listOfEmployes[firstEmployee].ProjectId};

                        if (!pairOfEmployeedWithWorkPeriods.ContainsKey(employeesKvp) && !pairOfEmployeedWithWorkPeriods.ContainsKey(employeesKvpInReverse))
                        {
                            pairOfEmployeedWithWorkPeriods.Add(employeesKvp, new List<WorkPeriod>());
                            pairOfEmployeedWithWorkPeriods[employeesKvp].Add(newWorkPeriod);
                        }
                        else if (pairOfEmployeedWithWorkPeriods.ContainsKey(employeesKvpInReverse))
                        {
                            pairOfEmployeedWithWorkPeriods[employeesKvpInReverse].Add(newWorkPeriod);
                        }
                        else
                        {
                            pairOfEmployeedWithWorkPeriods[employeesKvp].Add(newWorkPeriod);
                        }

                    }

                }
            }

            return pairOfEmployeedWithWorkPeriods;
        }

        private EmployeePairOutput GetResult(Dictionary<KeyValuePair<int, int>, List<WorkPeriod>> pairOfEmployeesWithWorkPeriods)
        {

            var result = new EmployeePairOutput();

            var totalOverLappingDays = 0;

            var totalNonOverlappingDays = 0;

            var totalDaysWorkedTogether = 0;

            //The purpose of this iteration is to calculate the total days a pair has worked, concidering the overlaps in dates, between the separate projects
            foreach (var kvp in pairOfEmployeesWithWorkPeriods)
            {
                //We assing this variable to 0 so we can reset it after each kvp
                totalDaysWorkedTogether = 0;

                //This loop goes through all possible combinations of each of the work periods for each kvp
                for (int firstWorkPeriod = 0; firstWorkPeriod < kvp.Value.Count; firstWorkPeriod++)
                {

                    totalNonOverlappingDays = 0;

                    totalOverLappingDays = 0;


                    //if there is only one work period for this kvp, we calculate the total days worked and we skip the code bellow
                    if (kvp.Value.Count == 1)
                    {
                        totalDaysWorkedTogether += kvp.Value[0].DateEnd.Subtract(kvp.Value[0].DateStart).Days + 1;
                        continue;
                    }

                    for (int nextWorkPeriod = firstWorkPeriod + 1; nextWorkPeriod < kvp.Value.Count; nextWorkPeriod++)
                    {

                        if (kvp.Value[firstWorkPeriod].DateStart <= kvp.Value[nextWorkPeriod].DateStart && kvp.Value[firstWorkPeriod].DateEnd >= kvp.Value[nextWorkPeriod].DateStart)
                        {
                            totalOverLappingDays += kvp.Value[nextWorkPeriod].DateEnd.Subtract(kvp.Value[firstWorkPeriod].DateStart).Days + 1;
                        }
                        else if (kvp.Value[nextWorkPeriod].DateStart <= kvp.Value[firstWorkPeriod].DateStart && kvp.Value[nextWorkPeriod].DateEnd >= kvp.Value[firstWorkPeriod].DateStart)
                        {
                            totalOverLappingDays += kvp.Value[firstWorkPeriod].DateEnd.Subtract(kvp.Value[nextWorkPeriod].DateStart).Days + 1;
                        }
                        else if (kvp.Value[nextWorkPeriod].DateStart >= kvp.Value[firstWorkPeriod].DateStart && kvp.Value[nextWorkPeriod].DateEnd <= kvp.Value[firstWorkPeriod].DateEnd)
                        {
                            totalOverLappingDays += kvp.Value[nextWorkPeriod].DateEnd.Subtract(kvp.Value[nextWorkPeriod].DateStart).Days + 1;
                        }
                        else if (kvp.Value[firstWorkPeriod].DateStart >= kvp.Value[nextWorkPeriod].DateStart && kvp.Value[firstWorkPeriod].DateEnd <= kvp.Value[nextWorkPeriod].DateEnd)
                        {
                            totalOverLappingDays += kvp.Value[firstWorkPeriod].DateEnd.Subtract(kvp.Value[firstWorkPeriod].DateStart).Days + 1;
                        }
                        else if ((kvp.Value[firstWorkPeriod].DateEnd < kvp.Value[nextWorkPeriod].DateStart && kvp.Value[firstWorkPeriod].DateStart < kvp.Value[nextWorkPeriod].DateStart) || (kvp.Value[firstWorkPeriod].DateStart > kvp.Value[nextWorkPeriod].DateStart && kvp.Value[firstWorkPeriod].DateEnd > kvp.Value[nextWorkPeriod].DateEnd))
                        {
                            //We enter here if the two dates compared have no overlap, the above code calculates the total working days of two overlapping periods
                            totalNonOverlappingDays += kvp.Value[nextWorkPeriod].DateEnd.Subtract(kvp.Value[nextWorkPeriod].DateStart).Days + 1;
                        }


                    }

                    //After each combination we check the sum of totalNonOverlappingDays and totalOverLappingDays and we compare it to the totalDaysWorkedTogether, after that we assign it accordingly:
                    if (totalNonOverlappingDays + totalOverLappingDays > totalDaysWorkedTogether)
                    {
                        totalDaysWorkedTogether = totalOverLappingDays + totalNonOverlappingDays;
                    }

                }

                //After we`v calculated the longest possible working period for the current kvp, we check if the result`s total working period is less, and if it is we assign the new kvp to the result

                if (totalDaysWorkedTogether > result.TotalDaysWorkedTogether)
                {
                    result.FirstEmployeeId = kvp.Key.Key;
                    result.SecondEmployeeId = kvp.Key.Value;
                    result.TotalDaysWorkedTogether = totalDaysWorkedTogether;
                    result.ProjectIdList.Clear();
                    result.ProjectIdList.AddRange(kvp.Value.Select(x => x.ProjectId).ToHashSet());
                }

            }


            return result;
        }
    }
}
