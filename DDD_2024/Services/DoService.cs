using DDD_2024.Data;
using DDD_2024.Interfaces;
using DDD_2024.Models;
using Elfie.Serialization;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Build.Evaluation;
using Microsoft.CodeAnalysis;
using Microsoft.DotNet.Scaffolding.Shared.Project;
using Microsoft.DotNet.Scaffolding.Shared.ProjectModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualBasic;
using System.Linq;

namespace DDD_2024.Services
{
    public class DoService: IDoService
    {
        private readonly ProjectDOContext _projectDOContext;
        private readonly ProjectMContext _projectMContext;
        private readonly ProjectDContext _projectDContext;
        private readonly Project_EmpContext _projectEmpContext;
        private readonly Project_DOASUpdateContext _project_DOASUpdateContext;
        private readonly ASCENDContext _AscendContext;
        private readonly ATIContext _ATIContext;
        private readonly KIR1NContext _KIR1NContext;
        private readonly INTERTEKContext _IntertekContext;
        private readonly TESTBContext _TestbContext;
        private readonly BizAutoContext _BizAutoContext;
        private readonly IEmployeeService _employeeService;
        private readonly ICusVendorService _cusVendoeService;

        public DoService(ProjectMContext projectMContext, ProjectDContext projectDContext, ProjectDOContext projectDOContext, 
            Project_EmpContext project_EmpContext, Project_DOASUpdateContext project_DOASUpdateContext,  ASCENDContext aSCENDContext, 
            ATIContext aTIContext, KIR1NContext kIR1NContext,INTERTEKContext iNTERTEKContext, TESTBContext tESTBContext, BizAutoContext bizAutoContext,
            IEmployeeService employeeService, ICusVendorService cusVendoeService)
        {
            _projectMContext = projectMContext;
            _projectDContext = projectDContext;
            _projectDOContext = projectDOContext;
            _project_DOASUpdateContext = project_DOASUpdateContext;
            _projectEmpContext = project_EmpContext;
            _AscendContext = aSCENDContext;
            _ATIContext = aTIContext;
            _KIR1NContext = kIR1NContext;
            _IntertekContext = iNTERTEKContext;
            _TestbContext = tESTBContext;
            _BizAutoContext = bizAutoContext;
            _employeeService = employeeService;
            _cusVendoeService = cusVendoeService;
        }

