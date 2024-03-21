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
using Humanizer;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;

namespace DDD_2024.Controllers
{
    public class DwinController : Controller
    {
        private readonly ProjectMContext _projectMContext;
        private readonly ProjectDContext _projectDContext;
        private readonly Project_DIDWContext _project_DIDWContext;
        private readonly Project_EmpContext _project_EmpContext;
        private readonly IDoService _doService;
        private readonly ICusVendoeService _cusVendoeService;
        private readonly IEmployeeService _employeeService;

        public DwinController(ProjectMContext projectMContext, ProjectDContext projectDContext, Project_DIDWContext project_DIDWContext, Project_EmpContext project_EmpContext,
            IDoService doService, ICusVendoeService cusVendoeService, IEmployeeService employeeService)
        {
            _projectMContext = projectMContext;
            _projectDContext = projectDContext;
            _project_DIDWContext = project_DIDWContext;
            _project_EmpContext = project_EmpContext;
            _doService = doService;
            _cusVendoeService = cusVendoeService;
            _employeeService = employeeService;
        }

        // GET: DwinViewModels
        public async Task<IActionResult> Index()
        {
            var modelProjectM = await _projectMContext.ProjectM.Where(p => p.Status == "DWIN").ToListAsync();
            var modelProjectD = await _projectDContext.ProjectD.Where(p => p.Stage == "DWIN").ToListAsync();
            var modelDWin = await _project_DIDWContext.Project_DIDW.Where(d => d.DwinDate != null).ToListAsync();

            //加入modelDwin的資料
            List<DwinViewModel> list_dwinViewModel = new List<DwinViewModel>();
            foreach (var item in modelDWin)
            {
                var model = new DwinViewModel
                {
                    ProjectID = item.ProjectID,
                    StatusName = _doService.GetStatusName(item.DwinStatus),
                    DwinStatus = item.DwinStatus,
                    PartNo = modelProjectD.FirstOrDefault(d => d.ProjectID == item.ProjectID)?.PartNo,
                    ProApp = modelProjectM.FirstOrDefault(d => d.ProjectID == item.ProjectID)?.ProApp,
                    ProModel = modelProjectM.FirstOrDefault(d => d.ProjectID == item.ProjectID)?.ProModel,
                    VendorID = modelProjectD.FirstOrDefault(d => d.ProjectID == item.ProjectID)?.VendorID,
                    Cus_DB = modelProjectM.FirstOrDefault(d => d.ProjectID == item.ProjectID)?.Cus_DB,
                    CusID = modelProjectM.FirstOrDefault(d => d.ProjectID == item.ProjectID)?.CusID
                };

                if (!string.IsNullOrEmpty(item.DwinDate))
                {
                    model.DwinDate = item.DwinDate.Substring(0, 4) + "/" + item.DwinDate.Substring(4, 2) + "/" + item.DwinDate.Substring(6, 2);
                }

                if (!string.IsNullOrEmpty(model.VendorID))
                {
                    model.VendorName = _cusVendoeService.GetvendorName("ASCEND", model.VendorID);
                }

                if (!string.IsNullOrEmpty(model.Cus_DB) && !string.IsNullOrEmpty(model.CusID))
                {
                    model.VendorName = _cusVendoeService.GetvendorName(model.Cus_DB, model.CusID);
                }

                list_dwinViewModel.Add(model);
            }

            if (list_dwinViewModel.Count > 0)
            {
                return View(list_dwinViewModel);
            }
            else
            {
                ViewBag.Message = "DWIN Data not exist.";
                return View();
            }
        }

