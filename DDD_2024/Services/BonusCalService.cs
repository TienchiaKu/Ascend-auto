using DDD_2024.Data;
using DDD_2024.Interfaces;
using DDD_2024.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Globalization;

namespace DDD_2024.Services
{
    public class BonusCalService : IBounsCalService
    {
        private readonly ProjectDOContext _projectDOContext;
        private readonly ProjectMContext _projectMContext;
        private readonly ProjectDContext _projectDContext;
        private readonly Project_EmpContext _projectEmpContext;
        private readonly Project_DIDWContext _projectDIDWContext;
        private readonly Project_DOASUpdateContext _project_DOASUpdateContext;
        private readonly BizAutoContext _BizAutoContext;
        private readonly IEmployeeService _employeeService;
        private readonly ICusVendorService _cusVendoeService;
        private readonly IDoService _doService;

        public BonusCalService(ProjectMContext projectMContext, ProjectDContext projectDContext, ProjectDOContext projectDOContext,
            Project_DIDWContext projectDIDWContext, Project_DOASUpdateContext project_DOASUpdateContext, BizAutoContext bizAutoContext,
            Project_EmpContext project_EmpContext, IEmployeeService employeeService, ICusVendorService cusVendoeService, IDoService doService)
        {
            _projectMContext = projectMContext;
            _projectDContext = projectDContext;
            _projectDOContext = projectDOContext;
            _projectDIDWContext = projectDIDWContext;
            _projectEmpContext = project_EmpContext;
            _employeeService = employeeService;
            _cusVendoeService = cusVendoeService;
            _BizAutoContext = bizAutoContext;
            _doService = doService;
            _project_DOASUpdateContext = project_DOASUpdateContext;
        }

        public async Task<List<BonusCalViewModel>> GetProjects_Do()
        {
            List<BonusCalViewModel> list_BonusCal = new List<BonusCalViewModel>();
            
            //加入DO資料
            var modelDo = await _projectDOContext.Project_DO.Where(p => p.Status == "N" ).ToListAsync();

            foreach (var item in modelDo)
            {
                var modelProjectM = _projectMContext.ProjectM.Where(p => p.ProjectID == item.ProjectID).FirstOrDefault();
                var modelProjectD = _projectDContext.ProjectD.Where(p => p.ProjectID == item.ProjectID && p.Stage == "DO").FirstOrDefault();

                if (modelProjectM != null && modelProjectD != null)
                {
                    var model = new BonusCalViewModel
                    {
                        ProjectID = item.ProjectID,
                        PartNo = modelProjectD.PartNo,
                        Status = "DO"
                    };

                    if (!string.IsNullOrEmpty(modelProjectM.Cus_DB) && !string.IsNullOrEmpty(modelProjectM.CusID))
                    {
                        model.CusName = _cusVendoeService.GetvendorName(modelProjectM.Cus_DB,modelProjectM.CusID);
                    }

                    if (!string.IsNullOrEmpty(modelProjectD.VendorID))
                    {
                        model.VendorName = _cusVendoeService.GetVendorName(modelProjectD.VendorID);
                        
                    }

                    list_BonusCal.Add(model);
                }
            }

            ////加入Din資料
            //var modelDIn = await _projectDIDWContext.Project_DIDW.Where(p => p.DinStatus == "C").ToListAsync();
            //foreach (var item in modelDIn)
            //{
            //    var modelProjectM = _projectMContext.ProjectM.Where(p => p.ProjectID == item.ProjectID).FirstOrDefault();
            //    var modelProjectD = _projectDContext.ProjectD.Where(p => p.ProjectID == item.ProjectID && p.Stage == "DIN").FirstOrDefault();
            //
            //    if (modelProjectM != null && modelProjectD != null)
            //    {
            //        var model = new BonusCalViewModel
            //        {
            //            ProjectID = item.ProjectID,
            //            PartNo = modelProjectD.PartNo,
            //            Status = "DIN"
            //        };
            //
            //        if (!string.IsNullOrEmpty(modelProjectM.Cus_DB) && !string.IsNullOrEmpty(modelProjectM.CusID))
            //        {
            //            model.CusName = _cusVendoeService.GetvendorName(modelProjectM.Cus_DB, modelProjectM.CusID);
            //        }
            //
            //        if (!string.IsNullOrEmpty(modelProjectD.VendorID))
            //        {
            //            model.VendorName = await _cusVendoeService.GetVenName(modelProjectD.VendorID);
            //        }
            //
            //        list_BonusCal.Add(model);
            //    }
            //}
            //
            ////加入DWIN資料
            //var modelDWin = await _projectDIDWContext.Project_DIDW.Where(p => p.DwinStatus == "C").ToListAsync();
            //foreach (var item in modelDWin)
            //{
            //    var modelProjectM = _projectMContext.ProjectM.Where(p => p.ProjectID == item.ProjectID).FirstOrDefault();
            //    var modelProjectD = _projectDContext.ProjectD.Where(p => p.ProjectID == item.ProjectID && p.Stage == "DWIN").FirstOrDefault();
            //
            //    if (modelProjectM != null && modelProjectD != null)
            //    {
            //        var model = new BonusCalViewModel
            //        {
            //            ProjectID = item.ProjectID,
            //            PartNo = modelProjectD.PartNo,
            //            Status = "DWIN"
            //        };
            //
            //        if (!string.IsNullOrEmpty(modelProjectM.Cus_DB) && !string.IsNullOrEmpty(modelProjectM.CusID))
            //        {
            //            model.CusName = _cusVendoeService.GetvendorName(modelProjectM.Cus_DB, modelProjectM.CusID);
            //        }
            //
            //        if (!string.IsNullOrEmpty(modelProjectD.VendorID))
            //        {
            //            model.VendorName = await _cusVendoeService.GetVenName(modelProjectD.VendorID);
            //        }
            //
            //        list_BonusCal.Add(model);
            //    }
            //}
            return list_BonusCal;
        }