        public async Task<List<DoViewModel>> GetDOsAsync()
        {          
            var modelDo = await _projectDOContext.Project_DO.Where(e => e.Status == "N").ToListAsync();

            var list_doViewModel = new List<DoViewModel>();

            foreach (var item in modelDo)
            {
                var model = new DoViewModel
                {
                    DoID = item.DoID,
                    ProjectID = item.ProjectID,
                    Applicant = _employeeService.GetEmployeeName(item.ApplicantID),
                    StatusName = GetStatusName(item.Status),
                    TradeStatus = item.TradeStatus,
                    DOStatus = item.Status
                };

                if (!string.IsNullOrEmpty(item.CreateDate))
                {
                    model.vmCreateDate = item.CreateDate.Substring(0, 4) + "/" + item.CreateDate.Substring(4, 2) + "/" + item.CreateDate.Substring(6, 2);
                }

                // 加入ProjectM的資料
                var modelProjectM = await _projectMContext.ProjectM.Where(e => e.ProjectID == item.ProjectID).FirstOrDefaultAsync();

                if (modelProjectM != null)
                {
                    if (!string.IsNullOrEmpty(modelProjectM.Cus_DB) && !string.IsNullOrEmpty(modelProjectM.CusID))
                    {
                        // 將 matchingProjectM 的資料設定給 Cus_DB 屬性
                        model.CusName = _cusVendoeService.GetvendorName(modelProjectM.Cus_DB, modelProjectM.CusID);
                    }
                }
                else
                {
                    continue;
                }

                // 加入ProjectD的資料
                var modelProjectD = await _projectDContext.ProjectD.Where(e => e.ProjectID == item.ProjectID).FirstOrDefaultAsync();

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

                list_doViewModel.Add(model);
            }

            return list_doViewModel;
        }
        public async Task<DoViewModel> GetDoAsync(string ProjectID)
        {
            var modelProjectM = await _projectMContext.ProjectM.Where(p => p.ProjectID == ProjectID).FirstOrDefaultAsync();
            var modelProjectD = await _projectDContext.ProjectD.Where(p => p.ProjectID == ProjectID && (p.Stage == "DO" || p.Stage == "DIN")).FirstOrDefaultAsync();
            var modelDo = await _projectDOContext.Project_DO.Where(p => p.ProjectID == ProjectID).FirstOrDefaultAsync();

            var model = new DoViewModel();

            model.ProjectID = ProjectID;

            if (modelDo != null)
            {
                model.DoID = modelDo.DoID;
                model.DOStatus = modelDo.Status;
                model.TradeStatus = modelDo.TradeStatus;
                model.StatusName = GetStatusName(modelDo.Status);
                model.ApplicantID = modelDo.ApplicantID;
                model.ApproverID = modelDo.ApproverID;

                if (!string.IsNullOrEmpty(modelDo.CreateDate) && modelDo.CreateDate.Length == 8)
                {
                    model.vmCreateDate = modelDo.CreateDate.Substring(0, 4) + "/" + modelDo.CreateDate.Substring(4, 2) + "/" + modelDo.CreateDate.Substring(6, 2);
                    model.CreateDate = DateTime.Parse(model.vmCreateDate);
                }

                model.Applicant = _employeeService.GetEmployeeName(modelDo.ApplicantID);
                model.Approver = _employeeService.GetEmployeeName(modelDo.ApproverID);

                var modelDoAS = await _project_DOASUpdateContext.Project_DOASUpdate
                    .Where(p => p.DoID == modelDo.DoID)
                    .OrderByDescending(p => p.DoUDate)
                    .FirstOrDefaultAsync();

                if (modelDoAS != null)
                {
                    if (!string.IsNullOrEmpty(modelDoAS.DoUDate))
                    {
                        model.DoUDate = modelDoAS.DoUDate.Substring(0, 4) + "/" + modelDoAS.DoUDate.Substring(4, 2);
                    }

                    model.DoUAction = modelDoAS.DoUAction;
                    model.DoUStatus = modelDoAS.DoUStatus;
                }
            }

            if (modelProjectM != null && modelProjectD != null)
            {
                model.ProjectID = modelProjectM.ProjectID;

                if (!string.IsNullOrEmpty(modelProjectM.Cus_DB) && !string.IsNullOrEmpty(modelProjectM.CusID))
                {
                    model.CusName = _cusVendoeService.GetvendorName(modelProjectM.Cus_DB, modelProjectM.CusID);
                }

                if (!string.IsNullOrEmpty(modelProjectD.VendorID))
                {
                    model.VendorName = _cusVendoeService.GetVendorName(modelProjectD.VendorID);
                }

                model.PartNo = modelProjectD.PartNo;
                model.ProApp = modelProjectM.ProApp;
            }

            return model;
        }
        public async Task<List<DoViewModel>> GetDOsFilterAsync(string projectStatus, string applicant, List<string> list_months)
        {
            List<Project_DO> list_ProjectDo = await _projectDOContext.Project_DO.ToListAsync();

            if (!string.IsNullOrEmpty(projectStatus))
            {
                list_ProjectDo = list_ProjectDo.Where(e => e.Status == projectStatus).ToList();
            }
            
            if(!string.IsNullOrEmpty(applicant) && applicant != "999")
            {
                list_ProjectDo = list_ProjectDo.Where(e => e.ApplicantID == Convert.ToInt32(applicant)).ToList();
            }

            if(list_months.Count > 0)
            {
                //如果list_months有多個值的話，先用List來儲存篩選結果，再把結果覆蓋list_ProjectDo
                List<Project_DO> list_DoFilter = new List<Project_DO>();

                foreach (var item in list_months)
                {
                    var FilterList = list_ProjectDo.Where(p => !string.IsNullOrEmpty(p.CreateDate) && p.CreateDate.Contains(item)).ToList();
                    list_DoFilter.AddRange(FilterList);
                }

                list_ProjectDo = list_DoFilter;
            }
            
            var list_doViewModel = new List<DoViewModel>();

            foreach (var item in list_ProjectDo)
            {
                var model = new DoViewModel
                {
                    DoID = item.DoID,
                    ProjectID = item.ProjectID,
                    Applicant = _employeeService.GetEmployeeName(item.ApplicantID),
                    StatusName = GetStatusName(item.Status),
                    TradeStatus = item.TradeStatus,
                    DOStatus = item.Status
                };

                if (!string.IsNullOrEmpty(item.CreateDate))
                {
                    model.vmCreateDate = item.CreateDate.Substring(0, 4) + "/" + item.CreateDate.Substring(4, 2) + "/" + item.CreateDate.Substring(6, 2);
                }

                // 加入ProjectM的資料
                var modelProjectM = await _projectMContext.ProjectM.Where(e => e.ProjectID == item.ProjectID).FirstOrDefaultAsync();

                if (modelProjectM != null)
                {
                    if (!string.IsNullOrEmpty(modelProjectM.Cus_DB) && !string.IsNullOrEmpty(modelProjectM.CusID))
                    {
                        // 將 matchingProjectM 的資料設定給 Cus_DB 屬性
                        model.CusName = _cusVendoeService.GetvendorName(modelProjectM.Cus_DB, modelProjectM.CusID);
                    }
                }
                else
                {
                    continue;
                }

                // 加入ProjectD的資料
                var modelProjectD = await _projectDContext.ProjectD.Where(e => e.ProjectID == item.ProjectID).FirstOrDefaultAsync();

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

                list_doViewModel.Add(model);
            }

            return list_doViewModel;
        }

