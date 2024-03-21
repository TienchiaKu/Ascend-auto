using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DDD_2024.Data;
using DDD_2024.Models;
using Microsoft.DotNet.Scaffolding.Shared.ProjectModel;
using Microsoft.CodeAnalysis;
using Microsoft.IdentityModel.Tokens;
using DDD_2024.Interfaces;

namespace DDD_2024.Controllers
{
    public class BonusCalController : Controller
    {
        private readonly ProjectMContext _projectMContext;
        private readonly ProjectDContext _projectDContext;
        private readonly DoContext _Docontext;
        private readonly Project_DIDWContext _project_DIDWContext;
        private readonly IDoService _doService;
        private readonly ICusVendoeService _cusVendoeService;
        private readonly IEmployeeService _employeeService;

        public BonusCalController(ProjectMContext projectMContext, ProjectDContext projectDContext, Project_DIDWContext project_DIDWContext, DoContext doContext,
            IDoService doService, ICusVendoeService cusVendoeService, IEmployeeService employeeService)
        {
            _projectMContext = projectMContext;
            _projectDContext = projectDContext;
            _Docontext = doContext;
            _project_DIDWContext = project_DIDWContext;
            _doService = doService;
            _cusVendoeService = cusVendoeService;
            _employeeService = employeeService;
        }

        // GET: BonusCal
        public async Task<IActionResult> Index()
        {
            var modelDo = await _Docontext.Project_DO.Where(p => p.Status == "C").ToListAsync();
            var modelDIn = await _project_DIDWContext.Project_DIDW.Where(p => p.DinStatus == "C").ToListAsync();
            var modelDWin = await _project_DIDWContext.Project_DIDW.Where(p => p.DwinStatus == "C").ToListAsync();

            var modelProjectMs = await _projectMContext.ProjectM.ToListAsync();
            var modelProjectDs = await _projectDContext.ProjectD.ToListAsync();

            List<BonusCalViewModel> list_BonusCal = new List<BonusCalViewModel>();
            //加入DO資料
            foreach (var item in modelDo)
            {
                var modelProjectM = modelProjectMs.Where(p => p.ProjectID == item.ProjectID).FirstOrDefault();
                var modelProjectD = modelProjectDs.Where(p => p.ProjectID == item.ProjectID && (p.Stage == "Do" || p.Stage == "DIN") ).FirstOrDefault();

                if(modelProjectM != null && modelProjectD != null)
                {
                    var model = new BonusCalViewModel
                    {
                        ProjectID = item.ProjectID,
                        PartNo = modelProjectD.PartNo,
                        Status = "DO"
                    };

                    if (!string.IsNullOrEmpty(modelProjectM.Cus_DB) && !string.IsNullOrEmpty(modelProjectM.CusID))
                    {
                        model.CusName = _cusVendoeService.GetvendorName(modelProjectM.Cus_DB, modelProjectM.CusID);
                    }

                    if (!string.IsNullOrEmpty(modelProjectD.VendorID))
                    {
                        model.VendorName = _cusVendoeService.GetvendorName("ASCEND", modelProjectD.VendorID);
                    }

                    list_BonusCal.Add(model);
                }
            }
            //加入DIN資料
            foreach (var item in modelDIn)
            {
                var modelProjectM = modelProjectMs.Where(p => p.ProjectID == item.ProjectID).FirstOrDefault();
                var modelProjectD = modelProjectDs.Where(p => p.ProjectID == item.ProjectID && p.Stage == "DIN").FirstOrDefault();

                if (modelProjectM != null && modelProjectD != null)
                {
                    var model = new BonusCalViewModel
                    {
                        ProjectID = item.ProjectID,
                        PartNo = modelProjectD.PartNo,
                        Status = "DIN"
                    };

                    if (!string.IsNullOrEmpty(modelProjectM.Cus_DB) && !string.IsNullOrEmpty(modelProjectM.CusID))
                    {
                        model.CusName = _cusVendoeService.GetvendorName(modelProjectM.Cus_DB, modelProjectM.CusID);
                    }

                    if (!string.IsNullOrEmpty(modelProjectD.VendorID))
                    {
                        model.VendorName = _cusVendoeService.GetvendorName("ASCEND", modelProjectD.VendorID);
                    }

                    list_BonusCal.Add(model);
                }
            }
            //加入DWIN資料
            foreach (var item in modelDIn)
            {
                var modelProjectM = modelProjectMs.Where(p => p.ProjectID == item.ProjectID).FirstOrDefault();
                var modelProjectD = modelProjectDs.Where(p => p.ProjectID == item.ProjectID && p.Stage == "DWIN").FirstOrDefault();

                if (modelProjectM != null && modelProjectD != null)
                {
                    var model = new BonusCalViewModel
                    {
                        ProjectID = item.ProjectID,
                        PartNo = modelProjectD.PartNo,
                        Status = "DWIN"
                    };

                    if (!string.IsNullOrEmpty(modelProjectM.Cus_DB) && !string.IsNullOrEmpty(modelProjectM.CusID))
                    {
                        model.CusName = _cusVendoeService.GetvendorName(modelProjectM.Cus_DB, modelProjectM.CusID);
                    }

                    if (!string.IsNullOrEmpty(modelProjectD.VendorID))
                    {
                        model.VendorName = _cusVendoeService.GetvendorName("ASCEND", modelProjectD.VendorID);
                    }

                    list_BonusCal.Add(model);
                }
            }

            if (list_BonusCal.Count > 0)
            {
                return View(list_BonusCal);
            }
            else
            {
                ViewBag.Message = "No match data.";
                return View();
            }
        }

        // GET: BonusCal/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null || _projectMContext.BonusCalViewModel == null)
            {
                return NotFound();
            }

            var bonusCalViewModel = await _projectMContext.BonusCalViewModel.FindAsync(id);
            if (bonusCalViewModel == null)
            {
                return NotFound();
            }
            return View(bonusCalViewModel);
        }

        // POST: BonusCal/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("ProjectID,Status,Cus_DB,CusID,VendorID,PartNo,VendorName,CusName")] BonusCalViewModel bonusCalViewModel)
        {
            if (id != bonusCalViewModel.ProjectID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _projectMContext.Update(bonusCalViewModel);
                    await _projectMContext.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BonusCalViewModelExists(bonusCalViewModel.ProjectID))
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
            return View(bonusCalViewModel);
        }

        [HttpPost]
        public IActionResult TransProjectDetail(string ProjectID, string Status)
        {
            if(Status == "DO")
            {
                return RedirectToAction("Details", "Do", new { ProjectID = ProjectID });
            }
            else if(Status == "DIN")
            {
                return RedirectToAction("Details", "Din", new { ProjectID = ProjectID });
            }
            else if (Status == "DWIN")
            {
                return RedirectToAction("Details", "Dwin", new { ProjectID = ProjectID });
            }
            else
            {
                return View();
            }
        }

        private bool BonusCalViewModelExists(string id)
        {
          return (_projectMContext.BonusCalViewModel?.Any(e => e.ProjectID == id)).GetValueOrDefault();
        }
    }
}
