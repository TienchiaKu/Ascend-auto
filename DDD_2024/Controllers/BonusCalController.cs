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
using MiniExcelLibs;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Build.Evaluation;
using DDD_2024.Services;

namespace DDD_2024.Controllers
{
    public class BonusCalController : Controller
    {
        private readonly ProjectMContext _projectMContext;
        private readonly ProjectDContext _projectDContext;
        private readonly ProjectDOContext _projectDOContext;
        private readonly DoContext _Docontext;
        private readonly Project_DIDWContext _project_DIDWContext;
        private readonly Project_EmpContext _empContext;
        private readonly IDoService _doService;
        private readonly ICusVendoeService _cusVendoeService;
        private readonly IEmployeeService _employeeService;
        private readonly IBounsCalService _bounsCalService;
        private readonly IWebHostEnvironment _env;

        public BonusCalController(ProjectMContext projectMContext, ProjectDContext projectDContext, Project_DIDWContext project_DIDWContext, 
            DoContext doContext, ProjectDOContext projectDOContext, Project_EmpContext project_EmpContext,
            IDoService doService, ICusVendoeService cusVendoeService, IEmployeeService employeeService,
            IBounsCalService bounsCalService, IWebHostEnvironment env)
        {
            _projectMContext = projectMContext;
            _projectDContext = projectDContext;
            _projectDOContext = projectDOContext;
            _Docontext = doContext;
            _project_DIDWContext = project_DIDWContext;
            _empContext = project_EmpContext;
            _doService = doService;
            _cusVendoeService = cusVendoeService;
            _employeeService = employeeService;
            _bounsCalService = bounsCalService;
            _env = env;
        }

        // GET: BonusCal
        public async Task<IActionResult> DoIndex()
        {
            var model = await _bounsCalService.GetProjects_DO();

            if (model != null)
            {
                return View(model);
            }
            else
            {
                ViewBag.Message = "No match data.";
                return View();
            }
        }

        // GET: BonusCal
        public async Task<IActionResult> DInWinIndex()
        {
            var model = await _bounsCalService.GetProjects_DINWIN();

            if (model != null)
            {
                return View(model);
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

        // GET: BonusCal
        public IActionResult ProjectIndex()
        {
            List<ProjectBonusViewModel> list_PBonusVM = new List<ProjectBonusViewModel>();

            //list_PBonusVM = _bounsCalService.GetBonusbyProject();

            return View(list_PBonusVM);
        }

        //public IActionResult ExportExcel_BonusbyProjects()
        //{
        //    var modelProjects = _bounsCalService.GetBonusbyProject();
        //
        //    var valuesProjects = modelProjects.Select(item => new {
        //        ProjectID = item.ProjectID,
        //        Status = item.Status,
        //        ApplicantName = item.ApplicantName,
        //        TradeStatus = item.TradeStatus,
        //        Applicant_Bonus = item.Applicant_Bonus
        //    }).ToArray();
        //
        //    var modelEmp = _bounsCalService.GetBonusbyEmployee();
        //
        //    var valuesEmp = modelEmp.Select(item => new {
        //        EmployeeName = item.EmployeeName,
        //        Bonus = item.Bonus
        //    }).ToArray();
        //
        //    var sheets = new Dictionary<string, object>
        //    {
        //        ["byProjects"] = valuesProjects,
        //        ["byEmployees"] = valuesEmp
        //    };
        //
        //    string path = @"D:\獎金計算by專案_" + DateTime.Now.ToString("yyyyMMdd") + ".xlsx";
        //
        //    if (!System.IO.File.Exists(path))
        //    {
        //        MiniExcel.SaveAs(path, sheets);
        //
        //        ViewBag.Message = "Excel匯出完成！";
        //    }      
        //
        //    //var memoryStream = new MemoryStream();
        //    //memoryStream.SaveAs(values);
        //    ////memoryStream.SaveAs(valuesTTL,true,"Sheet2");
        //    //memoryStream.Seek(0, SeekOrigin.Begin);
        //    //return new FileStreamResult(memoryStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
        //    //{
        //    //    FileDownloadName = "獎金計算by專案_" + DateTime.Now.ToString("yyyyMMdd") + ".xlsx"
        //    //};
        //    return RedirectToAction("ProjectIndex");
        //}

        [HttpPost]
        public ActionResult PreviewDo([FromBody] string[] projectIds)
        {
            var model = _bounsCalService.BonusConfirm(projectIds);

            if (model != null)
            {
                var empDIDWBonusModel = _bounsCalService.GetDIDWBonusbyEmployee(model);
                var empDOBonusModel = _bounsCalService.GetDOBonusbyEmployee(model);

                var sheets = new Dictionary<string, object>
                {
                    ["各專案獎金"] = model,
                    ["員工Do獎金"] = empDOBonusModel,
                    ["員工DIDW獎金"] = empDIDWBonusModel
                };

                string path = @"D:\獎金報表_" + DateTime.Now.ToString("yyyyMMdd") + ".xlsx";

                if (!System.IO.File.Exists(path))
                {
                    MiniExcel.SaveAs(path, sheets);

                    ViewBag.Message = "Excel匯出完成！";
                }

                return View("Index", model);
            }
            else
            {
                return Content("匯出錯誤");
            }
        }

        [HttpPost]
        public ActionResult BonusConfirm([FromBody] string[] projectIds)
        {
            var model = _bounsCalService.BonusConfirm(projectIds);

            if (model != null)
            {
                var empDIDWBonusModel = _bounsCalService.GetDIDWBonusbyEmployee(model);
                var empDOBonusModel = _bounsCalService.GetDOBonusbyEmployee(model);

                var sheets = new Dictionary<string, object>
                {
                    ["各專案獎金"] = model,
                    ["員工Do獎金"] = empDOBonusModel,
                    ["員工DIDW獎金"] = empDIDWBonusModel
                };

                string path = @"D:\獎金報表_" + DateTime.Now.ToString("yyyyMMdd") + ".xlsx";

                if (!System.IO.File.Exists(path))
                {
                    MiniExcel.SaveAs(path, sheets);

                    ViewBag.Message = "Excel匯出完成！";
                }

                return View("Index", model);
            }
            else
            {
                return Content("匯出錯誤");
            }
        }

        private bool BonusCalViewModelExists(string id)
        {
          return (_projectMContext.BonusCalViewModel?.Any(e => e.ProjectID == id)).GetValueOrDefault();
        }
    }
}