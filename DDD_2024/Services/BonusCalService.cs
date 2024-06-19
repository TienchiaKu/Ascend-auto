using DDD_2024.Data;
using DDD_2024.Interfaces;
using DDD_2024.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Drawing;

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
        private readonly IEmployeeService _employeeService;
        private readonly ICusVendorService _cusVendoeService;
        private readonly IDoService _doService;

        public BonusCalService(ProjectMContext projectMContext, ProjectDContext projectDContext, ProjectDOContext projectDOContext,
            Project_DIDWContext projectDIDWContext, Project_DOASUpdateContext project_DOASUpdateContext,
            Project_EmpContext project_EmpContext, IEmployeeService employeeService, ICusVendorService cusVendoeService, IDoService doService)
        {
            _projectMContext = projectMContext;
            _projectDContext = projectDContext;
            _projectDOContext = projectDOContext;
            _projectDIDWContext = projectDIDWContext;
            _projectEmpContext = project_EmpContext;
            _employeeService = employeeService;
            _cusVendoeService = cusVendoeService;
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

        private async Task<List<DoReportViewModel>> GetDoReport(string[] projectIds)
        {
            List<DoReportViewModel> list_DoReport = new List<DoReportViewModel>();

            foreach (var item in projectIds)
            {
                var modelDo = await _projectDOContext.Project_DO.Where(e => e.ProjectID == item).FirstOrDefaultAsync();

                if (modelDo != null)
                {
                    var ItemModel = new DoReportViewModel();

                    //取資料Project_DO
                    ItemModel.ProjectID = modelDo.ProjectID;

                    if (!string.IsNullOrEmpty(modelDo.CreateDate))
                    {
                        ItemModel.ApplicationDate = modelDo.CreateDate.Substring(0, 4) + "/" + modelDo.CreateDate.Substring(4, 2) + "/" + modelDo.CreateDate.Substring(6, 2);
                    }

                    ItemModel.ApplicantID = modelDo.ApplicantID;
                    ItemModel.Applicant = _employeeService.GetEmployeeName(modelDo.ApplicantID);
                    ItemModel.Approver = _employeeService.GetEmployeeName(modelDo.ApproverID);

                    if (!string.IsNullOrEmpty(modelDo.TradeStatus))
                    {
                        ItemModel.TradeStatus = _doService.GetTradingStatusName(modelDo.TradeStatus);
                    }

                    //取資料ProjectM
                    var modelM = await _projectMContext.ProjectM.Where(e => e.ProjectID == item).FirstOrDefaultAsync();

                    if (modelM != null)
                    {
                        if (!string.IsNullOrEmpty(modelM.Cus_DB) && !string.IsNullOrEmpty(modelM.CusID))
                        {
                            ItemModel.CusName = _cusVendoeService.GetvendorName(modelM.Cus_DB, modelM.CusID);
                        }

                        ItemModel.ProApp = modelM.ProApp;
                    }

                    //取資料ProjectD
                    var modelD = await _projectDContext.ProjectD.Where(e => e.ProjectID == item).FirstOrDefaultAsync();

                    if (modelD != null)
                    {
                        if (!string.IsNullOrEmpty(modelD.VendorID))
                        {
                            ItemModel.VendorName = _cusVendoeService.GetVendorName(modelD.VendorID);
                        }

                        ItemModel.PartNo = modelD.PartNo;
                    }

                    //取資料Project_DOASUpdate 還沒寫取最新的資料
                    if (!string.IsNullOrEmpty(modelDo.DoID))
                    {
                        var modelDOAS = _project_DOASUpdateContext.Project_DOASUpdate.
                                        Where(e => e.DoID == modelDo.DoID).ToList() ?? null;

                        if (modelDOAS != null)
                        {
                            var OrderDOAS = modelDOAS.OrderByDescending(p => p.DoUDate).FirstOrDefault();

                            if (OrderDOAS != null)
                            {
                                ItemModel.DoUStatus = OrderDOAS.DoUStatus;
                                ItemModel.DoUAction = OrderDOAS.DoUAction;
                            }
                        }
                    }
                    list_DoReport.Add(ItemModel);
                }
            }
            return list_DoReport;            
        }

        public async Task<(List<DoReportViewModel>?,List<DoBonusViewModel>)> GetDoBonus(string[] projectIds)
        {
            var model = await GetDoReport(projectIds);
            List<DoBonusViewModel> list_DoBouns = new List<DoBonusViewModel>();

            if (model != null)
            {
                //儲存每個人的獎金用的字典
                var EmpBonusTTL = new Dictionary<int, (int ActiveCount, int NewCount)>();

                foreach (var item in model)
                {
                    if (EmpBonusTTL.ContainsKey(item.ApplicantID))
                    {
                        if (item.TradeStatus == "Active")
                        {
                            EmpBonusTTL[item.ApplicantID] = (EmpBonusTTL[item.ApplicantID].ActiveCount + 1, EmpBonusTTL[item.ApplicantID].NewCount);
                        }
                        else if(item.TradeStatus == "New")
                        {
                            EmpBonusTTL[item.ApplicantID] = (EmpBonusTTL[item.ApplicantID].ActiveCount, EmpBonusTTL[item.ApplicantID].NewCount + 1);
                        }
                    }
                    else
                    {
                        if (item.TradeStatus == "Active")
                        {
                            EmpBonusTTL[item.ApplicantID] = (1, 0);
                        }
                        else if (item.TradeStatus == "New")
                        {
                            EmpBonusTTL[item.ApplicantID] = (0, 1);
                        }
                    }                   
                }

                if(EmpBonusTTL.Count > 0)
                {                                        
                    foreach(var item in EmpBonusTTL)
                    {
                        DoBonusViewModel doBonusViewModel = new DoBonusViewModel()
                        {
                            Region = _employeeService.GetEmpRegion(item.Key),
                            Owner = _employeeService.GetEmployeeName_Onduty(item.Key),
                            Active = item.Value.ActiveCount,
                            New = item.Value.NewCount,
                        };

                        if (string.IsNullOrEmpty(doBonusViewModel.Owner))
                        {
                            continue;
                        }
                        
                        if(doBonusViewModel.Region == "TW")
                        {
                            doBonusViewModel.Amount = doBonusViewModel.Active * 500 + doBonusViewModel.New * 1000;
                        }
                        else if (doBonusViewModel.Region == "CN")
                        {
                            doBonusViewModel.Amount = doBonusViewModel.Active * 100 + doBonusViewModel.New * 200;
                        }

                        list_DoBouns.Add(doBonusViewModel);
                    }
                    list_DoBouns = list_DoBouns.OrderBy(e => e.Region).ToList();
                }
            }
            return (model,list_DoBouns);
        }
    }
}
