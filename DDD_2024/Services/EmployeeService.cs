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
                new SelectListItem{ Text = "是", Value ="Y"},
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
            return employees.Select(e => new SelectListItem
            {
                Value = e.EmpID.ToString(),
                Text = e.EmpName,
                Selected = (e.EmpID == EmpID)         
            }).ToList();
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
    }
}