        public async Task<List<BonusCalViewModel>> GetProjects_DoFilter(List<string> list_months)
        {
            List<BonusCalViewModel> list_BonusCal = new List<BonusCalViewModel>();

            //加入DO資料
            var modelDo = await _projectDOContext.Project_DO.Where(p => p.Status == "N").ToListAsync();

            if (list_months.Count > 0)
            {
                //如果list_months有多個值的話，先用List來儲存篩選結果，再把結果覆蓋list_ProjectDo
                List<Project_DO> list_DoFilter = new List<Project_DO>();

                foreach (var item in list_months)
                {
                    var FilterList = modelDo.Where(p => !string.IsNullOrEmpty(p.CreateDate) && p.CreateDate.Contains(item)).ToList();
                    list_DoFilter.AddRange(FilterList);
                }

                modelDo = list_DoFilter;
            }

            foreach (var item in modelDo)
            {
                var modelProjectM = _projectMContext.ProjectM.Where(p => p.ProjectID == item.ProjectID).FirstOrDefault();
                var modelProjectD = _projectDContext.ProjectD.Where(p => p.ProjectID == item.ProjectID && p.Stage == "DO").FirstOrDefault();

                if (modelProjectM != null && modelProjectD != null)
                {
                    var model = new BonusCalViewModel
                    {
                        ProjectID = item.ProjectID,
                        PartNo = modelProjectD.PartNo,
                        Status = "DO"
                    };

                    if (!string.IsNullOrEmpty(modelProjectM.Cus_DB) && !string.IsNullOrEmpty(modelProjectM.CusID))
                    {
                        model.CusName = _cusVendoeService.GetvendorName(modelProjectM.Cus_DB, modelProjectM.CusID);
                    }

                    if (!string.IsNullOrEmpty(modelProjectD.VendorID))
                    {
                        model.VendorName = _cusVendoeService.GetVendorName(modelProjectD.VendorID);
                    }

                    list_BonusCal.Add(model);
                }
            }
            return list_BonusCal;
        }

