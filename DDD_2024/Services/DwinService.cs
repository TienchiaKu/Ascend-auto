using DDD_2024.Data;
using DDD_2024.Interfaces;
using DDD_2024.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.DotNet.Scaffolding.Shared.ProjectModel;
using Microsoft.EntityFrameworkCore;
using MiniExcelLibs;

namespace DDD_2024.Services
{
    public class DwinService: IDwinService
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
        private readonly ICusVendoeService _cusVendoeService;
        private readonly IProjectEmpService _projectEmpService;
        private readonly IDoService _doService;

        public DwinService(ProjectMContext projectMContext, ProjectDContext projectDContext, ProjectDOContext projectDOContext,
            Project_DIDWContext project_DIDWContext,
            Project_EmpContext project_EmpContext, ASCENDContext aSCENDContext, ATIContext aTIContext, KIR1NContext kIR1NContext,
            INTERTEKContext iNTERTEKContext, TESTBContext tESTBContext, IEmployeeService employeeService, ICusVendoeService cusVendoeService,
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

        public async Task<List<DwinViewModel>> GetDwinsAsync()
        {
            var modelDIn = await _projectDIDWContext.Project_DIDW.ToListAsync();

            //加入modelDin的資料
            List<DwinViewModel> list_dwinViewModel = new List<DwinViewModel>();
            foreach (var item in modelDIn)
            {
                var model = new DwinViewModel
                {
                    ProjectID = item.ProjectID,
                    StatusName = _doService.GetStatusName(item.DwinStatus),
                    DwinStatus = item.DwinStatus
                };

                if (!string.IsNullOrEmpty(item.DwinDate) && item.DwinDate.Length == 8)
                {
                    model.DwinDate = item.DwinDate.Substring(0, 4) + "/" + item.DwinDate.Substring(4, 2) + "/" + item.DwinDate.Substring(6, 2);
                }

                // 加入ProjectM的資料
                var modelProjectM = await _projectMContext.ProjectM.Where(e => e.ProjectID == item.ProjectID && e.Status == "DWIN").FirstOrDefaultAsync();

                if (modelProjectM != null)
                {
                    if (!string.IsNullOrEmpty(modelProjectM.Cus_DB) && !string.IsNullOrEmpty(modelProjectM.CusID))
                    {
                        model.CusName = _cusVendoeService.GetvendorName(modelProjectM.Cus_DB, modelProjectM.CusID);
                    }

                    model.ProApp = modelProjectM.ProApp;
                    model.ProModel = modelProjectM.ProModel;
                }
                else
                {
                    continue;
                }

                // 加入ProjectD的資料
                var modelProjectD = await _projectDContext.ProjectD.Where(e => e.ProjectID == item.ProjectID && e.Stage == "DWIN").FirstOrDefaultAsync();

                if (modelProjectD != null)
                {
                    if (!string.IsNullOrEmpty(modelProjectD.VendorID))
                    {
                        model.VendorName = _cusVendoeService.GetvendorName("ASCEND", modelProjectD.VendorID);

                        if (string.IsNullOrEmpty(model.VendorName))
                        {
                            model.VendorName = _cusVendoeService.GetvendorName("Auto", modelProjectD.VendorID);
                        }
                    }

                    model.PartNo = modelProjectD.PartNo;
                }
                else
                {
                    continue;
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

            return list_dwinViewModel;
        }

        public async Task<DwinViewModel> GetDwinAsync(string? ProjectID)
        {
            var model = new DwinViewModel();

            if (!string.IsNullOrEmpty(ProjectID))
            {
                var modelDWin = await _projectDIDWContext.Project_DIDW.Where(e => e.ProjectID == ProjectID).FirstOrDefaultAsync();

                if(modelDWin != null)
                {
                    model.ProjectID = modelDWin.ProjectID;

                    if (!string.IsNullOrEmpty(modelDWin.DwinDate) && modelDWin.DwinDate.Length == 8)
                    {
                        model.DwinDate = modelDWin.DwinDate.Substring(0, 4) + "/" + modelDWin.DwinDate.Substring(4, 2) + "/" + modelDWin.DwinDate.Substring(6, 2);
                    }

                    model.DwinStatus = modelDWin.DwinStatus;
                    model.StatusName = _doService.GetStatusName(modelDWin.DwinStatus);

                    var modelProjectM = await _projectMContext.ProjectM.Where(e => e.ProjectID == ProjectID && e.Status == "DWIN").FirstOrDefaultAsync();

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

                    var modelProjectD = await _projectDContext.ProjectD.Where(e => e.ProjectID == ProjectID && e.Stage == "DWIN").FirstOrDefaultAsync();

                    if(modelProjectD != null)
                    {
                        if (!string.IsNullOrEmpty(modelProjectD.VendorID))
                        {
                            model.VendorName = _cusVendoeService.GetvendorName("ASCEND", modelProjectD.VendorID);

                            if (string.IsNullOrEmpty(model.VendorName))
                            {
                                model.VendorName = _cusVendoeService.GetvendorName("Auto", modelProjectD.VendorID);
                            }
                        }
                        model.PartNo = modelProjectD.PartNo;
                        model.ELTR = modelProjectD.ELTR;
                        model.EGP = modelProjectD.EGP;
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

        public List<DwinViewModel> ReadExcel(IFormFile Excelfile)
        {
            List<DwinViewModel> list = new List<DwinViewModel>();

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

                        //檢查是否有資料
                        bool hasData = false;

                        foreach (var cellValue in rowData)
                        {
                            if (cellValue.Key == "供應商" && cellValue.Value != null)
                            {
                                hasData = true;
                                break;
                            }
                        }

                        if (!hasData)
                        {
                            continue;
                        }

                        var DwinViewModel = new DwinViewModel();

                        foreach (var cellValue in rowData)
                        {
                            //測試用
                            if(cellValue.Key == "項目")
                            {
                                continue;
                            }
                                                        
                            if (cellValue.Key == "Type" && cellValue.Value != null)
                            {
                                if (cellValue.Value == "DIN")
                                {
                                    continue;
                                }
                            }
                            
                            if (cellValue.Key == "立項日期\n( YYYY/MM/DD )" && cellValue.Value != null)
                            {
                                DwinViewModel.vmCreateDate = Convert.ToDateTime(cellValue.Value).ToString("yyyy/MM/dd");
                            }
                            if (cellValue.Key == "供應商" && cellValue.Value != null)
                            {
                                DwinViewModel.VendorName = cellValue.Value.ToString();

                                if (!string.IsNullOrEmpty(DwinViewModel.VendorName))
                                {
                                    DwinViewModel.VendorID = _cusVendoeService.GetVenID(DwinViewModel.VendorName);
                                }
                            }
                            if (cellValue.Key == "品名料號" && cellValue.Value != null)
                            {
                                DwinViewModel.PartNo = cellValue.Value.ToString();
                            }
                            if (cellValue.Key == "客戶名稱" && cellValue.Value != null)
                            {
                                DwinViewModel.CusName = cellValue.Value.ToString();

                                if (!string.IsNullOrEmpty(DwinViewModel.CusName))
                                {
                                    (DwinViewModel.Cus_DB, DwinViewModel.CusID) = _cusVendoeService.GetCusID(DwinViewModel.CusName);
                                }
                            }
                            if (cellValue.Key == "最終客戶" && cellValue.Value != null)
                            {
                                DwinViewModel.EndCus = cellValue.Value.ToString();
                            }
                            if (cellValue.Key == "產品應用" && cellValue.Value != null)
                            {
                                DwinViewModel.ProApp = cellValue.Value.ToString();
                            }
                            if (cellValue.Key == "產品型號" && cellValue.Value != null)
                            {
                                DwinViewModel.ProModel = cellValue.Value.ToString();
                            }
                            if (cellValue.Key == "預估量產年度\n( YYYY )" && cellValue.Value != null)
                            {
                                DwinViewModel.EProduceYS = cellValue.Value.ToString();
                            }
                            if (cellValue.Key == "預估量產季度\n( Q1 - Q4)" && cellValue.Value != null)
                            {
                                DwinViewModel.EProduceYS += cellValue.Value.ToString();
                            }
                            if (cellValue.Key == "預估第一年\n( 數量 )" && cellValue.Value != null)
                            {
                                DwinViewModel.EFirstYQty = (int)(double)cellValue.Value;
                            }
                            if (cellValue.Key == "預估第二年\n( 數量 )" && cellValue.Value != null)
                            {
                                DwinViewModel.ESecondYQty = (int)(double)cellValue.Value;
                            }
                            if (cellValue.Key == "預估第三年\n( 數量 )" && cellValue.Value != null)
                            {
                                DwinViewModel.EThirdYQty = (int)(double)cellValue.Value;
                            }
                            if (cellValue.Key == "單價_第一年" && cellValue.Value != null)
                            {
                                DwinViewModel.UFirstYPrice = (double)cellValue.Value;
                            }
                            if (cellValue.Key == "單價_第二年" && cellValue.Value != null)
                            {
                                DwinViewModel.USecondYPrice = (double)cellValue.Value;
                            }
                            if (cellValue.Key == "單價_第三年" && cellValue.Value != null)
                            {
                                DwinViewModel.UThirdYPrice = (double)cellValue.Value;
                            }
                            if (cellValue.Key == "預估三年銷售額\n( LTR )" && cellValue.Value != null)
                            {
                                DwinViewModel.ELTR = (int)(double)cellValue.Value;
                            }
                            if (cellValue.Key == "毛利率\n( 預估 )" && cellValue.Value != null)
                            {
                                DwinViewModel.EGP = (double)cellValue.Value;
                            }
                            if (cellValue.Key == "PM" && cellValue.Value != null)
                            {
                                DwinViewModel.PM_EmpName = cellValue.Value.ToString();

                                if (!string.IsNullOrEmpty(DwinViewModel.PM_EmpName))
                                {
                                    DwinViewModel.PMID = _employeeService.GetEmployeeID(DwinViewModel.PM_EmpName);
                                    DwinViewModel.PM_Bonus = 0.25;
                                }
                            }
                            if (cellValue.Key == "Sales" && cellValue.Value != null)
                            {
                                DwinViewModel.Sales_EmpName = cellValue.Value.ToString();

                                if (!string.IsNullOrEmpty(DwinViewModel.Sales_EmpName))
                                {
                                    DwinViewModel.SalesID = _employeeService.GetEmployeeID(DwinViewModel.Sales_EmpName);
                                    DwinViewModel.Sales_Bonus = 0.65;
                                }
                            }
                            if (cellValue.Key == "FAE1" && cellValue.Value != null)
                            {
                                DwinViewModel.FAE1_EmpName = cellValue.Value.ToString();

                                if (!string.IsNullOrEmpty(DwinViewModel.FAE1_EmpName))
                                {
                                    DwinViewModel.FAE1ID = _employeeService.GetEmployeeID(DwinViewModel.FAE1_EmpName);
                                }
                            }
                            if (cellValue.Key == "FAE1%" && cellValue.Value != null)
                            {
                                DwinViewModel.FAE1_Bonus = cellValue.Value;
                            }
                            if (cellValue.Key == "FAE2" && cellValue.Value != null)
                            {
                                DwinViewModel.FAE2_EmpName = cellValue.Value.ToString();

                                if (!string.IsNullOrEmpty(DwinViewModel.FAE2_EmpName))
                                {
                                    DwinViewModel.FAE2ID = _employeeService.GetEmployeeID(DwinViewModel.FAE2_EmpName);
                                }
                            }
                            if (cellValue.Key == "FAE2%" && cellValue.Value != null)
                            {
                                DwinViewModel.FAE2_Bonus = cellValue.Value;
                            }
                            if (cellValue.Key == "FAE3" && cellValue.Value != null)
                            {
                                DwinViewModel.FAE3_EmpName = cellValue.Value.ToString();

                                if (!string.IsNullOrEmpty(DwinViewModel.FAE3_EmpName))
                                {
                                    DwinViewModel.FAE3ID = _employeeService.GetEmployeeID(DwinViewModel.FAE3_EmpName);
                                }
                            }
                            if (cellValue.Key == "FAE3%" && cellValue.Value != null)
                            {
                                DwinViewModel.FAE3_Bonus = cellValue.Value;
                            }
                        }
                        list.Add(DwinViewModel);
                    }
                    InsertDins(list);
                }
            }

            return list;
        }

        private string CheckDinInsert(DwinViewModel dwinViewModel)
        {
            string Message = string.Empty;

            if (string.IsNullOrEmpty(dwinViewModel.Cus_DB) || string.IsNullOrEmpty(dwinViewModel.CusID))
            {
                Message += "找無相符合客戶資料;\n";
            }

            if (string.IsNullOrEmpty(dwinViewModel.ProApp))
            {
                Message += "產品應用無資料;\n";
            }

            if (!string.IsNullOrEmpty(dwinViewModel.ProApp) && dwinViewModel.ProApp.Length > 20)
            {
                Message += "產品應用文字長度大於20個字;\n";
            }

            if (string.IsNullOrEmpty(dwinViewModel.PartNo))
            {
                Message += "產品編號無資料;\n";
            }

            if (!string.IsNullOrEmpty(dwinViewModel.PartNo) && dwinViewModel.PartNo.Length > 25)
            {
                Message += "產品編號長度大於25個字;\n";
            }

            if (string.IsNullOrEmpty(dwinViewModel.VendorID))
            {
                Message += "供應商無資料;\n";
            }

            if (!string.IsNullOrEmpty(dwinViewModel.vmCreateDate) && dwinViewModel.vmCreateDate.Length > 10)
            {
                Message += "申請時間無資料或格式錯誤;\n";
            }

            if(!string.IsNullOrEmpty(dwinViewModel.EProduceYS) && dwinViewModel.EProduceYS.Length > 6)
            {
                Message += "預估量產時間文字長度超過6個字;\n";
            }

            if (dwinViewModel.PMID == 0)
            {
                Message += "PM資料未建立;\n";
            }

            if (dwinViewModel.SalesID == 0)
            {
                Message += "Sales資料未建立;\n";
            }

            //檢查是否有Do資料----20240424待定義


            //檢查是否有重複Din資料
            //if (!string.IsNullOrEmpty(doViewModel.Cus_DB) && !string.IsNullOrEmpty(doViewModel.CusID))
            //{
            //    var ProjectID = _projectMContext.ProjectM.Where(e => e.Cus_DB == doViewModel.Cus_DB && e.CusID == doViewModel.CusID && e.ProApp == doViewModel.ProApp).Select(e => e.ProjectID).FirstOrDefault();
            //
            //    if (ProjectID != null)
            //    {
            //        var ProjectD = _projectDContext.ProjectD.Where(e => e.ProjectID == ProjectID && e.VendorID == doViewModel.VendorID && e.PartNo == doViewModel.PartNo).ToList();
            //
            //        if (ProjectD.Count > 0)
            //        {
            //            Message += "Do資料重複;\n";
            //        }
            //    }
            //}
            return Message;
        }

        private List<DwinViewModel> InsertDins(List<DwinViewModel> list_dwinViewModels)
        {
            if (list_dwinViewModels.Count > 0)
            {
                foreach (var item in list_dwinViewModels)
                {
                    item.UploadStatus = CheckDinInsert(item);

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
                            Status = "DWIN", // Status: DIN
                            CreateDate = createDate,
                            Cus_DB = item.Cus_DB,
                            CusID = item.CusID,
                            EndCus = item.EndCus,
                            ProApp = item.ProApp,
                            ProModel = item.ProModel,
                            EProduceYS = item.EProduceYS

                        };
                        _projectMContext.Add(modelprojectM);
                        _projectMContext.SaveChanges();

                        // Insert a record into ProjectD
                        var modelprojectD = new ProjectD
                        {
                            ProjectID = projectID,
                            VendorID = item.VendorID,
                            PartNo = item.PartNo,
                            Stage = "DWIN",
                            ELTR = item.ELTR,
                            EGP = item.EGP,
                            EFirstYQty = item.EFirstYQty,
                            ESecondYQty = item.ESecondYQty,
                            EThirdYQty = item.EThirdYQty,
                            UFirstYPrice = item.UFirstYPrice,
                            USecondYPrice = item.USecondYPrice,
                            UThirdYPrice = item.UThirdYPrice
                        };
                        _projectDContext.Add(modelprojectD);
                        _projectDContext.SaveChanges();

                        // Insert a record into Project_DIDW
                        var modelprojectDIDW = new Project_DIDW
                        {
                            ProjectID = projectID,
                            DwinDate = createDate,
                            DwinStatus = "N"
                        };
                        _projectDIDWContext.Add(modelprojectDIDW);
                        _projectDIDWContext.SaveChanges();

                        //檢查Project_Emp是否有資料
                        var chk_model = _projectEmpContext.Project_Emp.Where(e => e.ProjectID == projectID).FirstOrDefault();

                        if(chk_model == null)
                        {
                            //檢查PM和Sales是否為同一人，if true PM獎金比例 = 0%
                            if (item.PMID != 0 && item.SalesID != 0 && item.PMID == item.SalesID)
                            {
                                item.PM_Bonus = 0;
                            }

                            //Insert a record into Project_Emp(PM)
                            if (item.PMID != 0)
                            {
                                var modelprojectEmp_PM = new Project_Emp
                                {
                                    SEQ = _projectEmpService.NewSEQ,
                                    ProjectID = projectID,
                                    EmpID = item.PMID,
                                    BonusP = item.PM_Bonus,
                                    Duty = "PM"
                                };
                                _projectEmpContext.Add(modelprojectEmp_PM);
                                _projectEmpContext.SaveChanges();
                            }

                            //Insert a record into Project_Emp(Sales)
                            if (item.SalesID != 0)
                            {
                                var modelprojectEmp_Sales = new Project_Emp
                                {
                                    SEQ = _projectEmpService.NewSEQ,
                                    ProjectID = projectID,
                                    EmpID = item.SalesID,
                                    BonusP = item.Sales_Bonus,
                                    Duty = "Sales"
                                };
                                _projectEmpContext.Add(modelprojectEmp_Sales);
                                _projectEmpContext.SaveChanges();
                            }

                            //Insert a record into Project_Emp(FAE1)
                            if (item.FAE1ID != 0)
                            {
                                var modelprojectEmp_FAE1 = new Project_Emp
                                {
                                    SEQ = _projectEmpService.NewSEQ,
                                    ProjectID = projectID,
                                    EmpID = item.FAE1ID,
                                    BonusP = item.FAE1_Bonus,
                                    Duty = "FAE1"
                                };
                                _projectEmpContext.Add(modelprojectEmp_FAE1);
                                _projectEmpContext.SaveChanges();
                            }

                            //Insert a record into Project_Emp(FAE2)
                            if (item.FAE2ID != 0)
                            {
                                var modelprojectEmp_FAE2 = new Project_Emp
                                {
                                    SEQ = _projectEmpService.NewSEQ,
                                    ProjectID = projectID,
                                    EmpID = item.FAE2ID,
                                    BonusP = item.FAE2_Bonus,
                                    Duty = "FAE2"
                                };
                                _projectEmpContext.Add(modelprojectEmp_FAE2);
                                _projectEmpContext.SaveChanges();
                            }

                            //Insert a record into Project_Emp(FAE3)
                            if (item.FAE3ID != 0)
                            {
                                var modelprojectEmp_FAE3 = new Project_Emp
                                {
                                    SEQ = _projectEmpService.NewSEQ,
                                    ProjectID = projectID,
                                    EmpID = item.FAE3ID,
                                    BonusP = item.FAE3_Bonus,
                                    Duty = "FAE3"
                                };
                                _projectEmpContext.Add(modelprojectEmp_FAE3);
                                _projectEmpContext.SaveChanges();
                            }
                        }

                        item.UploadStatus = "Success";
                    }
                }
            }
            return list_dwinViewModels;
        }

        public string RejectDwin(string ProjectID)
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
                    if (ProjectDin.DwinStatus == "R")
                    {
                        msg = "專案" + ProjectID + "已經Reject。不可重複Reject";
                    }
                    else
                    {
                        {
                            ProjectDin.DwinStatus = "R";  // Dwin狀態改為審核拒絕
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

        public string ConfirmDwin(string ProjectID)
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
                    if (ProjectDin.DwinStatus == "C")
                    {
                        msg = "專案" + ProjectID + "已經Confirm。不可重複Confirm";
                    }
                    else
                    {
                        {
                            ProjectDin.DwinStatus = "C";  // Din狀態改為審核同意
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