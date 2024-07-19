using DDD_2024.Data;
using DDD_2024.Interfaces;
using DDD_2024.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace DDD_2024.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly BizAutoContext _context;
        public List<SelectListItem> YesNo { get; set; }
        public List<SelectListItem> YesNo_NotSelected { get; set; }
        public List<SelectListItem> Auth_selector { get; set; }
        public List<SelectListItem> Region_selector { get; set; }

        public EmployeeService(BizAutoContext context)
        {
            _context = context;

            YesNo = new List<SelectListItem>
            {
                new SelectListItem{ Text = "---請選擇---", Value =""},
                new SelectListItem{ Text = "是", Value ="Y", Selected = true},
                new SelectListItem{ Text = "否", Value ="N"}
            };

            YesNo_NotSelected = new List<SelectListItem>
            {
                new SelectListItem{ Text = "---請選擇---", Value =""},
                new SelectListItem{ Text = "是", Value ="Y"},
                new SelectListItem{ Text = "否", Value ="N"}
            };

            Auth_selector = new List<SelectListItem>
            {
                new SelectListItem{ Text = "---請選擇---", Value =""},
                new SelectListItem{ Text = "User", Value ="User"},
                new SelectListItem{ Text = "AD", Value ="AD"},
                new SelectListItem{ Text = "Sys", Value ="Sys"}
            };

            Region_selector = new List<SelectListItem>
            {
            new SelectListItem {Text = "TW" , Value = "TW"},
            new SelectListItem {Text = "CN" , Value = "CN"}
            };
        }

        public List<SelectListItem> GetEmployeeNameList
        {
            get
            {
                var employees = _context.employeeM.Where(e => e.OnDuty == "Y").ToList();

                // 添加一個空值的 SelectListItem
                var emptyItem = new EmployeeM()
                {
                    EmpID = 999,
                    EmpName = string.Empty
                };
                employees.Insert(0, emptyItem);

                return employees.Select(e => new SelectListItem
                {
                    Value = e.EmpID.ToString(),
                    Text = e.EmpName
                }).ToList();
            }
        }

        public List<SelectListItem> GetEmployeeNameList_Selected(int? EmpID)
        {
            var employees = _context.employeeM.Where(e => e.OnDuty == "Y").ToList();

            var emptyItem = new EmployeeM()
            {
                EmpID = 999,
                EmpName = string.Empty
            };
            employees.Insert(0, emptyItem);

            var list = employees.Select(e => new SelectListItem
            {
                Value = e.EmpID.ToString(),
                Text = e.EmpName,
                Selected = (e.EmpID == EmpID)
            }).ToList();

            return list;
        }

        public List<SelectListItem> GetPMList_Selected(int? EmpID)
        {
            var employees = _context.employeeM.Where(e => e.OnDuty == "Y").ToList();
            var dutys = _context.DutyM.Where(e => e.Role == "PM" && e.IsUse == "Y").Select(d => d.EmpID).ToList();

            var PMs = employees.Where(e => dutys.Contains(e.EmpID)).ToList();

            var emptyItem = new EmployeeM()
            {
                EmpID = 999,
                EmpName = string.Empty
            };
            PMs.Insert(0, emptyItem);

            var list = PMs.Select(e => new SelectListItem
            {
                Value = e.EmpID.ToString(),
                Text = e.EmpName,
                Selected = (e.EmpID == EmpID)
            }).ToList();

            return list;
        }

        public List<SelectListItem> GetSalesList_Selected(int? EmpID)
        {
            var employees = _context.employeeM.Where(e => e.OnDuty == "Y").ToList();
            var dutys = _context.DutyM.Where(e => e.Role == "Sales" && e.IsUse == "Y").Select(d => d.EmpID).ToList();

            var Sales = employees.Where(e => dutys.Contains(e.EmpID)).ToList();

            var emptyItem = new EmployeeM()
            {
                EmpID = 999,
                EmpName = string.Empty
            };
            Sales.Insert(0, emptyItem);

            var list = Sales.Select(e => new SelectListItem
            {
                Value = e.EmpID.ToString(),
                Text = e.EmpName,
                Selected = (e.EmpID == EmpID)
            }).ToList();

            return list;
        }

        public List<SelectListItem> GetFAEList_Selected(int? EmpID)
        {
            var employees = _context.employeeM.Where(e => e.OnDuty == "Y").ToList();
            var dutys = _context.DutyM.Where(e => e.Role == "FAE" && e.IsUse == "Y").Select(d => d.EmpID).ToList();

            var FAEs = employees.Where(e => dutys.Contains(e.EmpID)).ToList();

            var emptyItem = new EmployeeM()
            {
                EmpID = 999,
                EmpName = string.Empty
            };
            FAEs.Insert(0, emptyItem);

            var list = FAEs.Select(e => new SelectListItem
            {
                Value = e.EmpID.ToString(),
                Text = e.EmpName,
                Selected = (e.EmpID == EmpID)
            }).ToList();

            return list;
        }

        public int NewEmployeeID
        {
            get
            {
                if (_context.employeeM.Count() == 0)
                {
                    return 1;
                }
                else
                {
                    return _context.employeeM.Max(e => e.EmpID) + 1;
                }
            }
        }

        public string GetEmployeeName(int? empID)
        {
            if (empID == null || string.IsNullOrEmpty(empID.ToString()))
            {
                return string.Empty;
            }
            else
            {
                return _context.employeeM
                    .Where(e => e.EmpID == empID)
                    .Select(e => e.EmpName)
                    .FirstOrDefault() ?? string.Empty;
            }
        }

        public string GetEmployeeName_Onduty(int? empID)
        {
            if (empID == null || string.IsNullOrEmpty(empID.ToString()))
            {
                return string.Empty;
            }
            else
            {
                return _context.employeeM
                    .Where(e => e.EmpID == empID && e.OnDuty == "Y")
                    .Select(e => e.EmpName)
                    .FirstOrDefault() ?? string.Empty;
            }
        }

        public int GetEmployeeID(string EmpName)
        {
            if (!string.IsNullOrEmpty(EmpName))
            {
                return _context.employeeM.Where(e => e.EmpName == EmpName).Select(e => e.EmpID).FirstOrDefault();
            }
            else
            {
                return 0;
            }
        }

        public string GetYesNoName(string yesNo)
        {
            if (!string.IsNullOrEmpty(yesNo) && yesNo.Equals("Y"))
            {
                return "是";
            }
            else if (!string.IsNullOrEmpty(yesNo) && yesNo.Equals("N"))
            {
                return "否";
            }
            else
            {
                return string.Empty;
            }
        }

        public bool CheckEmpName(string empName)
        {
            var vendors = _context.employeeM.Where(e => e.OnDuty == "Y").ToList();
            return vendors.Any(v => v.EmpName == empName);
        }
        public bool Check_Login(EmployeeM employeeM)
        {
            if (string.IsNullOrEmpty(employeeM.userPWD))
            {
                return false;
            }

            var model = _context.employeeM.Where(e => e.EmpID == employeeM.EmpID && e.OnDuty == "Y" && e.userPWD == employeeM.userPWD).FirstOrDefault();

            if (model != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public string GetEmpRegion(int EmpID)
        {
            var model = _context.employeeM.Where(e => e.EmpID == EmpID).Select(e => e.Region).FirstOrDefault();

            if (model != null)
            {
                return model;
            }
            else
            {
                return string.Empty;
            }
        }

        public bool CheckOnDuty(int empID, string empName)
        {
            if (empID != 0)
            {
                var emp = _context.employeeM.Where(e => e.EmpID == empID && e.OnDuty == "Y").FirstOrDefault();

                if (emp != null)
                {
                    return true;
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(empName))
                {
                    var emp = _context.employeeM.Where(e => e.EmpName == empName && e.OnDuty == "Y").FirstOrDefault();

                    if (emp != null)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public async Task<List<EmpIndexViewModel>> GetEmployees()
        {
            List<EmpIndexViewModel> list_Emp = new List<EmpIndexViewModel>();
            
            var employees = await _context.employeeM.Where(e => e.OnDuty == "Y").ToListAsync();

            if (employees != null)
            {
                foreach(var item in employees)
                {                                      
                    var model = new EmpIndexViewModel()
                    {
                        EmpID = item.EmpID,
                        EmpName = item.EmpName,
                        Region = item.Region,
                        isSales = QueryDuty(item.EmpID, "Sales"),
                        isPM = QueryDuty(item.EmpID, "PM"),
                        isFAE = QueryDuty(item.EmpID, "FAE"),
                        isRBU = QueryDuty(item.EmpID, "RBU"),
                        Auth = QueryAuth(item.EmpID)
                    };
                    list_Emp.Add(model);
                }
            }
            return list_Emp;
        }

        public async Task<List<EmpIndexViewModel>> GetEmployeesFilter(EmployeeFilterViewModel filter)
        {
            List<EmpIndexViewModel> list_Filter = await GetEmployees();
            list_Filter = list_Filter.OrderBy(e => e.EmpName).ToList();

            return list_Filter;

            //List<EmployeeViewModel> list_empVM = new List<EmployeeViewModel>();
            //
            //if (filter != null)
            //{
            //    if (!string.IsNullOrEmpty(filter.IsName))
            //    {
            //        if(filter.IsName == "Y")
            //        {
            //            var employees = await _context.employeeM.Where(e => e.OnDuty == "Y").OrderBy(e => e.EmpName).ToListAsync();
            //
            //            if (employees != null)
            //            {
            //                foreach (var item in employees)
            //                {
            //                    var model = new EmployeeViewModel()
            //                    {
            //                        employee = item
            //                    };
            //
            //                    if (!string.IsNullOrEmpty(item.OnDuty))
            //                    {
            //                        model.OnDuty_CN = GetYesNoName(item.OnDuty);
            //                    }
            //                    list_empVM.Add(model);
            //                }
            //            }
            //        }
            //        else {
            //            var employees = await _context.employeeM.Where(e => e.OnDuty == "Y").ToListAsync();
            //
            //            if (employees != null)
            //            {
            //                foreach (var item in employees)
            //                {
            //                    var model = new EmployeeViewModel()
            //                    {
            //                        employee = item
            //                    };
            //
            //                    if (!string.IsNullOrEmpty(item.OnDuty))
            //                    {
            //                        model.OnDuty_CN = GetYesNoName(item.OnDuty);
            //                    }
            //                    list_empVM.Add(model);
            //                }
            //            }
            //        }
            //        
            //        
            //    }
            //}
            //
            //return list_empVM;
        }

        public async Task<string> CreateEmp(EmpCreateViewModel model)
        {
            string msg = string.Empty;
            
            //檢查是否有重複的員工姓名
            var chkModel = await _context.employeeM.Where(e => e.EmpName == model.EmpName).FirstOrDefaultAsync();

            if (chkModel != null)
            {
                msg = "該員工姓名已存在資料庫";
                return msg;
            }
            else 
            {
                int newEmpId = NewEmployeeID;

                //加入EmployeeM
                var NewEmpModel = new EmployeeM()
                {
                    EmpID = newEmpId,
                    EmpName = model.EmpName,
                    OnDuty = model.OnDuty,
                    UpdateDate = DateTime.Now,
                    Region = model.Region,
                };

                if (!string.IsNullOrEmpty(model.userPWD))
                {
                    NewEmpModel.userPWD = model.userPWD;
                }

                _context.employeeM.Add(NewEmpModel);
                await _context.SaveChangesAsync();

                msg += "員工資料建立成功\n";

                //加入DutyM
                var NewDutyListY = new List<string>();
                var NewDutyListN = new List<string>();

                if (!string.IsNullOrEmpty(model.isSales) && model.isSales == "Y")
                {
                    NewDutyListY.Add("Sales");
                }
                else
                {
                    NewDutyListN.Add("Sales");
                }

                if (!string.IsNullOrEmpty(model.isPM) && model.isPM == "Y")
                {
                    NewDutyListY.Add("PM");
                }
                else
                {
                    NewDutyListN.Add("PM");
                }

                if (!string.IsNullOrEmpty(model.isFAE) && model.isFAE == "Y")
                {
                    NewDutyListY.Add("FAE");
                }
                else
                {
                    NewDutyListN.Add("FAE");
                }

                if (!string.IsNullOrEmpty(model.isRBU) && model.isRBU == "Y")
                {
                    NewDutyListY.Add("RBU");
                }
                else
                {
                    NewDutyListN.Add("RBU");
                }

                if (!string.IsNullOrEmpty(model.Auth))
                {
                    NewDutyListY.Add(model.Auth);
                }

                if (NewDutyListY.Count > 0) 
                {
                    foreach(var item in NewDutyListY)
                    {
                        var NewRoleModel = new DutyM()
                        {
                            DutyID = NewDutyID,
                            EmpID = newEmpId,
                            Role = item,
                            IsUse = "Y",
                            UpdateDate = DateTime.Now
                        };
                        _context.DutyM.Add(NewRoleModel);
                    }
                    await _context.SaveChangesAsync();
                }

                if (NewDutyListN.Count > 0)
                {
                    foreach (var item in NewDutyListN)
                    {
                        var NewRoleModel = new DutyM()
                        {
                            DutyID = NewDutyID,
                            EmpID = newEmpId,
                            Role = item,
                            IsUse = "N",
                            UpdateDate = DateTime.Now
                        };
                        _context.DutyM.Add(NewRoleModel);
                    }
                    await _context.SaveChangesAsync();
                }
                msg += "員工職務/角色建立成功";
            }
            return msg;
        }

        public async Task<EmpEditViewModel> GetEmployee(int EmpId)
        {
            var EmpModel = await _context.employeeM.Where(e => e.EmpID == EmpId).FirstOrDefaultAsync();
            var DutyModel = await _context.DutyM.Where(e => e.EmpID == EmpId).ToListAsync();

            if (EmpModel != null && DutyModel.Count > 0) 
            {
                var EditModel = new EmpEditViewModel()
                {
                    EmpId = EmpId,
                    EmpName = EmpModel.EmpName,
                    Region = EmpModel.Region,
                    userPWD = EmpModel.userPWD,
                    OnDuty = EmpModel.OnDuty,
                    isSales = DutyModel.Where(e => e.Role == "Sales").Select(e => e.IsUse).FirstOrDefault(),
                    isPM = DutyModel.Where(e => e.Role == "PM").Select(e => e.IsUse).FirstOrDefault(),
                    isFAE = DutyModel.Where(e => e.Role == "FAE").Select(e => e.IsUse).FirstOrDefault(),
                    isRBU = DutyModel.Where(e => e.Role == "RBU").Select(e => e.IsUse).FirstOrDefault(),
                    Auth = DutyModel.Where(e => e.Role == "AD" || e.Role == "User" || e.Role == "Sys" && e.IsUse == "Y").Select(e => e.Role).FirstOrDefault()
                };
                return EditModel;
            }
            else
            {
                return new EmpEditViewModel();
            }
        }

        public async Task<string> EditEmployee(EmpEditViewModel model)
        {
            string msg = string.Empty;
            
            if (model != null)
            {
                var EmpModel = await _context.employeeM.Where(e => e.EmpID == model.EmpId).FirstOrDefaultAsync();
                var DutyModel = await _context.DutyM.Where(e => e.EmpID == model.EmpId).ToListAsync();

                if (EmpModel != null && DutyModel.Count > 0)
                {
                    if (!string.IsNullOrEmpty(model.EmpName))
                    {
                        EmpModel.EmpName = model.EmpName;
                    }
                    EmpModel.OnDuty = model.OnDuty;
                    EmpModel.UpdateDate = DateTime.Now;
                    EmpModel.Region = model.Region;

                    _context.employeeM.Update(EmpModel);

                    if(!string.IsNullOrEmpty(model.isSales) && model.isSales == "Y")
                    {
                        var UDutyModel = DutyModel.Where(e => e.Role == "Sales" && e.IsUse == "N").FirstOrDefault();

                        if (UDutyModel != null)
                        {
                            UDutyModel.IsUse = "Y";
                            UDutyModel.UpdateDate = DateTime.Now;

                            _context.DutyM.Update(UDutyModel);
                        }
                    }

                    if (!string.IsNullOrEmpty(model.isPM) && model.isPM == "Y")
                    {
                        var UDutyModel = DutyModel.Where(e => e.Role == "PM" && e.IsUse == "N").FirstOrDefault();

                        if (UDutyModel != null)
                        {
                            UDutyModel.IsUse = "Y";
                            UDutyModel.UpdateDate = DateTime.Now;

                            _context.DutyM.Update(UDutyModel);
                        }
                    }

                    if (!string.IsNullOrEmpty(model.isPM) && model.isFAE == "Y")
                    {
                        var UDutyModel = DutyModel.Where(e => e.Role == "FAE" && e.IsUse == "N").FirstOrDefault();

                        if (UDutyModel != null)
                        {
                            UDutyModel.IsUse = "Y";
                            UDutyModel.UpdateDate = DateTime.Now;

                            _context.DutyM.Update(UDutyModel);
                        }
                    }

                    if (!string.IsNullOrEmpty(model.isPM) && model.isRBU == "Y")
                    {
                        var UDutyModel = DutyModel.Where(e => e.Role == "RBU" && e.IsUse == "N").FirstOrDefault();

                        if (UDutyModel != null)
                        {
                            UDutyModel.IsUse = "Y";
                            UDutyModel.UpdateDate = DateTime.Now;

                            _context.DutyM.Update(UDutyModel);
                        }
                    }

                    if (!string.IsNullOrEmpty(model.Auth))
                    {
                        var UDutyModel = DutyModel.Where(e => e.Role == "AD" || e.Role == "User" || e.Role == "Sys" && e.IsUse == "Y").FirstOrDefault();

                        if (UDutyModel != null)
                        {
                            UDutyModel.Role = model.Auth;
                            UDutyModel.UpdateDate = DateTime.Now;

                            _context.DutyM.Update(UDutyModel);
                        }
                    }
                    await _context.SaveChangesAsync();
                    msg = "員工資料更新成功";
                }
                else
                {
                    msg = "找不到員工資料或職務資料";
                }
            }
            else
            {
                msg = "資料為空值";
            }
            return msg;
        }

        private bool QueryDuty(int empID, string dutyName)
        {
            var model = _context.DutyM.Where(e => e.EmpID == empID && e.Role == dutyName && e.IsUse == "Y").FirstOrDefault();

            if (model != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        private string QueryAuth(int empID)
        {
            var authName = _context.DutyM.Where(e => e.EmpID == empID && (e.Role == "Sys" || e.Role == "User" || e.Role == "AD"))
                        .Select(e => e.Role).FirstOrDefault();

            if (!string.IsNullOrEmpty(authName))
            {
                return authName;
            }
            else
            {
                return string.Empty;
            }
        }
        private int NewDutyID
        {
            get
            {
                if (_context.DutyM == null)
                {
                    return 1;
                }

                if (_context.DutyM.Count() == 0)
                {
                    return 1;
                }
                else
                {
                    return _context.DutyM.Max(e => e.DutyID) + 1;
                }
            }
        }
    }
}