        public async Task<List<BonusCalViewModel>> GetProjects_DINWIN()
        {
            List<BonusCalViewModel> list_BonusCal = new List<BonusCalViewModel>();           

            //加入Din資料
            var modelDIn = await _projectDIDWContext.Project_DIDW.Where(p => p.DinStatus == "C").ToListAsync();
            foreach (var item in modelDIn)
            {
                var modelProjectM = _projectMContext.ProjectM.Where(p => p.ProjectID == item.ProjectID).FirstOrDefault();
                var modelProjectD = _projectDContext.ProjectD.Where(p => p.ProjectID == item.ProjectID && p.Stage == "DIN").FirstOrDefault();

                if (modelProjectM != null && modelProjectD != null)
                {
                    var model = new BonusCalViewModel
                    {
                        ProjectID = item.ProjectID,
                        PartNo = modelProjectD.PartNo,
                        Status = "DIN"
                    };

                    if (!string.IsNullOrEmpty(modelProjectM.Cus_DB) && !string.IsNullOrEmpty(modelProjectM.CusID))
                    {
                        model.CusName = _cusVendoeService.GetvendorName(modelProjectM.Cus_DB, modelProjectM.CusID);
                    }

                    if (!string.IsNullOrEmpty(modelProjectD.VendorID))
                    {
                        model.VendorName = _cusVendoeService.GetVendorName(modelProjectD.VendorID);
                    }

                    list_BonusCal.Add(model);
                }
            }

            //加入DWIN資料
            var modelDWin = await _projectDIDWContext.Project_DIDW.Where(p => p.DwinStatus == "C").ToListAsync();
            foreach (var item in modelDWin)
            {
                var modelProjectM = _projectMContext.ProjectM.Where(p => p.ProjectID == item.ProjectID).FirstOrDefault();
                var modelProjectD = _projectDContext.ProjectD.Where(p => p.ProjectID == item.ProjectID && p.Stage == "DWIN").FirstOrDefault();

                if (modelProjectM != null && modelProjectD != null)
                {
                    var model = new BonusCalViewModel
                    {
                        ProjectID = item.ProjectID,
                        PartNo = modelProjectD.PartNo,
                        Status = "DWIN"
                    };

                    if (!string.IsNullOrEmpty(modelProjectM.Cus_DB) && !string.IsNullOrEmpty(modelProjectM.CusID))
                    {
                        model.CusName = _cusVendoeService.GetvendorName(modelProjectM.Cus_DB, modelProjectM.CusID);
                    }

                    if (!string.IsNullOrEmpty(modelProjectD.VendorID))
                    {
                        model.VendorName = _cusVendoeService.GetVendorName(modelProjectD.VendorID);
                    }

                    list_BonusCal.Add(model);
                }
            }
            return list_BonusCal;
        }

        public List<ProjectBonusViewModel> BonusConfirm(string[] projectIds)
        {
            //根據類別為DO、DIN、DWIN發放獎金 --> 修改Status=F
            //抓專案的Do、Din、Dwin，若狀態皆為X或F，專案狀態=F
            //匯出Excel報表

            List<ProjectBonusViewModel> list_PBonus = new List<ProjectBonusViewModel>();

            foreach (var item in projectIds)
            {
                string projectsStage = item.Substring(13);
                string projectID = item.Substring(0,12);
                var model = new ProjectBonusViewModel();

                switch (projectsStage)
                {
                    case "DIN":
                        model = GetDinBonus(projectID);
                        break;
                    case "DWIN":
                        model = GetDwinBonus(projectID);
                        break;
                }

                list_PBonus.Add(model);
            }

            return list_PBonus;
        }
       
