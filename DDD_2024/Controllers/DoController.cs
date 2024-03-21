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

namespace DDD_2024.Controllers
{
    public class DoController : Controller
    {
        private readonly DoContext _Docontext;
        private readonly ProjectMContext _projectMContext;
        private readonly ProjectDContext _projectDContext;
        private readonly IDoService _doService;
        private readonly ICusVendoeService _cusVendoeService;
        private readonly IEmployeeService _employeeService;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public DoController(ProjectMContext projectMContext, ProjectDContext projectDContext, DoContext doContext, 
            IDoService doService, ICusVendoeService cusVendoeService, IEmployeeService employeeService, IWebHostEnvironment webHostEnvironment)
        {
            _projectMContext = projectMContext;
            _projectDContext = projectDContext;
            _Docontext = doContext;
            _doService = doService;
            _cusVendoeService = cusVendoeService;
            _employeeService = employeeService;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: Do
        public async Task<IActionResult> Index()
        {
            var modelDO = await _Docontext.Project_DO.ToListAsync();
            var modelProjectM = await _projectMContext.ProjectM.ToListAsync();
            var modelProjectD = await _projectDContext.ProjectD.ToListAsync();

            //加入modelDO的資料
            List<DoViewModel> list_doViewModel = new List<DoViewModel>();
            foreach (var item in modelDO)
            {
                var model = new DoViewModel
                {
                    DoID = item.DoID,
                    ProjectID = item.ProjectID,
                    ApplicantName = _employeeService.GetEmployeeName(item.ApplicantID),
                    StatusName = _doService.GetStatusName(item.Status),
                    DOStatus = item.Status
                };

                if (!string.IsNullOrEmpty(item.CreateDate))
                {
                    model.vmCreateDate = item.CreateDate.Substring(0, 4) + "/" + item.CreateDate.Substring(4, 2) + "/" + item.CreateDate.Substring(6, 2);
                }

                list_doViewModel.Add(model);
            }

            //加入modelProjectM和modelProjectD的資料
            foreach (var item in list_doViewModel)
            {
                var matchingProjectM = modelProjectM.FirstOrDefault(p => p.ProjectID == item.ProjectID);
                var matchingProjectD = modelProjectD.FirstOrDefault(k => k.ProjectID == item.ProjectID);

                if (matchingProjectM != null && matchingProjectD != null)
                {
                    if(!string.IsNullOrEmpty(matchingProjectM.Cus_DB) && !string.IsNullOrEmpty(matchingProjectM.CusID))
                    {
                        // 將 matchingProjectM 的資料設定給 Cus_DB 屬性
                        item.CusName = _cusVendoeService.GetvendorName(matchingProjectM.Cus_DB, matchingProjectM.CusID);
                    }

                    if (!string.IsNullOrEmpty(matchingProjectD.VendorID))
                    {
                        item.VendorName = _cusVendoeService.GetvendorName("ASCEND", matchingProjectD.VendorID);
                    }

                    item.PartNo = matchingProjectD.PartNo;
                }
            }

            if (list_doViewModel != null)
            {
                return View(list_doViewModel);
            }
            else
            {
                return Problem("Entity set 'Project_DO'  is null.");
            }
        }

        // GET: Do/Details/5
        public async Task<IActionResult> Details(string ProjectID)
        {
            if (ProjectID == null || _Docontext.Project_DO == null)
            {
                return NotFound();
            }

            var modelProjectM = await _projectMContext.ProjectM.Where(p => p.ProjectID == ProjectID).FirstOrDefaultAsync();
            var modelProjectD = await _projectDContext.ProjectD.Where(p => p.ProjectID == ProjectID && (p.Stage == "DO" || p.Stage == "DIN")).FirstOrDefaultAsync();
            var modelDo = await _Docontext.Project_DO.Where(p => p.ProjectID == ProjectID).FirstOrDefaultAsync();

            var model = new DoViewModel();

            if(modelDo != null)
            {
                model.DoID = modelDo.DoID;
                model.DOStatus = modelDo.Status;
                model.StatusName = _doService.GetStatusName(modelDo.Status);

                if (!string.IsNullOrEmpty(modelDo.CreateDate))
                {
                    model.vmCreateDate = modelDo.CreateDate.Substring(0, 4) + "/" + modelDo.CreateDate.Substring(4, 2) + "/" + modelDo.CreateDate.Substring(6, 2);
                }

                model.ApplicantName = _employeeService.GetEmployeeName(modelDo.ApplicantID);
            }

            if (modelProjectM != null && modelProjectD != null)
            {
                model.ProjectID = modelProjectM.ProjectID;

                if(!string.IsNullOrEmpty(modelProjectM.Cus_DB) && !string.IsNullOrEmpty(modelProjectM.CusID))
                {
                    model.CusName = _cusVendoeService.GetvendorName(modelProjectM.Cus_DB, modelProjectM.CusID);
                }

                if (!string.IsNullOrEmpty(modelProjectD.VendorID))
                {
                    model.VendorName = _cusVendoeService.GetvendorName("ASCEND", modelProjectD.VendorID);
                }

                model.PartNo = modelProjectD.PartNo;
                model.ProApp = modelProjectM.ProApp;
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
                _projectMContext.Add(modelprojectM);
                await _projectMContext.SaveChangesAsync();

                // Insert a record into ProjectD
                var modelprojectD = new ProjectD
                {
                    ProjectID = projectID,
                    VendorID = doViewModel.VendorID,
                    PartNo = doViewModel.PartNo,
                    Stage = "DO"
                };
                _projectDContext.Add(modelprojectD);
                await _projectDContext.SaveChangesAsync();

                // Insert a record into Project_DO
                var modelprojectDO = new Project_DO
                {
                    DoID = _doService.GetDOID(DateTime.Now.ToString("yyyyMMdd")),
                    ProjectID = projectID,
                    CreateDate = createDate,
                    ApplicantID = Convert.ToInt32(doViewModel.ApplicantName),
                    Status = "N" // Status: 新單
                };
                _Docontext.Add(modelprojectDO);
                await _Docontext.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            return View();
        }

        // GET: Do/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null || _Docontext.Project_DO == null)
            {
                return NotFound();
            }

            var project_DO = await _Docontext.Project_DO.FindAsync(id);
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
                    _projectDContext.Update(project_DO);
                    await _projectDContext.SaveChangesAsync();
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
            
            var allProjectDOs = _Docontext.Project_DO.ToList();

            foreach (var doId in DoIDs)
            {
                var modelToUpdate = allProjectDOs.FirstOrDefault(d => d.DoID == doId);

                if (modelToUpdate != null)
                {
                    modelToUpdate.Status = "C";  // 狀態改為審核通過
                    _Docontext.Update(modelToUpdate); 
                }
            }
            _Docontext.SaveChanges();

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

            var allProjectDOs = _Docontext.Project_DO.ToList();

            foreach (var doId in DoIDs)
            {
                var modelToUpdate = allProjectDOs.FirstOrDefault(d => d.DoID == doId);

                if (modelToUpdate != null)
                {
                    modelToUpdate.Status = "R";  // 狀態改為審核拒絕
                    _Docontext.Update(modelToUpdate);
                }
            }
            _Docontext.SaveChanges();

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

            var allProjectDOs = _Docontext.Project_DO.ToList();

            var modelToUpdate = allProjectDOs.FirstOrDefault(d => d.DoID == DoID);
            if (modelToUpdate != null)
            {
                modelToUpdate.Status = "C";  // 狀態改為審核通過
                _Docontext.Update(modelToUpdate);
            }
            _Docontext.SaveChanges();

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

            var allProjectDOs = _Docontext.Project_DO.ToList();

            var modelToUpdate = allProjectDOs.FirstOrDefault(d => d.DoID == DoID);
            if (modelToUpdate != null)
            {
                modelToUpdate.Status = "R";  // 狀態改為審核拒絕
                _Docontext.Update(modelToUpdate);
            }
            _Docontext.SaveChanges();

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
            Excelfile.CopyTo(stream);

            // 讀取stream中的所有資料
            var streamData = MiniExcel.Query(stream, true, sheetName: "DO", startCell: "A2").ToList();

            // 检查是否有数据
            if (streamData.Count > 0)
            {
                List< DoViewModel > list_doViewModels = new List< DoViewModel >();
                
                for(int i = 0; i < streamData.Count; i++)
                {
                    var rowData = streamData[i];

                    //檢查是否有資料
                    bool hasData = false;

                    foreach (var cellValue in rowData)
                    {
                        if (cellValue.Value != null)
                        {
                            hasData = true;
                            break;
                        }
                    }

                    if (!hasData)
                    {
                        continue;
                    }

                    var DoViewModel = new DoViewModel();

                    foreach (var cellValue in rowData)
                    {
                        if (cellValue.Key == "Date")
                        {
                            DoViewModel.vmCreateDate = Convert.ToDateTime(cellValue.Value).ToString("yyyy/MM/dd");
                        }
                        if (cellValue.Key == "Customer(中文)")
                        {
                            DoViewModel.CusName = cellValue.Value.ToString();
                        }
                        if (cellValue.Key == "Product")
                        {
                            DoViewModel.VendorName = cellValue.Value.ToString();
                        }
                        if (cellValue.Key == "Part number")
                        {
                            DoViewModel.PartNo = cellValue.Value.ToString();
                        }
                        if (cellValue.Key == "Application")
                        {
                            DoViewModel.ProApp = cellValue.Value.ToString();
                        }
                        if (cellValue.Key == "Owner")
                        {
                            DoViewModel.ApplicantName = cellValue.Value.ToString();
                        }
                    }
                    list_doViewModels.Add(DoViewModel);
                }
                return View(list_doViewModels);
            }
            return View();

        }

        private bool Project_DOExists(string id)
        {
          return (_Docontext.Project_DO?.Any(e => e.DoID == id)).GetValueOrDefault();
        }
    }
}