        public async Task<DoEditViewModel> GetEditDo(string ProjectID)
        {
            var modelProjectM = await _projectMContext.ProjectM.Where(p => p.ProjectID == ProjectID).FirstOrDefaultAsync();
            var modelProjectD = await _projectDContext.ProjectD.Where(p => p.ProjectID == ProjectID && (p.Stage == "DO" || p.Stage == "DIN")).FirstOrDefaultAsync();
            var modelDo = await _projectDOContext.Project_DO.Where(p => p.ProjectID == ProjectID).FirstOrDefaultAsync();

            var model = new DoEditViewModel(); 

            model.ProjectID = ProjectID;

            if (modelDo != null)
            {
                model.DoID = modelDo.DoID;
                model.TradeStatus = modelDo.TradeStatus;
                model.ApplicantID = modelDo.ApplicantID;
                model.ApproverID = modelDo.ApproverID;

                if (!string.IsNullOrEmpty(modelDo.CreateDate) && modelDo.CreateDate.Length == 8)
                {
                    model.CreateDate = DateTime.Parse(modelDo.CreateDate.Substring(0, 4) + "/" + modelDo.CreateDate.Substring(4, 2) + "/" + modelDo.CreateDate.Substring(6, 2));
                }

                var modelDoAS = await _project_DOASUpdateContext.Project_DOASUpdate
                    .Where(p => p.DoID == modelDo.DoID)
                    .OrderByDescending(p => p.DoUDate)
                    .FirstOrDefaultAsync();

                if (modelDoAS != null)
                {
                    if (!string.IsNullOrEmpty(modelDoAS.DoUDate))
                    {
                        model.DoUDate = modelDoAS.DoUDate.Substring(0, 4) + "/" + modelDoAS.DoUDate.Substring(4, 2);
                    }
                    model.DoUAction = modelDoAS.DoUAction;
                    model.DoUStatus = modelDoAS.DoUStatus;
                }
            }

            if (modelProjectM != null && modelProjectD != null)
            {
                model.ProjectID = modelProjectM.ProjectID;

                if (!string.IsNullOrEmpty(modelProjectM.Cus_DB) && !string.IsNullOrEmpty(modelProjectM.CusID))
                {
                    model.CusName = _cusVendoeService.GetvendorName(modelProjectM.Cus_DB, modelProjectM.CusID);
                }

                if (!string.IsNullOrEmpty(modelProjectD.VendorID))
                {
                    model.VendorName = _cusVendoeService.GetVendorName(modelProjectD.VendorID);
                }

                model.PartNo = modelProjectD.PartNo;
                model.ProApp = modelProjectM.ProApp;
            }
            return model;
        }
        public async Task EditDo(DoEditViewModel model)
        {
            if (model != null)
            {
                //修改Project_DO的資料
                var modolDo = await _projectDOContext.Project_DO.AsNoTracking().FirstOrDefaultAsync(p => p.DoID == model.DoID);
                if (modolDo != null)
                {
                    modolDo.CreateDate = model.CreateDate.ToString("yyyyMMdd");
                    modolDo.ApplicantID = model.ApplicantID;
                    modolDo.ApproverID = model.ApproverID;
                    modolDo.TradeStatus = model.TradeStatus;

                    _projectDOContext.Update(modolDo);
                    await _projectDOContext.SaveChangesAsync();
                }

                //修改model.DoUDate格式
                if (!string.IsNullOrEmpty(model.DoUDate))
                {
                    model.DoUDate = model.DoUDate.Replace("/", string.Empty);
                }

                //修改Project_DOASUpdate的資料
                var modelDoAS = await _project_DOASUpdateContext.Project_DOASUpdate.FirstOrDefaultAsync(p => p.DoID == model.DoID && p.DoUDate == model.DoUDate);
                if(modelDoAS != null)
                {
                    modelDoAS.DoUAction = model.DoUAction;
                    modelDoAS.DoUStatus = model.DoUStatus;

                    _project_DOASUpdateContext.Update(modelDoAS);
                    await _project_DOASUpdateContext.SaveChangesAsync();
                }
            }
        }

