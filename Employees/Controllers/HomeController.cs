using Employees.Helpers;
using Employees.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Employees.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IEmployeeService _employeeService;


        public HomeController(ILogger<HomeController> logger, IEmployeeService employeeService)
        {
            _logger = logger;
            _employeeService = employeeService;
        }


        [HttpPost]
        public IActionResult LongestWorkingPairOfEmployees(IFormFile file)
        {
            var employeeList = FileProcessHelper.GenerateEmployeeList(file);

            if(employeeList == null)
            {
                return RedirectToAction("Error", "Home");
            }

            var result = this._employeeService.GetLongestWorkingPair(employeeList);

            return View(result);
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View();
        }

    }
}
