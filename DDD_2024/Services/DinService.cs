using DDD_2024.Data;
using DDD_2024.Interfaces;
using DDD_2024.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis;
using Microsoft.DotNet.Scaffolding.Shared.ProjectModel;
using Microsoft.EntityFrameworkCore;
using MiniExcelLibs;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.IO;

namespace DDD_2024.Services
{
    public class DinService: IDinService
    {
        private readonly ProjectDOContext _projectDOContext;
        private readonly ProjectMContext _projectMContext;
        private readonly ProjectDContext _projectDContext;
        private readonly Project_EmpContext _projectEmpContext;
        private readonly Project_DIDWContext _projectDIDWContext;
        private readonly ASCENDContext _AscendContext;
        private readonly ATIContext _ATIContext;
        private readonly KIR1NContext _KIR1NContext;
        private readonly INTERTEKContext _IntertekContext;
        private readonly TESTBContext _TestbContext;
        private readonly IEmployeeService _employeeService;
        private readonly ICusVendorService _cusVendoeService;
        private readonly IProjectEmpService _projectEmpService;
        private readonly IDoService _doService;

        public DinService(ProjectMContext projectMContext, ProjectDContext projectDContext, ProjectDOContext projectDOContext,
            Project_DIDWContext project_DIDWContext,
            Project_EmpContext project_EmpContext, ASCENDContext aSCENDContext, ATIContext aTIContext, KIR1NContext kIR1NContext,
            INTERTEKContext iNTERTEKContext, TESTBContext tESTBContext, IEmployeeService employeeService, ICusVendorService cusVendoeService,
            IDoService doService, IProjectEmpService projectEmpService)
        {
            _projectMContext = projectMContext;
            _projectDContext = projectDContext;
            _projectDOContext = projectDOContext;
            _projectEmpContext = project_EmpContext;
            _projectDIDWContext = project_DIDWContext;
            _AscendContext = aSCENDContext;
            _ATIContext = aTIContext;
            _KIR1NContext = kIR1NContext;
            _IntertekContext = iNTERTEKContext;
            _TestbContext = tESTBContext;
            _employeeService = employeeService;
            _cusVendoeService = cusVendoeService;
            _projectEmpService = projectEmpService;
            _doService = doService;
        }

        public async Task<List<DinIndexViewModel>> GetDinsAsync()
        {
            var modelDin = await _projectDIDWContext.Project_DIDW.Where(e => e.DinStatus == "N") .ToListAsync();

            //加入modelDin的資料
            List<DinIndexViewModel> listModel = new List<DinIndexViewModel>();
            foreach (var item in modelDin)
            {
                var model = new DinIndexViewModel
                {
                    ProjectID = item.ProjectID,
                    DinStatus = _doService.GetStatusName(item.DinStatus)
                };

                if (!string.IsNullOrEmpty(item.DinDate) && item.DinDate.Length == 8)
                {
                    model.DinDate = item.DinDate.Substring(0, 4) + "/" + item.DinDate.Substring(4, 2) + "/" + item.DinDate.Substring(6, 2);
                }

                // 加入ProjectM的資料
                var modelProjectM = await _projectMContext.ProjectM.Where(e => e.ProjectID == item.ProjectID && e.Status == "DIN").FirstOrDefaultAsync();

                if (modelProjectM != null)
                {
                    if (!string.IsNullOrEmpty(modelProjectM.Cus_DB) && !string.IsNullOrEmpty(modelProjectM.CusID))
                    {
                        model.CusName = _cusVendoeService.GetvendorName(modelProjectM.Cus_DB, modelProjectM.CusID);
                    }
                }
                else
                {
                    continue;
                }

                // 加入ProjectD的資料
                var modelProjectD = await _projectDContext.ProjectD.Where(e => e.ProjectID == item.ProjectID && e.Stage == "DIN").FirstOrDefaultAsync();

                if (modelProjectD != null)
                {
                    if (!string.IsNullOrEmpty(modelProjectD.VendorID))
                    {
                        model.VendorName = _cusVendoeService.GetVendorName(modelProjectD.VendorID);
                    }

                    model.PartNo = modelProjectD.PartNo;
                }
                else
                {
                    continue;
                }

                listModel.Add(model);
            }
            return listModel;
        }

        public async Task<DinViewModel> GetDinAsync(string? ProjectID)
        {
            var model = new DinViewModel();

            if (!string.IsNullOrEmpty(ProjectID))
            {
                var modelDIn = await _projectDIDWContext.Project_DIDW.Where(e => e.ProjectID == ProjectID).FirstOrDefaultAsync();

                if(modelDIn != null)
                {
                    model.ProjectID = modelDIn.ProjectID;

                    if (!string.IsNullOrEmpty(modelDIn.DinDate) && modelDIn.DinDate.Length == 8)
                    {
                        model.DinDate = modelDIn.DinDate.Substring(0, 4) + "/" + modelDIn.DinDate.Substring(4, 2) + "/" + modelDIn.DinDate.Substring(6, 2);
                    }

                    model.DinStatus = modelDIn.DinStatus;
                    model.StatusName = _doService.GetStatusName(modelDIn.DinStatus);

                    var modelProjectM = await _projectMContext.ProjectM.Where(e => e.ProjectID == ProjectID && e.Status == "DIN").FirstOrDefaultAsync();

                    if(modelProjectM != null)
                    {
                        if (!string.IsNullOrEmpty(modelProjectM.Cus_DB) && !string.IsNullOrEmpty(modelProjectM.CusID))
                        {
                            model.CusName = _cusVendoeService.GetvendorName(modelProjectM.Cus_DB, modelProjectM.CusID);
                        }

                        model.EndCus = modelProjectM.EndCus;
                        model.ProApp = modelProjectM.ProApp;
                        model.ProModel = modelProjectM.ProModel;
                        model.EProduceYS = modelProjectM.EProduceYS;
                    }

                    var modelProjectD = await _projectDContext.ProjectD.Where(e => e.ProjectID == ProjectID && e.Stage == "DIN").FirstOrDefaultAsync();

                    if(modelProjectD != null)
                    {
                        if (!string.IsNullOrEmpty(modelProjectD.VendorID))
                        {                           
                            model.VendorName = _cusVendoeService.GetVendorName(modelProjectD.VendorID);
                        }
                        model.PartNo = modelProjectD.PartNo;
                        model.ELTR = modelProjectD.ELTR;
                        model.EGP = modelProjectD.EGP;
                        model.LTP = modelProjectD.ELTR * modelProjectD.EGP;
                        model.EFirstYQty = modelProjectD.EFirstYQty;
                        model.ESecondYQty = modelProjectD.ESecondYQty;
                        model.EThirdYQty = modelProjectD.EThirdYQty;
                        model.UFirstYPrice = modelProjectD.UFirstYPrice;
                        model.USecondYPrice = modelProjectD.USecondYPrice;
                        model.UThirdYPrice = modelProjectD.UThirdYPrice;
                    }

                    var modelProjectEmp_PM = await _projectEmpContext.Project_Emp.Where(e => e.ProjectID == ProjectID && e.Duty == "PM").FirstOrDefaultAsync();

                    if (modelProjectEmp_PM != null)
                    {
                        model.PM_EmpName = _employeeService.GetEmployeeName(modelProjectEmp_PM.EmpID);
                    }

                    var modelProjectEmp_Sales = await _projectEmpContext.Project_Emp.Where(e => e.ProjectID == ProjectID && e.Duty == "Sales").FirstOrDefaultAsync();

                    if (modelProjectEmp_Sales != null)
                    {
                        model.Sales_EmpName = _employeeService.GetEmployeeName(modelProjectEmp_Sales.EmpID);
                    }

                    var modelProjectEmp_FAE1 = await _projectEmpContext.Project_Emp.Where(e => e.ProjectID == ProjectID && e.Duty == "FAE1").FirstOrDefaultAsync();

                    if (modelProjectEmp_FAE1 != null)
                    {
                        model.FAE1_EmpName = _employeeService.GetEmployeeName(modelProjectEmp_FAE1.EmpID);
                    }

                    var modelProjectEmp_FAE2 = await _projectEmpContext.Project_Emp.Where(e => e.ProjectID == ProjectID && e.Duty == "FAE2").FirstOrDefaultAsync();

                    if (modelProjectEmp_FAE2 != null)
                    {
                        model.FAE2_EmpName = _employeeService.GetEmployeeName(modelProjectEmp_FAE2.EmpID);
                    }

                    var modelProjectEmp_FAE3 = await _projectEmpContext.Project_Emp.Where(e => e.ProjectID == ProjectID && e.Duty == "FAE3").FirstOrDefaultAsync();

                    if (modelProjectEmp_FAE3 != null)
                    {
                        model.FAE3_EmpName = _employeeService.GetEmployeeName(modelProjectEmp_FAE3.EmpID);
                    }
                }
            }
            return model;
        }