        //匯出Do Excel報表
        public async Task<List<DoReport_ViewModel>> GetDosReport(DoReportFilterViewModel filter)
        {
            var modelDo = await _projectDOContext.Project_DO.OrderBy(e => e.ProjectID).ToListAsync();

            //篩選條件---Start
            if (!string.IsNullOrEmpty(filter.IsFinsih))
            {
                //取未結案資料 || 取已結案資料
                if (filter.IsFinsih == "N" || filter.IsFinsih == "Y")
                {
                    modelDo = modelDo.Where(e => e.IsFinish == filter.IsFinsih).ToList();
                }
            }

            if (!string.IsNullOrEmpty(filter.Region) && filter.Region == "C")
            {
                var employees = _BizAutoContext.employeeM.Where(e => e.Region == "CN").Select(e => e.EmpID).ToList();
                if (employees.Any())
                {
                    modelDo = modelDo.Where(m => employees.Contains(m.ApplicantID)).ToList();
                }
            }
            else if (!string.IsNullOrEmpty(filter.Region) && filter.Region == "T")
            {
                var employees = _BizAutoContext.employeeM.Where(e => e.Region == "TW").Select(e => e.EmpID).ToList();
                if (employees.Any())
                {
                    modelDo = modelDo.Where(m => employees.Contains(m.ApplicantID)).ToList();
                }
            }

            if (filter.applicantID != 999)
            {
                modelDo = modelDo.Where(e => e.ApplicantID == filter.applicantID).ToList();
            }
            //篩選條件---End

            var list_dosReport = new List<DoReport_ViewModel>();
            int seq = 1;

            //加入DoStatus的日期在list_dosReport的第一列(為了顯示更新日期)
            if (modelDo.Any())
            {
                var dateModel = _project_DOASUpdateContext.Project_DOASUpdate.Where(e => e.DoID == modelDo[0].DoID).OrderByDescending(e => e.DoUDate).ToList();

                if (dateModel.Any())
                {
                    var newRow = new DoReport_ViewModel();

                    for(int i = 0; i < Math.Min(dateModel.Count,6); i++) 
                    {
                        var statusProperty = typeof(DoReport_ViewModel).GetProperty($"DoStatus{i + 1}"); // 確保替換 ModelType 為 model 的實際類型
                        if (statusProperty != null)
                        {
                            string dateformat = dateModel[i].DoUDate?.Substring(0, 4) + "/" + dateModel[i].DoUDate?.Substring(4, 2) ?? string.Empty;

                            statusProperty.SetValue(newRow, dateformat);
                        }
                    }
                    list_dosReport.Insert(0, newRow);
                }
            }

            //加入其餘Do資料
            foreach (var item in modelDo)
            {
                var model = new DoReport_ViewModel
                {
                    SEQ = seq,
                    ProjectID = item.ProjectID,
                    Applicant = _employeeService.GetEmployeeName(item.ApplicantID),
                    Approver = _employeeService.GetEmployeeName(item.ApproverID),
                    ApplicationDate = item.CreateDate?.Substring(0, 4) + "/" + item.CreateDate?.Substring(4, 2) + "/" + item.CreateDate?.Substring(6, 2) ?? string.Empty,
                    NewActive = item.TradeStatus != null ? GetTradingStatusName(item.TradeStatus) : string.Empty
                };

                // 加入ProjectM的資料
                var modelProjectM = await _projectMContext.ProjectM.Where(e => e.ProjectID == item.ProjectID).FirstOrDefaultAsync();

                if (modelProjectM != null)
                {
                    DateTime dtCreateDate;

                    if(modelProjectM.CreateDate != null )
                    {
                        if (!string.IsNullOrEmpty(item.CreateDate))
                        {
                            dtCreateDate = DateTime.Parse(modelProjectM.CreateDate.Substring(0, 4) + "/" + modelProjectM.CreateDate.Substring(4, 2) + "/" + modelProjectM.CreateDate.Substring(6, 2));

                            if ((filter.StartDate.HasValue && dtCreateDate < filter.StartDate.Value) || (filter.EndDate.HasValue && dtCreateDate > filter.EndDate.Value))
                            {
                                continue;
                            }
                        }
                    }
                   
                    if (!string.IsNullOrEmpty(modelProjectM.Cus_DB) && !string.IsNullOrEmpty(modelProjectM.CusID))
                    {
                        // 將 matchingProjectM 的資料設定給 Cus_DB 屬性
                        model.CusName = _cusVendoeService.GetvendorName(modelProjectM.Cus_DB, modelProjectM.CusID);
                    }

                    model.ProApp = modelProjectM.ProApp;
                }
                else
                {
                    continue;
                }

                // 加入ProjectD的資料
                var modelProjectD = await _projectDContext.ProjectD.Where(e => e.ProjectID == item.ProjectID).FirstOrDefaultAsync();

                if (modelProjectD != null)
                {
                    model.VendorName = modelProjectD.VendorID != null ? _cusVendoeService.GetVendorName(modelProjectD.VendorID) : string.Empty;
                    model.PartNo = modelProjectD.PartNo;
                }
                else
                {
                    continue;
                }

                //加入Project_DOASUpdate的資料
                if (!string.IsNullOrEmpty(item.DoID))
                {
                    var modelDOAS = _project_DOASUpdateContext.Project_DOASUpdate.
                                    Where(e => e.DoID == item.DoID).ToList() ?? null;

                    if (modelDOAS != null)
                    {
                        var OrderDOAS = modelDOAS.OrderByDescending(p => p.DoUDate).ToList();

                        if (OrderDOAS.Any())
                        {
                            //加入Action資料
                            foreach(var aItem in OrderDOAS)
                            {
                                if (!string.IsNullOrEmpty(aItem.DoUAction) && string.IsNullOrEmpty(model.DoUAction))
                                {
                                    model.DoUAction = aItem.DoUAction;
                                }
                            }

                            //加入Status資料
                            for (int i = 0; i < Math.Min(OrderDOAS.Count, 6); i++) // 確保不超過 6 個
                            {
                                if (OrderDOAS.Any())
                                {                                                                     
                                    var statusProperty = typeof(DoReport_ViewModel).GetProperty($"DoStatus{i + 1}"); // 確保替換 ModelType 為 model 的實際類型
                                    if (statusProperty != null)
                                    {
                                        statusProperty.SetValue(model, OrderDOAS[i].DoUStatus);
                                    }
                                }
                            }
                        }
                    }
                }
                list_dosReport.Add(model);
                seq++;
            }
            return list_dosReport;
        }

        public async Task<List<DOASUViewModel>> GetDOASUsAsync(string DoID)
        {
            var modelASU = await _project_DOASUpdateContext.Project_DOASUpdate.Where(e => e.DoID == DoID).OrderByDescending(e => e.DoUDate).ToListAsync();

            var listViewModel = new List<DOASUViewModel>();

            foreach (var item in modelASU)
            {
                var model = new DOASUViewModel
                {
                    DoID = item.DoID,
                    DoUAction = item.DoUAction,
                    DoUStatus = item.DoUStatus
                };

                if (!string.IsNullOrEmpty(item.DoUDate))
                {
                    model.DoUDate = item.DoUDate.Substring(0, 4) + "/" + item.DoUDate.Substring(4, 2);
                }

                listViewModel.Add(model);
            }

            return listViewModel;
        }