        private ProjectBonusViewModel GetDinBonus(string projectID)
        {
            var projectM = _projectMContext.ProjectM.Where(e => e.ProjectID == projectID).FirstOrDefault();
            var projectD = _projectDContext.ProjectD.Where(e => e.ProjectID == projectID && e.Stage == "DIN").FirstOrDefault();
            var projectEmp = _projectEmpContext.Project_Emp.Where(e => e.ProjectID == projectID).ToList();

            var model = new ProjectBonusViewModel();

            if (projectM != null && projectD != null && projectEmp != null)
            {
                model.ProjectID = projectID;
                model.Stage = "DIN";

                double bonus_Pool = projectD.ELTR * projectD.EGP * 0.02;

                if (_employeeService.GetEmployeeName(projectEmp.Where(p => p.Duty == "PM").Select(p => p.EmpID).FirstOrDefault()) != string.Empty)
                {
                    model.PM_EmpName = _employeeService.GetEmployeeName(projectEmp.Where(p => p.Duty == "PM").Select(p => p.EmpID).FirstOrDefault());
                    if(_employeeService.CheckOnDuty(0, model.PM_EmpName))
                    {
                        model.PM_Bonus = Math.Round(bonus_Pool * projectEmp.Where(p => p.Duty == "PM").Select(p => p.BonusP).FirstOrDefault(), 2);
                    }
                    else
                    {
                        model.PM_Bonus = 0;
                    }
                }

                if (_employeeService.GetEmployeeName(projectEmp.Where(p => p.Duty == "Sales").Select(p => p.EmpID).FirstOrDefault()) != string.Empty)
                {
                    model.Sales_EmpName = _employeeService.GetEmployeeName(projectEmp.Where(p => p.Duty == "Sales").Select(p => p.EmpID).FirstOrDefault());
                    if (_employeeService.CheckOnDuty(0, model.Sales_EmpName))
                    {
                        model.Sales_Bonus = Math.Round(bonus_Pool * projectEmp.Where(p => p.Duty == "Sales").Select(p => p.BonusP).FirstOrDefault(), 2);
                    }
                    else
                    {
                        model.Sales_Bonus = 0;
                    }
                }

                if (_employeeService.GetEmployeeName(projectEmp.Where(p => p.Duty == "FAE1").Select(p => p.EmpID).FirstOrDefault()) != string.Empty)
                {
                    model.FAE1_EmpName = _employeeService.GetEmployeeName(projectEmp.Where(p => p.Duty == "FAE1").Select(p => p.EmpID).FirstOrDefault());
                    if (_employeeService.CheckOnDuty(0, model.FAE1_EmpName))
                    {
                        model.FAE1_Bonus = Math.Round(bonus_Pool * projectEmp.Where(p => p.Duty == "FAE1").Select(p => p.BonusP).FirstOrDefault(), 2);
                    }
                    else
                    {
                        model.FAE1_Bonus = 0;
                    }
                }

                if (_employeeService.GetEmployeeName(projectEmp.Where(p => p.Duty == "FAE2").Select(p => p.EmpID).FirstOrDefault()) != string.Empty)
                {
                    model.FAE2_EmpName = _employeeService.GetEmployeeName(projectEmp.Where(p => p.Duty == "FAE2").Select(p => p.EmpID).FirstOrDefault());
                    if (_employeeService.CheckOnDuty(0, model.FAE2_EmpName))
                    {
                        model.FAE2_Bonus = Math.Round(bonus_Pool * projectEmp.Where(p => p.Duty == "FAE2").Select(p => p.BonusP).FirstOrDefault(), 2);
                    }
                    else
                    {
                        model.FAE2_Bonus = 0;
                    }
                }

                if (_employeeService.GetEmployeeName(projectEmp.Where(p => p.Duty == "FAE3").Select(p => p.EmpID).FirstOrDefault()) != string.Empty)
                {
                    model.FAE3_EmpName = _employeeService.GetEmployeeName(projectEmp.Where(p => p.Duty == "FAE3").Select(p => p.EmpID).FirstOrDefault());
                    if (_employeeService.CheckOnDuty(0, model.FAE3_EmpName))
                    {
                        model.FAE3_Bonus = Math.Round(bonus_Pool * projectEmp.Where(p => p.Duty == "FAE3").Select(p => p.BonusP).FirstOrDefault(), 2);
                    }
                    else
                    {
                        model.FAE3_Bonus = 0;
                    }
                }
            }

            return model;
        }

        private ProjectBonusViewModel GetDwinBonus(string projectID)
        {
            var projectM = _projectMContext.ProjectM.Where(e => e.ProjectID == projectID).FirstOrDefault();
            var projectD = _projectDContext.ProjectD.Where(e => e.ProjectID == projectID && e.Stage == "DWIN").FirstOrDefault();
            var projectEmp = _projectEmpContext.Project_Emp.Where(e => e.ProjectID == projectID).ToList();

            var model = new ProjectBonusViewModel();

            if (projectM != null && projectD != null && projectEmp != null)
            {
                model.ProjectID = projectID;
                model.Stage = "DWIN";

                double bonus_Pool = projectD.ELTR * projectD.EGP * 0.02;

                if (_employeeService.GetEmployeeName(projectEmp.Where(p => p.Duty == "PM").Select(p => p.EmpID).FirstOrDefault()) != string.Empty)
                {
                    model.PM_EmpName = _employeeService.GetEmployeeName(projectEmp.Where(p => p.Duty == "PM").Select(p => p.EmpID).FirstOrDefault());
                    if (_employeeService.CheckOnDuty(0, model.PM_EmpName))
                    {
                        model.PM_Bonus = Math.Round(bonus_Pool * projectEmp.Where(p => p.Duty == "PM").Select(p => p.BonusP).FirstOrDefault(), 2);
                    }
                    else
                    {
                        model.PM_Bonus = 0;
                    }
                }

                if (_employeeService.GetEmployeeName(projectEmp.Where(p => p.Duty == "Sales").Select(p => p.EmpID).FirstOrDefault()) != string.Empty)
                {
                    model.Sales_EmpName = _employeeService.GetEmployeeName(projectEmp.Where(p => p.Duty == "Sales").Select(p => p.EmpID).FirstOrDefault());
                    if (_employeeService.CheckOnDuty(0, model.Sales_EmpName))
                    {
                        model.Sales_Bonus = Math.Round(bonus_Pool * projectEmp.Where(p => p.Duty == "Sales").Select(p => p.BonusP).FirstOrDefault(), 2);
                    }
                    else
                    {
                        model.Sales_Bonus = 0;
                    }
                }

                if (_employeeService.GetEmployeeName(projectEmp.Where(p => p.Duty == "FAE1").Select(p => p.EmpID).FirstOrDefault()) != string.Empty)
                {
                    model.FAE1_EmpName = _employeeService.GetEmployeeName(projectEmp.Where(p => p.Duty == "FAE1").Select(p => p.EmpID).FirstOrDefault());
                    if (_employeeService.CheckOnDuty(0, model.FAE1_EmpName))
                    {
                        model.FAE1_Bonus = Math.Round(bonus_Pool * projectEmp.Where(p => p.Duty == "FAE1").Select(p => p.BonusP).FirstOrDefault(), 2);
                    }
                    else
                    {
                        model.FAE1_Bonus = 0;
                    }
                }

                if (_employeeService.GetEmployeeName(projectEmp.Where(p => p.Duty == "FAE2").Select(p => p.EmpID).FirstOrDefault()) != string.Empty)
                {
                    model.FAE2_EmpName = _employeeService.GetEmployeeName(projectEmp.Where(p => p.Duty == "FAE2").Select(p => p.EmpID).FirstOrDefault());
                    if (_employeeService.CheckOnDuty(0, model.FAE2_EmpName))
                    {
                        model.FAE2_Bonus = Math.Round(bonus_Pool * projectEmp.Where(p => p.Duty == "FAE2").Select(p => p.BonusP).FirstOrDefault(), 2);
                    }
                    else
                    {
                        model.FAE2_Bonus = 0;
                    }
                }

                if (_employeeService.GetEmployeeName(projectEmp.Where(p => p.Duty == "FAE3").Select(p => p.EmpID).FirstOrDefault()) != string.Empty)
                {
                    model.FAE3_EmpName = _employeeService.GetEmployeeName(projectEmp.Where(p => p.Duty == "FAE3").Select(p => p.EmpID).FirstOrDefault());
                    if (_employeeService.CheckOnDuty(0, model.FAE3_EmpName))
                    {
                        model.FAE3_Bonus = Math.Round(bonus_Pool * projectEmp.Where(p => p.Duty == "FAE3").Select(p => p.BonusP).FirstOrDefault(), 2);
                    }
                    else
                    {
                        model.FAE3_Bonus = 0;
                    }
                }
            }

            return model;
        }

