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
using Microsoft.CodeAnalysis;
using Microsoft.Build.Evaluation;
using System.Text.RegularExpressions;
using Microsoft.Identity.Client;

namespace DDD_2024.Controllers
{
    public class DinController : Controller
    {
        private readonly DoContext _context;
        private readonly ProjectMContext _projectMContext;
        private readonly ProjectDContext _projectDContext;
        private readonly Project_DIDWContext _project_DIDWContext;
        private readonly Project_EmpContext _project_EmpContext;
        private readonly IDoService _doService;
        private readonly ICusVendoeService _cusVendoeService;
        private readonly IEmployeeService _employeeService;

        public DinController(DoContext context, ProjectMContext projectMContext, ProjectDContext projectDContext, Project_DIDWContext project_DIDWContext, Project_EmpContext project_EmpContext,
            IDoService doService, ICusVendoeService cusVendoeService, IEmployeeService employeeService)
        {
            _context = context;
            _projectMContext = projectMContext;
            _projectDContext = projectDContext;
            _project_DIDWContext = project_DIDWContext;
            _project_EmpContext = project_EmpContext;
            _doService = doService;
            _cusVendoeService = cusVendoeService;
            _employeeService = employeeService;
        }

        // GET: Din
        public async Task<IActionResult> Index()
        {
            var modelProjectM = await _projectMContext.ProjectM.Where(p => p.Status == "DIN").ToListAsync();
            var modelProjectD = await _projectDContext.ProjectD.Where(p => p.Stage == "DIN").ToListAsync();
            var modelDIn = await _project_DIDWContext.Project_DIDW.ToListAsync();

            //加入modelDin的資料
            List<DinViewModel> list_dinViewModel = new List<DinViewModel>();
            foreach (var item in modelDIn)
            {
                var model = new DinViewModel
                {
                    DoID = item.DoID,
                    ProjectID = item.ProjectID,
                    StatusName = _doService.GetStatusName(item.DinStatus),
                    DinStatus = item.DinStatus,
                    PartNo = modelProjectD.FirstOrDefault(d => d.ProjectID == item.ProjectID)?.PartNo,
                    ProApp = modelProjectM.FirstOrDefault(d => d.ProjectID == item.ProjectID)?.ProApp,
                    ProModel = modelProjectM.FirstOrDefault(d => d.ProjectID == item.ProjectID)?.ProModel,
                    VendorID = modelProjectD.FirstOrDefault(d => d.ProjectID == item.ProjectID)?.VendorID,
                    Cus_DB = modelProjectM.FirstOrDefault(d => d.ProjectID == item.ProjectID)?.Cus_DB,
                    CusID = modelProjectM.FirstOrDefault(d => d.ProjectID == item.ProjectID)?.CusID
                };

                if (!string.IsNullOrEmpty(item.DinDate))
                {
                    model.DinDate = item.DinDate.Substring(0, 4) + "/" + item.DinDate.Substring(4, 2) + "/" + item.DinDate.Substring(6, 2);
                }

                if (!string.IsNullOrEmpty(model.VendorID))
                {
                    model.VendorName = _cusVendoeService.GetvendorName("ASCEND", model.VendorID);
                }

                if (!string.IsNullOrEmpty(model.Cus_DB) && !string.IsNullOrEmpty(model.CusID))
                {
                    model.VendorName = _cusVendoeService.GetvendorName(model.Cus_DB, model.CusID);
                }

                list_dinViewModel.Add(model);
            }

            if (list_dinViewModel != null)
            {
                return View(list_dinViewModel);
            }
            else
            {
                return Problem("Entity set Din is null.");
            }
        }