        public async Task CreateDO(DoCreateViewModel model)
        {
            if(model != null)
            {
                string projectID = GetProjectID(DateTime.Now.ToString("yyyyMMdd"));
                string createDate = model.CreateDate.ToString("yyyyMMdd");
                string DoID = GetDOID(DateTime.Now.ToString("yyyyMMdd"));

                //加一筆紀錄到ProjectM
                var modelprojectM = new ProjectM
                {
                    ProjectID = projectID,
                    Status = "DO", // Status: DO
                    CreateDate = createDate,
                    CusID = model.CusID,
                    ProApp = model.ProApp
                };
                if (!string.IsNullOrEmpty(model.CusID))
                {
                    modelprojectM.Cus_DB = await _cusVendoeService.GetCusDBName(model.CusID);
                }
                _projectMContext.Add(modelprojectM);
                await _projectMContext.SaveChangesAsync();

                //加一筆紀錄到ProjectD
                var modelprojectD = new ProjectD
                {
                    ProjectID = projectID,
                    VendorID = model.VendorID,
                    PartNo = model.PartNo,
                    Stage = "DO"
                };
                _projectDContext.Add(modelprojectD);
                await _projectDContext.SaveChangesAsync();

                //加一筆紀錄到Project_DO
                var modelprojectDO = new Project_DO
                {
                    DoID = DoID,
                    ProjectID = projectID,
                    CreateDate = createDate,
                    ApplicantID = model.ApplicantID,
                    ApproverID = model.ApproverID,
                    Status = "N", // Status: 新單
                    TradeStatus = model.TradeStatus
                };
                _projectDOContext.Add(modelprojectDO);
                await _projectDOContext.SaveChangesAsync();

                if(!string.IsNullOrEmpty(model.DoUAction) && !string.IsNullOrEmpty(model.DoUStatus))
                {
                    //加一筆紀錄到Project_DOASUpdate
                    var modelProjecyDOAS = new Project_DOASUpdate
                    {
                        SEQ = NewProjectDOASSEQ,
                        DoID = DoID,
                        DoUDate = model.CreateDate.ToString("yyyyMM"),
                        DoUAction = model.DoUAction,
                        DoUStatus = model.DoUStatus
                    };
                    _project_DOASUpdateContext.Add(modelProjecyDOAS);
                    await _project_DOASUpdateContext.SaveChangesAsync();
                }
            }
        }
        public async Task CreateDoAS(DOASUViewModel model)
        {
            if(model != null)
            {
                //檢查當月是否已經有資料
                var model_Chk = _project_DOASUpdateContext.Project_DOASUpdate.Where(e => e.DoID == model.DoID && e.DoUDate == model.vmDoUDate.ToString("yyyyMM")).FirstOrDefaultAsync();
                //如果沒有才能新增資料
                if (model_Chk.Result == null)
                {
                    var modelDOAS = new Project_DOASUpdate()
                    {
                        SEQ = NewProjectDOASSEQ,
                        DoID = model.DoID,
                        DoUDate = model.vmDoUDate.ToString("yyyyMM"),
                        DoUAction = model.DoUAction,
                        DoUStatus = model.DoUStatus
                    };

                    _project_DOASUpdateContext.Add(modelDOAS);
                    await _project_DOASUpdateContext.SaveChangesAsync();
                }                
            }
        }

        public string ConfirmDo(string DoId)
        {
            string msg = string.Empty;

            if (string.IsNullOrEmpty(DoId))
            {
                return "無對應DoId";
            }
            else
            {
                var ProjectDO = _projectDOContext.Project_DO.Where(e => e.DoID == DoId).ToList().FirstOrDefault();

                if (ProjectDO != null)
                {
                    ProjectDO.Status = "C";  // Do狀態改為審核通過
                    _projectDOContext.Update(ProjectDO);
                    _projectDOContext.SaveChanges();

                    var ProjectM = _projectMContext.ProjectM.Where(e => e.ProjectID == ProjectDO.ProjectID).ToList().FirstOrDefault();

                    if (ProjectM != null)
                    {
                        ProjectM.UpdateDate = DateTime.Now.ToString("yyyyMMdd");
                        _projectMContext.Update(ProjectM);
                        _projectMContext.SaveChanges();
                    }

                    return msg;
                }
                else
                {
                    return "無對應DoId";
                }
            }
        }
        public string ConfiirmDos(string[] DoIds)
        {
            string msg = string.Empty;

            if (DoIds.Length == 0)
            {
                return "DoIds為空值";
            }
            else
            {
                foreach (var doId in DoIds)
                {
                    if(string.IsNullOrEmpty(doId))
                    {
                        continue;
                    }
                    
                    var ProjectDO = _projectDOContext.Project_DO.Where(e => e.DoID == doId).ToList().FirstOrDefault();

                    if (ProjectDO != null)
                    {
                        ProjectDO.Status = "C";  // 狀態改為審核通過
                        _projectDOContext.Update(ProjectDO);
                        _projectDOContext.SaveChanges();

                        var ProjectM = _projectMContext.ProjectM.Where(e => e.ProjectID == ProjectDO.ProjectID).ToList().FirstOrDefault();

                        if (ProjectM != null)
                        {
                            ProjectM.UpdateDate = DateTime.Now.ToString("yyyyMMdd");
                            _projectMContext.Update(ProjectM);
                            _projectMContext.SaveChanges();
                        }
                    }
                    else
                    {
                        msg += "無對應DoId:" + doId + "\n";
                    }
                }

                return msg;
            }           
        }
        public string RejectDO(string DoId)
        {
            string msg = string.Empty;

            if (string.IsNullOrEmpty(DoId))
            {
                msg = "DoId為空值";
            }
            else
            {
                var ProjectDO = _projectDOContext.Project_DO.Where(e => e.DoID == DoId).ToList().FirstOrDefault();

                if (ProjectDO != null)
                {
                    if(ProjectDO.Status == "R")
                    {
                        msg = "Do編號" + DoId + "已經Reject。不可重複Reject";
                    }
                    else
                    {
                        ProjectDO.Status = "R";  // Do狀態改為審核拒絕
                        _projectDOContext.Update(ProjectDO);
                        _projectDOContext.SaveChanges();

                        var ProjectM = _projectMContext.ProjectM.Where(e => e.ProjectID == ProjectDO.ProjectID).ToList().FirstOrDefault();

                        if (ProjectM != null)
                        {
                            ProjectM.Status = "F"; //專案狀態改為結案
                            ProjectM.UpdateDate = DateTime.Now.ToString("yyyyMMdd");
                            _projectMContext.Update(ProjectM);
                            _projectMContext.SaveChanges();
                        }

                        msg = "Do編號" + DoId + "Reject完成";
                    }
                }
                else
                {
                    msg = "無對應DoId";
                }
            }

            return msg;
        }
        public string RejectDos(string[] DoIds)
        {
            string msg = string.Empty;

            if (DoIds.Length == 0)
            {
                return "DoIds為空值";
            }
            else
            {
                foreach (var doId in DoIds)
                {
                    if (string.IsNullOrEmpty(doId))
                    {
                        continue;
                    }

                    var ProjectDO = _projectDOContext.Project_DO.Where(e => e.DoID == doId).ToList().FirstOrDefault();

                    if (ProjectDO != null)
                    {
                        ProjectDO.Status = "R";  // 狀態改為審核拒絕
                        _projectDOContext.Update(ProjectDO);
                        _projectDOContext.SaveChanges();

                        var ProjectM = _projectMContext.ProjectM.Where(e => e.ProjectID == ProjectDO.ProjectID).ToList().FirstOrDefault();

                        if (ProjectM != null)
                        {
                            ProjectM.Status = "F"; //專案狀態改為結案
                            ProjectM.UpdateDate = DateTime.Now.ToString("yyyyMMdd");
                            _projectMContext.Update(ProjectM);
                            _projectMContext.SaveChanges();
                        }
                    }
                    else
                    {
                        msg += "無對應DoId:" + doId + "\n";
                    }
                }

                return msg;
            }
        }

