using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DDD_2024.Data;
using DDD_2024.Models;
using Microsoft.CodeAnalysis;
using DDD_2024.Interfaces;
using MiniExcelLibs;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Build.Evaluation;
using DDD_2024.Services;
using Microsoft.IdentityModel.Tokens;

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
        private readonly ICusVendorService _cusVendoeService;
        private readonly IEmployeeService _employeeService;
        private readonly IBounsCalService _bounsCalService;
        private readonly IWebHostEnvironment _env;

        public BonusCalController(ProjectMContext projectMContext, ProjectDContext projectDContext, Project_DIDWContext project_DIDWContext, 
            DoContext doContext, ProjectDOContext projectDOContext, Project_EmpContext project_EmpContext,
            IDoService doService, ICusVendorService cusVendoeService, IEmployeeService employeeService,
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

        // GET: DoIndex
        public async Task<IActionResult> DoIndex()
        {
            var model = await _bounsCalService.GetProjects_Do();

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

        // GET: DoIndexFilter
        public async Task<IActionResult> DoIndexFilter(List<string> months)
        {
            var model = await _bounsCalService.GetProjects_DoFilter(months);

            if (model != null)
            {
                return PartialView("_DoIndexPartial", model);
            }
            else
            {
                return Problem("Entity set is null.");
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

        // GET: BonusCal/DoReport
        public IActionResult DoReport()
        {
            return View();
        }

        public async Task<IActionResult> PreviewDo(BDoReportFilter model)
        {
            var DoReport = await _bounsCalService.GetDoReport(model);

            if(DoReport != null)
            {
                var EmpReport = _bounsCalService.GetEmpBonus(DoReport);

                if(EmpReport != null)
                {
                    var sheets = new Dictionary<string, object>
                    {
                        ["Do獎金試算"] = DoReport,
                        ["依業務計算"] = EmpReport
                    };

                    var memoryStream = new MemoryStream();
                    memoryStream.SaveAs(sheets);
                    memoryStream.Seek(0, SeekOrigin.Begin);
                    return new FileStreamResult(memoryStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
                    {
                        FileDownloadName = "Do獎金預覽_" + DateTime.Now.ToString("yyyyMMdd_HHmm") + ".xlsx"
                    };
                }
                else
                {
                    return Content("匯出錯誤");
                }
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

        // GET: Do/Upload
        public IActionResult DoUpload()
        {
            return View();
        }

        [HttpPost]
        public IActionResult DoUpload(IFormFile Excelfile)
        {
            var stream = new MemoryStream();
            if (Excelfile != null)
            {
                Excelfile.CopyTo(stream);

                // 讀取stream中的所有資料
                var streamData = MiniExcel.Query(stream, true, startCell: "A1").ToList();

                // 檢查是否有資料
                if (streamData.Count > 0)
                {
                    List<DoReportUpload> list_DoUpdate = new List<DoReportUpload>();

                    for (int i = 0; i < streamData.Count; i++)
                    {
                        var rowData = streamData[i];

                        var UploadModel = new DoReportUpload();

                        foreach (var cellValue in rowData)
                        {
                            if (cellValue.Key == "專案編號")
                            {
                                if (cellValue.Value == null)
                                {
                                    continue;
                                }
                                else
                                {
                                    UploadModel.ProjectID = cellValue.Value;
                                }
                            }
                            if (cellValue.Key == "發放獎金?")
                            {
                                if (cellValue.Value == null)
                                {
                                    continue;
                                }
                                else
                                {
                                    UploadModel.StatusUpdate = cellValue.Value;
                                }
                            }
                        }
                        if (!string.IsNullOrEmpty(UploadModel.ProjectID) && !string.IsNullOrEmpty(UploadModel.StatusUpdate))
                        {
                            list_DoUpdate.Add(UploadModel);
                        }
                    }
                    var model = _doService.UpdateDoStatus(list_DoUpdate);

                    if (model != null)
                    {
                        // 新增自定義資料
                        var status = new List<DoReportStatus_ViewModel>
                        {
                            new DoReportStatus_ViewModel { Status = "發放獎金", Text = "Y" },
                            new DoReportStatus_ViewModel { Status = "不發放獎金", Text = "U" },
                            new DoReportStatus_ViewModel { Status = "暫緩發放獎金", Text = "A" }
                        };

                        var sheets = new Dictionary<string, object>
                        { 
                            ["Do狀態更新"] = model,
                            ["Status"] = status
                        };

                        var memoryStream = new MemoryStream();
                        memoryStream.SaveAs(sheets);
                        memoryStream.Seek(0, SeekOrigin.Begin);
                        return new FileStreamResult(memoryStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
                        {
                            FileDownloadName = "Do狀態更新_" + DateTime.Now.ToString("yyyyMMdd_HHmm") + ".xlsx"
                        };
                    }
                    else
                    {
                        return Content("Excel導出錯誤");
                    }
                }
            }
            return View();
        }
    }
}