        public List<EmployeeBonusViewMode> GetDIDWBonusbyEmployee(List<ProjectBonusViewModel> list_ProjectBonus)
        {
            List<EmployeeBonusViewMode> list_EmpBonus = new List<EmployeeBonusViewMode>();

            if (list_ProjectBonus.Count > 0)
            {
                //儲存每個人的獎金用的字典
                var EmpBonusTTL = new Dictionary<string, double>();

                foreach(var item in list_ProjectBonus)
                {
                    if (!string.IsNullOrEmpty(item.PM_EmpName))
                    {
                        if (EmpBonusTTL.ContainsKey(item.PM_EmpName))
                        {
                            EmpBonusTTL[item.PM_EmpName] += item.PM_Bonus;
                        }
                        else
                        {
                            EmpBonusTTL[item.PM_EmpName] = item.PM_Bonus;
                        }
                    }

                    if (!string.IsNullOrEmpty(item.Sales_EmpName))
                    {
                        if (EmpBonusTTL.ContainsKey(item.Sales_EmpName))
                        {
                            EmpBonusTTL[item.Sales_EmpName] += item.Sales_Bonus;
                        }
                        else
                        {
                            EmpBonusTTL[item.Sales_EmpName] = item.Sales_Bonus;
                        }
                    }

                    if (!string.IsNullOrEmpty(item.FAE1_EmpName))
                    {
                        if (EmpBonusTTL.ContainsKey(item.FAE1_EmpName))
                        {
                            EmpBonusTTL[item.FAE1_EmpName] += item.FAE1_Bonus;
                        }
                        else
                        {
                            EmpBonusTTL[item.FAE1_EmpName] = item.FAE1_Bonus;
                        }
                    }

                    if (!string.IsNullOrEmpty(item.FAE2_EmpName))
                    {
                        if (EmpBonusTTL.ContainsKey(item.FAE2_EmpName))
                        {
                            EmpBonusTTL[item.FAE2_EmpName] += item.FAE2_Bonus;
                        }
                        else
                        {
                            EmpBonusTTL[item.FAE2_EmpName] = item.FAE2_Bonus;
                        }
                    }

                    if (!string.IsNullOrEmpty(item.FAE3_EmpName))
                    {
                        if (EmpBonusTTL.ContainsKey(item.FAE3_EmpName))
                        {
                            EmpBonusTTL[item.FAE3_EmpName] += item.FAE3_Bonus;
                        }
                        else
                        {
                            EmpBonusTTL[item.FAE3_EmpName] = item.FAE3_Bonus;
                        }
                    }
                }

                foreach(var entry in EmpBonusTTL)
                {
                    //排除掉離職人員(EmployeeM.OnDuty = "N")
                    if (_employeeService.CheckOnDuty(0, entry.Key))
                    {
                        var employeeBonus = new EmployeeBonusViewMode
                        {
                            EmployeeName = entry.Key,
                            Bonus = entry.Value
                        };
                        list_EmpBonus.Add(employeeBonus);
                    }
                }
            }
            return list_EmpBonus;
        }