        public string GetStatusName(string? doStatus)
        {
            if (string.IsNullOrEmpty(doStatus))
            {
                return string.Empty;
            }
            else if (doStatus == "N")
            {
                return "新單";
            }
            else if (doStatus == "A")
            {
                return "已發獎金";
            }
            else if (doStatus == "B")
            {
                return "未發獎金";
            }
            else if (doStatus == "C")
            {
                return "暫時不發獎金";
            }
            else if (doStatus == "D")
            {
                return "作廢";
            }
            else
            {
                return string.Empty;
            }
        }

        public string GetProjectID(string date)
        {
            var projectID = _projectMContext.ProjectM.Where(e => e.ProjectID != null && e.ProjectID.Substring(1,8) == date)
                .Select(e => e.ProjectID).ToList();

            if (projectID.Count == 0)
            {
                return "P" + date + "001";
            }
            else
            {
                var maxProjectID = projectID.Max();
                int maxID;

                if (!string.IsNullOrEmpty(maxProjectID) && int.TryParse(maxProjectID.Substring(9), out maxID))
                {
                    return "P" + date + (maxID + 1).ToString().PadLeft(3, '0');
                }
                else
                {
                    return string.Empty;
                }                
            }
        }

        public string GetDOID(string date)
        {
            var DoID = _projectDOContext.Project_DO.Where(e => e.DoID != null && e.DoID.Substring(2, 8) == date)
                .Select(e => e.DoID).ToList();

            if (DoID.Count == 0)
            {
                return "DO" + date + "001";
            }
            else
            {
                var maxDoID = DoID.Max();
                int maxID;

                if (!string.IsNullOrEmpty(maxDoID) && int.TryParse(maxDoID.Substring(10), out maxID))
                {
                    return "DO" + date + (maxID + 1).ToString().PadLeft(3, '0');
                }
                else
                {
                    return string.Empty;
                }
            }
        }

        public string QueryDoID(string projectID)
        {
            if (!string.IsNullOrEmpty(projectID))
            {
                return _projectDOContext.Project_DO.Where(e => e.ProjectID == projectID && e.DoID != null).Select(e => e.DoID).FirstOrDefault()?? string.Empty;
            }
            else
            {
                return string.Empty;
            }
        }

        public int NewProjectEmpSEQ
        {
            get
            {
                if (_projectEmpContext.Project_Emp.Count() == 0)
                {
                    return 1;
                }
                else
                {
                    return _projectEmpContext.Project_Emp.Max(e => e.SEQ) + 1;
                }
            }
        }

        public int NewProjectDOASSEQ
        {
            get
            {
                if (_project_DOASUpdateContext.Project_DOASUpdate.Count() == 0)
                {
                    return 1;
                }
                else
                {
                    return _project_DOASUpdateContext.Project_DOASUpdate.Max(e => e.SEQ) + 1;
                }
            }
        }

        public bool chk_DoTransDin(string projectID)
        {
            var projectStatus = _projectMContext.ProjectM.FirstOrDefault(e => e.ProjectID == projectID)?.Status;

            if (!string.IsNullOrEmpty(projectStatus) && projectStatus != "DO")
            {
                return true;
            }
            else
            {
                return false;
            }            
        }