        // GET: Din/Details/5
        public async Task<IActionResult> Details(string? ProjectID)
        {
            var modelProjectM = await _projectMContext.ProjectM.ToListAsync();
            var modelProjectD = await _projectDContext.ProjectD.ToListAsync();
            var modelDIn = await _project_DIDWContext.Project_DIDW.ToListAsync();
            var modelPEmp = await _project_EmpContext.Project_Emp.ToListAsync();
            int empNo = 0; //紀錄員工編號用

            if (ProjectID == null || _projectMContext.ProjectM == null || _projectDContext.ProjectD == null || _project_DIDWContext.Project_DIDW == null)
            {
                return NotFound();
            }

            var model = new DinViewModel();
            model.DinDate = modelDIn.FirstOrDefault(d => d.ProjectID == ProjectID)?.DinDate;

            if (!string.IsNullOrEmpty(model.DinDate))
            {
                model.vmCreateDate = model.DinDate.Substring(0, 4) + "/" + model.DinDate.Substring(4, 2) + "/" + model.DinDate.Substring(6, 2);
            }

            model.ProjectID = ProjectID;
            model.DinStatus = modelDIn.FirstOrDefault(d => d.ProjectID == ProjectID)?.DinStatus;
            model.StatusName = _doService.GetStatusName(model.DinStatus);
            model.Cus_DB = modelProjectM.FirstOrDefault(d => d.ProjectID == ProjectID)?.Cus_DB;
            model.CusID = modelProjectM.FirstOrDefault(d => d.ProjectID == ProjectID)?.CusID;

            if (!string.IsNullOrEmpty(model.Cus_DB) && !string.IsNullOrEmpty(model.CusID))
            {
                model.CusName = _cusVendoeService.GetvendorName(model.Cus_DB, model.CusID);
            }

            model.EndCus = modelProjectM.FirstOrDefault(d => d.ProjectID == ProjectID)?.EndCus;
            model.ProApp = modelProjectM.FirstOrDefault(d => d.ProjectID == ProjectID)?.ProApp;
            model.ProModel = modelProjectM.FirstOrDefault(d => d.ProjectID == ProjectID)?.ProModel;
            model.EProduceYS = modelProjectM.FirstOrDefault(d => d.ProjectID == ProjectID)?.EProduceYS;
            model.VendorID = modelProjectD.FirstOrDefault(d => d.ProjectID == ProjectID && d.Stage == "DIN")?.VendorID;

            if (!string.IsNullOrEmpty(model.VendorID))
            {
                model.VendorName = _cusVendoeService.GetvendorName("ASCEND", model.VendorID);
            }

            model.PartNo = modelProjectD.FirstOrDefault(d => d.ProjectID == ProjectID && d.Stage == "DIN")?.PartNo;

            model.ELTR = modelProjectD.FirstOrDefault(d => d.ProjectID == ProjectID && d.Stage == "DIN")?.ELTR ?? 0;
            model.EGP = modelProjectD.FirstOrDefault(d => d.ProjectID == ProjectID && d.Stage == "DIN")?.EGP ?? 0;
            model.EFirstYQty = modelProjectD.FirstOrDefault(d => d.ProjectID == ProjectID && d.Stage == "DIN")?.EFirstYQty ?? 0;
            model.ESecondYQty = modelProjectD.FirstOrDefault(d => d.ProjectID == ProjectID && d.Stage == "DIN")?.ESecondYQty ?? 0;
            model.EThirdYQty = modelProjectD.FirstOrDefault(d => d.ProjectID == ProjectID && d.Stage == "DIN")?.EThirdYQty ?? 0;
            model.UFirstYPrice = modelProjectD.FirstOrDefault(d => d.ProjectID == ProjectID && d.Stage == "DIN")?.UFirstYPrice ?? 0;
            model.USecondYPrice = modelProjectD.FirstOrDefault(d => d.ProjectID == ProjectID && d.Stage == "DIN")?.USecondYPrice ?? 0;
            model.UThirdYPrice = modelProjectD.FirstOrDefault(d => d.ProjectID == ProjectID && d.Stage == "DIN")?.UThirdYPrice ?? 0;

            empNo = modelPEmp.FirstOrDefault(d => d.ProjectID == ProjectID && d.Duty == "PM")?.EmpID ?? 0;

            if(empNo != 0)
            {
                model.PM_EmpName = _employeeService.GetEmployeeName(empNo);
            }

            empNo = modelPEmp.FirstOrDefault(d => d.ProjectID == ProjectID && d.Duty == "SALES")?.EmpID ?? 0;

            if (empNo != 0)
            {
                model.Sales_EmpName = _employeeService.GetEmployeeName(empNo);
            }

            empNo = modelPEmp.FirstOrDefault(d => d.ProjectID == ProjectID && d.Duty == "FAE1")?.EmpID ?? 0;

            if (empNo != 0)
            {
                model.FAE1_EmpName = _employeeService.GetEmployeeName(empNo);
            }

            empNo = modelPEmp.FirstOrDefault(d => d.ProjectID == ProjectID && d.Duty == "FAE2")?.EmpID ?? 0;

            if (empNo != 0)
            {
                model.FAE2_EmpName = _employeeService.GetEmployeeName(empNo);
            }

            empNo = modelPEmp.FirstOrDefault(d => d.ProjectID == ProjectID && d.Duty == "FAE3")?.EmpID ?? 0;

            if (empNo != 0)
            {
                model.FAE3_EmpName = _employeeService.GetEmployeeName(empNo);
            }

            if (model == null)
            {
                return NotFound();
            }

            return View(model);
        }

