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
using Microsoft.CodeAnalysis;
using System.Net.NetworkInformation;

namespace DDD_2024.Controllers
{
    public class DoController : Controller
    {
        private readonly DoContext _context;
        private readonly IDoService _doService;
        private readonly ICusVendoeService _cusVendoeService;
        private readonly IEmployeeService _employeeService;

        public DoController(DoContext context, IDoService doService, ICusVendoeService cusVendoeService, IEmployeeService employeeService)
        {
            _context = context;
            _doService = doService;
            _cusVendoeService = cusVendoeService;
            _employeeService = employeeService;
        }

        // GET: Do
        public async Task<IActionResult> Index()
        {
            var modelDO = await _context.Project_DO.ToListAsync();
            var modelProjectM = await _context.ProjectM.ToListAsync();
            var modelProjectD = await _context.ProjectD.ToListAsync();

            //加入modelDO的資料
            List<DoViewModel> list_doViewModel = new List<DoViewModel>();
            foreach (var item in modelDO)
            {
                var model = new DoViewModel
                {
                    DoID = item.DoID,
                    ProjectID = item.ProjectID,
                    vmCreateDate = item.CreateDate.Substring(0, 4) + "/" + item.CreateDate.Substring(4, 2) + "/" + item.CreateDate.Substring(6, 2),
                    ApplicantName = _employeeService.GetEmployeeName(item.ApplicantID),
                    StatusName = _doService.GetStatusName(item.Status),
                    DOStatus = item.Status

                };

                list_doViewModel.Add(model);
            }

            //加入modelProjectM和modelProjectD的資料
            foreach (var item in list_doViewModel)
            {
                var matchingProjectM = modelProjectM.FirstOrDefault(p => p.ProjectID == item.ProjectID);
                var matchingProjectD = modelProjectD.FirstOrDefault(k => k.ProjectID == item.ProjectID);

                if (matchingProjectM != null && matchingProjectD != null)
                {
                    // 將 matchingProjectM 的資料設定給 Cus_DB 屬性
                    item.CusName = _cusVendoeService.GetvendorName(matchingProjectM.Cus_DB, matchingProjectM.CusID);
                    item.VendorName = _cusVendoeService.GetvendorName("ASCEND", matchingProjectD.VendorID);
                    item.PartNo = matchingProjectD.PartNo;
                }
            }

            if (list_doViewModel != null)
            {
                return View(list_doViewModel);
            }
            else
            {
                return Problem("Entity set 'BizAutoContext.Project_DO'  is null.");
            }
        }

        // GET: Do/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null || _context.Project_DO == null)
            {
                return NotFound();
            }

            var project_DO = await _context.Project_DO
                .FirstOrDefaultAsync(m => m.DoID == id);
            if (project_DO == null)
            {
                return NotFound();
            }

            var modelProjectM = await _context.ProjectM.ToListAsync();
            var modelProjectD = await _context.ProjectD.ToListAsync();

            var matchingProjectM = modelProjectM.FirstOrDefault(p => p.ProjectID == project_DO.ProjectID);
            var matchingProjectD = modelProjectD.FirstOrDefault(k => k.ProjectID == project_DO.ProjectID);

            var model = new DoViewModel();

            if (matchingProjectM != null && matchingProjectD != null)
            {
                model.DoID = project_DO.DoID;
                model.ProjectID = matchingProjectM.ProjectID;
                model.DOStatus = project_DO.Status;
                model.StatusName = _doService.GetStatusName(project_DO.Status);
                model.vmCreateDate = project_DO.CreateDate.Substring(0, 4) + "/" + project_DO.CreateDate.Substring(4, 2) + "/" + project_DO.CreateDate.Substring(6, 2);
                model.CusName = _cusVendoeService.GetvendorName(matchingProjectM.Cus_DB, matchingProjectM.CusID);
                model.VendorName = _cusVendoeService.GetvendorName("ASCEND", matchingProjectD.VendorID);
                model.PartNo = matchingProjectD.PartNo;
                model.ProApp = matchingProjectM.ProApp;
                model.ApplicantName = _employeeService.GetEmployeeName(project_DO.ApplicantID);
            }
            return View(model);
        }

        // GET: Do/Create
        public IActionResult Create()
        {
            var model = new DoViewModel();

            
            return View();
        }