        //匯入Din/Dwin
        public List<DinUploadViewModel> ImportDIDW(IFormFile Excelfile)
        {
            List<DinUploadViewModel> list_DiDws = new List<DinUploadViewModel>();

            var stream = new MemoryStream();
            if (Excelfile != null)
            {
                Excelfile.CopyTo(stream);

                // 讀取stream中的所有資料
                var streamData = MiniExcel.Query(stream, true, startCell: "A4").ToList();

                    // 檢查是否有資料
                    if (streamData.Count > 0)
                {
                    for (int i = 0; i < streamData.Count; i++)
                    {
                        var rowData = streamData[i];

                        var dinsVM = new DinUploadViewModel();

                        foreach (var cellValue in rowData)
                        {
                            if(cellValue.Key == "項目")
                            {
                                continue;
                            }
                            if (cellValue.Key == "DIDWType" && cellValue.Value != null)
                            {
                                if (cellValue.Value == null)
                                {
                                    dinsVM.UploadStatus += "無Din/Dwin類別";
                                    continue;
                                }
                                else
                                {
                                    if(!string.IsNullOrEmpty(cellValue.Value) && cellValue.Value == "DIN")
                                    {
                                        dinsVM.DIDWType = cellValue.Value.ToString();
                                    }

                                    if (!string.IsNullOrEmpty(cellValue.Value) && cellValue.Value == "DWIN")
                                    {
                                        dinsVM.DIDWType = cellValue.Value.ToString();
                                    }
                                }
                            }
                            if(cellValue.Key == "專案編號")
                            {
                                if (cellValue.Value != null)
                                {
                                    dinsVM.ProjectID = cellValue.Value.ToString();
                                }
                            }
                            if (cellValue.Key == "立項日期\n( YYYY/MM/DD )")
                            {
                                if(cellValue.Value == null)
                                {
                                    dinsVM.UploadStatus += "立項日期無資料";
                                    continue;
                                }
                                else
                                {
                                    dinsVM.CreateDate = Convert.ToDateTime(cellValue.Value).ToString("yyyy/MM/dd");
                                }
                            }
                            if (cellValue.Key == "供應商")
                            {
                                if (cellValue.Value == null)
                                {
                                    dinsVM.UploadStatus += "供應商無資料;";
                                    continue;
                                }
                                else
                                {                                   
                                    dinsVM.VendorName = cellValue.Value.ToString();

                                    if (!string.IsNullOrEmpty(dinsVM.VendorName))
                                    {
                                        dinsVM.VendorID = _cusVendoeService.GetVenID(dinsVM.VendorName);
                                    }

                                    if (string.IsNullOrEmpty(dinsVM.VendorID))
                                    {
                                        dinsVM.UploadStatus += "找無相符合原廠資料-" + dinsVM.VendorName + "\n;";
                                    }
                                }
                            }
                            if (cellValue.Key == "品名料號")
                            {
                                if (cellValue.Value == null)
                                {
                                    dinsVM.UploadStatus += "品名料號無資料";
                                    continue;
                                }
                                else
                                {
                                    dinsVM.PartNo = cellValue.Value.ToString();
                                }
                            }
                            if (cellValue.Key == "客戶名稱")
                            {
                                if (cellValue.Value == null)
                                {
                                    dinsVM.UploadStatus += "客戶名稱無資料";
                                    continue;
                                }
                                else
                                {
                                    dinsVM.CusName = cellValue.Value.ToString();

                                    if (!string.IsNullOrEmpty(dinsVM.CusName))
                                    {
                                        (dinsVM.CusDB, dinsVM.CusID) = _cusVendoeService.GetCusID(dinsVM.CusName);

                                        if (string.IsNullOrEmpty(dinsVM.CusDB) || string.IsNullOrEmpty(dinsVM.CusID))
                                        {
                                            dinsVM.UploadStatus += "找無相符合客戶資料-" + dinsVM.CusName + "\n;";
                                        }
                                    }
                                }
                            }
                            if (cellValue.Key == "最終客戶")
                            {
                                if (cellValue.Value == null)
                                {
                                    dinsVM.UploadStatus += "最終客戶無資料";
                                    continue;
                                }
                                else
                                {
                                    dinsVM.EndCusName = cellValue.Value.ToString();
                                }
                            }
                            if (cellValue.Key == "產品應用")
                            {
                                if (cellValue.Value == null)
                                {
                                    dinsVM.UploadStatus += "產品應用無資料";
                                    continue;
                                }
                                else
                                {
                                    dinsVM.ProApp = cellValue.Value.ToString();
                                }
                            }
                            if (cellValue.Key == "產品型號")
                            {
                                if (cellValue.Value == null)
                                {
                                    dinsVM.UploadStatus += "產品型號無資料";
                                    continue;
                                }
                                else
                                {
                                    dinsVM.ProModel = cellValue.Value.ToString();
                                }
                            }
                            if (cellValue.Key == "預估量產年度\n( YYYY )")
                            {
                                if (cellValue.Value == null)
                                {
                                    dinsVM.UploadStatus += "預估量產年度無資料";
                                    continue;
                                }
                                else
                                {
                                    dinsVM.EProduceYear = cellValue.Value.ToString();
                                }
                            }
                            if (cellValue.Key == "預估量產季度\n( Q1 - Q4)")
                            {
                                if (cellValue.Value == null)
                                {
                                    dinsVM.UploadStatus += "預估量產季度無資料";
                                    continue;
                                }
                                else
                                {
                                    dinsVM.EProduceSeason = cellValue.Value.ToString();
                                }
                            }
                            if (cellValue.Key == "預估第一年\n( 數量 )")
                            {
                                if (cellValue.Value == null)
                                {
                                    dinsVM.UploadStatus += "預估第一年(數量)無資料";
                                    continue;
                                }
                                else
                                {
                                    dinsVM.EFirstYQty = (int)(double)cellValue.Value;
                                }
                            }
                            if (cellValue.Key == "預估第二年\n( 數量 )")
                            {
                                if (cellValue.Value == null)
                                {
                                    dinsVM.UploadStatus += "預估第二年(數量)無資料";
                                    continue;
                                }
                                else
                                {
                                    dinsVM.ESecondYQty = (int)(double)cellValue.Value;
                                }
                            }
                            if (cellValue.Key == "預估第三年\n( 數量 )")
                            {
                                if (cellValue.Value == null)
                                {
                                    dinsVM.UploadStatus += "預估第三年(數量)無資料";
                                    continue;
                                }
                                else
                                {
                                    dinsVM.EThirdYQty = (int)(double)cellValue.Value;
                                }
                            }
                            if (cellValue.Key == "單價_第一年")
                            {
                                if (cellValue.Value == null)
                                {
                                    dinsVM.UploadStatus += "單價第一年無資料";
                                    continue;
                                }
                                else
                                {
                                    dinsVM.UFirstYPrice = (double)cellValue.Value;
                                }
                            }
                            if (cellValue.Key == "單價_第二年")
                            {
                                if (cellValue.Value == null)
                                {
                                    dinsVM.UploadStatus += "單價第二年無資料";
                                    continue;
                                }
                                else
                                {
                                    dinsVM.USecondYPrice = (double)cellValue.Value;
                                }
                            }
                            if (cellValue.Key == "單價_第三年")
                            {
                                if (cellValue.Value == null)
                                {
                                    dinsVM.UploadStatus += "單價第三年無資料";
                                    continue;
                                }
                                else
                                {
                                    dinsVM.UThirdYPrice = (double)cellValue.Value;
                                }
                            }
                            if (cellValue.Key == "預估三年銷售額\n( LTR )")
                            {
                                if (cellValue.Value == null)
                                {
                                    dinsVM.UploadStatus += "預估三年銷售額無資料";
                                    continue;
                                }
                                else
                                {
                                    dinsVM.ELTR = (int)(double)cellValue.Value;
                                }
                            }
                            if (cellValue.Key == "毛利率\n( 預估 )")
                            {
                                if (cellValue.Value == null)
                                {
                                    dinsVM.UploadStatus += "毛利率無資料";
                                    continue;
                                }
                                else
                                {
                                    dinsVM.EGP = (double)cellValue.Value;
                                }
                            }
                            if (cellValue.Key == "Integration")
                            {
                                if (cellValue.Value == null)
                                {
                                    continue;
                                }
                                else
                                {
                                    if(!string.IsNullOrEmpty(cellValue.Value) && (cellValue.Value == "Y" || cellValue.Value == "N"))
                                    {
                                        if(cellValue.Value == "Y")
                                        {
                                            dinsVM.IsInte = true;
                                        }
                                        else if (cellValue.Value == "N")
                                        {
                                            dinsVM.IsInte = false;
                                        }
                                    }                                   
                                }
                            }
                            if (cellValue.Key == "PM")
                            {
                                if (cellValue.Value == null)
                                {
                                    continue;
                                }
                                else
                                {
                                    dinsVM.PM_EmpName = cellValue.Value.ToString();
                                    if (!string.IsNullOrEmpty(dinsVM.PM_EmpName))
                                    {
                                        dinsVM.PMID = _employeeService.GetEmployeeID(dinsVM.PM_EmpName);
                                        dinsVM.PM_Bonus = 0.2;
                                    }
                                }
                            }
                            if (cellValue.Key == "Sales")
                            {
                                if (cellValue.Value == null)
                                {
                                    continue;
                                }
                                else
                                {
                                    dinsVM.Sales_EmpName = cellValue.Value.ToString();
                                    if (!string.IsNullOrEmpty(dinsVM.Sales_EmpName))
                                    {
                                        dinsVM.SalesID = _employeeService.GetEmployeeID(dinsVM.Sales_EmpName);
                                        dinsVM.Sales_Bonus = 0.4;
                                    }
                                }
                            }
                            if (cellValue.Key == "FAE1")
                            {
                                if (cellValue.Value == null)
                                {
                                    continue;
                                }
                                else
                                {
                                    dinsVM.FAE1_EmpName = cellValue.Value.ToString();
                                    if (!string.IsNullOrEmpty(dinsVM.FAE1_EmpName))
                                    {
                                        dinsVM.FAE1ID = _employeeService.GetEmployeeID(dinsVM.FAE1_EmpName);
                                    }
                                }
                            }
                            if (cellValue.Key == "FAE1%")
                            {
                                if (cellValue.Value == null)
                                {
                                    continue;
                                }
                                else
                                {
                                    dinsVM.FAE1_Bonus = (double)cellValue.Value;
                                }
                            }
                            if (cellValue.Key == "FAE2")
                            {
                                if (cellValue.Value == null)
                                {
                                    continue;
                                }
                                else
                                {
                                    dinsVM.FAE2_EmpName = cellValue.Value.ToString();
                                    if (!string.IsNullOrEmpty(dinsVM.FAE2_EmpName))
                                    {
                                        dinsVM.FAE2ID = _employeeService.GetEmployeeID(dinsVM.FAE2_EmpName);
                                    }
                                }
                            }
                            if (cellValue.Key == "FAE2%")
                            {
                                if (cellValue.Value == null)
                                {
                                    continue;
                                }
                                else
                                {
                                    dinsVM.FAE2_Bonus = (double)cellValue.Value;
                                }
                            }
                            if (cellValue.Key == "FAE3")
                            {
                                if (cellValue.Value == null)
                                {
                                    continue;
                                }
                                else
                                {
                                    dinsVM.FAE3_EmpName = cellValue.Value.ToString();
                                    if (!string.IsNullOrEmpty(dinsVM.FAE3_EmpName))
                                    {
                                        dinsVM.FAE3ID = _employeeService.GetEmployeeID(dinsVM.FAE3_EmpName);
                                    }
                                }
                            }
                            if (cellValue.Key == "FAE3%")
                            {
                                if (cellValue.Value == null)
                                {
                                    continue;
                                }
                                else
                                {
                                    dinsVM.FAE3_Bonus = (double)cellValue.Value;
                                }
                            }                       
                        }
                        list_DiDws.Add(dinsVM);
                    }
                    InsertDiDws(list_DiDws); 
                }
            }
            return list_DiDws;
        }