        // GET: Din/Create
        public IActionResult Create(string projectID)
        {
            if (!string.IsNullOrEmpty(projectID))
            {
                if (_doService.chk_DoTransDin(projectID))
                {
                    return Content("此專案已生成Din或已結案");
                }

                var modelDOs = _context.Project_DO.ToList();
                var modelProjectMs = _context.ProjectM.ToList();
                var modelProjectDs = _context.ProjectD.ToList();
                
                var modelDO = modelDOs.FirstOrDefault(d => d.ProjectID == projectID);
                var modelProjectM = modelProjectMs.FirstOrDefault(d => d.ProjectID == projectID);
                var modelProjectD = modelProjectDs.FirstOrDefault(d => d.ProjectID == projectID);
                
                var model = new DinViewModel();
                
                if (modelDO != null && modelProjectM != null && modelProjectD != null)
                {
                    model.ProjectID = projectID;
                    model.DoID = modelDO.DoID;
                    model.ProApp = modelProjectM.ProApp;
                    model.VendorID = modelProjectD.VendorID;

                    if (!string.IsNullOrEmpty(modelProjectD.VendorID))
                    {
                        model.VendorName = _cusVendoeService.GetvendorName("ASCEND", modelProjectD.VendorID);
                    }

                    if (!string.IsNullOrEmpty(modelProjectM.Cus_DB) && !string.IsNullOrEmpty(modelProjectM.CusID))
                    {
                        model.Cus_DB = modelProjectM.Cus_DB;
                        model.CusID = modelProjectM.CusID;
                        model.CusName = _cusVendoeService.GetvendorName(modelProjectM.Cus_DB, modelProjectM.CusID);
                    }

                    model.vmCreateDate = modelProjectM.CreateDate;
                    model.ProApp = modelProjectM.ProApp;
                    model.PartNo = modelProjectD.PartNo;
                }               
                return View(model);
            }
            else
            {
                return View(new DinViewModel());
            }         
        }

