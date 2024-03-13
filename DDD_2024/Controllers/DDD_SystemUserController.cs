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
using Microsoft.DotNet.Scaffolding.Shared.Messaging;

namespace DDD_2024.Controllers
{
    public class DDD_SystemUserController : Controller
    {
        private readonly SystemUserContext _context;
        private readonly ISystemUserService _SystemUserService;

        public DDD_SystemUserController(SystemUserContext context, ISystemUserService systemUserService)
        {
            _context = context;
            _SystemUserService = systemUserService;
        }

        // GET: DDD_SystemUser
        public async Task<IActionResult> Index()
        {
              return _context.DDD_SystemUser != null ? 
                          View(await _context.DDD_SystemUser.ToListAsync()) :
                          Problem("Entity set 'SystemUserContext.DDD_SystemUser'  is null.");
        }

        // GET: DDD_SystemUser/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.DDD_SystemUser == null)
            {
                return NotFound();
            }

            var dDD_SystemUser = await _context.DDD_SystemUser
                .FirstOrDefaultAsync(m => m.UserID == id);
            if (dDD_SystemUser == null)
            {
                return NotFound();
            }

            return View(dDD_SystemUser);
        }

        // GET: DDD_SystemUser/Create
        public IActionResult Create()
        {
            DDD_SystemUser dDD_SystemUser = new DDD_SystemUser();
            dDD_SystemUser.UserID = _SystemUserService.NewUserID;
            dDD_SystemUser.CreateDate = DateTime.Now;

            return View(dDD_SystemUser);
        }

        // POST: DDD_SystemUser/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("UserID,UserName,UserPWD,Department,IsActive,CreateDate,UpdateDate")] DDD_SystemUser dDD_SystemUser)
        {
            if (ModelState.IsValid)
            {
                _context.Add(dDD_SystemUser);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(dDD_SystemUser);
        }

        // GET: DDD_SystemUser/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.DDD_SystemUser == null)
            {
                return NotFound();
            }

            var dDD_SystemUser = await _context.DDD_SystemUser.FindAsync(id);
            if (dDD_SystemUser == null)
            {
                return NotFound();
            }
            return View(dDD_SystemUser);
        }

        // POST: DDD_SystemUser/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("UserID,UserName,UserPWD,Department,IsActive,CreateDate,UpdateDate")] DDD_SystemUser dDD_SystemUser)
        {
            if (id != dDD_SystemUser.UserID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(dDD_SystemUser);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DDD_SystemUserExists(dDD_SystemUser.UserID))
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
            return View(dDD_SystemUser);
        }

        // GET: DDD_SystemUser/LogIn
        public IActionResult LogIn()
        {           
            return View();
        }

        // POST: DDD_SystemUser/LogIn
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogIn([Bind("UserID,UserName,UserPWD")] DDD_SystemUser dDD_SystemUser)
        {
            if (!_SystemUserService.IsCorrect(dDD_SystemUser.UserID, dDD_SystemUser.UserPWD))
            {
                ModelState.AddModelError("ErrorReport", "密碼錯誤");
                return View(dDD_SystemUser);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        private bool DDD_SystemUserExists(int id)
        {
          return (_context.DDD_SystemUser?.Any(e => e.UserID == id)).GetValueOrDefault();
        }
    }
}