        private string CheckInsert(DinUploadViewModel viewModel)
        {
            string Message = string.Empty;

            if (!string.IsNullOrEmpty(viewModel.ProApp) && viewModel.ProApp.Length > 40)
            {
                Message += "產品應用文字長度大於40個字;\n";
            }

            if (!string.IsNullOrEmpty(viewModel.ProModel) && viewModel.ProModel.Length > 20)
            {
                Message += "產品型號文字長度大於20個字;\n";
            }

            if (!string.IsNullOrEmpty(viewModel.PartNo) && viewModel.PartNo.Length > 50)
            {
                Message += "品名料號長度大於50個字;\n";
            }

            if (!string.IsNullOrEmpty(viewModel.CreateDate) && viewModel.CreateDate.Length > 10)
            {
                Message += "時間格式錯誤;\n";
            }

            if(!string.IsNullOrEmpty(viewModel.EProduceYear) && viewModel.EProduceYear.Length > 4)
            {
                Message += "預估量產年份文字長度超過4個字;\n";
            }

            if (!string.IsNullOrEmpty(viewModel.EProduceSeason) && viewModel.EProduceSeason.Length > 2)
            {
                Message += "預估量產季度文字長度超過2個字;\n";
            }

            if (!string.IsNullOrEmpty(viewModel.EndCusName) && viewModel.EndCusName.Length > 20)
            {
                Message += "最終客戶文字長度超過20個字;\n";
            }

            return Message;
        }