        // POST: Din/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ProjectID,DoID,VendorID,VendorName,PartNo,CusName,EndCus,ProApp,ProModel,EProduceYS,ELTR,EGP,EFirstYQty,ESecondYQty,EThirdYQty,UFirstYPrice,USecondYPrice,UThirdYPrice,PM_EmpName,Sales_EmpName,FAE1_EmpName,FAE1_Bonus,FAE2_EmpName,FAE2_Bonus,FAE3_EmpName,FAE3_Bonus")] DinViewModel dinViewModel)
        {
            if (ModelState.IsValid)
            {
                //檢查資料
                string error_msg = string.Empty;
                
                //dinViewModel.EProduceYS格式是否為2024Q1
                if (!string.IsNullOrEmpty(dinViewModel.EProduceYS))
                {
                    if(!Regex.IsMatch(dinViewModel.EProduceYS.Substring(0, 4), @"^\d+$") || dinViewModel.EProduceYS[4] != 'Q' || !"1234".Contains(dinViewModel.EProduceYS[5]))
                    {
                        error_msg += "預估量產時間格式錯誤\n";
                    }
                }
                //檢查FAE bonus相加是否小於100%
                if(dinViewModel.FAE1_Bonus + dinViewModel.FAE2_Bonus + dinViewModel.FAE3_Bonus > 1)
                {
                    error_msg += "FAE bonus相加必須小於100%\n";
                }
                
                if (!string.IsNullOrEmpty(error_msg)){
                
                    return Content(error_msg);
                }
                
                if (!string.IsNullOrEmpty(dinViewModel.ProjectID))
                {
                    string updateDate = DateTime.Now.ToString("yyyyMMdd");
                    int proEmpSEQ = _doService.NewProjectEmpSEQ;
                
                    // Update ProjectM
                    var modelprojectM = new ProjectM
                    {
                        ProjectID = dinViewModel.ProjectID,
                        Status = "DIN", // Status: DIN
                        EndCus = dinViewModel.EndCus,
                        ProModel = dinViewModel.ProModel,
                        EProduceYS = dinViewModel.EProduceYS,
                        UpdateDate = updateDate
                    };
                    _projectMContext.Update(modelprojectM);
                    await _projectMContext.SaveChangesAsync();

                    // Update ProjectD
                    var modelprojectD = new ProjectD
                    {
                        ProjectID = dinViewModel.ProjectID,
                        VendorID = dinViewModel.VendorID,
                        PartNo = dinViewModel.PartNo,
                        ELTR = dinViewModel.ELTR,
                        EGP = dinViewModel.EGP,
                        EFirstYQty = dinViewModel.EFirstYQty,
                        ESecondYQty = dinViewModel.ESecondYQty,
                        EThirdYQty  = dinViewModel.EThirdYQty,
                        UFirstYPrice = dinViewModel.UFirstYPrice,
                        USecondYPrice = dinViewModel.USecondYPrice,
                        UThirdYPrice = dinViewModel.UThirdYPrice,
                        Stage = "DIN"
                    };
                    _projectDContext.Update(modelprojectD);
                    await _projectDContext.SaveChangesAsync();

                    // Insert a record into Project_DIDW
                    var modelprojectDI = new Project_DIDW
                    {
                        ProjectID = dinViewModel.ProjectID,
                        DoID = dinViewModel.DoID,
                        DinDate = updateDate,
                        DinStatus = "N"
                    };
                    _project_DIDWContext.Add(modelprojectDI);
                    await _project_DIDWContext.SaveChangesAsync();

                    // Insert PM record into Project_Emp
                    var PM_Emp = new Project_Emp
                    {
                        SEQ = proEmpSEQ,
                        ProjectID = dinViewModel.ProjectID,
                        EmpID = Convert.ToInt32(dinViewModel.PM_EmpName),
                        Duty = "PM",
                        BonusP = 0.2
                    };
                    _project_EmpContext.Add(PM_Emp);
                    await _project_EmpContext.SaveChangesAsync();
                    proEmpSEQ++;

                    // Insert Sale record into Project_Emp
                    var Sale_Emp = new Project_Emp
                    {
                        SEQ = proEmpSEQ,
                        ProjectID = dinViewModel.ProjectID,
                        EmpID = Convert.ToInt32(dinViewModel.Sales_EmpName),
                        Duty = "SALES",
                        BonusP = 0.4
                    };
                    _project_EmpContext.Add(Sale_Emp);
                    await _project_EmpContext.SaveChangesAsync();
                    proEmpSEQ++;

                    // Insert FAE1 record into Project_Emp
                    if (dinViewModel.FAE1_EmpName != "999")
                    {
                        var FAE1_Emp = new Project_Emp
                        {
                            SEQ = proEmpSEQ,
                            ProjectID = dinViewModel.ProjectID,
                            EmpID = Convert.ToInt32(dinViewModel.FAE1_EmpName),
                            Duty = "FAE1",
                            BonusP = dinViewModel.FAE1_Bonus
                        };
                        _project_EmpContext.Add(FAE1_Emp);
                        await _project_EmpContext.SaveChangesAsync();
                        proEmpSEQ++;
                    }
                
                    // Insert FAE2 record into Project_Emp
                    if (dinViewModel.FAE2_EmpName != "999")
                    {
                        var FAE2_Emp = new Project_Emp
                        {
                            SEQ = proEmpSEQ,
                            ProjectID = dinViewModel.ProjectID,
                            EmpID = Convert.ToInt32(dinViewModel.FAE2_EmpName),
                            Duty = "FAE2",
                            BonusP = dinViewModel.FAE2_Bonus
                        };
                        _project_EmpContext.Add(FAE2_Emp);
                        await _project_EmpContext.SaveChangesAsync();
                        proEmpSEQ++;
                    }
                
                    // Insert FAE3 record into Project_Emp
                    if (dinViewModel.FAE3_EmpName != "999")
                    {
                        var FAE3_Emp = new Project_Emp
                        {
                            SEQ = proEmpSEQ,
                            ProjectID = dinViewModel.ProjectID,
                            EmpID = Convert.ToInt32(dinViewModel.FAE3_EmpName),
                            Duty = "FAE3",
                            BonusP = dinViewModel.FAE3_Bonus
                        };
                        _project_EmpContext.Add(FAE3_Emp);
                        await _project_EmpContext.SaveChangesAsync();
                    }                
                }

                return RedirectToAction(nameof(Index));
            }           
            return View();
        }