        public List<EmployeeBonusViewMode> GetDOBonusbyEmployee(List<ProjectBonusViewModel> list_ProjectBonus)
        {
            List<EmployeeBonusViewMode> list_EmpBonus = new List<EmployeeBonusViewMode>();

            if (list_ProjectBonus.Count > 0)
            {
                //儲存每個人的獎金用的字典
                var EmpBonusTTL = new Dictionary<string, double>();

                foreach (var item in list_ProjectBonus)
                {
                    if (!string.IsNullOrEmpty(item.ApplicantName))
                    {
                        if (EmpBonusTTL.ContainsKey(item.ApplicantName))
                        {
                            EmpBonusTTL[item.ApplicantName] += item.DOBonus;
                        }
                        else
                        {
                            EmpBonusTTL[item.ApplicantName] = item.DOBonus;
                        }
                    }                
                }

                foreach (var entry in EmpBonusTTL)
                {
                    //排除掉離職人員(EmployeeM.OnDuty = "N")
                    if (_employeeService.CheckOnDuty(0, entry.Key))
                    {
                        var employeeBonus = new EmployeeBonusViewMode
                        {
                            EmployeeName = entry.Key,
                            Bonus = entry.Value
                        };
                        list_EmpBonus.Add(employeeBonus);
                    }
                }
            }
            return list_EmpBonus;
        }

        public async Task<List<BDoReportViewModel>> GetDoReport(BDoReportFilter filterModel)
        {
            List<BDoReportViewModel> list_DoReport = new List<BDoReportViewModel>();

            var doModel = await _projectDOContext.Project_DO.Where(e => e.Status == "N").ToListAsync();
            var empModel = await _BizAutoContext.employeeM.ToListAsync();

            //篩選條件Start------------------------------
            if (filterModel.StartDate.HasValue)
            {
                doModel = doModel.Where(e =>
                {
                    DateTime createDate;
                    return DateTime.TryParseExact(e.CreateDate, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out createDate) &&
                           createDate >= filterModel.StartDate.Value;
                }).ToList();
            }

            if (filterModel.EndDate.HasValue)
            {
                doModel = doModel.Where(e =>
                {
                    DateTime createDate;
                    return DateTime.TryParseExact(e.CreateDate, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out createDate) &&
                           createDate <= filterModel.EndDate.Value;
                }).ToList();
            }

            if (filterModel.Region != "All")
            {
                var empIds = _BizAutoContext.employeeM
                                 .Where(e => e.Region == filterModel.Region)
                                 .Select(e => e.EmpID)
                                 .ToList();
                doModel = doModel.Where(e => empIds.Contains(e.ApplicantID)).ToList();
            }
            //篩選條件End------------------------------
            int seq = 1;

            if (doModel.Any())
            {
                var modelM = await _projectMContext.ProjectM.ToListAsync();
                var modelD = await _projectDContext.ProjectD.ToListAsync();
                var modelDoAS = await _project_DOASUpdateContext.Project_DOASUpdate.ToListAsync();
                
                foreach(var item in doModel)
                {
                    //取Project_DO資料
                    var newModel = new BDoReportViewModel()
                    {
                        SEQ = seq,
                        ProjectID = item.ProjectID,
                        ApplicationDate = item.CreateDate?.Substring(0, 4) + "/" + item.CreateDate?.Substring(4, 2) + "/" + item.CreateDate?.Substring(6, 2) ?? string.Empty,
                        ApplicantID = item.ApplicantID,
                        Applicant = _employeeService.GetEmployeeName(item.ApplicantID),
                        Approver = _employeeService.GetEmployeeName(item.ApproverID),
                        NewActive = item.TradeStatus != null ? _doService.GetTradingStatusName(item.TradeStatus) : string.Empty
                    };

                    //加入ProjectM的資料
                    if (modelM.Any())
                    {
                        var modelM_Filter = modelM.Where(e => e.ProjectID == item.ProjectID).FirstOrDefault();

                        if(modelM_Filter != null)
                        {
                            if (!string.IsNullOrEmpty(modelM_Filter.Cus_DB) && !string.IsNullOrEmpty(modelM_Filter.CusID))
                            {
                                // 將 matchingProjectM 的資料設定給 Cus_DB 屬性
                                newModel.CusName = _cusVendoeService.GetvendorName(modelM_Filter.Cus_DB, modelM_Filter.CusID);
                            }
                            newModel.ProApp = modelM_Filter.ProApp;
                        }
                    }

                    // 加入ProjectD的資料
                    if (modelD.Any())
                    {
                        var modelD_Filter = modelD.Where(e => e.ProjectID == item.ProjectID).FirstOrDefault();

                        if (modelD_Filter != null)
                        {
                            newModel.VendorName = modelD_Filter.VendorID != null ? _cusVendoeService.GetVendorName(modelD_Filter.VendorID) : string.Empty;
                            newModel.PartNo = modelD_Filter.PartNo;
                        }
                    }

                    //加入Project_DOASUpdate的資料
                    if (modelDoAS.Any())
                    {
                        var modelDoAS_Filter = modelDoAS.Where(e => e.DoID == item.DoID).OrderByDescending(e => e.DoUDate).FirstOrDefault();

                        if (modelDoAS_Filter != null)
                        {
                            newModel.DoUAction = modelDoAS_Filter.DoUAction;
                            newModel.DoStatus1 = modelDoAS_Filter.DoUStatus;
                            newModel.DoStatusDate = modelDoAS_Filter.DoUDate;
                        }
                    }
                    list_DoReport.Add(newModel);
                    seq++;
                }
            }           
            return list_DoReport;
        }

