using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using DDD_2024.Data;
using DDD_2024.Models;
using DDD_2024.Interfaces;
using Microsoft.CodeAnalysis;
using MiniExcelLibs;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using Microsoft.CodeAnalysis.Differencing;

namespace DDD_2024.Controllers
{
    public class DoController : Controller
    {
        private readonly DoContext _Docontext;
        private readonly ProjectDOContext _projectDOContext;
        private readonly ProjectMContext _projectMContext;
        private readonly ProjectDContext _projectDContext;
        private readonly Project_DIDWContext _projectDIDWContext;
        private readonly IDoService _doService;
        private readonly ICusVendorService _cusVendoeService;
        private readonly IEmployeeService _employeeService;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ICommonService _commonService;

        public DoController(ProjectMContext projectMContext, ProjectDContext projectDContext, DoContext doContext, ProjectDOContext projectDOContext,
            Project_DIDWContext project_DIDWContext, 
            IDoService doService, ICusVendorService cusVendoeService, IEmployeeService employeeService, ICommonService commonService,
            IWebHostEnvironment webHostEnvironment)
        {
            _projectMContext = projectMContext;
            _projectDContext = projectDContext;
            _projectDOContext = projectDOContext;
            _projectDIDWContext = project_DIDWContext;
            _Docontext = doContext;
            _doService = doService;
            _cusVendoeService = cusVendoeService;
            _employeeService = employeeService;
            _commonService = commonService;

            _webHostEnvironment = webHostEnvironment;
        }

        // GET: Do
        public async Task<IActionResult> Index()
        {
            var model = await _doService.GetDOsAsync();

            if (model != null)
            {
                return View(model);
            }
            else
            {
                return Problem("Entity set 'DOs' is null.");
            }
        }

        // GET: DoFilter
        public async Task<IActionResult> IndexFilter([FromQuery] string projectStatus, [FromQuery] string applicant, List<string> months)
        {
            var model = await _doService.GetDOsFilterAsync(projectStatus, applicant,months);

            if (model != null)
            {
                return PartialView("_DoPartial", model);
            }
            else
            {
                return Problem("Entity set 'DOs' is null.");
            }
        }

        // GET: Do/Details/5
        public async Task<IActionResult> Details(string ProjectID)
        {
            if (ProjectID == null || _Docontext.Project_DO == null)
            {
                return NotFound();
            }

            var model = await _doService.GetDoAsync(ProjectID);

            if(model != null)
            {
                return View(model);
            }
            else
            {
                return NotFound();
            }          
        }

        // GET: Do/Create
        public async Task<IActionResult> Create()
        {
            var model = new DoViewModel();

            var selectListItems_Cus = await _cusVendoeService.GetAllCus_Selector();
            ViewBag.CustomerList = selectListItems_Cus;

            var selectListItems_Ven = await _cusVendoeService.GetAllVendor_Selector();
            ViewBag.VendorList = selectListItems_Ven;

            return View();
        }

        // POST: Do/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CreateDate,CusID,VendorID,PartNo,ProApp,ApplicantID,ApproverID,TradeStatus,DoUAction,DoUStatus")] DoCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                await _doService.CreateDO(model);

