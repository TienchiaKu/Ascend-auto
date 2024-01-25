using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DDD_2024.Data;
using DDD_2024.Models;

namespace DDD_2024.Controllers
{
    public class DDD_SystemUserController : Controller
    {
        private readonly SystemUserContext _context;

        public DDD_SystemUserController(SystemUserContext context)
        {
            _context = context;
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
            return View();
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

        // GET: DDD_SystemUser/Delete/5
        public async Task<IActionResult> Delete(int? id)
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

        // POST: DDD_SystemUser/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.DDD_SystemUser == null)
            {
                return Problem("Entity set 'SystemUserContext.DDD_SystemUser'  is null.");
            }
            var dDD_SystemUser = await _context.DDD_SystemUser.FindAsync(id);
            if (dDD_SystemUser != null)
            {
                _context.DDD_SystemUser.Remove(dDD_SystemUser);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DDD_SystemUserExists(int id)
        {
          return (_context.DDD_SystemUser?.Any(e => e.UserID == id)).GetValueOrDefault();
        }
    }
}