        // POST: Do/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CreateDate,Cus_DB,CusID,ProApp,VendorID,PartNo,ApplicantName")] DoViewModel doViewModel)
        {
            if (ModelState.IsValid)
            {
                string projectID = _doService.GetProjectID(DateTime.Now.ToString("yyyyMMdd"));
                string createDate = doViewModel.CreateDate.ToString("yyyyMMdd"); 

                // Insert a record into ProjectM
                var modelprojectM = new ProjectM
                {
                    ProjectID = projectID,
                    Status = "DO", // Status: DO
                    CreateDate = createDate,
                    Cus_DB = doViewModel.Cus_DB,
                    CusID = doViewModel.CusID,
                    ProApp = doViewModel.ProApp
                };

                // Insert a record into ProjectD
                var modelprojectD = new ProjectD
                {
                    ProjectID = projectID,
                    VendorID = doViewModel.VendorID,
                    PartNo = doViewModel.PartNo
                };

                // Insert a record into Project_DO
                var modelprojectDO = new Project_DO
                {
                    DoID = _doService.GetDOID(DateTime.Now.ToString("yyyyMMdd")),
                    ProjectID = projectID,
                    CreateDate = createDate,
                    ApplicantID = Convert.ToInt32(doViewModel.ApplicantName),
                    Status = "N" // Status: 新單
                };

                _context.Add(modelprojectM);
                _context.Add(modelprojectD);
                _context.Add(modelprojectDO);

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View();
        }

        // GET: Do/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null || _context.Project_DO == null)
            {
                return NotFound();
            }

            var project_DO = await _context.Project_DO.FindAsync(id);
            if (project_DO == null)
            {
                return NotFound();
            }
            return View(project_DO);
        }

        // POST: Do/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("DoID,ProjectID,CreateDate,ApplicantID,Status")] Project_DO project_DO)
        {
            if (id != project_DO.DoID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(project_DO);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!Project_DOExists(project_DO.DoID))
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
            return View(project_DO);
        }

        // POST: /ConfirmDOs
        [HttpPost]
        public IActionResult ConfirmDOs([FromBody] string[] DoIDs)
        {
            if(DoIDs.Length == 0)
            {
                return View();
            }
            
            var allProjectDOs = _context.Project_DO.ToList();

            foreach (var doId in DoIDs)
            {
                var modelToUpdate = allProjectDOs.FirstOrDefault(d => d.DoID == doId);

                if (modelToUpdate != null)
                {
                    modelToUpdate.Status = "C";  // 狀態改為審核通過
                    _context.Update(modelToUpdate); 
                }
            }
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        // POST: /RejectDOs
        [HttpPost]
        public IActionResult RejectDOs([FromBody] string[] DoIDs)
        {
            if (DoIDs.Length == 0)
            {
                return View();
            }

            var allProjectDOs = _context.Project_DO.ToList();

            foreach (var doId in DoIDs)
            {
                var modelToUpdate = allProjectDOs.FirstOrDefault(d => d.DoID == doId);

                if (modelToUpdate != null)
                {
                    modelToUpdate.Status = "R";  // 狀態改為審核拒絕
                    _context.Update(modelToUpdate);
                }
            }
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        // POST: /ConfirmDO
        [HttpPost]
        public IActionResult ConfirmDO([FromBody] string DoID)
        {
            if (string.IsNullOrEmpty(DoID))
            {
                return View();
            }

            var allProjectDOs = _context.Project_DO.ToList();

            var modelToUpdate = allProjectDOs.FirstOrDefault(d => d.DoID == DoID);
            if (modelToUpdate != null)
            {
                modelToUpdate.Status = "C";  // 狀態改為審核通過
                _context.Update(modelToUpdate);
            }
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        // POST: /RejectDO
        [HttpPost]
        public IActionResult RejectDO([FromBody] string DoID)
        {
            if (string.IsNullOrEmpty(DoID))
            {
                return View();
            }

            var allProjectDOs = _context.Project_DO.ToList();

            var modelToUpdate = allProjectDOs.FirstOrDefault(d => d.DoID == DoID);
            if (modelToUpdate != null)
            {
                modelToUpdate.Status = "R";  // 狀態改為審核拒絕
                _context.Update(modelToUpdate);
            }
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        private bool Project_DOExists(string id)
        {
          return (_context.Project_DO?.Any(e => e.DoID == id)).GetValueOrDefault();
        }
    }
}
