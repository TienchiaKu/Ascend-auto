using DDD_2024.Data;
using DDD_2024.Interfaces;
using DDD_2024.Models;
using Elfie.Serialization;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Build.Evaluation;
using Microsoft.CodeAnalysis;
using Microsoft.DotNet.Scaffolding.Shared.ProjectModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
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
        private readonly IEmployeeService _employeeService;
        private readonly ICusVendorService _cusVendoeService;

        public DoService(ProjectMContext projectMContext, ProjectDContext projectDContext, ProjectDOContext projectDOContext, 
            Project_EmpContext project_EmpContext, Project_DOASUpdateContext project_DOASUpdateContext,  ASCENDContext aSCENDContext, 
            ATIContext aTIContext, KIR1NContext kIR1NContext,
            INTERTEKContext iNTERTEKContext, TESTBContext tESTBContext, IEmployeeService employeeService, ICusVendorService cusVendoeService)
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

        public async Task<List<DoReportViewModel>> GetDosReport(DoReportFilterViewModel filter)
        {
            var modelDo = await _projectDOContext.Project_DO.OrderBy(e => e.ProjectID).ToListAsync();

            if (!string.IsNullOrEmpty(filter.DoStatus))
            {
                modelDo = modelDo.Where(e => e.Status == filter.DoStatus).ToList();
            }

            if (filter.applicantID != 999)
            {
                modelDo = modelDo.Where(e => e.ApplicantID == filter.applicantID).ToList();
            }

            var list_dosReport = new List<DoReportViewModel>();

            foreach (var item in modelDo)
            {
                var model = new DoReportViewModel
                {
                    ProjectID = item.ProjectID,
                    Applicant = _employeeService.GetEmployeeName(item.ApplicantID),
                    Approver = _employeeService.GetEmployeeName(item.ApproverID),
                    TradeStatus = item.TradeStatus
                };

                if (!string.IsNullOrEmpty(item.CreateDate))
                {
                    model.ApplicationDate = item.CreateDate.Substring(0, 4) + "/" + item.CreateDate.Substring(4, 2) + "/" + item.CreateDate.Substring(6, 2);
                }

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

                //加入Project_DOASUpdate的資料
                if (!string.IsNullOrEmpty(item.DoID))
                {
                    var modelDOAS = _project_DOASUpdateContext.Project_DOASUpdate.
                                    Where(e => e.DoID == item.DoID).ToList() ?? null;

                    if (modelDOAS != null)
                    {
                        var OrderDOAS = modelDOAS.OrderByDescending(p => p.DoUDate).FirstOrDefault();

                        if (OrderDOAS != null)
                        {
                            model.DoUStatus = OrderDOAS.DoUStatus;
                            model.DoUAction = OrderDOAS.DoUAction;
                        }
                    }
                }

                list_dosReport.Add(model);
            }
            return list_dosReport;
        }
        public async Task EditDo(DoViewModel model)
        {
            if(model != null)
            {
                var existingEntity = await _projectDOContext.Project_DO.AsNoTracking().FirstOrDefaultAsync(p => p.DoID == model.DoID);

                if (existingEntity != null)
                {
                    existingEntity.CreateDate = model.CreateDate.ToString("yyyyMMdd");
                    existingEntity.ApplicantID = model.ApplicantID;
                    existingEntity.ApproverID = model.ApproverID;
                    existingEntity.TradeStatus = model.TradeStatus;

                    _projectDOContext.Update(existingEntity);
                    await _projectDOContext.SaveChangesAsync();
                }
            }
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
        public List<SelectListItem> GetTradingStatus()
        {
            List<SelectListItem> selectListItems = new List<SelectListItem>();

            selectListItems = new List<SelectListItem>
            {
                new SelectListItem{ Text = "New", Value ="N"},
                new SelectListItem{ Text = "Active", Value ="A"},
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
        //匯入Do進度
        public List<DOASU_Upload_ViewModel> ImportDOASU(List<DOASU_Upload_ViewModel> list_Upload)
        {
            List<DOASU_Upload_ViewModel> DOASU_Upload_ViewModels = new List<DOASU_Upload_ViewModel>();

            if (list_Upload.Count > 0)
            {
                foreach(var item in list_Upload)
                {
                    //搜尋DoID
                    var DoID = _projectDOContext.Project_DO.Where(e => e.ProjectID == item.ProjectID).Select(e => e.DoID).FirstOrDefault();

                    if (!string.IsNullOrEmpty(DoID))
                    {
                        DOASU_Upload_ViewModel modelVM = new DOASU_Upload_ViewModel();

                        if (!string.IsNullOrEmpty(item.DoUStatusCurrent))
                        {
                            //本月進度
                            Project_DOASUpdate model2 = new Project_DOASUpdate()
                            {
                                SEQ = NewProjectDOASSEQ,
                                DoID = DoID,
                                DoUDate = "202405",
                                DoUAction = item.DoUAction,
                                DoUStatus = item.DoUStatusCurrent
                            };
                            _project_DOASUpdateContext.Project_DOASUpdate.Add(model2);
                            _project_DOASUpdateContext.SaveChanges();

                            modelVM = new DOASU_Upload_ViewModel()
                            {
                                ProjectID = item.ProjectID,
                                DoUDate = "2024/05",
                                DoUAction = item.DoUAction,
                                DoUStatus = item.DoUStatusCurrent,
                                UploadStatus = "Success"
                            };
                            DOASU_Upload_ViewModels.Add(modelVM);
                        }

                        if (!string.IsNullOrEmpty(item.DoUStatus4))
                        {
                            //4月進度
                            Project_DOASUpdate model4 = new Project_DOASUpdate()
                            {
                                SEQ = NewProjectDOASSEQ,
                                DoID = DoID,
                                DoUDate = "202404",
                                DoUStatus = item.DoUStatus4
                            };
                            _project_DOASUpdateContext.Project_DOASUpdate.Add(model4);
                            _project_DOASUpdateContext.SaveChanges();

                            modelVM = new DOASU_Upload_ViewModel()
                            {
                                ProjectID = item.ProjectID,
                                DoUDate = "2024/04",
                                DoUStatus = item.DoUStatus4,
                                UploadStatus = "Success"
                            };
                            DOASU_Upload_ViewModels.Add(modelVM);
                        }

                        if (!string.IsNullOrEmpty(item.DoUStatus5))
                        {
                            //5月進度
                            Project_DOASUpdate model5 = new Project_DOASUpdate()
                            {
                                SEQ = NewProjectDOASSEQ,
                                DoID = DoID,
                                DoUDate = "202405",
                                DoUStatus = item.DoUStatus5
                            };
                            _project_DOASUpdateContext.Project_DOASUpdate.Add(model5);
                            _project_DOASUpdateContext.SaveChanges();

                            modelVM = new DOASU_Upload_ViewModel()
                            {
                                ProjectID = item.ProjectID,
                                DoUDate = "2024/05",
                                DoUStatus = item.DoUStatus5,
                                UploadStatus = "Success"
                            };
                            DOASU_Upload_ViewModels.Add(modelVM);
                        }
                    }
                }
            }
            return DOASU_Upload_ViewModels;
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
                    //更新獎金狀態，update Project_DO的Status欄位
                    if (!string.IsNullOrEmpty(item.StatusUpdate) && (item.StatusUpdate == "Y" || item.StatusUpdate == "N" || item.StatusUpdate == "T"
                        || item.StatusUpdate == "D"))
                    {
                        var model = _projectDOContext.Project_DO.Where(e => e.ProjectID == item.ProjectID && e.Status == "N").FirstOrDefault();
                        if (model != null)
                        {
                            switch (item.StatusUpdate)
                            {
                                // 發放獎金，Do狀態 =>A(獎金已發)
                                case "Y":
                                    model.Status = "A";
                                    break;

                                // 不發放獎金，Do狀態 =>B(不發獎金)
                                case "N":
                                    model.Status = "B";
                                    break;

                                // 暫緩發放，Do狀態 => C(暫緩發放獎金)
                                case "T":
                                    model.Status = "C";
                                    break;

                                // 暫緩發放，Do狀態 => D(作廢)
                                case "D":
                                    model.Status = "D";
                                    break;
                            }

                            _projectDOContext.Update(model);
                            _projectDOContext.SaveChanges();

                            //以上未出錯，則回寫success到list的message
                            item.message = "success";
                        }
                    }
                    //更新Do -> DesignLoss，update ProjectM的Status欄位
                    else if (!string.IsNullOrEmpty(item.StatusUpdate) && (item.StatusUpdate == "Z"))
                    {
                        var chkmodel = _projectDOContext.Project_DO.Where(e => e.ProjectID == item.ProjectID && e.Status == "D").FirstOrDefault();

                        if (chkmodel != null) 
                        {
                            var model = _projectMContext.ProjectM.Where(e => e.ProjectID == item.ProjectID && !string.IsNullOrEmpty(e.Status)
                                        && e.Status == "DO").FirstOrDefault();

                            if (model != null)
                            {
                                model.Status = "DL";
                            }
                        }

                        //以上未出錯，則回寫success到list的message
                        item.message = "success";
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