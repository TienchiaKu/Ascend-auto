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
    public class DDD_DutyController : Controller
    {
        private readonly BizAutoContext _context;
        private readonly IDutyService _dutyService;
        private readonly IEmployeeService _employeeService;
        private readonly ICusVendorService _cusVendoeService;

        public DDD_DutyController(BizAutoContext context, IDutyService dutyService, IEmployeeService employeeService, ICusVendorService cusVendoeService)
        {
            _context = context;
            _dutyService = dutyService;
            _employeeService = employeeService;
            _cusVendoeService = cusVendoeService;
        }

        // GET: DDD_Duty
        public async Task<IActionResult> Index()
        {
            var model = await _context.DutyM.ToListAsync();

            if (model != null)
            {
                var dutyViewModels = model.Select(dutymodel => new DutyViewModel
                {
                    duty = dutymodel,
                    EmpName = _employeeService.GetEmployeeName(dutymodel.EmpID),
                    IsUseName = _employeeService.GetYesNoName(dutymodel.IsUse)
                }).ToList();

                return View(dutyViewModels);
            }
            else
            {
                return Problem("Entity set 'DutyContext' is null.");
            }
        }

        // GET: DDD_Duty/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.DutyM == null)
            {
                return NotFound();
            }

            var Duty = await _context.DutyM
                .FirstOrDefaultAsync(m => m.DutyID == id);
            if (Duty == null)
            {
                return NotFound();
            }
            else
            {
                var model = new DutyViewModel();
                model.duty = Duty;
                model.EmpName = _employeeService.GetEmployeeName(Duty.EmpID);
                model.IsUseName = _employeeService.GetYesNoName(Duty.IsUse);
                return View(model);
            }
        }

        // GET: DDD_Duty/Create
        public IActionResult Create()
        {
            var model = new DutyViewModel();
            model.duty.DutyID = _dutyService.NewDutyID;

            return View(model);
        }

        // POST: DDD_Duty/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("duty,EmpName")] DutyViewModel dutyViewModel)
        {
            if (ModelState.IsValid)
            {
                DutyM dDD_Duty = new DutyM();
                dDD_Duty.DutyID = dutyViewModel.duty.DutyID;
                dDD_Duty.EmpID = Convert.ToInt32(dutyViewModel.EmpName);
                dDD_Duty.Duty = dutyViewModel.duty.Duty;
                dDD_Duty.IsUse = dutyViewModel.duty.IsUse;
                dDD_Duty.UpdateDate = DateTime.Now;

                _context.Add(dDD_Duty);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(dutyViewModel);
        }

        // GET: DDD_Duty/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.DutyM == null)
            {
                return NotFound();
            }

            var Duty = await _context.DutyM.FindAsync(id);
            if (Duty == null)
            {
                return NotFound();
            }
            else
            {
                var model = new DutyViewModel();
                model.duty = Duty;

                return View(model);
            }
        }

        // POST: DDD_Duty/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("duty,VendorName,EmpName,IsUseName")] DutyViewModel dutyViewModel)
        {
            if (id != dutyViewModel.duty.DutyID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var model = new DutyM();
                    model = dutyViewModel.duty;
                    model.UpdateDate = DateTime.Now;

                    _context.Update(model);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DDD_DutyExists(dutyViewModel.duty.DutyID))
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
            return View(dutyViewModel);
        }

        private bool DDD_DutyExists(int id)
        {
          return (_context.DutyM?.Any(e => e.DutyID == id)).GetValueOrDefault();
        }
    }
}
