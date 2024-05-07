using DDD_2024.Data;
using DDD_2024.Interfaces;
using DDD_2024.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel.DataAnnotations;

namespace DDD_2024.Services
{
    public class EmployeeService: IEmployeeService
    {
        private readonly BizAutoContext _context;
        public List<SelectListItem> YesNo { get; set; }

        public EmployeeService(BizAutoContext context)
        {
            _context = context;

            YesNo = new List<SelectListItem>
            {
                new SelectListItem{ Text = "---請選擇---", Value =""},
                new SelectListItem{ Text = "是", Value ="Y", Selected = true},
                new SelectListItem{ Text = "否", Value ="N"},
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
            if(!string.IsNullOrEmpty(yesNo) && yesNo.Equals("Y"))
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

            if(model != null)
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
            if(empID != 0) 
            {
                var emp = _context.employeeM.Where(e=> e.EmpID == empID && e.OnDuty == "Y").FirstOrDefault();

                if (emp != null) 
                {
                    return true;
                }
            }
            else
            {
                if(!string.IsNullOrEmpty(empName))
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
    }
}