        public List<SelectListItem> GetStatus
        {
            get
            {
                List<SelectListItem> DBSource = new List<SelectListItem>
        {
            new SelectListItem {Text = "" , Value = ""},
            new SelectListItem {Text = "新單" , Value = "N"},
            new SelectListItem {Text = "已發獎金" , Value = "A"},
            new SelectListItem {Text = "未發獎金" , Value = "B"},
            new SelectListItem {Text = "暫時不發獎金" , Value = "C"},
            new SelectListItem {Text = "作廢" , Value = "D"}
        };                
                return DBSource;
            }
        }
        public List<SelectListItem> GetTradingStatus(string? TradingStatus)
        {
            var selectListItems = new List<SelectListItem>
            {
                new SelectListItem { Text = "New", Value = "N", Selected = TradingStatus == "N" },
                new SelectListItem { Text = "Active", Value = "A", Selected = TradingStatus == "A" }
            };
            return selectListItems;
        }
        public List<SelectListItem> GetIsFinsih()
        {
            List<SelectListItem> selectListItems = new List<SelectListItem>();

            selectListItems = new List<SelectListItem>
            {
                new SelectListItem{ Text = "未結案", Value ="N"},
                new SelectListItem{ Text = "已結案", Value ="Y"},
                new SelectListItem{ Text = "全部", Value ="A"},
            };
            return selectListItems;
        }
        public List<SelectListItem> GetRegion()
        {
            List<SelectListItem> selectListItems = new List<SelectListItem>();

            selectListItems = new List<SelectListItem>
            {
                new SelectListItem{ Text = "全區域", Value ="A"},
                new SelectListItem{ Text = "台灣區", Value ="T"},
                new SelectListItem{ Text = "大中華區", Value ="C"},
            };
            return selectListItems;
        }

        public string GetTradingStatusName(string TradingStatus) 
        {
            string TradingStatusName = string.Empty;

            if (TradingStatus.Equals("N"))
            {
                TradingStatusName = "New";
            }
            else if (TradingStatus.Equals("A"))
            {
                TradingStatusName = "Active";
            }

            return TradingStatusName;
        }

