using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DDD_2024.Data;
using DDD_2024.Models;
using DDD_2024.Interfaces;
using DDD_2024.Services;
using Microsoft.IdentityModel.Tokens;

namespace DDD_2024.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly BizAutoContext _context;
        private readonly IEmployeeService _employeeService;
        private readonly IHttpContextAccessor _contxt;

        public EmployeeController(BizAutoContext context, IEmployeeService employeeService, IHttpContextAccessor contxt)
        {
            _context = context;
            _employeeService = employeeService;
            _contxt = contxt;
        }

        // GET: EmployeeM
        public async Task<IActionResult> Index()
        {
            var model = await _employeeService.GetEmployees();

            if(model != null)
            {
                return View(model);
            }
            else
            {
                return Problem("Entity set EmployeeM is null.");
            }
        }

        // GET: EmployeeM/IndexFilter
        public async Task<IActionResult> IndexFilter([FromQuery] string IsNameFilter)
        {
            var modelFilter = new EmployeeFilterViewModel();

            if (!string.IsNullOrEmpty(IsNameFilter))
            {
                modelFilter.IsName = IsNameFilter;
            }
            
            var model = await _employeeService.GetEmployeesFilter(modelFilter);

            if (model != null)
            {
                return PartialView("_EmployeePartial", model);
            }
            else
            {
                return Problem("Entity set 'Employee's' is null.");
            }
        }

        // GET: Employee/Details/5
        public async Task<IActionResult> Details(int EmpId)
        {
            var model = await _employeeService.GetEmployee(EmpId);

            return View(model);
        }

        // GET: Employee/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: DDD_Employee/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("EmpName,Region,userPWD,OnDuty,isSales,isPM,isFAE,isRBU,Auth")] EmpCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                TempData["Message"] = await _employeeService.CreateEmp(model);
            }
            return RedirectToAction("Index");
        }

        // GET: EmployeeM/Edit/5
        public async Task<IActionResult> Edit(int EmpId)
        {
            var model = await _employeeService.GetEmployee(EmpId);

            return View(model);
        }

        // POST: EmployeeM/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([Bind("EmpId,EmpName,Region,userPWD,OnDuty,isSales,isPM,isFAE,isRBU,Auth")] EmpEditViewModel model)
        {
            if (ModelState.IsValid)
            {
                TempData["Message"] = await _employeeService.EditEmployee(model);
            }
            return RedirectToAction("Index");
        }

        // GET: EmployeeM/LogIn
        public IActionResult LogIn()
        {
            return View();
        }

        // POST: EmployeeM/LogIn
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogIn([Bind("EmpID,EmpName,OnDuty,userPWD")] EmployeeM employeeM)
        {
            if (_employeeService.Check_Login(employeeM))
            {
                // 儲存EmpID到Session
                if (_contxt.HttpContext != null)
                {
                    employeeM.EmpName = _employeeService.GetEmployeeName(employeeM.EmpID);
                    _contxt.HttpContext.Session.SetString("EmpName", employeeM.EmpName);
                    _contxt.HttpContext.Session.SetInt32("EmpID", employeeM.EmpID);
                }

                ModelState.AddModelError("ErrorReport", "密碼錯誤");
                return RedirectToAction("Index", "Home");
            }
            else
            {
                // 儲存EmpID到Session
                //if(contxt.HttpContext != null && !string.IsNullOrEmpty(employeeM.EmpName))
                //{
                //    contxt.HttpContext.Session.SetString("EmpName", employeeM.EmpName);
                //    contxt.HttpContext.Session.SetInt32("EmpID", employeeM.EmpID);
                //}            
                return View(employeeM);
            }
        }

        private bool DDD_EmployeeExists(int id)
        {
          return (_context.employeeM?.Any(e => e.EmpID == id)).GetValueOrDefault();
        }

    }
}
