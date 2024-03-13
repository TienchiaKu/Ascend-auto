using DDD_2024.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace DDD_2024.Interfaces
{
    public interface IEmployeeService
    {
        int NewEmployeeID { get; }
        string GetEmployeeName(int? empID);
        string GetYesNoName(string yesNo);
        bool CheckEmpName(string empName);

        List<SelectListItem> YesNo { get; set; }
        List<SelectListItem> GetEmployeeNameList { get; }
        List<SelectListItem> GetEmployeeNameList_Selected(int? EmpID);
    }
}