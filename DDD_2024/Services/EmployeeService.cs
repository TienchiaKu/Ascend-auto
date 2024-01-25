using DDD_2024.Data;
using DDD_2024.Interfaces;
using DDD_2024.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace DDD_2024.Services
{
    public class EmployeeService: IEmployeeService
    {
        private readonly DDD_EmployeeContext _context;
        public List<SelectListItem> YesNo { get; set; }

        public EmployeeService(DDD_EmployeeContext context)
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
                var employees = _context.DDD_Employee.ToList();
                return employees.Select(e => new SelectListItem
                {
                    Value = e.EmpID.ToString(),
                    Text = e.EmpName
                }).ToList();
            }          
        }

        public int MaxEmployeeID
        {
            get
            {
                var maxEmpID = _context.DDD_Employee.Max(e => e.EmpID)+1;
                return maxEmpID;
            }
        }
    }
}
