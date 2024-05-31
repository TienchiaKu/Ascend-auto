using DDD_2024.Data;
using DDD_2024.Interfaces;
using DDD_2024.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

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
        private readonly ICusVendoeService _cusVendoeService;
        private readonly IDoService _doService;

        public BonusCalService(ProjectMContext projectMContext, ProjectDContext projectDContext, ProjectDOContext projectDOContext,
            Project_DIDWContext projectDIDWContext, Project_DOASUpdateContext project_DOASUpdateContext,
            Project_EmpContext project_EmpContext, IEmployeeService employeeService, ICusVendoeService cusVendoeService, IDoService doService)
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

        public async Task<List<BonusCalViewModel>> GetProjects_DO()
        {
            List<BonusCalViewModel> list_BonusCal = new List<BonusCalViewModel>();

            //加入DO資料
            var modelDo = await _projectDOContext.Project_DO.Where(p => p.Status == "C").ToListAsync();
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
                        model.VendorName = await _cusVendoeService.GetVenName(modelProjectD.VendorID);
                    }

                    list_BonusCal.Add(model);
                }
            }

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
                        model.VendorName = await _cusVendoeService.GetVenName(modelProjectD.VendorID);
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
                        model.VendorName = await _cusVendoeService.GetVenName(modelProjectD.VendorID);
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
                        model.VendorName = await _cusVendoeService.GetVenName(modelProjectD.VendorID);
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
                        model.VendorName = await _cusVendoeService.GetVenName(modelProjectD.VendorID);
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
                    case "DO":
                        model = GetDoBonus(projectID);
                        break;
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

        private ProjectBonusViewModel GetDoBonus(string projectID)
        {
            var projectM = _projectMContext.ProjectM.Where(e => e.ProjectID == projectID).FirstOrDefault();
            var projectDO = _projectDOContext.Project_DO.Where(e => e.ProjectID == projectID && e.Status == "C").FirstOrDefault();

            var model = new ProjectBonusViewModel();

            if (projectM != null && projectDO != null)
            {                             
                model.ProjectID = projectID;
                model.Stage = "DO";
                model.ApplicantID = projectDO.ApplicantID;
                model.ApplicantName = _employeeService.GetEmployeeName(projectDO.ApplicantID);
                model.TradeStatus = projectDO.TradeStatus;
                model.Region = _employeeService.GetEmpRegion(projectDO.ApplicantID);

                if (_employeeService.CheckOnDuty(model.ApplicantID, string.Empty))
                {
                    if (!string.IsNullOrEmpty(model.Region) && !string.IsNullOrEmpty(model.TradeStatus))
                    {
                        if (model.Region == "TW" && model.TradeStatus == "N")
                        {
                            model.DOBonus = 1000;
                        }
                        else if (model.Region == "TW" && model.TradeStatus == "A")
                        {
                            model.DOBonus = 500;
                        }
                        else if (model.Region == "CN" && model.TradeStatus == "N")
                        {
                            model.DOBonus = 200;
                        }
                        else if (model.Region == "CN" && model.TradeStatus == "A")
                        {
                            model.DOBonus = 100;
                        }
                    }
                }
                else
                {
                    model.DOBonus = 0;
                }
            }
            return model;
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

        public async Task<List<DoReportViewModel>> GetDoReport()
        {
            List<DoReportViewModel> list_DoReport = new List<DoReportViewModel>();

            var model = _projectDOContext.Project_DO.Where(e => e.Status == "N").ToList();

            foreach(var item in model)
            {
                var ItemModel = new DoReportViewModel();

                //取資料Project_DO
                ItemModel.ProjectID = item.ProjectID;

                if (!string.IsNullOrEmpty(item.CreateDate))
                {
                    ItemModel.ApplicationDate = item.CreateDate.Substring(0, 4) + "/" + item.CreateDate.Substring(4, 2) + "/" + item.CreateDate.Substring(6, 2);
                }

                ItemModel.Applicant = _employeeService.GetEmployeeName(item.ApplicantID);
                ItemModel.Approver = _employeeService.GetEmployeeName(item.ApproverID);

                if (!string.IsNullOrEmpty(item.TradeStatus))
                {
                    item.TradeStatus = _doService.GetTradingStatusName(item.TradeStatus);
                }

                //取資料ProjectM
                var modelM = _projectMContext.ProjectM.Where(e => e.ProjectID == item.ProjectID).FirstOrDefault();

                if(modelM != null)
                {
                    if(!string.IsNullOrEmpty(modelM.Cus_DB) && !string.IsNullOrEmpty(modelM.CusID))
                    {
                        ItemModel.CusName = _cusVendoeService.GetvendorName(modelM.Cus_DB,modelM.CusID);
                    }

                    ItemModel.ProApp = modelM.ProApp;
                }

                //取資料ProjectD
                var modelD = _projectDContext.ProjectD.Where(e => e.ProjectID == item.ProjectID).FirstOrDefault();

                if (modelD != null)
                {
                    if (!string.IsNullOrEmpty(modelD.VendorID))
                    {
                        ItemModel.VendorName = await _cusVendoeService.GetVenName(modelD.VendorID);
                    }

                    ItemModel.PartNo = modelD.PartNo;
                }

                //取資料Project_DOASUpdate 還沒寫取最新的資料
                var modelDOAS = _project_DOASUpdateContext.Project_DOASUpdate.Where(e => e.DoID == item.DoID).OrderByDescending(p => p.DoUDate).FirstOrDefault();

                if(modelDOAS != null)
                {
                    ItemModel.DoUStatus = modelDOAS.DoUStatus;
                    ItemModel.DoUAction = modelDOAS.DoUAction;
                }
            }

            return list_DoReport;
        }
       
    }
}