        //匯入新Do
        public List<DoViewModel> ImportDo(List<DoViewModel> list_doViewModels)
        {
            if (list_doViewModels.Count > 0)
            {
                foreach (var item in list_doViewModels)
                {
                    if (!string.IsNullOrEmpty(item.UploadStatus))
                    {
                        continue;
                    }
                    else
                    {
                        item.UploadStatus = CheckDOImport(item);
                    }

                    if (string.IsNullOrEmpty(item.UploadStatus))
                    {
                        string projectID = GetProjectID(DateTime.Now.ToString("yyyyMMdd"));
                        string createDate = string.Empty;
                        string DoID = GetDOID(DateTime.Now.ToString("yyyyMMdd"));

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
                            DoID = DoID,
                            ProjectID = projectID,
                            CreateDate = createDate,
                            ApplicantID = item.ApplicantID,
                            ApproverID = item.ApproverID,
                            TradeStatus = item.TradeStatus,
                            Status = "N" // Status: 新單
                        };
                        _projectDOContext.Add(modelprojectDO); 
                        _projectDOContext.SaveChanges();

                        // Insert a record into Project_DOASUpdate
                        var modelDOAS = new Project_DOASUpdate
                        {
                            DoID = DoID,
                            DoUDate = createDate.Substring(0,6),
                            DoUAction = item.DoUAction,
                            DoUStatus = item.DoUStatus,
                            SEQ = NewProjectDOASSEQ
                        };
                        _project_DOASUpdateContext.Add(modelDOAS);
                        _project_DOASUpdateContext.SaveChanges();

                        item.ProjectID = projectID;
                        item.UploadStatus = "Success";
                    }
                }
            }
            return list_doViewModels;
        }
        //檢查是否有重複Do，條件:相同客戶+產品應用
        private string CheckDOImport(DoViewModel doViewModel)
        {
            string Message = string.Empty;

            //檢查是否有重複Do資料
            if (!string.IsNullOrEmpty(doViewModel.Cus_DB) && !string.IsNullOrEmpty(doViewModel.CusID))
            {
                var ProjectID = _projectMContext.ProjectM.Where(e => e.Cus_DB == doViewModel.Cus_DB && e.CusID == doViewModel.CusID && e.ProApp == doViewModel.ProApp).Select(e => e.ProjectID).FirstOrDefault();

                if (ProjectID != null)
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
        //Do維護
        public List<DoMaintainModel_Result> DoMaintain(List<DoMaintainModel_F> list_finish, List<DoMaintainModel_U> list_update)
        {
            List<DoMaintainModel_Result> list_result = new List<DoMaintainModel_Result>();
            var projectModels = _projectMContext.ProjectM.ToList();
            var projectDoModels = _projectDOContext.Project_DO.ToList();

            if(projectModels != null && projectDoModels != null)
            {
                if (list_finish.Count > 0)
                {
                    foreach (var item in list_finish)
                    {
                        if (!string.IsNullOrEmpty(item.ProjectID) && !string.IsNullOrEmpty(item.IsFinish))
                        {
                            //更新Table ProjectM、Project_DO、Project_DOASUpdate
                            var MModle = projectModels.Where(e => e.ProjectID == item.ProjectID).FirstOrDefault();
                            var doModel = projectDoModels.Where(e => e.ProjectID == item.ProjectID).FirstOrDefault();
                            string finishResult = string.Empty;

                            //update 是否結案並新增一筆紀錄到Table Project_DOASUpdate
                            if (doModel != null)
                            {
                                if(item.IsFinish == "Y" )
                                {
                                    if (!string.IsNullOrEmpty(doModel.IsFinish) && doModel.IsFinish != "Y")
                                    {
                                        doModel.IsFinish = "Y";
                                        finishResult = "結案-成功";

                                        _projectDOContext.Update(doModel);
                                        _projectDOContext.SaveChanges();

                                        Project_DOASUpdate newModel = new Project_DOASUpdate()
                                        {
                                            SEQ = NewProjectDOASSEQ,
                                            DoID = doModel.DoID,
                                            DoUDate = DateTime.Now.ToString("yyyyMM"),
                                            DoUStatus = item.FinishReason,
                                            DoUAction = string.Empty
                                        };

                                        _project_DOASUpdateContext.Add(newModel);
                                        _project_DOASUpdateContext.SaveChanges();

                                        //update 更新日期
                                        if (MModle != null)
                                        {
                                            MModle.UpdateDate = DateTime.Now.ToString("yyyyMMdd");

                                            _projectMContext.Update(MModle);
                                            _projectMContext.SaveChanges();
                                        }

                                        DoMaintainModel_Result resultModel = new DoMaintainModel_Result()
                                        {
                                            ProjectID = item.ProjectID,
                                            UploadStatus = finishResult
                                        };
                                        list_result.Add(resultModel);
                                    }
                                }
                                else if (item.IsFinish == "R")
                                {
                                    if (!string.IsNullOrEmpty(doModel.IsFinish) && doModel.IsFinish != "R")
                                    {
                                        doModel.IsFinish = "N";
                                        finishResult = "結案復原-成功";

                                        _projectDOContext.Update(doModel);
                                        _projectDOContext.SaveChanges();

                                        //update 更新日期
                                        if (MModle != null)
                                        {
                                            MModle.UpdateDate = DateTime.Now.ToString("yyyyMMdd");

                                            _projectMContext.Update(MModle);
                                            _projectMContext.SaveChanges();
                                        }

                                        DoMaintainModel_Result resultModel = new DoMaintainModel_Result()
                                        {
                                            ProjectID = item.ProjectID,
                                            UploadStatus = finishResult
                                        };
                                        list_result.Add(resultModel);
                                    }
                                }
                            }
                        }
                    }
                }

                if(list_update.Count > 0)
                {
                    foreach (var item in list_update)
                    {
                        if (!string.IsNullOrEmpty(item.ProjectID) && !string.IsNullOrEmpty(item.DoUAction))
                        {
                            //更新Table ProjectM、Project_DO、Project_DOASUpdate
                            var MModle = projectModels.Where(e => e.ProjectID == item.ProjectID).FirstOrDefault();
                            var doModel = projectDoModels.Where(e => e.ProjectID == item.ProjectID).FirstOrDefault();

                            //update 是否結案並新增一筆紀錄到Table Project_DOASUpdate
                            if (doModel != null)
                            {
                                Project_DOASUpdate newModel = new Project_DOASUpdate()
                                {
                                    SEQ = NewProjectDOASSEQ,
                                    DoID = doModel.DoID,
                                    DoUDate = item.DoUDate,
                                    DoUStatus = item.DoUStatus,
                                    DoUAction = item.DoUAction
                                };

                                _project_DOASUpdateContext.Add(newModel);
                                _project_DOASUpdateContext.SaveChanges();
                            }

                            DoMaintainModel_Result resultModel = new DoMaintainModel_Result()
                            {
                                ProjectID = item.ProjectID,
                                UploadStatus = "Status上傳成功"
                            };
                            list_result.Add(resultModel);
                        }
                    }
                }
            }
            return list_result;
        }

        public List<DoReportUpload> UpdateDoStatus(List<DoReportUpload> list_DoUpload)
        {
            if (list_DoUpload.Count == 0)
            {
                return list_DoUpload;
            }

            foreach (var item in list_DoUpload)
            {
                try
                {
                    var DoModels = _projectDOContext.Project_DO.ToList();
                    var ProjectModels = _projectMContext.ProjectM.ToList();

                    //更新獎金狀態，update Project_DO的Status欄位
                    if (!string.IsNullOrEmpty(item.StatusUpdate) && (item.StatusUpdate == "Y" || item.StatusUpdate == "U" || item.StatusUpdate == "A"))
                    {
                        var model = DoModels.Where(e => e.ProjectID == item.ProjectID && (e.Status == "N" || e.Status == "A")).FirstOrDefault();
                        if (model != null)
                        {
                            switch (item.StatusUpdate)
                            {
                                // 發放獎金，Do狀態 =>Y(獎金已發)
                                case "Y":
                                    model.Status = "Y";
                                    break;

                                // 不發放獎金，Do狀態 =>U(不發獎金)
                                case "N":
                                    model.Status = "U";
                                    break;

                                // 暫緩發放，Do狀態 => A(暫緩發放獎金)
                                case "T":
                                    model.Status = "A";
                                    break;
                            }
                            _projectDOContext.Update(model);
                            _projectDOContext.SaveChanges();

                            //更新ProjectM的日期
                            var NewModel = ProjectModels.Where(e => e.ProjectID == item.ProjectID).FirstOrDefault();
                            if (NewModel != null)
                            {
                                NewModel.UpdateDate = DateTime.Now.ToString("yyyyMMdd");

                                _projectMContext.Update(NewModel);
                                _projectMContext.SaveChanges();
                            }

                            //以上未出錯，則回寫success到list的message
                            item.message = "success";
                        }
                    }
                }
                catch (Exception ex)
                {
                    // 處理異常（可以記錄異常或根據需要進行處理）
                    Console.WriteLine("Error updating project status: " + ex.Message);
                    item.message = "error: " + ex.Message;
                }
            }
            return list_DoUpload;
        }
    } 
}