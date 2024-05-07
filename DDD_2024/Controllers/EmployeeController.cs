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
            var employees = await _context.employeeM
                .Where(e => e.OnDuty == "Y")
                .ToListAsync();

            if (employees != null)
            {
                var employeeViewModels = employees.Select(employee => new EmployeeViewModel
                {
                    employee = employee,
                    OnDuty_CN = _employeeService.GetYesNoName(employee.OnDuty)
                }).ToList();

                return View(employeeViewModels);
            }
            else
            {
                return Problem("Entity set EmployeeM is null.");
            }
        }

        // GET: DDD_Employee/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.employeeM == null)
            {
                return NotFound();
            }

            var dDD_Employee = await _context.employeeM
                .FirstOrDefaultAsync(m => m.EmpID == id);
            if (dDD_Employee == null)
            {
                return NotFound();
            }
            if (string.IsNullOrEmpty(dDD_Employee.OnDuty))
            {
                return NotFound("找不到職務資料");
            }
            else
            {
                EmployeeViewModel employeeViewModel = new EmployeeViewModel
                {
                    employee = dDD_Employee,
                    OnDuty_CN = _employeeService.GetYesNoName(dDD_Employee.OnDuty)
                };

                return View(employeeViewModel);
            }
        }

        // GET: DDD_Employee/Create
        public IActionResult Create()
        {
            var model = new EmployeeM();

            model.EmpID = _employeeService.NewEmployeeID                                                                                              ;
            model.UpdateDate = DateTime.Now;

            return View(model);
        }

        // POST: DDD_Employee/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("EmpID,EmpName,OnDuty")] EmployeeM dDD_Employee)
        {
            if (ModelState.IsValid)
            {
                //檢查名稱是否重複
                if (_employeeService.CheckEmpName(dDD_Employee.EmpName))
                {
                    ModelState.AddModelError(string.Empty, "員工姓名重複");
                }
                else
                {
                    dDD_Employee.UpdateDate = DateTime.Now;
                    _context.Add(dDD_Employee);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            return View(dDD_Employee);
        }

        // GET: DDD_Employee/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.employeeM == null)
            {
                return NotFound();
            }

            var dDD_Employee = await _context.employeeM.FindAsync(id);
            if (dDD_Employee == null)
            {
                return NotFound();
            }

            return View(dDD_Employee);
        }

        // POST: EmployeeM/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("EmpID,EmpName,OnDuty")] EmployeeM dDD_Employee)
        {
            if (id != dDD_Employee.EmpID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    dDD_Employee.UpdateDate = DateTime.Now;

                    _context.Update(dDD_Employee);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DDD_EmployeeExists(dDD_Employee.EmpID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(dDD_Employee);
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