        private List<DinUploadViewModel> InsertDiDws(List<DinUploadViewModel> list_uploads)
        {
            List<DinUploadViewModel> list_Result = new List<DinUploadViewModel>();
            
            if (list_uploads.Count > 0)
            {
                foreach (var item in list_uploads) 
                {
                    item.UploadStatus = CheckInsert(item);

                    if (string.IsNullOrEmpty(item.UploadStatus))
                    {
                        if (!string.IsNullOrEmpty(item.CreateDate))
                        {
                            item.CreateDate = item.CreateDate.Replace("/", "");
                        }

                        //如果有ProjectID:Update ProjectM 、 ProjectD ; Insert Project DIDW
                        //如果沒有ProjectID: Insert ProjectM 、 ProjectD、Project DIDW
                        DinUploadViewModel NewModel = new DinUploadViewModel();

                        if (!string.IsNullOrEmpty(item.ProjectID))
                        {
                            if (!string.IsNullOrEmpty(item.DIDWType) && item.DIDWType == "DIN")
                            {
                                NewModel = UpdateDin(item);
                            }

                            if (!string.IsNullOrEmpty(item.DIDWType) && item.DIDWType == "DWIN")
                            {
                                NewModel = UpdateDwin(item);
                            }
                            list_Result.Add(NewModel);
                        }
                        else
                        {
                            item.ProjectID = _doService.GetProjectID(DateTime.Now.ToString("yyyyMMdd"));

                            if (!string.IsNullOrEmpty(item.DIDWType) && item.DIDWType == "DIN")
                            {
                                NewModel = InsertDin(item);
                            }

                            if (!string.IsNullOrEmpty(item.DIDWType) && item.DIDWType == "DWIN")
                            {
                                NewModel = InsertDwin(item);
                            }
                            list_Result.Add(NewModel);
                        }
                    }
                }
            }
            return list_Result;
        }

