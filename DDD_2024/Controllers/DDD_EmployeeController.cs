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

namespace DDD_2024.Controllers
{
    public class DDD_EmployeeController : Controller
    {
        private readonly DDD_EmployeeContext _context;
        private readonly IEmployeeService _employeeService;

        public DDD_EmployeeController(DDD_EmployeeContext context, IEmployeeService employeeService)
        {
            _context = context;
            _employeeService = employeeService;
        }

        // GET: DDD_Employee
        public async Task<IActionResult> Index()
        {
            var Dutyemployees = await _context.DDD_Employee
                .Where(e => e.OnDuty == "Y")
                .ToListAsync();

            return Dutyemployees != null ? 
                          View(Dutyemployees) :
                          Problem("查無資料");
        }

        // GET: DDD_Employee/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.DDD_Employee == null)
            {
                return NotFound();
            }

            var dDD_Employee = await _context.DDD_Employee
                .FirstOrDefaultAsync(m => m.EmpID == id);
            if (dDD_Employee == null)
            {
                return NotFound();
            }

            return View(dDD_Employee);
        }

        // GET: DDD_Employee/Create
        public IActionResult Create()
        {
            var model = new DDD_Employee();

            model.EmpID = _employeeService.MaxEmployeeID;
            model.CreateDate = DateTime.Now;

            return View(model);
        }

        // POST: DDD_Employee/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("EmpID,EmpName,OnDuty,PM,Sales,FAE,RBU,CreateDate")] DDD_Employee dDD_Employee)
        {
            if (ModelState.IsValid)
            {
                _context.Add(dDD_Employee);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(dDD_Employee);
        }

        // GET: DDD_Employee/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.DDD_Employee == null)
            {
                return NotFound();
            }

            var dDD_Employee = await _context.DDD_Employee.FindAsync(id);
            if (dDD_Employee == null)
            {
                return NotFound();
            }

            return View(dDD_Employee);
        }

        // POST: DDD_Employee/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("EmpID,EmpName,OnDuty,PM,Sales,FAE,RBU,CreateDate")] DDD_Employee dDD_Employee)
        {
            if (id != dDD_Employee.EmpID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
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

        private bool DDD_EmployeeExists(int id)
        {
          return (_context.DDD_Employee?.Any(e => e.EmpID == id)).GetValueOrDefault();
        }
    }
}
