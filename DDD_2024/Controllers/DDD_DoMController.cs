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
    public class DDD_DoMController : Controller
    {
        private readonly DDD_DoMContext _context;

        public DDD_DoMController(DDD_DoMContext context)
        {
            _context = context;
        }

        // GET: DDD_DoM
        public async Task<IActionResult> Index()
        {
              return _context.DDD_DoM != null ? 
                          View(await _context.DDD_DoM.ToListAsync()) :
                          Problem("Entity set 'DDD_DoMContext.DDD_DoM'  is null.");
        }

        // GET: DDD_DoM/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null || _context.DDD_DoM == null)
            {
                return NotFound();
            }

            var dDD_DoM = await _context.DDD_DoM
                .FirstOrDefaultAsync(m => m.DoID == id);
            if (dDD_DoM == null)
            {
                return NotFound();
            }

            return View(dDD_DoM);
        }

        // GET: DDD_DoM/Create
        public IActionResult Create()
        {
            DDD_DoM dDD_DoM = new DDD_DoM();
            dDD_DoM.CreateDate = DateTime.Now.ToString("yyyy/MM/dd");

            return View(dDD_DoM);
        }

        // POST: DDD_DoM/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("DoID,VendorName,PartNo,CusName_CN,CusName_EN,ProApplication,ProjectNoted,ProjectAction,CreateDate,Applicant,Approver,ApproveDate")] DDD_DoM dDD_DoM)
        {
            if (ModelState.IsValid)
            {
                dDD_DoM.DoID = Guid.NewGuid();
                _context.Add(dDD_DoM);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(dDD_DoM);
        }

        // GET: DDD_DoM/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null || _context.DDD_DoM == null)
            {
                return NotFound();
            }

            var dDD_DoM = await _context.DDD_DoM.FindAsync(id);
            if (dDD_DoM == null)
            {
                return NotFound();
            }
            return View(dDD_DoM);
        }

        // POST: DDD_DoM/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("DoID,VendorName,PartNo,CusName_CN,CusName_EN,ProApplication,ProjectNoted,ProjectAction,CreateDate,Applicant,Approver,ApproveDate")] DDD_DoM dDD_DoM)
        {
            if (id != dDD_DoM.DoID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(dDD_DoM);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DDD_DoMExists(dDD_DoM.DoID))
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
            return View(dDD_DoM);
        }

        // GET: DDD_DoM/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null || _context.DDD_DoM == null)
            {
                return NotFound();
            }

            var dDD_DoM = await _context.DDD_DoM
                .FirstOrDefaultAsync(m => m.DoID == id);
            if (dDD_DoM == null)
            {
                return NotFound();
            }

            return View(dDD_DoM);
        }

        // POST: DDD_DoM/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            if (_context.DDD_DoM == null)
            {
                return Problem("Entity set 'DDD_DoMContext.DDD_DoM'  is null.");
            }
            var dDD_DoM = await _context.DDD_DoM.FindAsync(id);
            if (dDD_DoM != null)
            {
                _context.DDD_DoM.Remove(dDD_DoM);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DDD_DoMExists(Guid id)
        {
          return (_context.DDD_DoM?.Any(e => e.DoID == id)).GetValueOrDefault();
        }
    }
}