        private DinUploadViewModel InsertDin(DinUploadViewModel model)
        {
            try
            {
                // Insert a record into ProjectM
                var modelprojectM = new ProjectM
                {
                    ProjectID = model.ProjectID,
                    Status = "DIN", // Status: DIN
                    CreateDate = model.CreateDate,
                    Cus_DB = model.CusDB,
                    CusID = model.CusID,
                    EndCus = model.EndCusName,
                    ProApp = model.ProApp,
                    ProModel = model.ProModel
                };
                if(!string.IsNullOrEmpty(model.EProduceYear) && !string.IsNullOrEmpty(model.EProduceSeason))
                {
                    modelprojectM.EProduceYS = model.EProduceYear + model.EProduceSeason;
                }
                _projectMContext.Add(modelprojectM);
                _projectMContext.SaveChanges();

                // Insert a record into ProjectD
                var modelprojectD = new ProjectD
                {
                    ProjectID = model.ProjectID,
                    VendorID = model.VendorID,
                    PartNo = model.PartNo,
                    Stage = "DIN",
                    ELTR = model.ELTR,
                    EGP = model.EGP,
                    EFirstYQty = model.EFirstYQty,
                    ESecondYQty = model.ESecondYQty,
                    EThirdYQty = model.EThirdYQty,
                    UFirstYPrice = model.UFirstYPrice,
                    USecondYPrice = model.USecondYPrice,
                    UThirdYPrice = model.UThirdYPrice
                };
                _projectDContext.Add(modelprojectD);
                _projectDContext.SaveChanges();

                // Insert a record into Project_DIDW
                var modelprojectDIDW = new Project_DIDW
                {
                    ProjectID = model.ProjectID,
                    DinDate = model.CreateDate,
                    DinStatus = "N"
                };
                if (!string.IsNullOrEmpty(model.ProjectID))
                {
                    modelprojectDIDW.DoID = _doService.QueryDoID(model.ProjectID);
                }
                _projectDIDWContext.Add(modelprojectDIDW);
                _projectDIDWContext.SaveChanges();

                //檢查PM和Sales是否為同一人，if true PM獎金比例 = 0%
                if (model.PMID != 0 && model.SalesID != 0 && model.PMID == model.SalesID)
                {
                    model.PM_Bonus = 0;
                }

                //Insert a record into Project_Emp(PM)
                if (model.PMID != 0)
                {
                    var modelprojectEmp_PM = new Project_Emp
                    {
                        SEQ = _projectEmpService.NewSEQ,
                        ProjectID = model.ProjectID,
                        EmpID = model.PMID,
                        BonusP = model.PM_Bonus,
                        Duty = "PM"
                    };
                    _projectEmpContext.Add(modelprojectEmp_PM);
                    _projectEmpContext.SaveChanges();
                }

                //Insert a record into Project_Emp(Sales)
                if (model.SalesID != 0)
                {
                    var modelprojectEmp_Sales = new Project_Emp
                    {
                        SEQ = _projectEmpService.NewSEQ,
                        ProjectID = model.ProjectID,
                        EmpID = model.SalesID,
                        BonusP = model.Sales_Bonus,
                        Duty = "Sales"
                    };
                    _projectEmpContext.Add(modelprojectEmp_Sales);
                    _projectEmpContext.SaveChanges();
                }

                //Insert a record into Project_Emp(FAE1)
                if (model.FAE1ID != 0)
                {
                    var modelprojectEmp_FAE1 = new Project_Emp
                    {
                        SEQ = _projectEmpService.NewSEQ,
                        ProjectID = model.ProjectID,
                        EmpID = model.FAE1ID,
                        BonusP = model.FAE1_Bonus,
                        Duty = "FAE1"
                    };
                    _projectEmpContext.Add(modelprojectEmp_FAE1);
                    _projectEmpContext.SaveChanges();
                }

                //Insert a record into Project_Emp(FAE2)
                if (model.FAE2ID != 0)
                {
                    var modelprojectEmp_FAE2 = new Project_Emp
                    {
                        SEQ = _projectEmpService.NewSEQ,
                        ProjectID = model.ProjectID,
                        EmpID = model.FAE2ID,
                        BonusP = model.FAE2_Bonus,
                        Duty = "FAE2"
                    };
                    _projectEmpContext.Add(modelprojectEmp_FAE2);
                    _projectEmpContext.SaveChanges();
                }

                //Insert a record into Project_Emp(FAE3)
                if (model.FAE3ID != 0)
                {
                    var modelprojectEmp_FAE3 = new Project_Emp
                    {
                        SEQ = _projectEmpService.NewSEQ,
                        ProjectID = model.ProjectID,
                        EmpID = model.FAE3ID,
                        BonusP = model.FAE3_Bonus,
                        Duty = "FAE3"
                    };
                    _projectEmpContext.Add(modelprojectEmp_FAE3);
                    _projectEmpContext.SaveChanges();
                }
                model.UploadStatus = "Success";
            }
            catch (DbUpdateException ex)
            {
                var innerException = ex.InnerException;
                if (innerException != null)
                {
                    // 記錄詳細的 SQL 錯誤訊息
                    model.UploadStatus = $"SQL Error: {innerException.Message}";
                }
                else
                {
                    // 記錄其他資料庫更新錯誤
                    model.UploadStatus = $"Database Update Error: {ex.Message}";
                }
            }
            catch (Exception ex)
            {
                // 記錄一般錯誤
                model.UploadStatus = $"General Error: {ex.Message}";
            }

            return model;
        }
        private DinUploadViewModel InsertDwin(DinUploadViewModel model)
        {
            try
            {
                // Insert a record into ProjectM
                var modelprojectM = new ProjectM
                {
                    ProjectID = model.ProjectID,
                    Status = "DWIN", // Status: DIN
                    CreateDate = model.CreateDate,
                    Cus_DB = model.CusDB,
                    CusID = model.CusID,
                    EndCus = model.EndCusName,
                    ProApp = model.ProApp,
                    ProModel = model.ProModel
                };
                if (!string.IsNullOrEmpty(model.EProduceYear) && !string.IsNullOrEmpty(model.EProduceSeason))
                {
                    modelprojectM.EProduceYS = model.EProduceYear + model.EProduceSeason;
                }
                _projectMContext.Add(modelprojectM);
                _projectMContext.SaveChanges();

                // Insert a record into ProjectD
                var modelprojectD = new ProjectD
                {
                    ProjectID = model.ProjectID,
                    VendorID = model.VendorID,
                    PartNo = model.PartNo,
                    Stage = "DWIN",
                    ELTR = model.ELTR,
                    EGP = model.EGP,
                    EFirstYQty = model.EFirstYQty,
                    ESecondYQty = model.ESecondYQty,
                    EThirdYQty = model.EThirdYQty,
                    UFirstYPrice = model.UFirstYPrice,
                    USecondYPrice = model.USecondYPrice,
                    UThirdYPrice = model.UThirdYPrice
                };
                _projectDContext.Add(modelprojectD);
                _projectDContext.SaveChanges();

                // Insert a record into Project_DIDW
                var modelprojectDIDW = new Project_DIDW
                {
                    ProjectID = model.ProjectID,
                    DwinDate = model.CreateDate,
                    DwinStatus = "N"
                };
                if (!string.IsNullOrEmpty(model.ProjectID))
                {
                    modelprojectDIDW.DoID = _doService.QueryDoID(model.ProjectID);
                }
                _projectDIDWContext.Add(modelprojectDIDW);
                _projectDIDWContext.SaveChanges();

                //檢查PM和Sales是否為同一人，if true PM獎金比例 = 0%
                if (model.PMID != 0 && model.SalesID != 0 && model.PMID == model.SalesID)
                {
                    model.PM_Bonus = 0;
                }

                //Insert a record into Project_Emp(PM)
                if (model.PMID != 0)
                {
                    var modelprojectEmp_PM = new Project_Emp
                    {
                        SEQ = _projectEmpService.NewSEQ,
                        ProjectID = model.ProjectID,
                        EmpID = model.PMID,
                        BonusP = model.PM_Bonus,
                        Duty = "PM"
                    };
                    _projectEmpContext.Add(modelprojectEmp_PM);
                    _projectEmpContext.SaveChanges();
                }

                //Insert a record into Project_Emp(Sales)
                if (model.SalesID != 0)
                {
                    var modelprojectEmp_Sales = new Project_Emp
                    {
                        SEQ = _projectEmpService.NewSEQ,
                        ProjectID = model.ProjectID,
                        EmpID = model.SalesID,
                        BonusP = model.Sales_Bonus,
                        Duty = "Sales"
                    };
                    _projectEmpContext.Add(modelprojectEmp_Sales);
                    _projectEmpContext.SaveChanges();
                }

                //Insert a record into Project_Emp(FAE1)
                if (model.FAE1ID != 0)
                {
                    var modelprojectEmp_FAE1 = new Project_Emp
                    {
                        SEQ = _projectEmpService.NewSEQ,
                        ProjectID = model.ProjectID,
                        EmpID = model.FAE1ID,
                        BonusP = model.FAE1_Bonus,
                        Duty = "FAE1"
                    };
                    _projectEmpContext.Add(modelprojectEmp_FAE1);
                    _projectEmpContext.SaveChanges();
                }

                //Insert a record into Project_Emp(FAE2)
                if (model.FAE2ID != 0)
                {
                    var modelprojectEmp_FAE2 = new Project_Emp
                    {
                        SEQ = _projectEmpService.NewSEQ,
                        ProjectID = model.ProjectID,
                        EmpID = model.FAE2ID,
                        BonusP = model.FAE2_Bonus,
                        Duty = "FAE2"
                    };
                    _projectEmpContext.Add(modelprojectEmp_FAE2);
                    _projectEmpContext.SaveChanges();
                }

                //Insert a record into Project_Emp(FAE3)
                if (model.FAE3ID != 0)
                {
                    var modelprojectEmp_FAE3 = new Project_Emp
                    {
                        SEQ = _projectEmpService.NewSEQ,
                        ProjectID = model.ProjectID,
                        EmpID = model.FAE3ID,
                        BonusP = model.FAE3_Bonus,
                        Duty = "FAE3"
                    };
                    _projectEmpContext.Add(modelprojectEmp_FAE3);
                    _projectEmpContext.SaveChanges();
                }
                model.UploadStatus = "Success";
            }
            catch (Exception ex)
            {
                model.UploadStatus = ex.ToString();
            }

            return model;
        }
        private DinUploadViewModel UpdateDin(DinUploadViewModel model)
        {
            try
            {
                //Update record from ProjectM
                var modelprojectM = _projectMContext.ProjectM.Where(e => e.ProjectID == model.ProjectID).FirstOrDefault();
                var modelprojectD = _projectDContext.ProjectD.Where(e => e.ProjectID == model.ProjectID).FirstOrDefault();

                if (modelprojectM != null && modelprojectD != null)
                {
                    if(modelprojectM.Status != "DO")
                    {
                        model.UploadStatus += "專案狀態不為Do，不可更新Din";
                        return model;
                    }
                    else
                    {
                        modelprojectM.Status = "DIN";
                        modelprojectM.UpdateDate = model.CreateDate;
                        modelprojectM.EndCus = model.EndCusName;
                        modelprojectM.ProApp = model.ProApp;
                        modelprojectM.ProModel = model.ProModel;
                        modelprojectM.IsInte = model.IsInte;
                    }
                    if (!string.IsNullOrEmpty(model.EProduceYear) && !string.IsNullOrEmpty(model.EProduceSeason))
                    {
                        modelprojectM.EProduceYS = model.EProduceYear + model.EProduceSeason;
                    }
                    _projectMContext.SaveChanges();

                    //Update record from ProjectD
                    modelprojectD.PartNo = model.PartNo;
                    modelprojectD.ELTR = model.ELTR;
                    modelprojectD.EGP = model.EGP;
                    modelprojectD.EFirstYQty = model.EFirstYQty;
                    modelprojectD.ESecondYQty = model.ESecondYQty;
                    modelprojectD.EThirdYQty = model.EThirdYQty;
                    modelprojectD.UFirstYPrice = model.UFirstYPrice;
                    modelprojectD.USecondYPrice = model.USecondYPrice;
                    modelprojectD.UThirdYPrice = model.UThirdYPrice;
                    modelprojectD.Stage = "DIN";

                    _projectDContext.SaveChanges();
                }
                else
                {
                    model.UploadStatus += "找不到專案檔，不能更新資料";
                    return model;
                }

                // Insert a record into Project_DIDW
                var modelprojectDIDW = new Project_DIDW
                {
                    ProjectID = model.ProjectID,
                    DinDate = model.CreateDate,
                    DinStatus = "N"
                };
                if (!string.IsNullOrEmpty(model.ProjectID))
                {
                    modelprojectDIDW.DoID = _doService.QueryDoID(model.ProjectID);
                }
                _projectDIDWContext.Add(modelprojectDIDW);
                _projectDIDWContext.SaveChanges();

                //檢查PM和Sales是否為同一人，if true PM獎金比例 = 0%
                if (model.PMID != 0 && model.SalesID != 0 && model.PMID == model.SalesID)
                {
                    model.PM_Bonus = 0;
                }

                //Insert a record into Project_Emp(PM)
                if (model.PMID != 0)
                {
                    var modelprojectEmp_PM = new Project_Emp
                    {
                        SEQ = _projectEmpService.NewSEQ,
                        ProjectID = model.ProjectID,
                        EmpID = model.PMID,
                        BonusP = model.PM_Bonus,
                        Duty = "PM"
                    };
                    _projectEmpContext.Add(modelprojectEmp_PM);
                    _projectEmpContext.SaveChanges();
                }

                //Insert a record into Project_Emp(Sales)
                if (model.SalesID != 0)
                {
                    var modelprojectEmp_Sales = new Project_Emp
                    {
                        SEQ = _projectEmpService.NewSEQ,
                        ProjectID = model.ProjectID,
                        EmpID = model.SalesID,
                        BonusP = model.Sales_Bonus,
                        Duty = "Sales"
                    };
                    _projectEmpContext.Add(modelprojectEmp_Sales);
                    _projectEmpContext.SaveChanges();
                }

                //Insert a record into Project_Emp(FAE1)
                if (model.FAE1ID != 0)
                {
                    var modelprojectEmp_FAE1 = new Project_Emp
                    {
                        SEQ = _projectEmpService.NewSEQ,
                        ProjectID = model.ProjectID,
                        EmpID = model.FAE1ID,
                        BonusP = model.FAE1_Bonus,
                        Duty = "FAE1"
                    };
                    _projectEmpContext.Add(modelprojectEmp_FAE1);
                    _projectEmpContext.SaveChanges();
                }

                //Insert a record into Project_Emp(FAE2)
                if (model.FAE2ID != 0)
                {
                    var modelprojectEmp_FAE2 = new Project_Emp
                    {
                        SEQ = _projectEmpService.NewSEQ,
                        ProjectID = model.ProjectID,
                        EmpID = model.FAE2ID,
                        BonusP = model.FAE2_Bonus,
                        Duty = "FAE2"
                    };
                    _projectEmpContext.Add(modelprojectEmp_FAE2);
                    _projectEmpContext.SaveChanges();
                }

                //Insert a record into Project_Emp(FAE3)
                if (model.FAE3ID != 0)
                {
                    var modelprojectEmp_FAE3 = new Project_Emp
                    {
                        SEQ = _projectEmpService.NewSEQ,
                        ProjectID = model.ProjectID,
                        EmpID = model.FAE3ID,
                        BonusP = model.FAE3_Bonus,
                        Duty = "FAE3"
                    };
                    _projectEmpContext.Add(modelprojectEmp_FAE3);
                    _projectEmpContext.SaveChanges();
                }
                model.UploadStatus = "Success";
            }
            catch (Exception ex)
            {
                model.UploadStatus = ex.ToString();
            }

            return model;
        }
        private DinUploadViewModel UpdateDwin(DinUploadViewModel model)
        {
            try
            {
                //Update record from ProjectM、Project_DIDW
                var modelprojectM = _projectMContext.ProjectM.Where(e => e.ProjectID == model.ProjectID).FirstOrDefault();
                var modelprojectDIDW = _projectDIDWContext.Project_DIDW.Where(e => e.ProjectID == model.ProjectID).FirstOrDefault();

                if (modelprojectM != null && modelprojectDIDW != null)
                {
                    if (modelprojectM.Status != "DIN")
                    {
                        model.UploadStatus += "專案狀態不為Din，不可更新Din";
                        return model;
                    }
                    else
                    {
                        modelprojectM.Status = "DWIN";
                        modelprojectM.UpdateDate = model.CreateDate;
                    }
                    _projectMContext.SaveChanges();

                    modelprojectDIDW.DwinDate = model.CreateDate;
                    modelprojectDIDW.DwinStatus = "N";

                    _projectDIDWContext.SaveChanges();
                }
                else
                {
                    model.UploadStatus += "找不到專案檔，不能更新資料";
                    return model;
                }

                // Insert a record into ProjectD
                var modelprojectD = new ProjectD
                {
                    ProjectID = model.ProjectID,
                    VendorID = model.VendorID,
                    PartNo = model.PartNo,
                    Stage = "DWIN",
                    ELTR = model.ELTR,
                    EGP = model.EGP,
                    EFirstYQty = model.EFirstYQty,
                    ESecondYQty = model.ESecondYQty,
                    EThirdYQty = model.EThirdYQty,
                    UFirstYPrice = model.UFirstYPrice,
                    USecondYPrice = model.USecondYPrice,
                    UThirdYPrice = model.UThirdYPrice
                };
                _projectDContext.Add(modelprojectD);
                _projectDContext.SaveChanges();

                //檢查PM和Sales是否為同一人，if true PM獎金比例 = 0%
                if (model.PMID != 0 && model.SalesID != 0 && model.PMID == model.SalesID)
                {
                    model.PM_Bonus = 0;
                }

                if (model.PMID != 0)
                {
                    var chkPM = _projectEmpContext.Project_Emp.Where(e => e.ProjectID == model.ProjectID && e.Duty == "PM").FirstOrDefault();

                    if (chkPM == null)
                    {
                        //Insert a record into Project_Emp(PM)
                        var modelprojectEmp_PM = new Project_Emp
                        {
                            SEQ = _projectEmpService.NewSEQ,
                            ProjectID = model.ProjectID,
                            EmpID = model.PMID,
                            BonusP = model.PM_Bonus,
                            Duty = "PM"
                        };
                        _projectEmpContext.Add(modelprojectEmp_PM);
                        _projectEmpContext.SaveChanges();
                    }
                }

                if (model.SalesID != 0)
                {
                    var chkPM = _projectEmpContext.Project_Emp.Where(e => e.ProjectID == model.ProjectID && e.Duty == "Sales").FirstOrDefault();

                    if (chkPM == null)
                    {
                        //Insert a record into Project_Emp(Sales)
                        var modelprojectEmp_Sales = new Project_Emp
                        {
                            SEQ = _projectEmpService.NewSEQ,
                            ProjectID = model.ProjectID,
                            EmpID = model.SalesID,
                            BonusP = model.Sales_Bonus,
                            Duty = "Sales"
                        };
                        _projectEmpContext.Add(modelprojectEmp_Sales);
                        _projectEmpContext.SaveChanges();
                    }
                }

                if (model.FAE1ID != 0)
                {
                    var chkPM = _projectEmpContext.Project_Emp.Where(e => e.ProjectID == model.ProjectID && e.Duty == "FAE1").FirstOrDefault();

                    if (chkPM == null)
                    {
                        //Insert a record into Project_Emp(FAE1)
                        var modelprojectEmp_FAE1 = new Project_Emp
                        {
                            SEQ = _projectEmpService.NewSEQ,
                            ProjectID = model.ProjectID,
                            EmpID = model.FAE1ID,
                            BonusP = model.FAE1_Bonus,
                            Duty = "FAE1"
                        };
                        _projectEmpContext.Add(modelprojectEmp_FAE1);
                        _projectEmpContext.SaveChanges();
                    }
                }

                if (model.FAE2ID != 0)
                {
                    var chkPM = _projectEmpContext.Project_Emp.Where(e => e.ProjectID == model.ProjectID && e.Duty == "FAE2").FirstOrDefault();

                    if (chkPM == null)
                    {
                        //Insert a record into Project_Emp(FAE2)
                        var modelprojectEmp_FAE2 = new Project_Emp
                        {
                            SEQ = _projectEmpService.NewSEQ,
                            ProjectID = model.ProjectID,
                            EmpID = model.FAE1ID,
                            BonusP = model.FAE1_Bonus,
                            Duty = "FAE2"
                        };
                        _projectEmpContext.Add(modelprojectEmp_FAE2);
                        _projectEmpContext.SaveChanges();
                    }
                }

                if (model.FAE3ID != 0)
                {
                    var chkPM = _projectEmpContext.Project_Emp.Where(e => e.ProjectID == model.ProjectID && e.Duty == "FAE3").FirstOrDefault();

                    if (chkPM == null)
                    {
                        //Insert a record into Project_Emp(FAE2)
                        var modelprojectEmp_FAE3 = new Project_Emp
                        {
                            SEQ = _projectEmpService.NewSEQ,
                            ProjectID = model.ProjectID,
                            EmpID = model.FAE1ID,
                            BonusP = model.FAE1_Bonus,
                            Duty = "FAE3"
                        };
                        _projectEmpContext.Add(modelprojectEmp_FAE3);
                        _projectEmpContext.SaveChanges();
                    }
                }
                model.UploadStatus = "Success";
            }
            catch (Exception ex)
            {
                model.UploadStatus = ex.ToString();
            }

            return model;
        }