                return RedirectToAction(nameof(Index));
            }
            else
            {              
                TempData["ErrorMessage"] = "表單資料驗證失敗。";
            }
            return View();
        }

        // GET: Do/Edit/5
        public async Task<IActionResult> Edit(string ProjectID)
        {
            if (ProjectID == null || _Docontext.Project_DO == null)
            {
                return NotFound();
            }

            var model = await _doService.GetEditDo(ProjectID);

            if (model != null)
            {
                return View("Edit",model);
            }
            else
            {
                return NotFound();
            }
        }

        // POST: Do/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string DoID, [Bind("DoID,ProjectID,CreateDate,PartNo,TradeStatus,ApplicantID,ApproverID,DoUDate,DoUAction,DoUStatus")] DoEditViewModel model)
        {
            if (DoID != model.DoID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                await _doService.EditDo(model);
                return RedirectToAction(nameof(Edit), new { ProjectID = model.ProjectID });
            }
            return View("Edit", model);
        }

        [HttpPost]
        public IActionResult TransDin([FromBody] string DoID)
        {
            return RedirectToAction("Create", "Din", new { projectID = DoID });
        }

        // GET: Do/Upload
        public IActionResult Upload()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Upload(IFormFile Excelfile) 
        {
            var stream = new MemoryStream();
            if(Excelfile != null)
            {
                Excelfile.CopyTo(stream);

                // 讀取stream中的所有資料
                var streamData = MiniExcel.Query(stream, true, startCell: "A1").ToList();

                // 檢查是否有資料
                if (streamData.Count > 0)
                {
                    List<DoViewModel> list_doViewModels = new List<DoViewModel>();

                    for (int i = 0; i < streamData.Count; i++)
                    {
                        var rowData = streamData[i];

                        var DoViewModel = new DoViewModel();

                        foreach (var cellValue in rowData)
                        {
                            if (cellValue.Key == "Date")
                            {                 
                                if(cellValue.Value == null)
                                {
                                    DoViewModel.UploadStatus += "申請日期無資料;\n";
                                    continue;
                                }
                                else
                                {
                                    DoViewModel.vmCreateDate = Convert.ToDateTime(cellValue.Value).ToString("yyyy/MM/dd");
                                }
                            }
                            if (cellValue.Key == "Customer")
                            {
                                if (cellValue.Value == null)
                                {
                                    DoViewModel.UploadStatus += "客戶名稱無資料;\n";
                                    continue;
                                }

                                DoViewModel.CusName = cellValue.Value.ToString();

                                if(DoViewModel.CusName.Length > 25)
                                {
                                    DoViewModel.UploadStatus += "客戶名稱長度超過25;\n";
                                    continue;
                                }

                                if (!string.IsNullOrEmpty(DoViewModel.CusName))
                                {
                                    (DoViewModel.Cus_DB, DoViewModel.CusID) = _cusVendoeService.GetCusID(DoViewModel.CusName);

                                    if (string.IsNullOrEmpty(DoViewModel.Cus_DB) || string.IsNullOrEmpty(DoViewModel.CusID))
                                    {
                                        DoViewModel.UploadStatus += "找無相符合客戶資料;\n";
                                    }
                                }
                            }
                            if (cellValue.Key == "Product") 
                            {
                                if (cellValue.Value == null)
                                {
                                    DoViewModel.UploadStatus += "原廠名稱無資料;\n";
                                    continue;
                                }

                                DoViewModel.VendorName = cellValue.Value.ToString();

                                if(DoViewModel.VendorName.Length > 25)
                                {
                                    DoViewModel.UploadStatus += "原廠名稱長度超過25;\n";
                                    continue;
                                }

                                if (!string.IsNullOrEmpty(DoViewModel.VendorName))
                                {
                                    DoViewModel.VendorID = _cusVendoeService.GetVenID(DoViewModel.VendorName);
                                }

                                if (string.IsNullOrEmpty(DoViewModel.VendorID))
                                {
                                    DoViewModel.UploadStatus += "找無相符合原廠資料;\n";
                                }
                            }
                            if (cellValue.Key == "Part number")
                            {
                                if (cellValue.Value == null)
                                {
                                    DoViewModel.UploadStatus += "品號無資料;\n";
                                    continue;
                                } 

                                DoViewModel.PartNo = cellValue.Value.ToString();

                                if(DoViewModel.PartNo.Length > 50)
                                {
                                    DoViewModel.UploadStatus += "品號長度超過50;\n";
                                    continue;
                                }
                            }
                            if (cellValue.Key == "Application")
                            {
                                if (cellValue.Value == null)
                                {
                                    DoViewModel.UploadStatus += "產品應用無資料;\n";
                                    continue;
                                }

                                DoViewModel.ProApp = cellValue.Value.ToString();

                                if(DoViewModel.ProApp.Length > 40)
                                {
                                    DoViewModel.UploadStatus += "產品應用長度超過40;\n";
                                    continue;
                                }
                            }
                            if (cellValue.Key == "Owner")
                            {
                                if (cellValue.Value == null)
                                {
                                    DoViewModel.UploadStatus += "申請者無資料;\n";
                                    continue;
                                }

                                DoViewModel.Applicant = cellValue.Value.ToString();

                                if (!string.IsNullOrEmpty(DoViewModel.Applicant))
                                {
                                    DoViewModel.ApplicantID = _employeeService.GetEmployeeID(DoViewModel.Applicant);
                                }
                            }
                            if (cellValue.Key == "Approved By")
                            {
                                if (cellValue.Value == null)
                                {
                                    continue;
                                }

                                DoViewModel.Approver = cellValue.Value.ToString();

                                if (!string.IsNullOrEmpty(DoViewModel.Approver))
                                {
                                    DoViewModel.ApproverID = _employeeService.GetEmployeeID(DoViewModel.Approver);
                                }
                            }
                            if (cellValue.Key == "New/Active")
                            {
                                if (cellValue.Value == null)
                                {
                                    DoViewModel.UploadStatus += "New/Active無資料;\n";
                                    continue;
                                }

                                if (cellValue.Value.ToString() == "Active")
                                {
                                    DoViewModel.TradeStatus = "A";
                                }
                                else if (cellValue.Value.ToString() == "New")
                                {
                                    DoViewModel.TradeStatus = "N";
                                }
                                else
                                {
                                    DoViewModel.TradeStatus = string.Empty;
                                }                                
                            }
                            if(cellValue.Key == "Status")
                            {
                                if (cellValue.Value == null)
                                {
                                    DoViewModel.UploadStatus += "Status無資料;\n";
                                    continue;
                                }
                                DoViewModel.DoUStatus = cellValue.Value.ToString();
                            }
                            if (cellValue.Key == "Action")
                            {
                                if (cellValue.Value == null)
                                {
                                    DoViewModel.UploadStatus += "Action無資料;\n";
                                    continue;
                                }
                                DoViewModel.DoUAction = cellValue.Value.ToString();
                            }
                        }
                        list_doViewModels.Add(DoViewModel);
                    }
                    var model = _doService.ImportDo(list_doViewModels);
                    return View(model);
                }
            }            
            return View();
        }

        public IActionResult DoIndex()
        {
            var modelDO = _Docontext.Project_DO.ToList();

            //加入modelDO的資料
            List<DoViewModel> list_doViewModel = new List<DoViewModel>();
            foreach (var item in modelDO)
            {
                var model = new DoViewModel
                {
                    DoID = item.DoID,
                    ProjectID = item.ProjectID,
                    Applicant = _employeeService.GetEmployeeName(item.ApplicantID),
                    StatusName = _doService.GetStatusName(item.Status),
                    TradeStatus = item.TradeStatus,
                    DOStatus = item.Status
                };

                if (!string.IsNullOrEmpty(item.CreateDate))
                {
                    model.vmCreateDate = item.CreateDate.Substring(0, 4) + "/" + item.CreateDate.Substring(4, 2) + "/" + item.CreateDate.Substring(6, 2);
                }

                //加入ProjectM的資料
                var modelProjectM = _projectMContext.ProjectM.Where(e => e.ProjectID == item.ProjectID).FirstOrDefault();

                if (modelProjectM != null)
                {
                    if (!string.IsNullOrEmpty(modelProjectM.Cus_DB) && !string.IsNullOrEmpty(modelProjectM.CusID))
                    {
                        // 將 matchingProjectM 的資料設定給 Cus_DB 屬性
                        model.CusName = _cusVendoeService.GetvendorName(modelProjectM.Cus_DB, modelProjectM.CusID);
                    }
                }

                var modelProjectD = _projectDContext.ProjectD.Where(e => e.ProjectID == item.ProjectID).FirstOrDefault();
                //加入ProjectD的資料

                if (modelProjectD != null)
                {
                    if (!string.IsNullOrEmpty(modelProjectD.VendorID))
                    {
                        model.VendorName = _cusVendoeService.GetvendorName("ASCEND", modelProjectD.VendorID);
                    }

                    model.PartNo = modelProjectD.PartNo;
                }

                list_doViewModel.Add(model);
            }

            if (list_doViewModel != null)
            {
                return View(list_doViewModel);
            }
            else
            {
                return Problem("Entity set 'Project_DO' is null.");
            }
        }

        // GET: Do/DoReport
        public IActionResult DoReport()
        {
            return View();
        }

        //匯出Do Excel報表
        public async Task<IActionResult> ExportDoReport(DoReportFilterViewModel model)
        {
            var ReportModel = await _doService.GetDosReport(model);

            if (ReportModel != null && ReportModel.Count > 0)
            {
                // 新增自定義資料
                var status = new List<DoReportStatus_ViewModel>
                {
                    new DoReportStatus_ViewModel { Status = "結案", Text = "Y" },
                    new DoReportStatus_ViewModel { Status = "結案復原", Text = "R" }
                };

                var sheets = new Dictionary<string, object>
                {
                    ["DO"] = ReportModel,
                    ["DoStatus"] = status
                };

                var memoryStream = new MemoryStream();
                memoryStream.SaveAs(sheets);
                memoryStream.Seek(0, SeekOrigin.Begin);
                return new FileStreamResult(memoryStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
                {
                    FileDownloadName = "Do報表_" + DateTime.Now.ToString("yyyyMMdd") + ".xlsx"
                };
            }
            else
            {
                return Content("匯出錯誤");
            }
        }

        // GET: Do
        public async Task<IActionResult> DoASIndex(string DoID)
        {
            var model = await _doService.GetDOASUsAsync(DoID);

            if (model != null)
            {
                return View(model);
            }
            else
            {
                return Problem("Entity set 'DOAS' is null.");
            }
        }

        // GET: Do/DoASCreate
        public IActionResult DoASCreate(string DoID)
        {
            var model = new DOASUViewModel();
            model.DoID = DoID;

            return View();
        }

        // POST: Do/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DoASCreate([Bind("DoID,vmDoUDate,DoUAction,DoUStatus")] DOASUViewModel model)
        {
            if (ModelState.IsValid)
            {
                await _doService.CreateDoAS(model);

                return RedirectToAction("DoASIndex","Do",model.DoID);
            }
            return View();
        }

        private bool Project_DOExists(string id)
        {
          return (_projectDOContext.Project_DO?.Any(e => e.DoID == id)).GetValueOrDefault();
        }

        // GET: Do/Upload
        public IActionResult DoMaintain()
        {
            return View();
        }

        [HttpPost]
        public IActionResult DoMaintain(IFormFile Excelfile, string colName, bool updateAction)
        {
            if (Excelfile != null)
            {
                var stream = new MemoryStream();
                Excelfile.CopyTo(stream);

                // 讀取stream中的所有資料
                var streamData = MiniExcel.Query(stream, true, startCell: "A1").ToList();

                // 檢查是否有資料
                if (streamData.Count > 0)
                {
                    //檢查colName格式是否相符
                    if(!string.IsNullOrEmpty(colName) && colName.Trim().Length != 23)
                    {
                        ViewData["ErrorMessage"] = "請檢查輸入Excel欄位名稱";
                        return View();
                    }

                    List<DoMaintainModel_F> list_finish = new List<DoMaintainModel_F>();
                    List<DoMaintainModel_U> list_update = new List<DoMaintainModel_U>();

                    string yearMonth = string.Empty;

                    if (!string.IsNullOrEmpty(colName) && colName.Length == 23)
                    {
                        yearMonth = colName.Trim().Substring(15, 7).Replace("/", "");
                    }

                    for (int i = 0; i < streamData.Count; i++)
                     {
                         var rowData = streamData[i];
                         var finishModel = new DoMaintainModel_F();
                         var updateModel = new DoMaintainModel_U();
                         bool check = false;

                         foreach (var cellValue in rowData)
                         {
                            if (cellValue.Key == "專案編號")
                            {
                                if (cellValue.Value == null)
                                {
                                    check = true;
                                    break;
                                }
                                else
                                {
                                    finishModel.ProjectID = cellValue.Value.ToString();
                                    updateModel.ProjectID = cellValue.Value.ToString();
                                }
                            }

                            if (cellValue.Key == "結案")
                            {
                                if (cellValue.Value == null)
                                {
                                    continue;
                                }
                                else
                                {
                                    finishModel.IsFinish = cellValue.Value.ToString();
                                }
                            }

                            if (cellValue.Key == "結案原因" && !string.IsNullOrEmpty(finishModel.IsFinish))
                            {
                                if (cellValue.Value == null)
                                {
                                    check = true;
                                    break;
                                }
                                else
                                {
                                    finishModel.FinishReason = cellValue.Value.ToString();
                                }
                            }

                            if (cellValue.Key == colName)
                            {
                                updateModel.DoUDate = yearMonth;

                                if (cellValue.Value == null)
                                {
                                    if (string.IsNullOrEmpty(finishModel.IsFinish))
                                    {
                                        check = true;
                                        break;
                                    }
                                    else
                                    {
                                        continue;
                                    }                                    
                                }
                                else
                                {
                                    updateModel.DoUStatus = cellValue.Value.ToString();
                                }
                            }

                            //使用者勾選update Action時，才要讀取Action的值
                            if (updateAction)
                            {
                                if (cellValue.Key == "Action")
                                {
                                    if (cellValue.Value != null)
                                    {
                                        updateModel.DoUAction = cellValue.Value.ToString();
                                    }
                                }
                            }

                            if (!check)
                            {
                                list_finish.Add(finishModel);
                                list_update.Add(updateModel);
                            }


                        }
                    }                   
                    if (list_finish.Count > 0 || list_update.Count > 0)
                    {
                        return View(_doService.DoMaintain(list_finish, list_update));
                    }
                }
            }
            else
            {
                ViewData["ErrorMessage"] = "請選擇Excel檔";
                return View();
            }
            return View(nameof(Index));
        }

        //匯出Do範本
        [HttpPost]
        public IActionResult DoTemplate()
        {
            var ReportModel = new List<DoReport_ViewModel>();

            var sheets = new Dictionary<string, object>
            {
                ["DoTemplate"] = ReportModel
            };

            var memoryStream = new MemoryStream();
            memoryStream.SaveAs(sheets);
            memoryStream.Seek(0, SeekOrigin.Begin);
            return new FileStreamResult(memoryStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
            {
                FileDownloadName = "Do範本_" + DateTime.Now.ToString("yyyyMMdd") + ".xlsx"
            };
        }
    }
}