        public List<DoBonusViewModel> GetEmpBonus(List<BDoReportViewModel> list)
        {
            List<DoBonusViewModel> list_DoBouns = new List<DoBonusViewModel>();

            if (list.Count > 0)
            {
                //儲存每個人的獎金用的字典
                var EmpBonusTTL = new Dictionary<int, (int ActiveCount, int NewCount)>();

                foreach (var item in list)
                {
                    if (EmpBonusTTL.ContainsKey(item.ApplicantID))
                    {
                        if (item.NewActive == "Active")
                        {
                            EmpBonusTTL[item.ApplicantID] = (EmpBonusTTL[item.ApplicantID].ActiveCount + 1, EmpBonusTTL[item.ApplicantID].NewCount);
                        }
                        else if(item.NewActive == "New")
                        {
                            EmpBonusTTL[item.ApplicantID] = (EmpBonusTTL[item.ApplicantID].ActiveCount, EmpBonusTTL[item.ApplicantID].NewCount + 1);
                        }
                    }
                    else
                    {
                        if (item.NewActive == "Active")
                        {
                            EmpBonusTTL[item.ApplicantID] = (1, 0);
                        }
                        else if (item.NewActive == "New")
                        {
                            EmpBonusTTL[item.ApplicantID] = (0, 1);
                        }
                    }                   
                }

                if (EmpBonusTTL.Count > 0)
                {
                    List<DoBonusViewModel> list_CN = new List<DoBonusViewModel>();
                    List<DoBonusViewModel> list_TW = new List<DoBonusViewModel>();

                    foreach (var item in EmpBonusTTL)
                    {
                        string region = _employeeService.GetEmpRegion(item.Key);

                        if (string.IsNullOrEmpty(region))
                        {
                            continue;
                        }

                        if (region == "CN")
                        {
                            list_CN.Add(new DoBonusViewModel
                            {
                                Region_CN = region,
                                Owner_CN = _employeeService.GetEmployeeName_Onduty(item.Key),
                                Active_CN = item.Value.ActiveCount,
                                New_CN = item.Value.NewCount,
                                Amount_CN = item.Value.ActiveCount * 100 + item.Value.NewCount * 200
                            });
                        }
                        else
                        {
                            list_TW.Add(new DoBonusViewModel
                            {
                                Region_TW = region,
                                Owner_TW = _employeeService.GetEmployeeName_Onduty(item.Key),
                                Active_TW = item.Value.ActiveCount,
                                New_TW = item.Value.NewCount,
                                Amount_TW = item.Value.ActiveCount * 500 + item.Value.NewCount * 1000
                            });
                        }
                    }

                    // 計算每個區域的總額
                    var cnTotal = new DoBonusViewModel
                    {
                        Region_CN = "CN",
                        Owner_CN = "TTL",
                        Active_CN = list_CN.Sum(e => e.Active_CN),
                        New_CN = list_CN.Sum(e => e.New_CN),
                        Amount_CN = list_CN.Sum(e => e.Amount_CN)
                    };
                    list_CN.Add(cnTotal);

                    var twTotal = new DoBonusViewModel
                    {
                        Region_TW = "TW",
                        Owner_TW = "TTL",
                        Active_TW = list_TW.Sum(e => e.Active_TW),
                        New_TW = list_TW.Sum(e => e.New_TW),
                        Amount_TW = list_TW.Sum(e => e.Amount_TW)
                    };
                    list_TW.Add(twTotal);

                    // 配對 CN 和 TW 的資料
                    int maxCount = Math.Max(list_CN.Count, list_TW.Count);
                    for (int i = 0; i < maxCount; i++)
                    {
                        var cnEntry = i < list_CN.Count ? list_CN[i] : new DoBonusViewModel();
                        var twEntry = i < list_TW.Count ? list_TW[i] : new DoBonusViewModel();

                        list_DoBouns.Add(new DoBonusViewModel
                        {
                            Region_CN = cnEntry.Region_CN,
                            Owner_CN = cnEntry.Owner_CN,
                            Active_CN = cnEntry.Active_CN,
                            New_CN = cnEntry.New_CN,
                            Amount_CN = cnEntry.Amount_CN,
                            noted1 = "",
                            Region_TW = twEntry.Region_TW,
                            Owner_TW = twEntry.Owner_TW,
                            Active_TW = twEntry.Active_TW,
                            New_TW = twEntry.New_TW,
                            Amount_TW = twEntry.Amount_TW
                        });
                    }
                }
            }
            return list_DoBouns;
        }