        public async Task<DinCreateViewModel> GetEditDin(string? ProjectID)
        {
            var model = new DinCreateViewModel();

            if (!string.IsNullOrEmpty(ProjectID))
            {
                var modelDIn = await _projectDIDWContext.Project_DIDW.Where(e => e.ProjectID == ProjectID).FirstOrDefaultAsync();

                if (modelDIn != null)
                {
                    model.ProjectID = modelDIn.ProjectID;

                    if (!string.IsNullOrEmpty(modelDIn.DinDate) && modelDIn.DinDate.Length == 8)
                    {
                        model.DinDate = modelDIn.DinDate.Substring(0, 4) + "/" + modelDIn.DinDate.Substring(4, 2) + "/" + modelDIn.DinDate.Substring(6, 2);
                    }

                    var modelProjectM = await _projectMContext.ProjectM.Where(e => e.ProjectID == ProjectID && e.Status == "DIN").FirstOrDefaultAsync();

                    if (modelProjectM != null)
                    {
                        if (!string.IsNullOrEmpty(modelProjectM.Cus_DB) && !string.IsNullOrEmpty(modelProjectM.CusID))
                        {
                            model.CusName = _cusVendoeService.GetvendorName(modelProjectM.Cus_DB, modelProjectM.CusID);
                        }

                        model.EndCus = modelProjectM.EndCus;
                        model.ProApp = modelProjectM.ProApp;
                        model.ProModel = modelProjectM.ProModel;
                        model.EProduceYS = modelProjectM.EProduceYS;
                    }

                    var modelProjectD = await _projectDContext.ProjectD.Where(e => e.ProjectID == ProjectID && e.Stage == "DIN").FirstOrDefaultAsync();

                    if (modelProjectD != null)
                    {
                        if (!string.IsNullOrEmpty(modelProjectD.VendorID))
                        {
                            model.VendorName = _cusVendoeService.GetVendorName(modelProjectD.VendorID);
                        }
                        model.PartNo = modelProjectD.PartNo;
                        model.ELTR = modelProjectD.ELTR;
                        model.EGP = modelProjectD.EGP;
                        model.LTP = modelProjectD.ELTR * modelProjectD.EGP;
                        model.EFirstYQty = modelProjectD.EFirstYQty;
                        model.ESecondYQty = modelProjectD.ESecondYQty;
                        model.EThirdYQty = modelProjectD.EThirdYQty;
                        model.UFirstYPrice = modelProjectD.UFirstYPrice;
                        model.USecondYPrice = modelProjectD.USecondYPrice;
                        model.UThirdYPrice = modelProjectD.UThirdYPrice;
                    }

                    var modelProjectEmp_PM = await _projectEmpContext.Project_Emp.Where(e => e.ProjectID == ProjectID && e.Duty == "PM").FirstOrDefaultAsync();

                    if (modelProjectEmp_PM != null)
                    {
                        model.PMID = modelProjectEmp_PM.EmpID;
                    }

                    var modelProjectEmp_Sales = await _projectEmpContext.Project_Emp.Where(e => e.ProjectID == ProjectID && e.Duty == "Sales").FirstOrDefaultAsync();

                    if (modelProjectEmp_Sales != null)
                    {
                        model.SalesID = modelProjectEmp_Sales.EmpID;
                    }

                    var modelProjectEmp_FAE1 = await _projectEmpContext.Project_Emp.Where(e => e.ProjectID == ProjectID && e.Duty == "FAE1").FirstOrDefaultAsync();

                    if (modelProjectEmp_FAE1 != null)
                    {
                        model.FAE1ID = modelProjectEmp_FAE1.EmpID;
                    }

                    var modelProjectEmp_FAE2 = await _projectEmpContext.Project_Emp.Where(e => e.ProjectID == ProjectID && e.Duty == "FAE2").FirstOrDefaultAsync();

                    if (modelProjectEmp_FAE2 != null)
                    {
                        model.FAE2ID = modelProjectEmp_FAE2.EmpID;
                    }

                    var modelProjectEmp_FAE3 = await _projectEmpContext.Project_Emp.Where(e => e.ProjectID == ProjectID && e.Duty == "FAE3").FirstOrDefaultAsync();

                    if (modelProjectEmp_FAE3 != null)
                    {
                        model.FAE3ID = modelProjectEmp_FAE3.EmpID;
                    }
                }
            }
            return model;
        }