        // GET: Din/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Project_DIDW == null)
            {
                return NotFound();
            }

            var dinViewModel = await _context.Project_DIDW.FindAsync(id);
            if (dinViewModel == null)
            {
                return NotFound();
            }
            return View(dinViewModel);
        }

        // POST: Din/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string ProjectID, [Bind("DinID,DinDate,ProjectID,DoID,DinStatus,Cus_DB,CusID,EndCus,ProApp,ProModel,IsInte,EProduceYS,VendorID,PartNo,ELTR,EGP,EFirstYQty,ESecondYQty,EThirdYQty,UFirstYPrice,USecondYPrice,UThirdYPrice,PM_Emp,PM_Bonus,Sales_Emp,Sales_Bonus,FAE1_Emp,FAE1_Bonus,FAE2_Emp,FAE2_Bonus,FAE3_Emp,FAE3_Bonus")] DinViewModel dinViewModel)
        {
            if (ProjectID != dinViewModel.ProjectID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(dinViewModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DinViewModelExists(dinViewModel.ProjectID))
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
            return View(dinViewModel);
        }

        // POST: /ConfirmDin
        [HttpPost]
        public IActionResult ConfirmDin([FromBody] string ProjectID)
        {
            if (string.IsNullOrEmpty(ProjectID))
            {
                return View();
            }

            var allProjectDins = _project_DIDWContext.Project_DIDW.ToList();

            var modelToUpdate = allProjectDins.FirstOrDefault(d => d.ProjectID == ProjectID);
            if (modelToUpdate != null)
            {
                modelToUpdate.DinStatus = "C";  // 狀態改為審核通過
                _project_DIDWContext.Update(modelToUpdate);
            }
            _project_DIDWContext.SaveChanges();

            return RedirectToAction("Index");
        }

        // POST: /RejectDin
        [HttpPost]
        public IActionResult RejectDin([FromBody] string ProjectID)
        {
            if (string.IsNullOrEmpty(ProjectID))
            {
                return View();
            }

            var allProjectDins = _project_DIDWContext.Project_DIDW.ToList();

            var modelToUpdate = allProjectDins.FirstOrDefault(d => d.ProjectID == ProjectID);
            if (modelToUpdate != null)
            {
                modelToUpdate.DinStatus = "R";  // 狀態改為審核拒絕
                _project_DIDWContext.Update(modelToUpdate);
            }
            _project_DIDWContext.SaveChanges();

            return RedirectToAction("Index");
        }

        private bool DinViewModelExists(string ProjectID)
        {
          return (_context.Project_DIDW?.Any(e => e.ProjectID == ProjectID)).GetValueOrDefault();
        }
    }
}