        // GET: DwinViewModels/Details/5
        public async Task<IActionResult> Details(string? ProjectID)
        {
            var modelProjectM = await _projectMContext.ProjectM.ToListAsync();
            var modelProjectD = await _projectDContext.ProjectD.ToListAsync();
            var modelDWin = await _project_DIDWContext.Project_DIDW.ToListAsync();
            var modelPEmp = await _project_EmpContext.Project_Emp.ToListAsync();
            int empNo = 0; //紀錄員工編號用

            if (ProjectID == null || _projectMContext.ProjectM == null || _projectDContext.ProjectD == null || _project_DIDWContext.Project_DIDW == null)
            {
                return NotFound();
            }

            var model = new DwinViewModel();
            model.DwinDate = modelDWin.FirstOrDefault(d => d.ProjectID == ProjectID)?.DwinDate;

            if (!string.IsNullOrEmpty(model.DwinDate))
            {
                model.vmCreateDate = model.DwinDate.Substring(0, 4) + "/" + model.DwinDate.Substring(4, 2) + "/" + model.DwinDate.Substring(6, 2);
            }

            model.ProjectID = ProjectID;
            model.DwinStatus = modelDWin.FirstOrDefault(d => d.ProjectID == ProjectID)?.DinStatus;
            model.StatusName = _doService.GetStatusName(model.DwinStatus);
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
            model.VendorID = modelProjectD.FirstOrDefault(d => d.ProjectID == ProjectID && d.Stage == "DWIN")?.VendorID;

            if (!string.IsNullOrEmpty(model.VendorID))
            {
                model.VendorName = _cusVendoeService.GetvendorName("ASCEND", model.VendorID);
            }

            model.PartNo = modelProjectD.FirstOrDefault(d => d.ProjectID == ProjectID && d.Stage == "DWIN")?.PartNo;

            model.ELTR = modelProjectD.FirstOrDefault(d => d.ProjectID == ProjectID && d.Stage == "DWIN")?.ELTR ?? 0;
            model.EGP = modelProjectD.FirstOrDefault(d => d.ProjectID == ProjectID && d.Stage == "DWIN")?.EGP ?? 0;
            model.EFirstYQty = modelProjectD.FirstOrDefault(d => d.ProjectID == ProjectID && d.Stage == "DWIN")?.EFirstYQty ?? 0;
            model.ESecondYQty = modelProjectD.FirstOrDefault(d => d.ProjectID == ProjectID && d.Stage == "DWIN")?.ESecondYQty ?? 0;
            model.EThirdYQty = modelProjectD.FirstOrDefault(d => d.ProjectID == ProjectID && d.Stage == "DWIN")?.EThirdYQty ?? 0;
            model.UFirstYPrice = modelProjectD.FirstOrDefault(d => d.ProjectID == ProjectID && d.Stage == "DWIN")?.UFirstYPrice ?? 0;
            model.USecondYPrice = modelProjectD.FirstOrDefault(d => d.ProjectID == ProjectID && d.Stage == "DWIN")?.USecondYPrice ?? 0;
            model.UThirdYPrice = modelProjectD.FirstOrDefault(d => d.ProjectID == ProjectID && d.Stage == "DWIN")?.UThirdYPrice ?? 0;

            empNo = modelPEmp.FirstOrDefault(d => d.ProjectID == ProjectID && d.Duty == "PM")?.EmpID ?? 0;

            if (empNo != 0)
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

        // GET: DwinViewModels/Create
        public IActionResult Create(string projectID)
        {
            if (!string.IsNullOrEmpty(projectID))
            {
                if (_doService.chk_DoTransDin(projectID))
                {
                    return Content("此專案已生成Dwin或已結案");
                }

                var modelProjectMs = _projectMContext.ProjectM.ToList();
                var modelProjectDs = _projectDContext.ProjectD.ToList();
                var modelProjectDins = _project_DIDWContext.Project_DIDW.ToList();
                var modelProjectEmps = _project_EmpContext.Project_Emp.ToList();

                var modelProjectM = modelProjectMs.FirstOrDefault(d => d.ProjectID == projectID);
                var modelProjectD = modelProjectDs.FirstOrDefault(d => d.ProjectID == projectID & d.Stage == "DIN");
                var modelProjectDin = modelProjectDins.FirstOrDefault(d => d.ProjectID == projectID);
                var Emp_PM = modelProjectEmps.FirstOrDefault(d => d.ProjectID == projectID  && d.Duty == "PM");
                var Emp_SALES = modelProjectEmps.FirstOrDefault(d => d.ProjectID == projectID && d.Duty == "SALES");
                var Emp_FAE1 = modelProjectEmps.FirstOrDefault(d => d.ProjectID == projectID && d.Duty == "FAE1");
                var Emp_FAE2 = modelProjectEmps.FirstOrDefault(d => d.ProjectID == projectID && d.Duty == "FAE2");
                var Emp_FAE3 = modelProjectEmps.FirstOrDefault(d => d.ProjectID == projectID && d.Duty == "FAE3");

                var model = new DwinViewModel();

                if (modelProjectDin != null && modelProjectM != null && modelProjectD != null)
                {
                    model.ProjectID = projectID;

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
                    model.EndCus = modelProjectM.EndCus;
                    model.ProApp = modelProjectM.ProApp;
                    model.ProModel = modelProjectM.ProModel;
                    model.IsInte = modelProjectM.IsInte;
                    model.EProduceYS = modelProjectM.EProduceYS;
                    model.PartNo = modelProjectD.PartNo;
                    model.ELTR = modelProjectD.ELTR;
                    model.EGP = modelProjectD.EGP;
                    model.EFirstYQty = modelProjectD.EFirstYQty;
                    model.ESecondYQty = modelProjectD.ESecondYQty;
                    model.EThirdYQty = modelProjectD.EThirdYQty;
                    model.UFirstYPrice = modelProjectD.UFirstYPrice;
                    model.USecondYPrice = modelProjectD.USecondYPrice;
                    model.UThirdYPrice = modelProjectD.UThirdYPrice;

                    if (Emp_PM != null)
                    {
                        model.PM_EmpName = _employeeService.GetEmployeeName(Emp_PM.EmpID);
                    }

                    if (Emp_SALES != null) 
                    {
                        model.Sales_EmpName = _employeeService.GetEmployeeName(Emp_SALES.EmpID);
                    }

                    if (Emp_FAE1 != null)
                    {
                        model.FAE1_EmpName = _employeeService.GetEmployeeName(Emp_FAE1.EmpID);
                    }

                    if (Emp_FAE2 != null)
                    {
                        model.FAE2_EmpName = _employeeService.GetEmployeeName(Emp_FAE2.EmpID);
                    }

                    if (Emp_FAE3 != null)
                    {
                        model.FAE3_EmpName = _employeeService.GetEmployeeName(Emp_FAE3.EmpID);
                    }
                }
                return View(model);
            }
            else
            {
                return View(new DwinViewModel());
            }
        }

        // POST: DwinViewModels/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ProjectID,DwinDate,DwinStatus,Cus_DB,CusID,EndCus,ProApp,ProModel,IsInte,EProduceYS,VendorID,PartNo,ELTR,EGP,EFirstYQty,ESecondYQty,EThirdYQty,UFirstYPrice,USecondYPrice,UThirdYPrice,PM_EmpName,PM_Bonus,Sales_EmpName,Sales_Bonus,FAE1_EmpName,FAE1_Bonus,FAE2_EmpName,FAE2_Bonus,FAE3_EmpName,FAE3_Bonus,vmCreateDate,StatusName,VendorName,CusName")] DwinViewModel dwinViewModel)
        {
            if (ModelState.IsValid)
            {
                //檢查資料
                string error_msg = string.Empty;

                if (!string.IsNullOrEmpty(dwinViewModel.EProduceYS))
                {
                    if (!Regex.IsMatch(dwinViewModel.EProduceYS.Substring(0, 4), @"^\d+$") || dwinViewModel.EProduceYS[4] != 'Q' || !"1234".Contains(dwinViewModel.EProduceYS[5]))
                    {
                        error_msg += "預估量產時間格式錯誤\n";
                    }
                }
                //檢查FAE bonus相加是否小於100%
                if (dwinViewModel.FAE1_Bonus + dwinViewModel.FAE2_Bonus + dwinViewModel.FAE3_Bonus > 1)
                {
                    error_msg += "FAE bonus相加必須小於100%\n";
                }

                if (!string.IsNullOrEmpty(error_msg))
                {

                    return Content(error_msg);
                }

                if (!string.IsNullOrEmpty(dwinViewModel.ProjectID))
                {
                    string updateDate = DateTime.Now.ToString("yyyyMMdd");

                    // Update ProjectM
                    var modelProjectM = _projectMContext.ProjectM.FirstOrDefault(p => p.ProjectID == dwinViewModel.ProjectID);

                    if(modelProjectM != null)
                    {
                        modelProjectM.Status = "DIN";
                        modelProjectM.UpdateDate = updateDate;
                    }
                    await _projectMContext.SaveChangesAsync();

                    // Insert ProjectD
                    var modelprojectD = new ProjectD
                    {
                        ProjectID = dwinViewModel.ProjectID,
                        VendorID = dwinViewModel.VendorID,
                        PartNo = dwinViewModel.PartNo,
                        ELTR = dwinViewModel.ELTR,
                        EGP = dwinViewModel.EGP,
                        EFirstYQty = dwinViewModel.EFirstYQty,
                        ESecondYQty = dwinViewModel.ESecondYQty,
                        EThirdYQty = dwinViewModel.EThirdYQty,
                        UFirstYPrice = dwinViewModel.UFirstYPrice,
                        USecondYPrice = dwinViewModel.USecondYPrice,
                        UThirdYPrice = dwinViewModel.UThirdYPrice,
                        Stage = "DWIN"
                    };
                    _projectDContext.Add(modelprojectD);
                    await _projectDContext.SaveChangesAsync();

                    // Update Project_DIDW
                    var modelProjectDwin = _project_DIDWContext.Project_DIDW.FirstOrDefault(p => p.ProjectID == dwinViewModel.ProjectID);

                    if (modelProjectDwin != null)
                    {
                        modelProjectDwin.DwinStatus = "N";
                        modelProjectDwin.DwinDate = updateDate;
                    }
                    await _project_DIDWContext.SaveChangesAsync();
                }
                return RedirectToAction(nameof(Index));
            }
            return View();
        }

        // GET: DwinViewModels/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null || _project_DIDWContext.DwinViewModel == null)
            {
                return NotFound();
            }

            var dwinViewModel = await _project_DIDWContext.DwinViewModel.FindAsync(id);
            if (dwinViewModel == null)
            {
                return NotFound();
            }
            return View(dwinViewModel);
        }

        // POST: DwinViewModels/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("ProjectID,DwinDate,DwinStatus,Cus_DB,CusID,EndCus,ProApp,ProModel,IsInte,EProduceYS,VendorID,PartNo,ELTR,EGP,EFirstYQty,ESecondYQty,EThirdYQty,UFirstYPrice,USecondYPrice,UThirdYPrice,PM_EmpName,PM_Bonus,Sales_EmpName,Sales_Bonus,FAE1_EmpName,FAE1_Bonus,FAE2_EmpName,FAE2_Bonus,FAE3_EmpName,FAE3_Bonus,vmCreateDate,StatusName,VendorName,CusName")] DwinViewModel dwinViewModel)
        {
            if (id != dwinViewModel.ProjectID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _project_DIDWContext.Update(dwinViewModel);
                    await _project_DIDWContext.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DwinViewModelExists(dwinViewModel.ProjectID))
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
            return View(dwinViewModel);
        }

        private bool DwinViewModelExists(string id)
        {
          return (_project_DIDWContext.DwinViewModel?.Any(e => e.ProjectID == id)).GetValueOrDefault();
        }
    }
}