        public string RejectDin(string ProjectID)
        {
            string msg = string.Empty;

            if (string.IsNullOrEmpty(ProjectID))
            {
                msg = "ProjectID為空值";
            }
            else
            {
                var ProjectDin = _projectDIDWContext.Project_DIDW.Where(e => e.ProjectID == ProjectID).FirstOrDefault();

                if (ProjectDin != null)
                {
                    if(ProjectDin.DinStatus == "R")
                    {
                        msg = "專案" + ProjectID + "已經Reject。不可重複Reject";
                    }
                    else
                    {
                        {
                            ProjectDin.DinStatus = "R";  // Din狀態改為審核拒絕
                            _projectDIDWContext.Update(ProjectDin);
                            _projectDIDWContext.SaveChanges();

                            var ProjectM = _projectMContext.ProjectM.Where(e => e.ProjectID == ProjectID).FirstOrDefault();

                            if (ProjectM != null)
                            {
                                ProjectM.Status = "F"; //專案狀態改為結案
                                ProjectM.UpdateDate = DateTime.Now.ToString("yyyyMMdd");
                                _projectMContext.Update(ProjectM);
                                _projectMContext.SaveChanges();
                            }

                            msg = "專案" + ProjectID + "Reject完成";
                        }
                    }
                }
                else
                {
                    msg = "無對應ProjectID";
                }
            }

            return msg;
        }
        public string ConfirmDin(string ProjectID)
        {
            string msg = string.Empty;

            if (string.IsNullOrEmpty(ProjectID))
            {
                msg = "ProjectID為空值";
            }
            else
            {
                var ProjectDin = _projectDIDWContext.Project_DIDW.Where(e => e.ProjectID == ProjectID).FirstOrDefault();

                if (ProjectDin != null)
                {
                    if (ProjectDin.DinStatus == "C")
                    {
                        msg = "專案" + ProjectID + "已經Confirm。不可重複Confirm";
                    }
                    else
                    {
                        {
                            ProjectDin.DinStatus = "C";  // Din狀態改為審核同意
                            _projectDIDWContext.Update(ProjectDin);
                            _projectDIDWContext.SaveChanges();

                            var ProjectM = _projectMContext.ProjectM.Where(e => e.ProjectID == ProjectID).FirstOrDefault();

                            if (ProjectM != null)
                            {
                                ProjectM.UpdateDate = DateTime.Now.ToString("yyyyMMdd");
                                _projectMContext.Update(ProjectM);
                                _projectMContext.SaveChanges();
                            }

                            msg = "專案" + ProjectID + "Confirm完成";
                        }
                    }
                }
                else
                {
                    msg = "無對應ProjectID";
                }
            }

            return msg;
        }

    }
}