        private async Task<List<DiDwReportViewModel>> GetDinReport(string[] projectIds)
        {
            List<DiDwReportViewModel> list_model= new List<DiDwReportViewModel> ();

            foreach (var item in projectIds)
            {
                var modelDiDw = await _projectDIDWContext.Project_DIDW.Where(e => e.ProjectID == item).FirstOrDefaultAsync();

                //先加入Din資料
                if (modelDiDw != null) 
                {
                    var ItemModel = new DiDwReportViewModel();

                    if (!string.IsNullOrEmpty(modelDiDw.DinDate))
                    {
                        ItemModel.ApplicationDate = modelDiDw.DinDate.Substring(0, 4) + "/" + modelDiDw.DinDate.Substring(4, 2) + "/" + modelDiDw.DinDate.Substring(6, 2);
                    }

                    ItemModel.ProjectID = item;

                    var modelM = _projectMContext.ProjectM.Where(e => e.ProjectID == item ).FirstOrDefault();

                    if(modelM != null)
                    {
                        if (!string.IsNullOrEmpty(modelM.Cus_DB) && !string.IsNullOrEmpty(modelM.CusID))
                        {
                            ItemModel.CusName = _cusVendoeService.GetvendorName(modelM.Cus_DB, modelM.CusID);
                        }
                        ItemModel.EndCusName = modelM.EndCus;
                        ItemModel.ProApp = modelM.ProApp;
                        ItemModel.ProModel = modelM.ProModel;

                        if (!string.IsNullOrEmpty(modelM.EProduceYS) && modelM.EProduceYS.Length == 6)
                        {
                            ItemModel.PYear = modelM.EProduceYS.Substring(0, 4);
                            ItemModel.PSeason = modelM.EProduceYS.Substring(4, 2);
                        }



                    }

                    var modelD = _projectDContext.ProjectD.Where(e => e.ProjectID == item && e.Stage == "DIN").FirstOrDefault();

                    if (modelD != null)
                    {
                        if (!string.IsNullOrEmpty(modelD.VendorID))
                        {
                            ItemModel.VendorName = _cusVendoeService.GetVendorName(modelD.VendorID);
                        }

                        ItemModel.PartName = modelD.PartNo;
                        ItemModel.FirstQty = modelD.EFirstYQty;
                        ItemModel.SecondQty = modelD.ESecondYQty;
                        ItemModel.ThirdQty = modelD.EThirdYQty;
                        ItemModel.FirstPrice = modelD.UFirstYPrice;
                        ItemModel.SecondPrice = modelD.USecondYPrice;
                        ItemModel.ThirdPrice = modelD.UThirdYPrice;
                    }

                }
            }
            return list_model;
        }

        public List<SelectListItem> GetRegion()
        {
            List<SelectListItem> selectListItems = new List<SelectListItem>();

            selectListItems = new List<SelectListItem>
            {
                new SelectListItem{ Text = "全區域", Value ="All"},
                new SelectListItem{ Text = "台灣區", Value ="TW"},
                new SelectListItem{ Text = "大陸區", Value ="CN"}
            };
            return selectListItems;
        }
    }
}