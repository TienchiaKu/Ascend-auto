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
using Microsoft.AspNetCore.Http.HttpResults;
using MiniExcelLibs;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using DDD_2024.Services;

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
        private readonly ICusVendoeService _cusVendoeService;
        private readonly IEmployeeService _employeeService;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public DoController(ProjectMContext projectMContext, ProjectDContext projectDContext, DoContext doContext, ProjectDOContext projectDOContext,
            Project_DIDWContext project_DIDWContext, 
            IDoService doService, ICusVendoeService cusVendoeService, IEmployeeService employeeService, 
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

        // GET: Do
        public async Task<IActionResult> IndexFilter([FromQuery] string projectStatus, [FromQuery] string applicant)
        {
            var model = await _doService.GetDOsFilterAsync(projectStatus, applicant);

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
        public async Task<IActionResult> Create([Bind("CreateDate,CusID,VendorID,PartNo,ProApp,ApplicantID,ApproverID,TradeStatus,DoUAction,DoUStatus")] DoViewModel doViewModel)
        {
            if (ModelState.IsValid)
            {
                await _doService.CreateDO(doViewModel);

                return RedirectToAction(nameof(Index));
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

            var model = await _doService.GetDoAsync(ProjectID);

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
        public async Task<IActionResult> Edit(string DoID, [Bind("DoID,DOStatus,ProjectID,CusID,VendorID,PartNo,ProApp,CreateDate,ApplicantID,ApproverID,DOStatus,TradeStatus")] DoViewModel doViewModel)
        {
            if (DoID != doViewModel.DoID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                await _doService.EditDo(doViewModel);

                return RedirectToAction(nameof(Index));
            }
            return View(doViewModel);
        }

        // POST: /ConfirmDOs
        [HttpPost]
        public IActionResult ConfirmDOs([FromBody] string[] DoIDs)
        {
            string msg = _doService.ConfiirmDos(DoIDs);

            return RedirectToAction("Index");
        }

        // POST: /RejectDOs
        [HttpPost]
        public IActionResult RejectDOs([FromBody] string[] DoIDs)
        {
            string msg = _doService.RejectDos(DoIDs);

            return RedirectToAction("Index");
        }

        // POST: /ConfirmDO
        [HttpPost]
        public IActionResult ConfirmDO([FromBody] string DoID)
        {
            string msg = _doService.ConfirmDo(DoID);

            return RedirectToAction("Index");
        }

        // POST: /RejectDO
        [HttpPost]
        public IActionResult RejectDO([FromBody] string DoID)
        {
            string msg = _doService.RejectDO(DoID);

            return RedirectToAction("Index");
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
                            if (cellValue.Key == "Customer(中文)")
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

                                if (string.IsNullOrEmpty(DoViewModel.Cus_DB) || string.IsNullOrEmpty(DoViewModel.CusID))
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
                        }
                        list_doViewModels.Add(DoViewModel);
                    }
                    InsertDos(list_doViewModels);
                    
                    //匯出Excel
                    var memoryStream = new MemoryStream();
                    memoryStream.SaveAs(list_doViewModels);
                    memoryStream.Seek(0, SeekOrigin.Begin);
                    new FileStreamResult(memoryStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");

                    var filePath = "D:\\" + DateTime.Now.ToString("yyyyMMdd") + "ImportDo.xlsx"; // 你想要保存的文件路径
                    if (System.IO.File.Exists(filePath))
                    {
                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            memoryStream.WriteTo(fileStream);                      
                        }
                    }

                    return View(list_doViewModels);
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

        public async Task<IActionResult> DoReport_Excel(DoReportFilterViewModel model)
        {
            var DosModel = await _doService.GetDosReport(model);

            if (DosModel != null && DosModel.Count > 0)
            {
                var sheets = new Dictionary<string, object>
                {
                    ["DO"] = DosModel
                };

                var memoryStream = new MemoryStream();
                memoryStream.SaveAs(DosModel);
                memoryStream.Seek(0, SeekOrigin.Begin);
                return new FileStreamResult(memoryStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
                {
                    FileDownloadName = "Do報表_" + DateTime.Now.ToString("yyyyMMdd") + ".xlsx"
                };

                //string path = @"D:\Do報表_" + DateTime.Now.ToString("yyyyMMdd") + ".xlsx";
                //
                //if (!System.IO.File.Exists(path))
                //{
                //    MiniExcel.SaveAs(path, sheets);
                //
                //    ViewBag.Message = "Excel匯出完成！";
                //}

                //return View("Index", DosModel);
            }
            else
            {
                return Content("匯出錯誤");
            }

        }

        private List<DoViewModel> InsertDos(List<DoViewModel> list_doViewModels)
        {
            if(list_doViewModels.Count > 0)
            {
                foreach(var item in list_doViewModels)
                {
                    if (!string.IsNullOrEmpty(item.UploadStatus))
                    {
                        continue;
                    }
                    else
                    {
                        item.UploadStatus = CheckDOInsert(item);
                    }

                    if (string.IsNullOrEmpty(item.UploadStatus))
                    {                       
                        string projectID = _doService.GetProjectID(DateTime.Now.ToString("yyyyMMdd"));
                        string createDate = string.Empty;

                        if (!string.IsNullOrEmpty(item.vmCreateDate))
                        {
                            createDate = item.vmCreateDate.Replace("/", "");
                        }

                        // Insert a record into ProjectM
                        var modelprojectM = new ProjectM
                        {
                            ProjectID = projectID,
                            Status = "DO", // Status: DO
                            CreateDate = createDate,
                            Cus_DB = item.Cus_DB,
                            CusID = item.CusID,
                            ProApp = item.ProApp
                        };
                        _projectMContext.Add(modelprojectM);
                        _projectMContext.SaveChanges();

                        // Insert a record into ProjectD
                        var modelprojectD = new ProjectD
                        {
                            ProjectID = projectID,
                            VendorID = item.VendorID,
                            PartNo = item.PartNo,
                            Stage = "DO"
                        };
                        _projectDContext.Add(modelprojectD);
                        _projectDContext.SaveChanges();

                        // Insert a record into Project_DO
                        var modelprojectDO = new Project_DO
                        {
                            DoID = _doService.GetDOID(DateTime.Now.ToString("yyyyMMdd")),
                            ProjectID = projectID,
                            CreateDate = createDate,
                            ApplicantID = item.ApplicantID,
                            ApproverID = item.ApproverID,
                            TradeStatus = item.TradeStatus,
                            Status = "N" // Status: 新單
                        };
                        _Docontext.Add(modelprojectDO);
                        _Docontext.SaveChanges();

                        item.UploadStatus = "Success";
                    }                                    
                }               
            }
            return list_doViewModels;
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

        private string CheckDOInsert(DoViewModel doViewModel)
        {
            string Message = string.Empty;

            //檢查是否有重複Do資料
            if (!string.IsNullOrEmpty(doViewModel.Cus_DB) && !string.IsNullOrEmpty(doViewModel.CusID))
            {
                var ProjectID = _projectMContext.ProjectM.Where(e => e.Cus_DB == doViewModel.Cus_DB && e.CusID == doViewModel.CusID && e.ProApp == doViewModel.ProApp).Select(e => e.ProjectID).FirstOrDefault();
            
                if(ProjectID != null)
                {
                    var ProjectD = _projectDContext.ProjectD.Where(e => e.ProjectID == ProjectID && e.VendorID == doViewModel.VendorID && e.PartNo == doViewModel.PartNo).ToList();
            
                    if (ProjectD.Count > 0)
                    {
                        Message += "Do資料重複;\n";
                    }
                }
            }
            return Message;
        }

        private bool Project_DOExists(string id)
        {
          return (_projectDOContext.Project_DO?.Any(e => e.DoID == id)).GetValueOrDefault();
        }

        // GET: Do/Upload
        public IActionResult UploadDOAS()
        {
            return View();
        }

        [HttpPost]
        public IActionResult UploadDOAS(IFormFile Excelfile)
        {
            //此功能在使用前參數都要改，包含ViewModel和月份都是人工定義的
            
            var stream = new MemoryStream();
            if (Excelfile != null)
            {
                Excelfile.CopyTo(stream);

                // 讀取stream中的所有資料
                var streamData = MiniExcel.Query(stream, true, startCell: "A1").ToList();

                // 檢查是否有資料
                if (streamData.Count > 0)
                {
                    List<DOASU_Upload_ViewModel> list_upload = new List<DOASU_Upload_ViewModel>();

                    for (int i = 0; i < streamData.Count; i++)
                    {
                        var rowData = streamData[i];

                        var UploadViewModel = new DOASU_Upload_ViewModel();

                        foreach (var cellValue in rowData)
                        {
                            if (cellValue.Key == "專案編號")
                            {
                                if (cellValue.Value == null)
                                {
                                    UploadViewModel.UploadStatus += "無專案編號;\n";
                                    continue;
                                }
                                else
                                {
                                    UploadViewModel.ProjectID = cellValue.Value.ToString();
                                }
                            }
                            if (cellValue.Key == "Status")
                            {
                                if (cellValue.Value != null)
                                {
                                    UploadViewModel.DoUStatus2 = cellValue.Value.ToString();
                                }
                            }
                            if (cellValue.Key == "Status Update-(3月更新)")
                            {
                                if (cellValue.Value != null)
                                {
                                    UploadViewModel.DoUStatus3 = cellValue.Value.ToString();
                                }
                            }
                            if (cellValue.Key == "Status Update-(4月更新)")
                            {
                                if (cellValue.Value != null)
                                {
                                    UploadViewModel.DoUStatus4 = cellValue.Value.ToString();
                                }
                            }
                            if (cellValue.Key == "Action")
                            {
                                if (cellValue.Value != null)
                                {
                                    UploadViewModel.DoUAction = cellValue.Value.ToString();
                                }
                            }                         
                        }
                        if (string.IsNullOrEmpty(UploadViewModel.UploadStatus))
                        {
                            list_upload.Add(UploadViewModel);
                        }
                    }
                    var model = _doService.ImportDOASU(list_upload);

                    return View(model);
                }
            }
            return View(nameof(Index));
        }
    }
}
